using EPiServer.Framework.Localization;
using EPiServer.Security;
using EPiServer.Shell.Navigation;
using System.Collections.Generic;

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
                uiUrl += "/Virtual.Template.System/";
                return uiUrl;
            }
        }

        public IEnumerable<MenuItem> GetMenuItems()
        {
            var editMenu =
                new UrlMenuItem(_localizationService.GetString("/virtualtemplatesystem/menus/edit", "Templates"),
                    "/global/cms/templates", "/" + RootUiUrl + "VirtualTemplates")
            {
                IsAvailable = (request) => IsTemplateEditor,
                SortIndex = int.MaxValue
            };

            var list = new List<MenuItem>();
            if (this.IsTemplateEditor)
            {
                list.Add(editMenu);
            }
            return list;
        }

        private bool IsTemplateEditor => PrincipalInfo.HasAdminAccess || PrincipalInfo.Current.HasPathAccess(RootUiUrl);
    }
}
