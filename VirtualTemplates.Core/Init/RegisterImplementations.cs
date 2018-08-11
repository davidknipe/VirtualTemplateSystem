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
            context.StructureMap().Configure(x => { x.For<IVirtualTemplateRepository>().Use<VirtualTemplateRepository>(); });
            context.StructureMap().Configure(x => { x.For<IPhysicalFileLister>().Use<PhysicalFileLister>(); });
            context.StructureMap().Configure(x => { x.For<IVirtualTemplatesCache>().Use<VirtualTemplatesCache>().Singleton(); });
            context.StructureMap().Configure(x => { x.For<IPhysicalFileReader>().Use<PhysicalFileReader>(); });
        }

        public void Initialize(InitializationEngine context) { }
        public void Uninitialize(InitializationEngine context) { }
    }
}