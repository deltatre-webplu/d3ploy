using System.Collections.Generic;

namespace d3ploy.Assets
{
	public interface IAssetsFolderRepository
	{
		AssetDirectory GetRoot();

		string GetFileContent(string filePath);

		IEnumerable<string> GetScriptIncludes(string filePath);
		AssetFile SaveFile(string filePath, string fileContent);

		AssetDirectory NewDirectory(string path);

		AssetFile NewFile(string path);

		void DeleteItem(string path);

		Asset GetItem(string path);
	}
}
