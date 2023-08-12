using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Utilities;
using AppCoreLite.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace AppCoreLite.Managers.Bases
{
    public abstract class FileBrowserManagerBase
    {
        private readonly SessionUtil _sessionUtil;

        private string _controller = "";
        private string _action = "";
        private string _area = "";
        private string _rootPath = "";
        private string _startLink = "";
        private string? _fullPath;

        private byte? _hideAfterLevel;
        private bool _useSession;

        private string _sessionKeySuffix = "HierarchicalDirectories";
        private string _ulRootTagClass = "class=\"hierarchicaldirectories\"";
        private string _ulRootTagStyle = "style=\"list-style-type: none;\"";
        private string _aTagStyleUnderline = "style=\"text-decoration: underline;\"";
        private string _aTagStyleNone = "style=\"text-decoration: none;\"";
        private string _ulTagStyleShow = "style=\"list-style-type: none;\"";
        private string _ulTagStyleHide = "style=\"display: none;\"";
        private string _liClassCurrent = "class=\"currenthierarchicaldirectory\"";
        private string _aTagHref = "";

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
                { ".sql", "sql" },
                { ".cshtml", "html" }
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

        protected FileBrowserManagerBase(IHttpContextAccessor httpContextAccessor)
        {
            _sessionUtil = new SessionUtil(httpContextAccessor);
        }

        public void Set(string wwwrootPath, string rootPath, string controller, string action, byte? hideAfterLevel = null, bool useSession = true, string startLink = "Home", string area = "")
        {
            _controller = controller;
            _action = action;
            _area = area;
            _rootPath = $"{wwwrootPath}\\{rootPath}\\";
            _startLink = startLink;
            _aTagHref = "href=\"" + (string.IsNullOrWhiteSpace(_area) ? "/" : "/" + _area + "/") + _controller + "/" + _action + "?path";
            _hideAfterLevel = hideAfterLevel ?? 1;
            _useSession = useSession;
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
                using (StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding(1254)))
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
                    link = "<a " + _aTagStyleUnderline + " " + _aTagHref + "=" + linkedItem.TrimEnd('\\') + "\">" + items[i] + "</a>";
                    linkedItems.Add(link);
                }
            }
            return string.Join("\\", linkedItems);
        }

        private string GetHierarchicalDirectoryLinks(DirectoryInfo rootDirectory, string? path, string linkPath, byte level = 0)
        {
            string hierarchicalDirectoryLinksResult = "";
            List<HierarchicalDirectoryHtmlModel>? hierarchicalDirectoriesResult;
            string sessionKey = _useSession && !string.IsNullOrWhiteSpace(path) ? this.ToString()?.Split('.').LastOrDefault() + _sessionKeySuffix : "";
            List<HierarchicalDirectoryModel>? hierarchicalDirectories = null;
            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                hierarchicalDirectories = _sessionUtil?.GetSessionObject<List<HierarchicalDirectoryModel>>(sessionKey);
            }
            if (hierarchicalDirectories is null)
            {
                hierarchicalDirectories = new List<HierarchicalDirectoryModel>();
                UpdateHierarchicalDirectories(hierarchicalDirectories, rootDirectory, path, linkPath, level);
                if (!string.IsNullOrWhiteSpace(sessionKey))
                {
                    _sessionUtil?.SetSessionObject(sessionKey, hierarchicalDirectories);
                }
            }
            if (hierarchicalDirectories is not null)
            {
                hierarchicalDirectoriesResult = hierarchicalDirectories.Select(hd => new HierarchicalDirectoryHtmlModel()
                {
                    Path = hd.Path,
                    Level = hd.Level,
                    Name = hd.Name,
                    IsUlTagHidden = hd.Level > _hideAfterLevel,
                    IsLiTagCurrent = hd.Path == UpdatePath(path)
                }).ToList();
                UpdateHtmlHierarchicalDirectories(hierarchicalDirectoriesResult); 
                hierarchicalDirectoryLinksResult = $"<ul {_ulRootTagClass} {_ulRootTagStyle}>{string.Join("", hierarchicalDirectoriesResult.Select(d => d.Link))}</ul>";
            }
            return hierarchicalDirectoryLinksResult;
        }

        private void UpdateHierarchicalDirectories(List<HierarchicalDirectoryModel>? hierarchicalDirectories, DirectoryInfo rootDirectory, string? path, string linkPath, byte level = 0)
        {
            if (hierarchicalDirectories is null)
                return;
            HierarchicalDirectoryModel? hierarchicalDirectory;
            HierarchicalDirectoryModel? lastHierarchicalDirectory;
            DirectoryInfo[] subDirectories = rootDirectory.GetDirectories().OrderBy(d => d.Name).ToArray();
            int? lastHierarchicalDirectoryLevel;
            if (subDirectories.Length > 0)
                level++;
            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                hierarchicalDirectory = new HierarchicalDirectoryModel();
                lastHierarchicalDirectory = hierarchicalDirectories?.LastOrDefault();
                lastHierarchicalDirectoryLevel = lastHierarchicalDirectory?.Level;
                linkPath = UpdateLinkPath(linkPath, level, lastHierarchicalDirectoryLevel, subDirectory.Name);
                hierarchicalDirectory.Path = linkPath;
                hierarchicalDirectory.Level = level;
                hierarchicalDirectory.Name = subDirectory.Name;
                hierarchicalDirectories?.Add(hierarchicalDirectory);
                UpdateHierarchicalDirectories(hierarchicalDirectories, subDirectory, path, linkPath, level);
            }
        }

        private void UpdateHtmlHierarchicalDirectories(List<HierarchicalDirectoryHtmlModel>? hierarchicalDirectories)
        {
            int index, childIndex, childLastIndex;
            HierarchicalDirectoryHtmlModel? hierarchicalDirectory, previousHierarchicalDirectory, childHierarchicalDirectory;
            bool currentFound, breakLoop;
            if (hierarchicalDirectories is not null)
            {
                for (index = 0; index < hierarchicalDirectories.Count; index++)
                {
                    hierarchicalDirectory = hierarchicalDirectories[index];
                    previousHierarchicalDirectory = index > 0 ? hierarchicalDirectories[index - 1] : null;
                    hierarchicalDirectory.Link = GetLink(hierarchicalDirectory, previousHierarchicalDirectory?.Level);
                    currentFound = hierarchicalDirectory.IsLiTagCurrent;
                    if (hierarchicalDirectory.Level == _hideAfterLevel)
                    {
                        childLastIndex = index;
                        breakLoop = false;
                        for (childIndex = index + 1; childIndex < hierarchicalDirectories.Count && !breakLoop; childIndex++)
                        {
                            childHierarchicalDirectory = hierarchicalDirectories[childIndex];
                            if (childHierarchicalDirectory.Level <= hierarchicalDirectory.Level)
                            {
                                breakLoop = true;
                            }
                            else
                            {
                                if (!currentFound)
                                    currentFound = childHierarchicalDirectory.IsLiTagCurrent;
                                childLastIndex++;
                            }
                        }
                        if (currentFound)
                        {
                            for (childIndex = index + 1; childIndex <= childLastIndex; childIndex++)
                            {
                                childHierarchicalDirectory = hierarchicalDirectories[childIndex];
                                previousHierarchicalDirectory = childIndex > 0 ? hierarchicalDirectories[childIndex - 1] : null;
                                childHierarchicalDirectory.IsUlTagHidden = false;
                                childHierarchicalDirectory.Link = GetLink(childHierarchicalDirectory, previousHierarchicalDirectory?.Level);
                            }
                            index = childLastIndex;
                        }
                    }
                }
            }
        }

        private string GetLink(HierarchicalDirectoryHtmlModel hierarchicalDirectory, int? lastHierarchicalDirectoryLevel)
        {
            return $"{GetUlTag(hierarchicalDirectory.Level, lastHierarchicalDirectoryLevel, hierarchicalDirectory.IsUlTagHidden)}" +
                $"<li{(hierarchicalDirectory.IsLiTagCurrent ? " " + _liClassCurrent : "")}>" +
                $"<a {(hierarchicalDirectory.IsLiTagCurrent ? _aTagStyleUnderline : _aTagStyleNone)} {_aTagHref}={hierarchicalDirectory.Path}\">" +
                $"{(hierarchicalDirectory.IsLiTagCurrent ? "<b>" + hierarchicalDirectory.Name + "</b>" : hierarchicalDirectory.Name)}</a></li>";
        }

        private string GetUlTag(int level, int? lastHierarchicalDirectoryLevel, bool isUlTagHidden)
        {
            string ulTag = "";
            if (lastHierarchicalDirectoryLevel is not null)
            {
                if (lastHierarchicalDirectoryLevel < level)
                {
                    ulTag = $"<ul {(isUlTagHidden ? _ulTagStyleHide : _ulTagStyleShow)}>";
                }
                else if (lastHierarchicalDirectoryLevel > level)
                {
                    ulTag = "</ul>";
                    for (int i = level; i < lastHierarchicalDirectoryLevel - 1; i++)
                    {
                        ulTag += "</ul>";
                    }
                }
            }
            return ulTag;
        }

        private string UpdateLinkPath(string linkPath, int level, int? lastHierarchicalDirectoryLevel, string subDirectoryName)
        {
            string[] pathItems;
            if (lastHierarchicalDirectoryLevel is null || lastHierarchicalDirectoryLevel < level)
            {
                linkPath += $"\\{subDirectoryName}";
            }
            else
            {
                pathItems = linkPath.Split('\\');
                linkPath = "";
                for (int i = 0; i < pathItems.Length - 1; i++)
                {
                    linkPath += pathItems[i] + "\\";
                }
                linkPath += subDirectoryName;
            }
            return linkPath;
        }

        private string? UpdatePath(string? path)
        {
            string extension;
            string[] pathItems;
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
            return path;
        }
    }
}