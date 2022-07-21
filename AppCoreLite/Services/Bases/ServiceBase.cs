using AppCoreLite.Configs;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Records.Bases;
using AppCoreLite.Utils;
using AppCoreLite.Utils.Bases.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AppCoreLite.Services.Bases
{
    /// <summary>
    /// Base service class that includes common fields, properties and methods for its sub classes EntityService and Service.
    /// </summary>
    public abstract class ServiceBase<T> : IDisposable where T : Record, new()
    {
        protected readonly DbContext _db;
        protected readonly RecordFileUtilBase? _fileUtil;
        protected readonly SessionUtilBase? _sessionUtil;

        protected ReflectionUtility _reflectionUtil;
        protected PageOrderFilter? _pageOrderFilter;
        protected List<Property>? _propertiesForOrdering;
        protected List<Property>? _propertiesForFiltering;
        protected string? _isDeleted;

        private readonly UserUtilBase? _userUtil;
        
        private string? _createDate;
        private string? _updateDate;
        private string? _createdBy;
        private string? _updatedBy;
        private string? _guid;
        private string? _fileData;
        private string? _fileContent;
        private string? _filePath;

        public ServiceConfig Config { get; set; }
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

        protected ServiceBase(DbContext db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, RecordFileUtilBase? fileUtil)
        {
            _db = db;
            _userUtil = userUtil;
            _sessionUtil = sessionUtil;
            _fileUtil = fileUtil;
            _reflectionUtil = new ReflectionUtility();
            Config = new ServiceConfig();
            _pageOrderFilter = null;
            SetEntityInterfacePropertyNames();
        }

        public void SetConfig(Languages language)
        {
            Config.Set(language);
        }

        public void SetFileUtilConfig(string acceptedFileExtensions, double acceptedFileLengthInMegaBytes, params string[] hierarchicalDirectories)
        {
            _fileUtil?.SetConfig(acceptedFileExtensions, acceptedFileLengthInMegaBytes, hierarchicalDirectories);
        }

        public void SetFileUtilConfig(IFormFile formFile)
        {
            _fileUtil?.SetConfig(formFile);
        }

        public virtual int Save()
        {
            foreach (var entry in _db.ChangeTracker.Entries<T>())
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
                            entry.CurrentValues[_createdBy] = _userUtil.User.UserName;
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
                            entry.CurrentValues[_updatedBy] = _userUtil.User.UserName;
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
                                entry.CurrentValues[_updatedBy] = _userUtil.User.UserName;
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
            _fileUtil?.DeleteFiles(ids);
            if (updateEntity)
            {
                var entities = _db.Set<T>().Where(e => ids.Contains(e.Id)).ToList();
                foreach (var entity in entities)
                {
                    _db.Set<T>().Update(entity);
                }
                _fileUtil?.SetConfig(true);
                Save();
            }
        }

        public virtual void DeleteFile(int id, bool updateEntity = false)
        {
            DeleteFiles(new List<int>() { id }, updateEntity);
        }

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected Property? GetProperty(string orderExpression)
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
            _isDeleted = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[0];
            _createDate = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[1];
            _createdBy = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[2];
            _updateDate = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IModifiedBy>()?[3];
            _updatedBy = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            _guid = _reflectionUtil.GetProperties<Record>()?[1]?.Name;
            property = _reflectionUtil.GetProperties<IRecordFile>()?[0];
            _fileData = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IRecordFile>()?[1];
            _fileContent = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = _reflectionUtil.GetProperties<IRecordFile>()?[2];
            _filePath = _reflectionUtil.GetProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
        }
    }
}
