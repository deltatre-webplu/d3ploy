using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using d3ploy.Assets;
using Newtonsoft.Json;

namespace d3ploy.Api
{
	[Authorize]
	[RoutePrefix("api")]
	public class AssetsController : ApiController
	{
		private readonly IAssetsFolderRepository _folderRepository;

		public AssetsController(IAssetsFolderRepository folderRepository)
		{
			_folderRepository = folderRepository;
		}

		[HttpGet, Route("assets")]
		public AssetDirectory Get()
		{
			return _folderRepository.GetRoot();
		}

		[HttpGet, Route("asset")]
		public HttpResponseMessage Get([FromUri] string path)
		{
			var response = new HttpResponseMessage { Content = new StringContent(_folderRepository.GetFileContent(path)) };
			return response;
		}

		[HttpPut, Route("asset")]
		public AssetFile SaveFile([FromUri] string path)
		{
			var fileContent = Request.Content.ReadAsStringAsync().Result;
			return _folderRepository.SaveFile(path, fileContent);
		}

		[HttpPost, Route("asset")]
		public Asset New([FromUri] string path, [FromUri] AssetType type = AssetType.Script)
		{
			switch (type)
			{
				case AssetType.Directory:
					return _folderRepository.NewDirectory(path);
				case AssetType.Script:
					return _folderRepository.NewFile(path);
				default:
					throw new ArgumentException("type");
			}
		}

		[HttpDelete, Route("asset")]
		public void Delete([FromUri] string path)
		{
			_folderRepository.DeleteItem(path);
		}
	}
}