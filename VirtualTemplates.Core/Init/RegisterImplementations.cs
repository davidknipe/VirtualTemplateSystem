using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Init
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class RegisterImplementations : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.StructureMap().Configure(x => { x.For<ITemplatePersistenceService>().Use<TemplatePersistenceService>(); });
            context.StructureMap().Configure(x => { x.For<IPhysicalFileLister>().Use<PhysicalFileLister>(); });
        }

        public void Initialize(InitializationEngine context) { }
        public void Uninitialize(InitializationEngine context) { }
        public void Preload(string[] parameters) 
        {
        }
    }
}