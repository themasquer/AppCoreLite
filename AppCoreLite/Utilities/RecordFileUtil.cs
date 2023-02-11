using AppCoreLite.Models;
using AppCoreLite.Records.Bases;
using Microsoft.AspNetCore.Http;

namespace AppCoreLite.Utilities
{
    public class RecordFileUtil
    {
        private IFormFile? _formFile;
        private string? _acceptedFileExtensions;
        private double? _acceptedFileLengthInMegaBytes;
        private char _acceptedFileExtensionsSeperator;
        private List<string>? _hierarchicalDirectories;
        private RecordFile? _file;
        private bool _isFileDeleted;

        public bool HasFormFile => _formFile == null ? false : true;
        public bool IsFileDeleted => _isFileDeleted;

        public RecordFileUtil()
        {
            _formFile = null;
            _acceptedFileExtensions = null;
            _acceptedFileLengthInMegaBytes = null;
            _acceptedFileExtensionsSeperator = ',';
            _hierarchicalDirectories = null;
            _file = null;
            _isFileDeleted = false;
        }

        public void Set(string? acceptedFileExtensions = null, double? acceptedFileLengthInMegaBytes = null, params string[] hierarchicalDirectories)
        {
            _acceptedFileExtensions = acceptedFileExtensions;
            _acceptedFileLengthInMegaBytes = acceptedFileLengthInMegaBytes;
            _hierarchicalDirectories = hierarchicalDirectories.Length == 0 ? null : hierarchicalDirectories.ToList();
        }

        public void Set(IFormFile formFile)
        {
            _formFile = formFile;
        }

        public void Set(bool isFileDeleted)
        {
            _isFileDeleted = isFileDeleted;
        }

        public static string GetImgSrc(IRecordFile record)
        {
            string imgSrc = "";
            if (record.FileData != null && !string.IsNullOrWhiteSpace(record.FileContent))
                imgSrc = GetContentType(record.FileContent) + Convert.ToBase64String(record.FileData);
            return imgSrc;
        }

        public static string GetImgSrc(IRecordFile record, int entityId)
        {
            string imgSrc = "";
            if (!string.IsNullOrWhiteSpace(record.FileContent) && !string.IsNullOrWhiteSpace(record.FilePath))
                imgSrc = record.FilePath + entityId + record.FileContent;
            return imgSrc;
        }

        public RecordFile? SaveFile(int? entityId = null)
        {
            if (_formFile != null)
            {
                var hierarchicalDirectories = _hierarchicalDirectories != null ? _hierarchicalDirectories.ToList() : null;
                hierarchicalDirectories?.Remove("wwwroot");
                _file = new RecordFile()
                {
                    FileContent = Path.GetExtension(_formFile.FileName).ToLower(),
                    FilePath = hierarchicalDirectories != null ? "/" + string.Join("/", hierarchicalDirectories) + "/" : null
                };
                if (_hierarchicalDirectories == null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        _formFile.CopyTo(memoryStream);
                        _file.FileData = memoryStream.ToArray();
                        _file.FilePath = null;
                    }
                }
                else
                {
                    if (entityId.HasValue)
                    {
                        using (FileStream fileStream = new FileStream(CreatePath(entityId + _file.FileContent), FileMode.Create))
                        {
                            _formFile.CopyTo(fileStream);
                        }
                    }
                }
            }
            return _file;
        }

        public bool? CheckFile()
        {
            bool? result = null;
            if (_formFile != null && !string.IsNullOrWhiteSpace(_acceptedFileExtensions) && _acceptedFileLengthInMegaBytes != null)
            {
                string fileExtension = Path.GetExtension(_formFile.FileName);
                string[] acceptedFileExtensionsArray = _acceptedFileExtensions.Split(_acceptedFileExtensionsSeperator);
                foreach (string acceptedFileExtensionsItem in acceptedFileExtensionsArray)
                {
                    if (acceptedFileExtensionsItem.Trim().Equals(fileExtension.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                        break;
                    }
                }
                if (result == true)
                {
                    if (_formFile.Length > _acceptedFileLengthInMegaBytes * Math.Pow(1024, 2))
                        result = false;
                }
            }
            return result;
        }

        public void DeleteFiles(List<int> ids)
        {
            if (_hierarchicalDirectories != null)
            {
                List<string> filePaths = Directory.GetFiles(CreatePath()).Where(file => ids.Select(id => id.ToString()).Contains(Path.GetFileNameWithoutExtension(file))).ToList();
                foreach (string filePath in filePaths)
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
        }

        public void DeleteFile(int id)
        {
            DeleteFiles(new List<int>() { id });
        }

        public static string GetContentType(string? fileNameOrExtension, bool includeData = true, bool inclueBase64 = true)
        {
            if (string.IsNullOrWhiteSpace(fileNameOrExtension))
                return "";
            Dictionary<string, string> mimeTypes = new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".csv", "text/csv" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" }
            };
            string contentType;
            string fileExtension = Path.GetExtension(fileNameOrExtension).ToLower();
            contentType = mimeTypes[fileExtension];
            if (includeData)
                contentType = "data:" + contentType;
            if (inclueBase64)
                contentType = contentType + ";base64,";
            return contentType;
        }

        public virtual RecordFileToDownload? DownloadFile(int entityId, string? fileToDownloadFileNameWithoutExtension = null, bool useOctetStreamContentType = false)
        {
            RecordFileToDownload? file = null;
            if (_hierarchicalDirectories != null)
            {
                string filePath = CreatePath();
                string? fileNameWithoutPath = GetFileNameWithoutPath(entityId.ToString(), filePath);
                if (string.IsNullOrWhiteSpace(fileNameWithoutPath))
                    return null;
                string fileExtension = Path.GetExtension(fileNameWithoutPath);
                file = new RecordFileToDownload()
                {
                    Stream = new FileStream(Path.Combine(filePath, fileNameWithoutPath), FileMode.Open),
                    ContentType = useOctetStreamContentType ? "application/octet-stream" : GetContentType(fileNameWithoutPath, false, false),
                    FileName = string.IsNullOrWhiteSpace(fileToDownloadFileNameWithoutExtension) ? entityId + fileExtension : fileToDownloadFileNameWithoutExtension + fileExtension
                };
            }
            return file;
        }

        private string? GetFileNameWithoutPath(string fileNameWithoutExtension, string filePath)
        {
            string[] files = Directory.GetFiles(filePath);
            if (files == null || files.Length == 0)
                return null;
            string? file = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileNameWithoutExtension);
            if (file == null)
                return null;
            return Path.GetFileName(file);
        }

        private string CreatePath()
        {
            if (_hierarchicalDirectories != null)
            {
                string[] path = new string[_hierarchicalDirectories.Count];
                int i;
                for (i = 0; i < _hierarchicalDirectories.Count; i++)
                {
                    path[i] = _hierarchicalDirectories[i];
                }
                return Path.Combine(path);
            }
            return "";
        }

        private string CreatePath(string fileName)
        {
            string path = CreatePath();
            if (path != "")
                return path + @"\" + fileName;
            return "";
        }
    }
}
