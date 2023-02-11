using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.ViewModels;
using System.Text;

namespace AppCoreLite.Managers.Bases
{
    public abstract class FileBrowserManagerBase
    {
        private string _controller = "";
        private string _action = "";
        private string _rootPath = "";
        private string _startLink = "";

        private string? _fullPath;

        public void Set(string controller, string action, string wwwrootPath, string rootPath, string startLink = "Home")
        {
            _controller = controller;
            _action = action;
            _rootPath = $"{wwwrootPath}\\{rootPath}\\";
            _startLink = startLink;
        }

        public virtual FileBrowserViewModel? GetContents(string? path, bool includeLineNumbers = false)
        {
            if (string.IsNullOrWhiteSpace(_rootPath))
                return null;
            _fullPath = _rootPath;
            if (path is not null && path.StartsWith(_startLink))
                _fullPath += path.Remove(0, _startLink.Length).TrimStart('\\');
            FileBrowserViewModel? fileBrowserViewModel;
            if (File.Exists(_fullPath))
            {
                fileBrowserViewModel = GetFile(_fullPath, includeLineNumbers);
                if (fileBrowserViewModel is not null)
                {
                    if (!string.IsNullOrWhiteSpace(fileBrowserViewModel.FileContent))
                        fileBrowserViewModel.Title = AddLinks(path ?? _startLink);
                    else
                        fileBrowserViewModel.Title = Path.GetFileName(_fullPath);
                }
            }
            else
            {
                fileBrowserViewModel = GetDirectories(_fullPath);
                var files = Directory.GetFiles(_fullPath).Select(f => new FileBrowserModel()
                {
                    FileName = Path.GetFileName(f),
                    FileFolders = _startLink + "\\" + f.Substring(f.IndexOf(_rootPath) + _rootPath.Length),
                    IsFile = true
                }).ToList();
                fileBrowserViewModel.Contents?.AddRange(files);
                fileBrowserViewModel.Title = AddLinks(path ?? _startLink);
            }
            return fileBrowserViewModel;
        }

        private FileBrowserViewModel? GetFile(string path, bool includeLineNumbers)
        {
            Dictionary<string, string> textFiles = new Dictionary<string, string>
            {
                { ".txt", "plaintext" },
                { ".json", "json" },
                { ".xml", "xml" },
                { ".htm", "html" },
                { ".html", "html" },
                { ".css", "css" },
                { ".js", "javascript" },
                { ".cs", "csharp" },
                { ".sql", "sql" }
            };
            Dictionary<string, string> imageFiles = new Dictionary<string, string>()
            {
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" }
            };
            Dictionary<string, string> compressedFiles = new Dictionary<string, string>()
            {
                { ".zip", "application/zip" }
            };
            string content = "";
            string? line;
            int lineNumber = 0;
            FileBrowserViewModel? fileBrowserViewModel = null;
            string extension = Path.GetExtension(path).ToLower();
            if (textFiles.Keys.Contains(extension))
            {
                using (var streamReader = new StreamReader(path, Encoding.GetEncoding(1254)))
                {
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        content += "\n" + (includeLineNumbers ? $"{++lineNumber}.\t{line}" : line);
                    }
                }
                fileBrowserViewModel = new FileBrowserViewModel()
                {
                    FileContent = content,
                    FileType = FileTypes.Text,
                    FileContentType = textFiles[extension]
                };
            }
            else if (imageFiles.Keys.Contains(extension))
            {
                fileBrowserViewModel = new FileBrowserViewModel()
                {
                    FileBinaryContent = File.ReadAllBytes(path),
                    FileType = FileTypes.Image,
                    FileContentType = imageFiles[extension]
                };
                fileBrowserViewModel.FileContent = "data:" + imageFiles[extension] + ";base64," + Convert.ToBase64String(fileBrowserViewModel.FileBinaryContent);
            }
            else if (compressedFiles.Keys.Contains(extension))
            {
                fileBrowserViewModel = new FileBrowserViewModel()
                {
                    FileBinaryContent = File.ReadAllBytes(path),
                    FileType = FileTypes.Compressed,
                    FileContentType = compressedFiles[extension]
                };
                fileBrowserViewModel.Title = Path.GetFileName(path);
            }
            return fileBrowserViewModel;
        }

        private FileBrowserViewModel GetDirectories(string path)
        {
            return new FileBrowserViewModel()
            {
                Contents = Directory.GetDirectories(path).Select(d => new FileBrowserModel()
                {
                    FileName = Path.GetFileName(d),
                    FileFolders = _startLink + "\\" + (string.IsNullOrWhiteSpace(path) ?
                    Path.GetFileName(d) :
                    d.Substring(d.IndexOf(_rootPath) + _rootPath.Length))
                }).ToList()
            };
        }

        private string AddLinks(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = _startLink;
            string[] items = path.Split('\\');
            var linkedItems = new List<string>();
            string linkedItem;
            for (int i = 0; i < items.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(items[i]))
                {
                    linkedItem = "";
                    for (int j = 0; j <= i; j++)
                    {
                        linkedItem += items[j] + "\\";
                    }
                    linkedItems.Add("<a style=\"text-decoration: none;\" class=\"text-dark\" href=\"/" + _controller + "/" + _action + "?path=" + linkedItem.TrimEnd('\\') + "\">" + items[i] + "</a>");
                }
            }
            return string.Join("\\", linkedItems);
        }
    }
}
