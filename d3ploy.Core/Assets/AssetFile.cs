namespace d3ploy.Assets
{
	public class AssetFile : Asset
	{
		public string Path { get; set; }
		public readonly string Name;

		public AssetFile(string path, string name, AssetType type)
		{
			Path = path;
			Type = type;
			Name = name;
		}
	}
}
