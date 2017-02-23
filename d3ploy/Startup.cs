using Castle.MicroKernel.Registration;
using d3ploy.Windsor;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Owin;

[assembly: OwinStartup(typeof(d3ploy.Startup))]

namespace d3ploy
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			StartSignalR(app);
		}

		private static void StartSignalR(IAppBuilder app)
		{
			var settings = new JsonSerializerSettings();

			// Use camel case for signalr contracts
			settings.ContractResolver = new SignalRCamelCaseContractResolver();

			// Convert enum to string
			settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

			// Register json.net setting 
			var serializer = JsonSerializer.Create(settings);
			Global.Container.Register(Component.For<JsonSerializer>().Instance(serializer));


			// Use castle windsor for hub resolver
			var resolver = new SignalRDependencyResolver(Global.Container.Kernel);

			var config = new HubConfiguration();
			config.Resolver = resolver;

			app.MapSignalR(config);
		}
	}
}