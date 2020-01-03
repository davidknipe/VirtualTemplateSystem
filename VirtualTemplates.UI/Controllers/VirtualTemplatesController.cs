using EPiServer.Framework.Localization;
using System.Web.Mvc;
using EPiServer.Core;
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
        private readonly IFileSearcher _templateSearcher;
        private readonly IProfileHelper _profileHelper;
        private ITemplateComparer _templateComparer;

        private const string ProfileKeyShowAllTemplates = "vts:ShowAllTemplates";
        private const string ProfileKeyLastSearch = "vts:LastSearch";
        private const string ProfileSearchFileNamesOnly = "vts:SearchFileNamesOnly";

        public VirtualTemplatesController(
            IVirtualTemplateRepository viewPersistenceService
            , LocalizationService localizationService
            , IUiTemplateLister uITemplateLister
            , IPhysicalFileReader physicalFileReader
            , IVirtualTemplateVersionRepository versionRepository
            , IFileSearcher templateSearcher
            , IProfileHelper profileHelper, 
            ITemplateComparer templateComparer)
        {
            _viewPersistenceService = viewPersistenceService;
            _localizationService = localizationService;
            _uITemplateLister = uITemplateLister;
            _fileReader = physicalFileReader;
            _versionRepository = versionRepository;
            _templateSearcher = templateSearcher;
            _profileHelper = profileHelper;
            _templateComparer = templateComparer;
        }

        public ActionResult Index(bool? ShowAllTemplates)
        {
            if (ShowAllTemplates.HasValue)
            {
                _profileHelper.SetProfileValue<bool>(User.Identity.Name, ProfileKeyShowAllTemplates, ShowAllTemplates.Value);
            }
            return View("Index", this.PopulateViewModel());
        }

        private void SetShowAllTemplatesValue(bool ShowAllTemplates)
        {
        }

        [AuthoriseVirtualPath]        
        public ActionResult MakeVirtual(string VirtualPath)
        {
            var result = SaveTemplate(VirtualPath,
                _fileReader.ReadFile(Server.MapPath("~" + VirtualPath)));

            var editModel = new { VirtualPath = VirtualPath };
            return RedirectToAction("Edit", editModel);
        }

        [AuthoriseVirtualPath]        
        public ActionResult Display(string VirtualPath)
        {
            var isVirtual = _viewPersistenceService.Exists(VirtualPath);
            var templateContents = isVirtual
                ? _viewPersistenceService.GetTemplate(VirtualPath).FileContents
                : _fileReader.ReadFile(Server.MapPath("~" + VirtualPath));
            return View(
                new VirtualTemplateItemModel()
                {
                    IsVirtual = _viewPersistenceService.Exists(VirtualPath),
                    VirtualPath = VirtualPath,
                    TemplateContents = templateContents,
                    TemplateIsChanged = _templateComparer.TemplateIsChanged(templateContents, VirtualPath)
                });
        }

        [AuthoriseVirtualPath]        
        public ActionResult Edit(string VirtualPath)
        {
            var templateContents = _viewPersistenceService.GetTemplate(VirtualPath).FileContents;
            return View(
                new VirtualTemplateItemModel()
                {
                    IsVirtual = true,
                    VirtualPath = VirtualPath,
                    TemplateContents = templateContents,
                    TemplateIsChanged = _templateComparer.TemplateIsChanged(templateContents, VirtualPath)
                });
        }

        [HttpPost]
        public ActionResult Edit(VirtualTemplateItemModel model)
        {
            var viewModel = SaveTemplate(model.VirtualPath, model.TemplateContents);
            if (model.Button == "Save and close")
            {
                return RedirectToAction("Index");
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
        public JsonResult SaveTemplateContents(VirtualTemplateItemModel model)
        {
            var viewModel = SaveTemplate(model.VirtualPath, model.TemplateContents);
            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
            {
                return Json(new { success = false, message = viewModel.ErrorMessage });
            }
            return Json(new { success = true, message = viewModel.ConfirmMessage });
        }

        [AuthoriseVirtualPath]        
        public ActionResult Compare(string VirtualPath, string leftVersion, string rightVersion)
        {
            return View(GetCompareModel(VirtualPath, leftVersion, rightVersion, true));
        }


        [HttpPost]
        public ActionResult Compare(VirtualTemplatesCompareModel model)
        {
            var viewModel = SaveTemplate(model.VirtualPath, model.LeftContents);
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

        [AuthoriseVirtualPath]        
        public ActionResult CompareDisplay(string VirtualPath, string leftVersion, string rightVersion)
        {
            return View(GetCompareModel(VirtualPath, leftVersion, rightVersion, false));
        }

        public ActionResult Revert(string VirtualPath)
        {
            VirtualTemplatesListViewModel viewModel;
            if (this._viewPersistenceService.RevertTemplate(VirtualPath))
            {
                viewModel = this.PopulateViewModel();
                var confirmMessage = _localizationService.GetString("/virtualtemplatesystem/messages/deleteconfirm",
                    "Template: <strong>{0}</strong> successfully reverted to original");
                viewModel.ConfirmMessage = string.Format(confirmMessage, VirtualPath);
            }
            else
            {
                viewModel = this.PopulateViewModel();
                var errorMessage = _localizationService.GetString("/virtualtemplatesystem/messages/deleteerror",
                    "Error when deleting: <strong>{0}</strong> from template repository");
                viewModel.ErrorMessage = string.Format(errorMessage, VirtualPath);
            }
            return View("Index", viewModel);
        }

        public JsonResult SearchFiles(SearchViewModel search)
        {
            _profileHelper.SetProfileValue<string>(User.Identity.Name, ProfileKeyLastSearch, search.searchString);
            _profileHelper.SetProfileValue<bool>(User.Identity.Name, ProfileSearchFileNamesOnly, search.searchFileNamesOnly);
            var results = _templateSearcher.SearchFiles(search.searchString, search.searchFileNamesOnly);
            return Json(new { total = results.Count, data = results }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ClearSearch()
        {
            _profileHelper.SetProfileValue<string>(User.Identity.Name, ProfileKeyLastSearch, string.Empty);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private VirtualTemplatesListViewModel PopulateViewModel()
        {
            var showAll = GetShowAllTemplatesValue();
            var viewModel = new VirtualTemplatesListViewModel();
            viewModel.ShowAllTemplates = showAll;
            viewModel.TemplateList = _uITemplateLister.GetViewList(showAll);
            viewModel.LastSearch =
                _profileHelper.GetProfileValue<string>(User.Identity.Name, ProfileKeyLastSearch, string.Empty);
            viewModel.SearchFileNamesOnly =
                _profileHelper.GetProfileValue<bool>(User.Identity.Name, ProfileSearchFileNamesOnly, false);
            return viewModel;
        }

        private VirtualTemplatesListViewModel SaveTemplate(string VirtualPath, string FileContents)
        {
            VirtualTemplatesListViewModel viewModel;
            if (this._viewPersistenceService.SaveTemplate(VirtualPath, FileContents))
            {
                viewModel = PopulateViewModel();
                viewModel.LastActionPath = VirtualPath;
                var confirmMessage = _localizationService.GetString("/virtualtemplatesystem/messages/saveconfirm",
                    "Template: <strong>{0}</strong> successfully saved");
                viewModel.ConfirmMessage = string.Format(confirmMessage, VirtualPath);
            }
            else
            {
                viewModel = PopulateViewModel();
                var errorMessage = _localizationService.GetString("/virtualtemplatesystem/messages/saveerror",
                    "Error when saving: <strong>{0}</strong> into template repository");
                viewModel.ErrorMessage = string.Format(errorMessage, VirtualPath);

            }
            return viewModel;
        }

        private VirtualTemplatesCompareModel GetCompareModel(string virtualPath, string leftVersion, string rightVersion, bool populateHistory)
        {
            var model = new VirtualTemplatesCompareModel()
            {
                VirtualPath = virtualPath
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

            model.TemplateIsChanged =
                _templateComparer.TemplateIsChanged(leftTemplateContent.FileContents, virtualPath);

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

        //private bool SetShowAllTemplatesValue(bool? ShowAllTemplates)
        //{
        //    string profileKey = "vts:ShowAllTemplates";
        //    bool showAll = false;
        //    if (ShowAllTemplates.HasValue)
        //    {
        //        var profile = _profileHelper.GetOrCreateProfile(User.Identity.Name);
        //        profile.Settings[profileKey] = ShowAllTemplates.Value;
        //        _profileHelper.Save(profile);
        //        showAll = ShowAllTemplates.Value;
        //    }

        //    object showAllObj;
        //    if (_profileHelper.GetProfile(User.Identity.Name).Settings
        //        .TryGetValue(profileKey, out showAllObj))
        //    {
        //        showAll = (bool)showAllObj;
        //    }

        //    return showAll;
        //}

        private bool GetShowAllTemplatesValue()
        {
            return _profileHelper.GetProfileValue<bool>(User.Identity.Name, ProfileKeyShowAllTemplates, false);
        }
    }
}
