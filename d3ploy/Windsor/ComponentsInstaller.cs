using System.IO.Abstractions;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using d3ploy.Assets;
using d3ploy.Configuration;
using d3ploy.PSManagement;

namespace d3ploy.Windsor
{
	public class ComponentsInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IAssetsFolderRepository>().ImplementedBy<AssetsFolderRepository>());
			container.Register(Component.For<IConfigurationProvider>().ImplementedBy<ConfigurationProvider>());
			container.Register(Component.For<D3PloySessionManager>());
			container.Register(Component.For<IFileSystem>().ImplementedBy<FileSystem>());
		}
	}
}