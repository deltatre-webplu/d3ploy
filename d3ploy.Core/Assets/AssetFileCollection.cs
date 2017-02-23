using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace d3ploy.Assets
{
	public class AssetFileCollection : Collection<AssetFile>
	{
		public AssetFileCollection()
		{
		}

		public AssetFileCollection(IEnumerable<AssetFile> files) : base(files.ToList())
		{
		}
	}
}
