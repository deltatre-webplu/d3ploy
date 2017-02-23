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
	[RoutePrefix("api/account")]
	public class AccountController : ApiController
	{
		[HttpGet, Route()]
		public UserInfo Get()
		{
			return new UserInfo
			{
				Name = RequestContext.Principal.Identity.Name
			};
		}

		public class UserInfo
		{
			public string Name { get; set; }
		}
	}
}