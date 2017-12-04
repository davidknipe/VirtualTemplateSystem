using EPiServer.Framework.Localization;
using System.Web.Mvc;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.UI.Interfaces;
using VirtualTemplates.UI.Models;

namespace VirtualTemplates.UI.Controllers
{
    [AuthoriseUi]
    public class VirtualTemplatesController : Controller
    {
        private readonly ITemplatePersistenceService _viewPersistenceService;
        private readonly LocalizationService _localizationService;
        private readonly IUITemplateLister _uITemplateLister;

        public VirtualTemplatesController(
              ITemplatePersistenceService viewPersistenceService
            , LocalizationService localizationService
            , IUITemplateLister uITemplateLister)
        {
            _viewPersistenceService = viewPersistenceService;
            _localizationService = localizationService;
            _uITemplateLister = uITemplateLister;
        }

        public ActionResult Index()
        {
            return View(this.PopulateViewModel(false));
        }

        public ActionResult List(bool ShowAllTemplates)
        {
            return View("Index", this.PopulateViewModel(ShowAllTemplates));
        }

        public ActionResult MakeVirtual(string VirtualPath, bool ShowAllTemplates)
        {
            return View("Index", this.SaveTemplate(ShowAllTemplates, VirtualPath, System.IO.File.ReadAllBytes(this.Server.MapPath("~" + VirtualPath))));
        }

        public ActionResult Display(string VirtualPath, bool ShowAllTemplates)
        {
            var isVirtual = _viewPersistenceService.Exists(VirtualPath);
            return View(
                new VirtualTemplatesEditModel()
                {
                    IsVirtual = _viewPersistenceService.Exists(VirtualPath),
                    VirtualPath = VirtualPath,
                    TemplateContents = isVirtual ? _viewPersistenceService.GetViewFile(VirtualPath).FileContents : System.IO.File.ReadAllText(this.Server.MapPath("~" + VirtualPath))
                });
        }

        public ActionResult Edit(string VirtualPath, bool ShowAllTemplates)
        {
            return View(
                new VirtualTemplatesEditModel()
                {
                    IsVirtual = true,
                    VirtualPath = VirtualPath,
                    TemplateContents = _viewPersistenceService.GetViewFile(VirtualPath).FileContents
                });
        }

        [HttpPost]
        public ActionResult Edit(VirtualTemplatesEditModel model)
        {
            return View("Index", this.SaveTemplate(false, model.VirtualPath, System.Text.Encoding.UTF8.GetBytes(model.TemplateContents)));            
        }

        public ActionResult Delete(string VirtualPath, bool ShowAllTemplates)
        {
            VirtualTemplatesViewModel viewModel;
            if (this._viewPersistenceService.DeleteViewFile(VirtualPath))
            {
                viewModel = this.PopulateViewModel(ShowAllTemplates);
                var confirmMessage = _localizationService.GetString("/virtualtemplatesystem/messages/deleteconfirm", "Template: <strong>{0}</strong> successfully deleted repository");
                viewModel.ConfirmMessage = string.Format(confirmMessage, VirtualPath);
            }
            else
            {
                viewModel = this.PopulateViewModel(ShowAllTemplates);
                var errorMessage = _localizationService.GetString("/virtualtemplatesystem/messages/deleteerror", "Error when deleting: <strong>{0}</strong> from template repository");
                viewModel.ErrorMessage = string.Format(errorMessage, VirtualPath);
            }
            return View("Index", viewModel);
        }

        private VirtualTemplatesViewModel PopulateViewModel(bool ShowAllTemplates)
        {
            var viewModel = new VirtualTemplatesViewModel();
            viewModel.ShowAllTemplates = false;
            viewModel.TemplateList = _uITemplateLister.GetViewList(ShowAllTemplates);
            return viewModel;
        }

        private VirtualTemplatesViewModel SaveTemplate(bool ShowAllTemplates, string VirtualPath, byte[] FileContents)
        {
            VirtualTemplatesViewModel viewModel;
            if (this._viewPersistenceService.SaveViewFile(VirtualPath, FileContents))
            {
                viewModel = this.PopulateViewModel(ShowAllTemplates);
                viewModel.LastActionPath = VirtualPath;
                var confirmMessage = _localizationService.GetString("/virtualtemplatesystem/messages/saveconfirm", "Template: <strong>{0}</strong> successfully saved to template repository");
                viewModel.ConfirmMessage = string.Format(confirmMessage, VirtualPath);
            }
            else
            {
                viewModel = this.PopulateViewModel(ShowAllTemplates);
                var errorMessage = _localizationService.GetString("/virtualtemplatesystem/messages/saveerror", "Error when saving: <strong>{0}</strong> into template repository");
                viewModel.ErrorMessage = string.Format(errorMessage, VirtualPath);

            }
            return viewModel;
        }
    }
}
