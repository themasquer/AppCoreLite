using AppCoreLite.Configs;
using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Records.Bases;
using AppCoreLite.Results;
using AppCoreLite.Results.Bases;
using AppCoreLite.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCoreLite.Services.Bases
{
    /// <summary>
    /// Base service class that includes common fields, properties and methods for its sub classes EntityService and Service.
    /// </summary>
    public abstract class ServiceBase<TEntity> : IDisposable, IConfig where TEntity : Record, new()
    {
        protected readonly DbContext _db;
        protected readonly RecordFileUtil _fileUtil;
        protected readonly SessionUtil _sessionUtil;
        protected readonly AccountUtil _userUtil;
        protected Languages _language;

        protected ReflectionUtil _reflectionUtil;
        protected PageOrderFilter? _pageOrderFilter;
        protected List<Property>? _propertiesForOrdering;
        protected List<Property>? _propertiesForFiltering;
        protected string? _isDeleted;

        private string? _createDate;
        private string? _updateDate;
        private string? _createdBy;
        private string? _updatedBy;
        private string? _guid;
        private string? _fileData;
        private string? _fileContent;
        private string? _filePath;

        public ServiceConfig Config { get; }
        public List<int> PageNumbers
        {
            get
            {
                var pageNumbers = new List<int>();
                if (_pageOrderFilter != null)
                {
                    if (_pageOrderFilter.TotalRecordsCountResult == 0 || RecordsPerPageCounts != null && RecordsPerPageCounts.Count > 0 && _pageOrderFilter.RecordsPerPageCount == RecordsPerPageCounts.LastOrDefault())
                    {
                        pageNumbers.Add(1);
                    }
                    else
                    {
                        int numberOfPages = Convert.ToInt32(Math.Ceiling(_pageOrderFilter.TotalRecordsCountResult / Convert.ToDecimal(_pageOrderFilter.RecordsPerPageCount)));
                        for (int page = 1; page <= numberOfPages; page++)
                        {
                            pageNumbers.Add(page);
                        }
                    }
                }
                return pageNumbers;
            }
        }
        public List<string>? RecordsPerPageCounts { get; set; }
        public List<string>? OrderExpressions => _propertiesForOrdering?.Select(pm => !string.IsNullOrWhiteSpace(pm.DisplayName) ? pm.DisplayName : pm.Name).ToList();
        public string? TotalRecordsCount { get; set; }

        protected ServiceBase(DbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userUtil = new AccountUtil(httpContextAccessor);
            _sessionUtil = new SessionUtil(httpContextAccessor);
            _fileUtil = new RecordFileUtil();
            _reflectionUtil = new ReflectionUtil();
            Config = new ServiceConfig();
            _language = Languages.Turkish;
            _pageOrderFilter = null;
            SetEntityInterfacePropertyNames();
        }

        public virtual void Set(Languages language)
        {
            Config.Set(language);
            _language = language;
        }

        public void Set(string acceptedFileExtensions, double acceptedFileLengthInMegaBytes, params string[] hierarchicalDirectories)
        {
            _fileUtil.Set(acceptedFileExtensions, acceptedFileLengthInMegaBytes, hierarchicalDirectories);
        }

        public void Set(IFormFile formFile)
        {
            _fileUtil.Set(formFile);
        }

		public virtual Result Delete<TRelationalEntity>(Expression<Func<TRelationalEntity, bool>> predicate, bool save = false) where TRelationalEntity : class, new()
		{
			_db.Set<TRelationalEntity>().RemoveRange(_db.Set<TRelationalEntity>().Where(predicate));
			if (save)
			{
                _db.SaveChanges();
				return Success(Config.RelatedRecordsDeletedSuccessfully);
			}
			return Error(Config.ChangesNotSaved);
		}

		public virtual int Save()
        {
            foreach (var entry in _db.ChangeTracker.Entries<TEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (_guid != null)
                        {
                            entry.CurrentValues[_guid] = Guid.NewGuid().ToString();
                        }
                        if (_createDate != null && _createdBy != null && _userUtil != null)
                        {
                            entry.CurrentValues[_createDate] = DateTime.Now;
                            entry.CurrentValues[_createdBy] = _userUtil.GetUser()?.UserName;
                        }
                        if (_fileUtil != null && _fileData != null && _fileContent != null && _filePath != null)
                        {
                            var file = _fileUtil.SaveFile();
                            entry.CurrentValues[_fileData] = file?.FileData;
                            entry.CurrentValues[_fileContent] = file?.FileContent;
                            entry.CurrentValues[_filePath] = file?.FilePath;
                        }
                        break;
                    case EntityState.Modified:
                        if (_guid != null)
                        {
                            entry.Property(_guid).IsModified = false;
                        }
                        if (_updateDate != null && _updatedBy != null && _createDate != null && _createdBy != null && _userUtil != null)
                        {
                            entry.Property(_createDate).IsModified = false;
                            entry.Property(_createdBy).IsModified = false;
                            entry.CurrentValues[_updateDate] = DateTime.Now;
                            entry.CurrentValues[_updatedBy] = _userUtil.GetUser()?.UserName;
                        }
                        if (_fileUtil != null && _fileData != null && _fileContent != null && _filePath != null)
                        {
                            if (_fileUtil.HasFormFile)
                            {
                                var file = _fileUtil.SaveFile();
                                entry.CurrentValues[_fileData] = file?.FileData;
                                entry.CurrentValues[_fileContent] = file?.FileContent;
                                entry.CurrentValues[_filePath] = file?.FilePath;
                            }
                            else if (_fileUtil.IsFileDeleted)
                            {
                                entry.CurrentValues[_fileData] = null;
                                entry.CurrentValues[_fileContent] = null;
                                entry.CurrentValues[_filePath] = null;
                            }
                            else
                            {
                                entry.Property(_fileData).IsModified = false;
                                entry.Property(_fileContent).IsModified = false;
                                entry.Property(_filePath).IsModified = false;
                            }
                        }
                        break;
                    case EntityState.Deleted:
                        if (_guid != null)
                        {
                            entry.Property(_guid).IsModified = false;
                        }
                        if (_isDeleted != null)
                        {
                            entry.CurrentValues[_isDeleted] = true;
                            if (_updateDate != null && _updatedBy != null && _createDate != null && _createdBy != null && _userUtil != null)
                            {
                                entry.Property(_createDate).IsModified = false;
                                entry.Property(_createdBy).IsModified = false;
                                entry.CurrentValues[_updateDate] = DateTime.Now;
                                entry.CurrentValues[_updatedBy] = _userUtil.GetUser()?.UserName;
                            }
                            if (_fileUtil != null && _fileData != null && _fileContent != null && _filePath != null)
                            {
                                entry.Property(_fileData).IsModified = false;
                                entry.Property(_fileContent).IsModified = false;
                                entry.Property(_filePath).IsModified = false;
                            }
                            entry.State = EntityState.Modified;
                        }
                        break;
                }
            }
            return _db.SaveChanges();
        }

        public virtual void DeleteFiles(List<int> ids, bool updateEntity = false)
        {
            _fileUtil.DeleteFiles(ids);
            if (updateEntity)
            {
                var entities = _db.Set<TEntity>().Where(e => ids.Contains(e.Id)).ToList();
                foreach (var entity in entities)
                {
                    _db.Set<TEntity>().Update(entity);
                }
                _fileUtil.Set(true);
                Save();
            }
        }

        public virtual void DeleteFile(int id, bool updateEntity = false)
        {
            DeleteFiles(new List<int>() { id }, updateEntity);
        }

        public virtual RecordFileToDownload? DownloadFile(int id, string? fileToDownloadFileNameWithoutExtension = null, bool useOctetStreamContentType = false)
        {
            RecordFileToDownload? file = _fileUtil.DownloadFile(id, fileToDownloadFileNameWithoutExtension, useOctetStreamContentType);
            if (file == null)
            {
                if (_fileData != null && _fileContent != null && _filePath != null)
                {
                    var entity = _db.Set<TEntity>().SingleOrDefault(e => e.Id == id);
                    if (entity == null)
                        return null;
                    var fileDataPropertyInfo = _reflectionUtil.GetPropertyInfo(entity, _fileData);
                    var fileData = fileDataPropertyInfo?.GetValue(entity);
                    var fileContentPropertyInfo = _reflectionUtil.GetPropertyInfo(entity, _fileContent);
                    var fileContent = fileContentPropertyInfo?.GetValue(entity);
                    file = new RecordFileToDownload()
                    {
                        Stream = fileData != null ? new MemoryStream((byte[])fileData) : null,
                        ContentType = fileContent != null ? RecordFileUtil.GetContentType(fileContent.ToString(), false, false) : null,
                        FileName = fileContent != null ?
                            (string.IsNullOrWhiteSpace(fileToDownloadFileNameWithoutExtension) ? id + fileContent.ToString() : fileToDownloadFileNameWithoutExtension + fileContent.ToString())
                            : null
                    };
                }
            }
            return file;
        }

        public ErrorResult Error(string message)
        {
            return new ErrorResult(message);
        }

        public ErrorResult Error()
        {
            return new ErrorResult();
        }

		public ErrorResult<TResultType> Error<TResultType>(string message, TResultType data)
		{
			return new ErrorResult<TResultType>(message, data);
		}

		public ErrorResult<TResultType> Error<TResultType>(string message)
		{
			return new ErrorResult<TResultType>(message);
		}

		public ErrorResult<TResultType> Error<TResultType>(TResultType data)
		{
			return new ErrorResult<TResultType>(data);
		}

		public ErrorResult<TResultType> Error<TResultType>()
		{
			return new ErrorResult<TResultType>();
		}

		public SuccessResult Success(string message)
		{
			return new SuccessResult(message);
		}

		public SuccessResult Success()
		{
			return new SuccessResult();
		}

		public SuccessResult<TResultType> Success<TResultType>(string message, TResultType data)
		{
			return new SuccessResult<TResultType>(message, data);
		}

		public SuccessResult<TResultType> Success<TResultType>(string message)
		{
			return new SuccessResult<TResultType>(message);
		}

		public SuccessResult<TResultType> Success<TResultType>(TResultType data)
		{
			return new SuccessResult<TResultType>(data);
		}

		public SuccessResult<TResultType> Success<TResultType>()
		{
			return new SuccessResult<TResultType>();
		}

		public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected Property? GetOrderingProperty(string orderExpression)
        {
            Property? propertyForOrdering = _propertiesForOrdering?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(orderExpression))
            {
                propertyForOrdering = _propertiesForOrdering?.FirstOrDefault(pm => pm.DisplayName == orderExpression);
                if (propertyForOrdering == null)
                    propertyForOrdering = _propertiesForOrdering?.FirstOrDefault(pm => pm.Name == orderExpression);
            }
            return propertyForOrdering;
        }

        private void SetEntityInterfacePropertyNames()
        {
            var property = _reflectionUtil.GetProperties<ISoftDelete>()?[0];
            _isDeleted = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[0];
            _createDate = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[1];
            _createdBy = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[2];
            _updateDate = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[3];
            _updatedBy = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            _guid = _reflectionUtil.GetProperties<Record>()?[1]?.Name;
            property = _reflectionUtil.GetProperties<IRecordFile>()?[0];
            _fileData = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IRecordFile>()?[1];
            _fileContent = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IRecordFile>()?[2];
            _filePath = _reflectionUtil.GetProperties<TEntity>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
        }
    }
}
