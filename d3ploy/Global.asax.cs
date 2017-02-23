using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using d3ploy.Windsor;
using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace d3ploy
{
	public class Global : System.Web.HttpApplication
	{
		private static IWindsorContainer _container;

		public static IWindsorContainer Container { get { return _container; }}

		protected void Application_Start(object sender, EventArgs e)
		{
			_container = CreateContainer();

			GlobalConfiguration.Configure(WebApiConfig.Register);

			ConfigureJsonCamelCaseFormatters();
		}

		protected void Application_End(object sender, EventArgs e)
		{
			_container.Dispose();
		}

		private static IWindsorContainer CreateContainer()
		{
			var container = new WindsorContainer();
			var subResolver = new CollectionResolver(container.Kernel, true);
			var httpControllerActivator = new WindsorHttpControllerActivator(container);

			container.Kernel.Resolver.AddSubResolver(subResolver);

			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), httpControllerActivator);

			container.Install(FromAssembly.This());

			return container;
		}

		private static void ConfigureJsonCamelCaseFormatters()
		{
			var formatters = GlobalConfiguration.Configuration.Formatters;
			var jsonFormatter = formatters.JsonFormatter;
			var settings = jsonFormatter.SerializerSettings;
			settings.Formatting = Formatting.Indented;
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		}
	}
}