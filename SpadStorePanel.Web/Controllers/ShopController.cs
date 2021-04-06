using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Core.Utility;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;

namespace SpadCompanyPanel.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProductGalleriesRepository _productGalleryRepo;
        //private readonly GalleryVideosRepository _galleryVideosRepo;
        private readonly ProductsRepository _productsRepo;
        private readonly FeaturesRepository _featuresRepo;
        private readonly SubFeaturesRepository _subFeaturesRepo;
        private readonly ProductMainFeaturesRepository _productMainFeaturesRepo;
        private readonly ProductFeatureValuesRepository _productFeatureValuesRepo;
        private readonly ContactFormsRepository _contactFormRepo;
        private readonly ProductGroupsRepository _productGroupsRepo;
        private readonly StaticContentDetailsRepository _staticContentDetailsRepo;

        public ShopController(
            ProductGalleriesRepository productGalleryRepo,
            ProductsRepository productsRepo,
            FeaturesRepository featuresRepo,
            SubFeaturesRepository subFeatureRepo,
            ProductMainFeaturesRepository productMainFeaturesRepo,
            ProductFeatureValuesRepository productFeatureValuesRepo,
            ContactFormsRepository contactFormRepo,
            //GalleryVideosRepository galleryVideosRepo,
            ProductGroupsRepository productGroupRepo,
            StaticContentDetailsRepository staticContentDetailsRepo
            )
        {
            _productGalleryRepo = productGalleryRepo;
            _productsRepo = productsRepo;
            this._featuresRepo = featuresRepo;
            this._subFeaturesRepo = subFeatureRepo;
            this._productMainFeaturesRepo = productMainFeaturesRepo;
            this._productFeatureValuesRepo = productFeatureValuesRepo;
            _contactFormRepo = contactFormRepo;
            //_galleryVideosRepo = galleryVideosRepo;
            this._productGroupsRepo = productGroupRepo;
            this._staticContentDetailsRepo = staticContentDetailsRepo;
        }

        [Route("Shop")]
        [Route("Shop/{id}")]
        public ActionResult Index(int? id)
        {
            var viewModel = new ProductViewModel();

            if (id == null)
            {
                viewModel.Products = _productsRepo.GetAllProducts();

                return PartialView(viewModel);
            }

            viewModel.Products = _productsRepo.getProductsByGroupId(id.Value);

            ViewBag.CategoryTitle = _productGroupsRepo.Get(id.Value).Title;

            return PartialView(viewModel);
        }

        [Route("Shop/Details/{id}")]
        public ActionResult Details(int id)
        {
            //_productsRepo.UpdateProductViewCount(id);

            var product = _productsRepo.GetProduct(id);

            var productDetailsVm = new ProductDetailsViewModel(product);
            var productComments = _productsRepo.GetProductComments(id);

            var productCommentsVm = new List<ProductCommentViewModel>();
            foreach (var item in productComments)
                productCommentsVm.Add(new ProductCommentViewModel(item));
            productDetailsVm.ProductComments = productCommentsVm;

            var productTags = _productsRepo.GetProductTags(id);
            productDetailsVm.Tags = productTags;

            return View(productDetailsVm);
        }

        //public ActionResult HomeSlider()
        //{
        //    var sliderContent = _contentDetailsRepo.GetContentByTypeId((int)StaticContentTypes.Slider);
        //    return PartialView(sliderContent);
        //}
        //public ActionResult Gallery()
        //{
        //    var galleryContent = _galleryRepo.GetAll();
        //    return PartialView(galleryContent);
        //}
        //public ActionResult CompanyHistory()
        //{
        //    var content = _contentDetailsRepo.GetContentByTypeId((int)StaticContentTypes.CompanyHistory).FirstOrDefault();
        //    return PartialView(content);
        //}

        //public ActionResult GallerySlider()
        //{
        //    var galleryContent = _galleryRepo.GetAll();
        //    return PartialView(galleryContent);
        //}


        //public ActionResult ContactUs()
        //{
        //    var contactUsContent = new ContactUsViewModel();
        //    contactUsContent.ContactInfo = _contentDetailsRepo.Get((int)StaticContents.ContactInfo);
        //    contactUsContent.Email = _contentDetailsRepo.Get((int)StaticContents.Email);
        //    contactUsContent.Address = _contentDetailsRepo.Get((int)StaticContents.Address);
        //    contactUsContent.Phone = _contentDetailsRepo.Get((int)StaticContents.Phone);
        //    //contactUsContent.Youtube = _contentDetailsRepo.Get((int)StaticContents.Youtube);
        //    contactUsContent.Instagram = _contentDetailsRepo.Get((int)StaticContents.Instagram);
        //    contactUsContent.Twitter = _contentDetailsRepo.Get((int)StaticContents.Twitter);
        //    contactUsContent.Pinterest = _contentDetailsRepo.Get((int)StaticContents.Pinterest);
        //    contactUsContent.Facebook = _contentDetailsRepo.Get((int)StaticContents.Facebook);
        //    contactUsContent.Map = _contentDetailsRepo.Get((int)StaticContents.Map);
        //    //return View(contactUsContent);

        //    return PartialView(contactUsContent);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsViewModel viewModel)
        {
            var customerContactForm = new ContactForm()
            {
                Name = viewModel.Name,
                Email = viewModel.CustomerEmail,
                Message = viewModel.Message,
            };

            if (ModelState.IsValid)
            {
                _contactFormRepo.Add(customerContactForm);
                return RedirectToAction("ContactUsSummary");
            }

            return View(viewModel);
        }

        public ActionResult ContactUsSummary()
        {
            return View();
        }

        [Route("Gallery")]
        public ActionResult GalleryPage()
        {
            var images = _productGalleryRepo.GetAll();
            //var videos = _productGalleryVideosRepo.GetAll();
            var vm = new GalleryPageViewModel()
            {
                Images = images,
                //Videos = videos
            };
            return View(vm);
        }

        public ActionResult About()
        {
            var aboutViewModel = new AboutViewModel();

            //aboutViewModel.AboutDescription = _contentDetailsRepo.GetStaticContentDetailsByStaticContentTypeId((int)StaticContentTypes.CompanyHistory).FirstOrDefault().Description;

            return PartialView(aboutViewModel);
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

        //public ActionResult IndexPartOne()
        //{
        //    var section1Cover = _staticContentDetailsRepo.GetStaticContentDetail(3024);
        //    var section1Part2 = _staticContentDetailsRepo.GetStaticContentDetail(3029);
        //    var section1part3 = _staticContentDetailsRepo.GetStaticContentDetail(3031);
        //    var section1Video = _galleryVideosRepo.GetFirstVideo();
        //    var section1VideoDescription = _staticContentDetailsRepo.GetStaticContentDetail(3034);

        //    var model = new IndexPartViewModels()
        //    {
        //        Description = section1Cover.Description,
        //        ButtonLink = section1Cover.Link,
        //        CoverImage = section1Cover.Image,
        //        Part2Image = section1Part2.Image,
        //        part2Description = section1Part2.Description,
        //        part3Image = section1part3.Image,
        //        part3Description = section1part3.Description,
        //        VideoDescriptions = section1VideoDescription.Description,
        //        Video = section1Video.Video
        //    };

        //    return PartialView(model);
        //}
        //public ActionResult IndexPartTwo()
        //{
        //    var section2Cover = _staticContentDetailsRepo.GetStaticContentDetail(3036);
        //    var section2Part2 = _staticContentDetailsRepo.GetStaticContentDetail(3037);
        //    var section2part3 = _staticContentDetailsRepo.GetStaticContentDetail(3038);
        //    var section2Video = _galleryVideosRepo.GetFirstVideo();
        //    var section2VideoDescription = _staticContentDetailsRepo.GetStaticContentDetail(3040);

        //    var model = new IndexPartViewModels()
        //    {
        //        Description = section2Cover.Description,
        //        ButtonLink = section2Cover.Link,
        //        CoverImage = section2Cover.Image,
        //        Part2Image = section2Part2.Image,
        //        part2Description = section2Part2.Description,
        //        part3Image = section2part3.Image,
        //        part3Description = section2part3.Description,
        //        VideoDescriptions = section2VideoDescription.Description,
        //        Video = section2Video.Video
        //    };

        //    return PartialView(model);
        //}

        //public ActionResult IndexPartThree()
        //{
        //    var section3Cover = _staticContentDetailsRepo.GetStaticContentDetail(3041);
        //    var section3Part2 = _staticContentDetailsRepo.GetStaticContentDetail(3042);
        //    var section3part3 = _staticContentDetailsRepo.GetStaticContentDetail(3043);
        //    //var section3Video = _galleryVideosRepo.GetFirstVideo();
        //    var section3VideoDescription = _staticContentDetailsRepo.GetStaticContentDetail(3044);

        //    var model = new IndexPartViewModels()
        //    {
        //        Description = section3Cover.Description,
        //        ButtonLink = section3Cover.Link,
        //        CoverImage = section3Cover.Image,
        //        Part2Image = section3Part2.Image,
        //        part2Description = section3Part2.Description,
        //        part3Image = section3part3.Image,
        //        part3Description = section3part3.Description,
        //        VideoDescriptions = section3VideoDescription.Description,
        //        Video = section3Video.Video
        //    };

        //    return PartialView(model);
        //}

        public ActionResult SocialsSection()
        {
            SocialViewModel model = new SocialViewModel();

            model.Facebook = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Facebook).Link;
            model.Twitter = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Twitter).Link;
            model.Pinterest = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Pinterest).Link;
            model.Instagram = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.Instagram).Link;

            return PartialView(model);
        }

        public ActionResult ColorSearchSection()
        {
            var model = CreatingColor_SizeSearchViewModel((int)ProductFeatures.Color);

            return PartialView(model);
        }

        public ActionResult SizeSearchSection()
        {
            var model = CreatingColor_SizeSearchViewModel((int)ProductFeatures.Size);

            return PartialView(model);
        }

        private List<Color_SizeSearchViewModel> CreatingColor_SizeSearchViewModel(int featureId)
        {
            var model = new List<Color_SizeSearchViewModel>();

            var subFeatures = _productsRepo.GetSubFeaturesByFeatureId(featureId);

            foreach (var item in subFeatures)
            {
                var viewModel = new Color_SizeSearchViewModel()
                {
                    Id = item.Id,
                    Value = item.Value,
                    OtherInfo = item.OtherInfo,
                    ProductCount = _productFeatureValuesRepo.GetProductsCountBySubFeatureId(item.Id)
                };

                model.Add(viewModel);
            }

            return model;
        }

        public ActionResult ProductGroupSection()
        {
            var categories = _productGroupsRepo.GetProductGroupTable();
            var articleCategoriesVm = new List<ProductCategoriesViewModel>();
            foreach (var item in categories)
            {
                var vm = new ProductCategoriesViewModel();
                vm.Id = item.Id;
                vm.Title = item.Title;
                vm.ProductCount = _productsRepo.getProductsByGroupId(item.Id).Count();
                articleCategoriesVm.Add(vm);
            }
            return PartialView(articleCategoriesVm);
        }

        public ActionResult TopProductGroupSection()
        {
            var categories = _productGroupsRepo.GetProductGroupTable();
            var articleCategoriesVm = new List<ProductCategoriesViewModel>();
            foreach (var item in categories)
            {
                var vm = new ProductCategoriesViewModel();
                vm.Id = item.Id;
                vm.Title = item.Title;
                vm.Image = item.Image;
                vm.ProductCount = _productsRepo.getProductsByGroupId(item.Id).Count();
                articleCategoriesVm.Add(vm);
            }
            return PartialView(articleCategoriesVm);
        }

        public ActionResult ProductDescriptionsSection(int productId)
        {
            var model = new List<ProductDescriptionViewModel>();

            var allDescrioptions = _staticContentDetailsRepo.GetStaticContentDetailsByStaticContentTypeId((int)StaticContentTypes.DescriptionProduct1);

            foreach (var item in allDescrioptions)
            {
                var descriptionVM = new ProductDescriptionViewModel
                {
                    ShortDescriptionTitle = item.Title,
                    Descrription = item.Description,
                    Image = item.Image
                };

                model.Add(descriptionVM);
            }

            return PartialView(model);
        }

        public ActionResult ProductFeaturesSection(int productId)
        {
            var model = new List<ProductFeatures_SubFeaturesViewModel>();

            var allFeatures = _featuresRepo.GetFeaturesWithSubFeaturesByProductId(productId);

            foreach (var item in allFeatures)
            {
                var FeaturesVM = new ProductFeatures_SubFeaturesViewModel
                {
                    Title = item.Title,
                };

                foreach (var subFeature in item.SubFeatures)
                {
                    FeaturesVM.SubFeatures.Add(subFeature.Value);
                }

                model.Add(FeaturesVM);
            }

            return PartialView(model);
        }

        public ActionResult ProductGallerySection(int productId)
        {
            var model = _productGalleryRepo.GetProductGalleries(productId);

            return PartialView(model);
        }
    }
}