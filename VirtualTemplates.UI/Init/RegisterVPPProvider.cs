using System;
using System.Reflection;
using System.Web.Hosting;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.UI;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.Core.Providers;

namespace VirtualTemplates.UI.Init
{
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkAspNetInitialization))]
    [ModuleDependency(typeof(CmsCoreInitialization))]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    [ModuleDependency(typeof(EPiServerUIInitialization))]
    [ModuleDependency(typeof(EPiServer.Cms.Shell.InitializableModule))]
    public class RegisterVppProvider : IInitializableModule
    {
        private bool _registered;
#pragma warning disable 649
        private readonly Injected<IVirtualTemplatesCache> _virtualTemplatesCache;
#pragma warning restore 649

        /// <inheritdoc />
        public void Initialize(InitializationEngine context)
        {
            if (!_registered)
            {
                RegisterVppProviderAtTopOfList();
                _registered = true;
            }
        }

        private void RegisterVppProviderAtTopOfList()
        {
            try
            {
                //This code is required to ensure our custom provider runs first in the list, and allows the tool 
                //to intercept requests for files on disk and replace them with a view saved on disk.
                //Note: Requires FULL trust ASP.net permissions

                //Get static hosting environment instance
                var hostingEnvironmentInstance = (HostingEnvironment)typeof(HostingEnvironment).InvokeMember("_theHostingEnvironment", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
                if (hostingEnvironmentInstance == null)
                    return;

                //Get Virtual path provider private field
                var fi = typeof(HostingEnvironment).GetField("_virtualPathProvider", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi == null)
                    return;

                var currentVpp = (VirtualPathProvider)fi.GetValue(hostingEnvironmentInstance);

                var customVppProvider = new VirtualTemplatesVirtualPathProvider();
                var mi = typeof(VirtualPathProvider).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(VirtualPathProvider) }, null);
                if (mi == null)
                    return;

                //Ensure we remove the reference to the current VPP so we can replace it with our new customer VPP provider
                fi.SetValue(hostingEnvironmentInstance, null);

                //Initialise the new VPP provider setting the currentVPP up as the Previous
                mi.Invoke(customVppProvider, new object[] { currentVpp });

                //Finally replace the top level provider to ensure it receives requests for each and every file
                fi.SetValue(hostingEnvironmentInstance, customVppProvider);

                _virtualTemplatesCache.Service.Reset();
            }
            catch (Exception ex)
            {
                //TODO: Log an init errors
            }
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}