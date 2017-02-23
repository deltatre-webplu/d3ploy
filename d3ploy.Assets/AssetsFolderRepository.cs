using System.Collections.Generic;
using d3ploy.Configuration;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace d3ploy.Assets
{
	public class AssetsFolderRepository : IAssetsFolderRepository
	{
		private readonly IFileSystem _fileSystem;
		private readonly IConfigurationProvider _configurationProvider;

		public AssetsFolderRepository(IFileSystem fileSystem, IConfigurationProvider configurationProvider)
		{
			_fileSystem = fileSystem;
			_configurationProvider = configurationProvider;
		}

		public AssetDirectory GetRoot()
		{
			var path = _configurationProvider.AssetsPath;
			var root = GetDirectory(path);

			return root;
		}

		public string GetFileContent(string filePath)
		{
			var fullPath = GetFileFullPath(filePath);

			return _fileSystem.File.ReadAllText(fullPath);
		}

		public AssetFile SaveFile(string filePath, string fileContent)
		{
			var fullPath = GetFileFullPath(filePath);

			_fileSystem.File.WriteAllText(fullPath, fileContent);

			return GetFile(fullPath);
		}

		public IEnumerable<string> GetScriptIncludes(string filePath)
		{
			var finalIncludeScript = new List<string>();

			var directories = filePath.Split('/');

			var currentDirectory = GetRoot();
			finalIncludeScript.AddRange(currentDirectory.Files.Where(IsLibrary).Select(file => file.Name));

			var currentDirectoryPath = "/";

			foreach (var directory in directories)
			{
				var dir = currentDirectory.Directories.FirstOrDefault(d => d.Name == directory);

				if (dir == null) continue;

				currentDirectoryPath += dir.Name + "/";

				finalIncludeScript.AddRange(dir.Files.Where(IsLibrary).Select(file => currentDirectoryPath + file.Name));

				currentDirectory = dir;
			}

			return finalIncludeScript;
		}

		public AssetDirectory NewDirectory(string path)
		{
			var localPath = ToLocalPath(path);
			Directory.CreateDirectory(localPath);

			return GetDirectory(localPath);
		}

		public AssetFile NewFile(string path)
		{
			var localPath = ToLocalPath(path);

			if (!localPath.EndsWith(".ps1", StringComparison.InvariantCultureIgnoreCase))
			{
				localPath += ".ps1";
			}

			File.Create(localPath).Dispose();

			return GetFile(localPath);
		}

		public void DeleteItem(string path)
		{
			var localPath = ToLocalPath(path);

			if (IsRoot(localPath))
			{
				throw new Exception("Root cannot be deleted");
			}

			if (Directory.Exists(localPath))
			{
				Directory.Delete(localPath, true);
			}
			else
			{
				File.Delete(localPath);
			}
		}


		public Asset GetItem(string path)
		{
			var localPath = ToLocalPath(path);

			if (Directory.Exists(localPath))
			{
				return GetDirectory(localPath);
			}

			return GetFile(localPath);
		}

		private string ToLocalPath(string path)
		{
			var fullPath = Path.Combine(_configurationProvider.AssetsPath, path.TrimStart('/').Replace("/", @"\"));
			return fullPath;
		}

		private string ToVirtualPath(string path)
		{
			var virtualPath = path.ToLowerInvariant().Replace(_configurationProvider.AssetsPath.TrimEnd('/').ToLowerInvariant(), "/").Replace(@"\", "/");
			return virtualPath;
		}

		private string GetFileFullPath(string filePath)
		{
			var fullPath = ToLocalPath(filePath);

			if (!_fileSystem.File.Exists(fullPath))
			{
				throw new FileNotFoundException(string.Format("Specified file {0} was not found", filePath));
			}
			return fullPath;
		}

		private AssetDirectoryCollection GetDirectories(string path)
		{
			var files = _fileSystem
				.Directory
				.EnumerateDirectories(path)
				.Where(IsAvailableToUser)
				.Select(GetDirectory);

			return new AssetDirectoryCollection(files);
		}

		private AssetFileCollection GetFiles(string path)
		{
			var files = _fileSystem
				.Directory
				.EnumerateFiles(path, "*.ps1")
				.Select(GetFile);

			return new AssetFileCollection(files);
		}

		private AssetDirectory GetDirectory(string path)
		{
			var name = _fileSystem.Path.GetFileName(path.TrimEnd('\\'));
			if (IsRoot(path))
			{
				name = "root";
			}
			var files = GetFiles(path);
			var directories = GetDirectories(path);
			var directory = new AssetDirectory(ToVirtualPath(path), name, files, directories);

			return directory;
		}

		private bool IsRoot(string path)
		{
			return string.Equals(path.TrimEnd('\\'), _configurationProvider.AssetsPath.TrimEnd('\\'),
				StringComparison.InvariantCultureIgnoreCase);
		}

		private AssetFile GetFile(string path)
		{
			var name = _fileSystem.Path.GetFileName(path);

			var file = new AssetFile(ToVirtualPath(path), name, name.StartsWith("_") ? AssetType.LibraryScript : AssetType.Script);

			return file;
		}

		private bool IsLibrary(AssetFile file)
		{
			return file.Type == AssetType.LibraryScript;
		}

		private bool IsAvailableToUser(string path)
		{
			try
			{
				_fileSystem.Directory.EnumerateFiles(path);

				return true;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
		}


	}
}
