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
        private string _area = "";
        private string _rootPath = "";
        private string _startLink = "";

        private string? _fullPath;

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
                { ".java", "java" },
                { ".sql", "sql" }
        };
        Dictionary<string, string> imageFiles = new Dictionary<string, string>()
        {
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" }
        };
        Dictionary<string, string> otherFiles = new Dictionary<string, string>()
        {
                { ".zip", "application/zip" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
        };

        public void Set(string wwwrootPath, string rootPath, string controller, string action, string startLink = "Home", string area = "")
        {
            _controller = controller;
            _action = action;
            _area = area;
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
                List<FileBrowserItemModel> files = Directory.GetFiles(_fullPath).OrderBy(f => f).Select(f => new FileBrowserItemModel()
                {
                    Name = Path.GetFileName(f),
                    Folders = _startLink + "\\" + f.Substring(f.IndexOf(_rootPath) + _rootPath.Length),
                    IsFile = true
                }).ToList();
                fileBrowserViewModel.Contents?.AddRange(files);
                fileBrowserViewModel.Title = AddLinks(path ?? _startLink);
            }
            if (fileBrowserViewModel is not null)
                fileBrowserViewModel.HierarchicalDirectoryLinks = GetHierarchicalDirectoryLinks(new DirectoryInfo(_rootPath), path, _startLink);
            return fileBrowserViewModel;
        }

        private FileBrowserViewModel? GetFile(string path, bool includeLineNumbers)
        {
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
            else if (otherFiles.Keys.Contains(extension))
            {
                fileBrowserViewModel = new FileBrowserViewModel()
                {
                    FileBinaryContent = File.ReadAllBytes(path),
                    FileType = FileTypes.Other,
                    FileContentType = otherFiles[extension]
                };
                fileBrowserViewModel.Title = Path.GetFileName(path);
            }
            return fileBrowserViewModel;
        }

        private FileBrowserViewModel GetDirectories(string path)
        {
            return new FileBrowserViewModel()
            {
                Contents = Directory.GetDirectories(path).OrderBy(d => d).Select(d => new FileBrowserItemModel()
                {
                    Name = Path.GetFileName(d),
                    Folders = _startLink + "\\" + (string.IsNullOrWhiteSpace(path) ?
                    Path.GetFileName(d) :
                    d.Substring(d.IndexOf(_rootPath) + _rootPath.Length))
                }).ToList()
            };
        }

        private string AddLinks(string path)
        {
            string[] items = path.Split('\\');
            List<string> linkedItems = new List<string>();
            string linkedItem;
            string link;
            for (int i = 0; i < items.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(items[i]))
                {
                    linkedItem = "";
                    for (int j = 0; j <= i; j++)
                    {
                        linkedItem += items[j] + "\\";
                    }
                    link = "<a style=\"text-decoration: none;\" ";
                    link += "href=\"" + (string.IsNullOrWhiteSpace(_area) ? "/" : "/" + _area + "/");
                    link += _controller + "/" + _action;
                    link += "?path=" + linkedItem.TrimEnd('\\') + "\">";
                    link += items[i] + "</a>";
                    linkedItems.Add(link);
                }
            }
            return string.Join("\\", linkedItems);
        }

        private string GetHierarchicalDirectoryLinks(DirectoryInfo rootDirectory, string? path, string linkPath, byte level = 0, List<HierarchicalDirectoryModel>? hierarchicalDirectories = null)
        {
            HierarchicalDirectoryModel? lastHierarchicalDirectory;
            string ulTag;
            string extension;
            string[] pathItems;
            bool underlineLink;
            DirectoryInfo[] subDirectories = rootDirectory.GetDirectories().OrderBy(d => d.Name).ToArray();
            if (!string.IsNullOrWhiteSpace(path))
            {
                extension = Path.GetExtension(path).ToLower();
                if (!string.IsNullOrWhiteSpace(extension))
                {
                    pathItems = path.Split('\\');
                    path = "";
                    for (int i = 0; i < pathItems.Length - 1; i++)
                    {
                        path += pathItems[i] + "\\";
                    }
                    path = path.TrimEnd('\\');
                }
            }
            if (hierarchicalDirectories is null)
                hierarchicalDirectories = new List<HierarchicalDirectoryModel>();
            if (subDirectories.Length > 0)
                level++;
            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                lastHierarchicalDirectory = hierarchicalDirectories.LastOrDefault();
                ulTag = "";
                underlineLink = false;
                if (lastHierarchicalDirectory is not null)
                {
                    if (lastHierarchicalDirectory.Level < level)
                    {
                        ulTag = "<ul style=\"list-style-type: none;\">";
                        linkPath += $"\\{subDirectory.Name}";
                    }
                    else 
                    {
                        if (lastHierarchicalDirectory.Level > level)
                        {
                            ulTag = "</ul>";
                            for (int i = level; i < lastHierarchicalDirectory.Level - 1; i++)
                            {
                                ulTag += "</ul>";
                            }
                        }
                        pathItems = linkPath.Split('\\');
                        linkPath = "";
                        for (int i = 0; i < pathItems.Length - 1; i++)
                        {
                            linkPath += pathItems[i] + "\\";
                        }
                        linkPath += subDirectory.Name;
                    }
                }
                else
                {
                    linkPath += $"\\{subDirectory.Name}";
                }
                if (linkPath == path)
                    underlineLink = true;
                hierarchicalDirectories.Add(new HierarchicalDirectoryModel()
                {
                    Path = linkPath,
                    Link = $"{ulTag}<li class=\"{(underlineLink ? "currenthierarchicaldirectory" : "")}\">" +
                        $"<a style=\"{(underlineLink ? "text-decoration: underline;": "text-decoration: none;")}\" " +
                        $"href=\"{(string.IsNullOrWhiteSpace(_area) ? "/" : "/" + _area + "/")}" +
                        $"{_controller}/{_action}?path={linkPath}\">" +
                        $"{(underlineLink ? "<b>" + subDirectory.Name + "</b>" : subDirectory.Name)}</a></li>",
                    Level = level
                });
                GetHierarchicalDirectoryLinks(subDirectory, path, linkPath, level, hierarchicalDirectories);
            }
            return $"<ul class=\"hierarchicaldirectories\" style=\"list-style-type: none;\">{string.Join("", hierarchicalDirectories.Select(d => d.Link))}</ul>";
        }
    }
}
