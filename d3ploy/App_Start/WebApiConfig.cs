using System.Web.Http;

namespace d3ploy
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
		}
	}
}