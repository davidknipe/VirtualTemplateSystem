using EPiServer.Framework.Localization;
using System.Web.Mvc;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.UI.Filter;
using VirtualTemplates.UI.Interfaces;
using VirtualTemplates.UI.Models;

namespace VirtualTemplates.UI.Controllers
{
    [AddXssHeader]
    [AuthoriseUi]
    public class VirtualTemplatesController : Controller
    {
        private readonly IVirtualTemplateRepository _viewPersistenceService;
        private readonly LocalizationService _localizationService;
        private readonly IUiTemplateLister _uITemplateLister;
        private readonly IPhysicalFileReader _fileReader;

        public VirtualTemplatesController(
              IVirtualTemplateRepository viewPersistenceService
            , LocalizationService localizationService
            , IUiTemplateLister uITemplateLister
            , IPhysicalFileReader physicalFileReader)
        {
            _viewPersistenceService = viewPersistenceService;
            _localizationService = localizationService;
            _uITemplateLister = uITemplateLister;
            _fileReader = physicalFileReader;
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
            return View("Index",
                SaveTemplate(ShowAllTemplates, VirtualPath,
                    _fileReader.ReadFile(Server.MapPath("~" + VirtualPath))));
        }

        public ActionResult Display(string VirtualPath, bool ShowAllTemplates)
        {
            var isVirtual = _viewPersistenceService.Exists(VirtualPath);
            return View(
                new VirtualTemplateItemModel()
                {
                    IsVirtual = _viewPersistenceService.Exists(VirtualPath),
                    VirtualPath = VirtualPath,
                    TemplateContents =
                        isVirtual
                            ? _viewPersistenceService.GetTemplate(VirtualPath).FileContents
                            : _fileReader.ReadFile(Server.MapPath("~" + VirtualPath)),
                    ShowAllTemplates =  ShowAllTemplates
                });
        }

        public ActionResult Edit(string VirtualPath, bool ShowAllTemplates)
        {
            return View(
                new VirtualTemplateItemModel()
                {
                    IsVirtual = true,
                    VirtualPath = VirtualPath,
                    TemplateContents = _viewPersistenceService.GetTemplate(VirtualPath).FileContents,
                    ShowAllTemplates = ShowAllTemplates
                });
        }

        [HttpPost]
        public ActionResult Edit(VirtualTemplateItemModel model)
        {
            var viewModel = SaveTemplate(false, model.VirtualPath, model.TemplateContents);
            if (model.Button == "Save and close")
            {
                return View("Index", viewModel);
            }
            else
            {
                model.ConfirmMessage =
                    string.Format(
                        _localizationService.GetString("/virtualtemplatesystem/messages/saveconfirm",
                            "Template: <strong>{0}</strong> successfully saved"), model.VirtualPath);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Compare(VirtualTemplatesCompareModel model)
        {
            var viewModel = SaveTemplate(false, model.VirtualPath, model.TemplateContents);
            if (model.Button == "Save and close")
            {
                return View("Index", viewModel);
            }
            else
            {
                model.ConfirmMessage =
                    string.Format(
                        _localizationService.GetString("/virtualtemplatesystem/messages/saveconfirm",
                            "Template: <strong>{0}</strong> successfully saved"), model.VirtualPath);
                return View(model);
            }
        }

        public ActionResult Compare(string VirtualPath, bool ShowAllTemplates)
        {
            return View(
                new VirtualTemplatesCompareModel()
                {
                    IsVirtual = true,
                    VirtualPath = VirtualPath,
                    TemplateContents = _viewPersistenceService.GetTemplate(VirtualPath).FileContents,
                    OriginalContents = _fileReader.ReadFile(Server.MapPath("~" + VirtualPath))
                });
        }

        public ActionResult CompareDisplay(string VirtualPath, bool ShowAllTemplates)
        {
            return View(
                new VirtualTemplatesCompareModel()
                {
                    IsVirtual = true,
                    VirtualPath = VirtualPath,
                    TemplateContents = _viewPersistenceService.GetTemplate(VirtualPath).FileContents,
                    OriginalContents = _fileReader.ReadFile(Server.MapPath("~" + VirtualPath))
                });
        }

        public ActionResult Revert(string VirtualPath, bool ShowAllTemplates)
        {
            VirtualTemplatesListViewModel viewModel;
            if (this._viewPersistenceService.RevertTemplate(VirtualPath))
            {
                viewModel = this.PopulateViewModel(ShowAllTemplates);
                var confirmMessage = _localizationService.GetString("/virtualtemplatesystem/messages/deleteconfirm",
                    "Template: <strong>{0}</strong> successfully reverted to original");
                viewModel.ConfirmMessage = string.Format(confirmMessage, VirtualPath);
            }
            else
            {
                viewModel = this.PopulateViewModel(ShowAllTemplates);
                var errorMessage = _localizationService.GetString("/virtualtemplatesystem/messages/deleteerror",
                    "Error when deleting: <strong>{0}</strong> from template repository");
                viewModel.ErrorMessage = string.Format(errorMessage, VirtualPath);
            }
            return View("Index", viewModel);
        }

        private VirtualTemplatesListViewModel PopulateViewModel(bool ShowAllTemplates)
        {
            var viewModel = new VirtualTemplatesListViewModel();
            viewModel.ShowAllTemplates = false;
            viewModel.TemplateList = _uITemplateLister.GetViewList(ShowAllTemplates);
            return viewModel;
        }

        private VirtualTemplatesListViewModel SaveTemplate(bool ShowAllTemplates, string VirtualPath, string FileContents)
        {
            VirtualTemplatesListViewModel viewModel;
            if (this._viewPersistenceService.SaveTemplate(VirtualPath, FileContents))
            {
                viewModel = PopulateViewModel(ShowAllTemplates);
                viewModel.LastActionPath = VirtualPath;
                var confirmMessage = _localizationService.GetString("/virtualtemplatesystem/messages/saveconfirm",
                    "Template: <strong>{0}</strong> successfully saved");
                viewModel.ConfirmMessage = string.Format(confirmMessage, VirtualPath);
            }
            else
            {
                viewModel = PopulateViewModel(ShowAllTemplates);
                var errorMessage = _localizationService.GetString("/virtualtemplatesystem/messages/saveerror",
                    "Error when saving: <strong>{0}</strong> into template repository");
                viewModel.ErrorMessage = string.Format(errorMessage, VirtualPath);

            }
            return viewModel;
        }
    }
}
