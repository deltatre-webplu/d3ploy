namespace d3ploy.Assets
{
	public class AssetDirectory : Asset
	{
		public string Path { get; set; }
		public readonly string Name;
		public readonly AssetFileCollection Files;
		public readonly AssetDirectoryCollection Directories;

		public AssetDirectory(string path, string name, AssetFileCollection files = null, AssetDirectoryCollection directories = null)
		{
			Path = path;
			Type = AssetType.Directory;
			Name = name;
			Files = files ?? new AssetFileCollection();
			Directories = directories ?? new AssetDirectoryCollection();
		}
	}
}
