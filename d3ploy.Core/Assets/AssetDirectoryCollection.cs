using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace d3ploy.Assets
{
	public class AssetDirectoryCollection : Collection<AssetDirectory>
	{
		public AssetDirectoryCollection()
		{
		}

		public AssetDirectoryCollection(IEnumerable<AssetDirectory> files) : base(files.ToList())
		{
		}
	}
}
