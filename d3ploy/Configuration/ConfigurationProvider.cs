using System.Configuration;
using System.Web.Hosting;

namespace d3ploy.Configuration
{
	public class ConfigurationProvider : IConfigurationProvider
	{
		public string AssetsPath { get; private set; }

		public ConfigurationProvider()
		{
			var configuredPath = ConfigurationManager.AppSettings["AssetFolderPath"];

			configuredPath = configuredPath ?? "~/App_Data";

			if (configuredPath.StartsWith("~"))
			{
				AssetsPath = HostingEnvironment.MapPath(configuredPath);
			}
			else
			{
				AssetsPath = configuredPath;
			}
		}
	}
}