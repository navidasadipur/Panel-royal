using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpadCompanyPanel.Infrastructure.Repositories;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Core.Utility;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;

namespace SpadCompanyPanel.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductGalleriesRepository _productGalleryRepo;
        //private readonly GalleryVideosRepository _galleryVideosRepo;
        private readonly ProductsRepository _productRepo;
        private readonly ContactFormsRepository _contactFormRepo;
        private readonly ProductGroupsRepository _prodectGroupsRepo;
        private readonly StaticContentDetailsRepository _staticContentDetailsRepo;
        private readonly OurTeamRepository _ourTeamRepo;

        public HomeController(
            ProductGalleriesRepository productGalleryRepo,
            ProductsRepository productRepo,
            ContactFormsRepository contactFormRepo,
            //GalleryVideosRepository galleryVideosRepo,
            ProductGroupsRepository productGroupRepo,
            Product product,
            StaticContentDetailsRepository staticContentDetailsRepo,
            OurTeamRepository ourTeamRepo
            
            )
        {
            _productGalleryRepo = productGalleryRepo;
            _productRepo = productRepo;
            _contactFormRepo = contactFormRepo;
            //_galleryVideosRepo = galleryVideosRepo;
            this._prodectGroupsRepo = productGroupRepo;
            this._staticContentDetailsRepo = staticContentDetailsRepo;
            this._ourTeamRepo = ourTeamRepo;
        }
        public ActionResult Index()
        {
            //return Redirect("/Admin/Dashboard");
            //ViewBag.Instagram = _contentDetailsRepo.GetContentByTypeId(3);

            return View();
        }
        public ActionResult Navbar()
        {
            //ViewBag.Phone = _contentDetailsRepo.GetStaticContentDetail((int) StaticContents.Phone).ShortDescription;

            var viewModel = new NavbarViewModel
            {
                ProductCategories = _prodectGroupsRepo.GetAllProductGroupsWithProducts()
            };

            return PartialView(viewModel);
        }

        public ActionResult Shop(int? id)
        {
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage);

            var viewModel = new ProductViewModel();

            if (id == null)
            {
                viewModel.Products = _productRepo.GetAllProducts();

                return PartialView(viewModel);
            }

            viewModel.Products = _productRepo.getProductsByGroupId(id.Value);

            ViewBag.CategoryTitle = _prodectGroupsRepo.Get(id.Value).Title;

            return PartialView(viewModel);
        }

        public ActionResult ContactUs()
        {
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage).Image;

            var contactUsContent = new ContactUsViewModel();

            contactUsContent.ContactInfo = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.ContactInfo);
            contactUsContent.Email = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Email);
            contactUsContent.Address = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Address);
            contactUsContent.WorkingHours = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.WorkingHours);
            contactUsContent.Phone = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Phone);
            //contactUsContent.Youtube = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Youtube);
            contactUsContent.Instagram = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Instagram);
            contactUsContent.Twitter = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Twitter);
            contactUsContent.Pinterest = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Pinterest);
            contactUsContent.Facebook = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Facebook);
            contactUsContent.Map = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Map);

            return View(contactUsContent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsViewModel model)
        {
            var customerContactForm = new ContactForm()
            {
                Name = model.Name,
                Email = model.CustomerEmail,
                Message = model.Message,
            };

            if (ModelState.IsValid)
            {
                _contactFormRepo.Add(customerContactForm);
                return RedirectToAction("ContactUsSummary");
            }

            return RedirectToAction("ContactUs");
        }

        public ActionResult ContactUsSummary()
        {
            return View();
        }


        public ActionResult Footer()
        {
            var footerContent = new FooterViewModel()
            {
                Address = _staticContentDetailsRepo.Get((int)StaticContents.Address),
                Phone = _staticContentDetailsRepo.Get((int)StaticContents.Phone),
                //Youtube = _staticContentDetailsRepo.Get((int)StaticContents.Youtube),
                Instagram = _staticContentDetailsRepo.Get((int)StaticContents.Instagram),
                Twitter = _staticContentDetailsRepo.Get((int)StaticContents.Twitter),
                Pinterest = _staticContentDetailsRepo.Get((int)StaticContents.Pinterest),
                Facebook = _staticContentDetailsRepo.Get((int)StaticContents.Facebook),
                CopyRightShortDescrioption = _staticContentDetailsRepo.Get((int)StaticContents.CopyRight),
                ImplementationShortDescription = _staticContentDetailsRepo.Get((int)StaticContents.ImplementaitonService),
                CompanyServices = _staticContentDetailsRepo.Get((int)StaticContents.companyServices)
            };

            return PartialView(footerContent);
        }

        [Route("Gallery")]
        public ActionResult GalleryPage()
        {
            var images = _productGalleryRepo.GetAll();
            
            var vm = new GalleryPageViewModel()
            {
                Images = images,
                //Videos = videos
            };
            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage).Image;

            var aboutViewModel = new AboutViewModel();

            return PartialView(aboutViewModel);
        }

        public ActionResult OurTeamsSection()
        {
            var ourTeam = _ourTeamRepo.GetAll();

            return PartialView(ourTeam);
        }

        public ActionResult InstaGalleriesSection()
        {
            var model = new InstaGalleryViewModel()
            {
                Images = _staticContentDetailsRepo.GetStaticContentDetailsByStaticContentTypeId((int)StaticContentTypes.InstagramImages)
            };

            return PartialView(model);
        }

        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var vFileName = DateTime.Now.ToString("yyyyMMdd-HHMMssff") +
                                    Path.GetExtension(upload.FileName).ToLower();
                    var vFolderPath = Server.MapPath("/Upload/");
                    if (!Directory.Exists(vFolderPath))
                    {
                        Directory.CreateDirectory(vFolderPath);
                    }
                    vFilePath = Path.Combine(vFolderPath, vFileName);
                    upload.SaveAs(vFilePath);
                    vImagePath = Url.Content("/Upload/" + vFileName);
                    vMessage = "Image was saved correctly";
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(vOutput);
        }

        public ActionResult Account()
        {
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage).Image;

            return View();
        }

        public ActionResult AccountLoginSection()
        {
            var model = new LoginViewModel();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AccountLoginSection(LoginViewModel model)
        {


            return RedirectToAction("Account");
        }

        public ActionResult AccountRegisterSection()
        {
            var model = new RegisterViewModel();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AccounRegisterSection(RegisterViewModel model)
        {
            

            return RedirectToAction("Account");
        }

        public ActionResult ServicesSection()
        {
            var model = _staticContentDetailsRepo.GetStaticContentDetailsByStaticContentTypeId((int)StaticContentTypes.OurServices);

            return PartialView(model);
        }

        public ActionResult HomeTopSlidersSection()
        {
            var model = new HomeTopSliderViewModel();

            model.Slides = _staticContentDetailsRepo.GetStaticContentDetailsByStaticContentTypeId((int)StaticContentTypes.HomeTopSlider);

            model.LogoAndButton = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Logo);

            return PartialView(model);
        }

        public ActionResult HomeSecondSlidersSection()
        {
            var model = _staticContentDetailsRepo.GetStaticContentDetailsByStaticContentTypeId((int)StaticContentTypes.HomeSecondSlider);

            return PartialView(model);
        }

        public ActionResult HomeLastSlidersSection()
        {
            var model = _staticContentDetailsRepo.GetStaticContentDetailsByStaticContentTypeId((int)StaticContentTypes.HomeLastSlider);

            return PartialView(model);
        }

        public ActionResult LatestProductsSection()
        {
            var model = _productRepo.Get6NewProducts();

            return PartialView(model);
        }
    }
}