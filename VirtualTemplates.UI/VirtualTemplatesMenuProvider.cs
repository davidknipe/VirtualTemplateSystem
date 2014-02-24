using EPiServer.Framework.Localization;
using EPiServer.Security;
using EPiServer.Shell.Navigation;
using System.Collections.Generic;

namespace VirtualTemplates.UI
{
    [MenuProvider]
    public class VirtualTemplatesMenuProvider : IMenuProvider
    {
        private LocalizationService _localizationService;

        public VirtualTemplatesMenuProvider(LocalizationService localizationService)
        {
            this._localizationService = localizationService;
        }

        public static string RootUIUrl
        {
            get
            {
                var uiUrl = EPiServer.Configuration.Settings.Instance.UIUrl.OriginalString;
                uiUrl = uiUrl.TrimStart('~').TrimStart('/').TrimEnd('/');
                uiUrl += "/Virtual.Template.System/";
                return uiUrl;
            }
        }

        public IEnumerable<MenuItem> GetMenuItems()
        {
            var section = new EPiServer.Shell.Navigation.SectionMenuItem(_localizationService.GetString("/virtualtemplatesystem/menus/mainmenu", "Templates"), "/global/templates");
            section.SortIndex = 999;

            UrlMenuItem editMenu = new UrlMenuItem(_localizationService.GetString("/virtualtemplatesystem/menus/edit", "Edit"), "/global/templates/edit", "/" + RootUIUrl + "VirtualTemplates");
            editMenu.IsAvailable = (request) => IsTemplateEditor;

            List<MenuItem> list = new List<MenuItem>();
            if (this.IsTemplateEditor)
            {
                list.Add(section);
                list.Add(editMenu);
            }
            return list;
        }

        private bool IsTemplateEditor
        {
            get
            {
                return EPiServer.Security.PrincipalInfo.HasAdminAccess || PrincipalInfo.Current.HasPathAccess(RootUIUrl);
            }
        }

    }
}
