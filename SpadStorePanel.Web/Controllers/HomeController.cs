using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpadCompanyPanel.Infrastructure.Repositories;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Core.Utility;
using SpadStorePanel.Infrastructure.Dtos.Product;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Infrastructure.Services;
using SpadStorePanel.Infratructure.Repositories;
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
        private readonly ProductService _productService;
        private readonly EmailSubscriptionRepository _emailSubscriptionRepo;
        private readonly OffersRepository _offersRepo;

        public HomeController(
            ProductGalleriesRepository productGalleryRepo,
            ProductsRepository productRepo,
            ContactFormsRepository contactFormRepo,
            //GalleryVideosRepository galleryVideosRepo,
            ProductGroupsRepository productGroupRepo,
            Product product,
            StaticContentDetailsRepository staticContentDetailsRepo,
            OurTeamRepository ourTeamRepo,
            ProductService productService,
            EmailSubscriptionRepository emailSubscriptionRepo,
            OffersRepository offersRepo
            )
        {
            _productGalleryRepo = productGalleryRepo;
            _productRepo = productRepo;
            _contactFormRepo = contactFormRepo;
            //_galleryVideosRepo = galleryVideosRepo;
            this._prodectGroupsRepo = productGroupRepo;
            this._staticContentDetailsRepo = staticContentDetailsRepo;
            this._ourTeamRepo = ourTeamRepo;
            this._productService = productService;
            this._emailSubscriptionRepo = emailSubscriptionRepo;
            this._offersRepo = offersRepo;
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

        public ActionResult About()
        {
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage).Image;


            var about = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.About);



            return PartialView(about);
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

        public ActionResult FolowOurInsta()
        {
            var model = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Instagram);

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
            var model = new List<Offer>();

            model.Add(_offersRepo.Get(1));
            model.Add(_offersRepo.Get(2));

            return PartialView(model);
        }

        public ActionResult HomeLastSlidersSection()
        {
            var model = new List<Offer>();

            model.Add(_offersRepo.Get(3));
            model.Add(_offersRepo.Get(4));
            model.Add(_offersRepo.Get(5));

            return PartialView(model);
        }

        public ActionResult LatestProductsSection()
        {
            var model = new List<ProductWithPriceDto>();

            var products = _productRepo.Get6NewProducts();

            foreach (var item in products)
            {
                //ImageResizer image = new ImageResizer(850, 400, true);

                var vm = _productService.CreateProductWithPriceDto(item.Id);

                vm.Rate = item.Rate;

                model.Add(vm);
            }

            return PartialView(model);
        }

        [Route("Cart")]
        [Route("Cart/{id}")]
        public ActionResult Cart(int? customerId)
        {
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage).Image;

            try
            {
                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                return View(cartModel);

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;


                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                return View(cartModel);

            }
        }

        public ActionResult CartTable()
        {
            var cartModel = new CartModel();

            HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

            if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
            {
                string cartJsonStr = cartCookie.Values["cart"];
                cartModel = new CartModel(cartJsonStr);
            }

            ViewBag.Phone = _staticContentDetailsRepo.Get((int)StaticContents.Phone);

            return PartialView(cartModel);
        }

        public ActionResult EmailSubscriptionSection()
        {
            var model = new EmailSubscriptionViewModel();

            model.DiscountDetails = _staticContentDetailsRepo.Get((int)StaticContents.DiscountNews);

            return PartialView(model);
        }

        [HttpPost]
        public string EmailSubscriptionSection(string email)
        {
            try
            {
                var contactform = new ContactForm();

                contactform.Email = email;
                contactform.Name = "فرم دریافت خبرنامه";
                contactform.Message = "دریافت خبرنامه";
                contactform.Phone = "_";

                contactform.IsDeleted = false;
                contactform.InsertUser = "_";

                _contactFormRepo.Add(contactform);
                return "success";
            }
            catch
            {
                return "fail";
            }

        }
    }
}