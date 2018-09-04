using System.Drawing.Text;
using EPiServer.Framework.Localization;
using System.Web.Mvc;
using EPiServer.Core;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.Core.Models;
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
        private readonly IVirtualTemplateVersionRepository _versionRepository;

        public VirtualTemplatesController(
            IVirtualTemplateRepository viewPersistenceService
            , LocalizationService localizationService
            , IUiTemplateLister uITemplateLister
            , IPhysicalFileReader physicalFileReader
            , IVirtualTemplateVersionRepository versionRepository)
        {
            _viewPersistenceService = viewPersistenceService;
            _localizationService = localizationService;
            _uITemplateLister = uITemplateLister;
            _fileReader = physicalFileReader;
            _versionRepository = versionRepository;
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
            var result = SaveTemplate(ShowAllTemplates, VirtualPath,
                _fileReader.ReadFile(Server.MapPath("~" + VirtualPath)));

            var editModel = new { VirtualPath = VirtualPath, ShowAllTemplates = ShowAllTemplates};
            return RedirectToAction("Edit", editModel);
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
                return RedirectToAction("List", new { ShowAllTemplates = model.ShowAllTemplates });
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

        private VirtualTemplatesCompareModel GetCompareModel(string virtualPath, bool showAllTemplates, string leftVersion, string rightVersion, bool populateHistory)
        {
            var model = new VirtualTemplatesCompareModel()
            {
                VirtualPath = virtualPath,
                ShowAllTemplates = showAllTemplates
            };

            // Set up references for content
            ContentReference leftReference;
            ContentReference rightReference = ContentReference.EmptyReference;

            if (!string.IsNullOrEmpty(leftVersion))
            {
                leftReference = ContentReference.Parse(leftVersion);
                ContentReference.TryParse(rightVersion, out rightReference);
            }
            else
            {
                leftReference = _viewPersistenceService.GetContentReference(virtualPath);
            }

            // Left panel content
            var leftTemplateContent = _versionRepository.GetVersion(leftReference);
            model.LeftContents = leftTemplateContent.FileContents;
            model.LeftVersionText = leftTemplateContent.StatusText + ": " +
                                    leftTemplateContent.ChangedDate.ToString("dd-MMM-yy, hh:mm") + " by " +
                                    leftTemplateContent.ChangedBy;

            // Right panel content
            if (rightReference == null || rightReference == ContentReference.EmptyReference)
            {
                // Comparing original deployed version
                model.RightContents = _fileReader.ReadFile(Server.MapPath("~" + leftTemplateContent.VirtualPath));
                model.RightVersionText = "Original version";
            }
            else
            {
                var rightTemplateContent = _versionRepository.GetVersion(rightReference);
                model.RightContents = rightTemplateContent.FileContents;
                model.RightVersionText = rightTemplateContent.StatusText + ": " +
                                         rightTemplateContent.ChangedDate.ToString("dd-MMM-yy, hh:mm") + " by " +
                                         rightTemplateContent.ChangedBy;
            }

            if (populateHistory)
            {
                var allVersions = _versionRepository.GetAllVersions(leftReference, virtualPath);
                allVersions.Add(new UiTemplateVersion()
                {
                    ChangedBy = "-",
                    VirtualPath = virtualPath,
                    Reference = leftReference,
                    Status = VersionStatus.Published,
                    StatusText = "Original version"
                });
                model.Versions = allVersions;
            }
            return model;
        }

        public ActionResult Compare(string VirtualPath, bool ShowAllTemplates, string leftVersion, string rightVersion)
        {
            return View(GetCompareModel(VirtualPath, ShowAllTemplates, leftVersion, rightVersion, true));
        }


        [HttpPost]
        public ActionResult Compare(VirtualTemplatesCompareModel model)
        {
            var viewModel = SaveTemplate(false, model.VirtualPath, model.LeftContents);
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

        public ActionResult CompareDisplay(string VirtualPath, bool ShowAllTemplates, string leftVersion, string rightVersion)
        {
            return View(GetCompareModel(VirtualPath, ShowAllTemplates, leftVersion, rightVersion, false));
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
