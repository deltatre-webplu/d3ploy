using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using d3ploy.Hubs;

namespace d3ploy.Windsor
{
	public class HubsInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<D3PloyHub>());
		}
	}
}