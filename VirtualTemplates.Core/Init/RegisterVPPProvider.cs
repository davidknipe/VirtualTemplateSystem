using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System;
using System.Reflection;
using System.Web.Hosting;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Providers;

namespace VirtualTemplates.Core.Init
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class RegisterVPPProvider : IInitializableModule
    {
        private bool _registered = false;

        public void Initialize(InitializationEngine context)
        {
            if (!_registered)
            {
                this.RegisterVPPProviderAtTopOfList();
                this._registered = true;
            }
        }

        private void RegisterVPPProviderAtTopOfList()
        {
            try
            {
                //This code is required to ensure our custom provider runs first in the list, and allows the tool 
                //to intercept requests for files on disk and replace them with a view saved on disk.
                //Note: Requires FULL trust ASP.net permissions

                //Get static hosting environment instance
                HostingEnvironment hostingEnvironmentInstance = (HostingEnvironment)typeof(HostingEnvironment).InvokeMember("_theHostingEnvironment", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
                if (hostingEnvironmentInstance == null)
                    return;

                //Get Virtual path provider private field
                FieldInfo fi = typeof(HostingEnvironment).GetField("_virtualPathProvider", BindingFlags.NonPublic | BindingFlags.Instance);
                VirtualPathProvider currentVPP = (VirtualPathProvider)fi.GetValue(hostingEnvironmentInstance);

                var customVPPProvider = new VirtualTemplatesVirtualPathProvider();
                MethodInfo mi = typeof(VirtualPathProvider).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(VirtualPathProvider) }, null);
                if (mi == null)
                    return;

                //Ensure we remove the reference to the current VPP so we can replace it with our new customer VPP provider
                fi.SetValue(hostingEnvironmentInstance, null);

                //Initialise the new VPP provider setting the currentVPP up as the Previous
                mi.Invoke(customVPPProvider, new object[] { currentVPP });

                //Finally replace the top level provider to ensure it receives requests for each and every file
                fi.SetValue(hostingEnvironmentInstance, customVPPProvider);

                VirtualTemplatesCache.Reset();
            }
            catch (Exception ex)
            {
                //TODO: Log an init errors
            }
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}