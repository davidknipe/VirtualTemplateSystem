using EPiServer.Framework.Localization;
using EPiServer.Security;
using EPiServer.Shell.Navigation;
using System.Collections.Generic;
using System.Web.Routing;

namespace VirtualTemplates.UI
{
    [MenuProvider]
    public class VirtualTemplatesMenuProvider : IMenuProvider
    {
        private readonly LocalizationService _localizationService;

        public VirtualTemplatesMenuProvider(LocalizationService localizationService)
        {
            this._localizationService = localizationService;
        }

        public static string RootUiUrl
        {
            get
            {
                var uiUrl = EPiServer.Configuration.Settings.Instance.UIUrl.OriginalString;
                uiUrl = uiUrl.TrimStart('~').TrimStart('/').TrimEnd('/');
                uiUrl += "/VTS/";
                return uiUrl;
            } 
        }

        public IEnumerable<MenuItem> GetMenuItems()
        {
            var menuPath = "/global/cms/templates";

            // Register menu items for each controller action but with the same path 
            // key to ensure the menu displays in the new Episerver UI
            var menuList =
                new UrlMenuItem(
                    _localizationService.GetString("/virtualtemplatesystem/menus/menuitem", "Templates"),
                    menuPath, 
                    "/" + RootUiUrl + "VirtualTemplates")
            {
                IsAvailable = (request) => IsTemplateEditor,
                SortIndex = int.MaxValue
            };

            var menuDisplay =
                new UrlMenuItem(
                    _localizationService.GetString("/virtualtemplatesystem/menus/menuitem", "Templates"),
                    menuPath, 
                    "/" + RootUiUrl + "VirtualTemplates/Display")
                {
                    IsAvailable = (request) => false,
                    SortIndex = int.MaxValue
                };

            var menuEdit =
                new UrlMenuItem(
                    _localizationService.GetString("/virtualtemplatesystem/menus/menuitem", "Templates"),
                    menuPath, 
                    "/" + RootUiUrl + "VirtualTemplates/Edit")
                {
                    IsAvailable = (request) => false,
                    SortIndex = int.MaxValue
                };

            var menuCompare =
                new UrlMenuItem(
                    _localizationService.GetString("/virtualtemplatesystem/menus/menuitem", "Templates"),
                    menuPath, 
                    "/" + RootUiUrl + "VirtualTemplates/Compare")
                {
                    IsAvailable = (request) => false,
                    SortIndex = int.MaxValue
                };

            var menuRevert =
                new UrlMenuItem(
                    _localizationService.GetString("/virtualtemplatesystem/menus/menuitem", "Templates"),
                    menuPath, 
                    "/" + RootUiUrl + "VirtualTemplates/Revert")
                {
                    IsAvailable = (request) => false,
                    SortIndex = int.MaxValue
                };

            var list = new List<MenuItem>();
            if (this.IsTemplateEditor)
            {
                list.Add(menuList);
                list.Add(menuDisplay);
                list.Add(menuEdit);
                list.Add(menuCompare);
                list.Add(menuRevert);
            }
            return list;
        }

        private bool IsTemplateEditor => PrincipalInfo.HasAdminAccess || PrincipalInfo.Current.HasPathAccess(RootUiUrl);
    }
}
