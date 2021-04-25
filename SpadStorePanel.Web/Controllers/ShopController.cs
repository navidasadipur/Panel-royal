using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Core.Utility;
using SpadStorePanel.Infrastructure.Extensions;
using SpadStorePanel.Infrastructure.Helpers;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Infrastructure.Services;
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
        private readonly ProductService _productService;

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
            StaticContentDetailsRepository staticContentDetailsRepo,
            ProductService productService
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
            this._productService = productService;
        }

        //public ActionResult Test()
        //{
        //    return View();
        //}

        [Route("Shop")]
        [Route("Shop/{id}")]
        public ActionResult Index(int? id, string searchString = null)
        {
            var model = new List<Color_SizeSearchViewModel>();

            var subFeatures = _productsRepo.GetSubFeaturesByFeatureId((int)ProductFeatures.Size);

            foreach (var item in subFeatures)
            {

                var viewModel = new Color_SizeSearchViewModel()
                {
                    Id = item.Id,
                    Value = item.Value,
                    OtherInfo = item.OtherInfo,
                };

                var allProducts = _productMainFeaturesRepo.GetProductMainFeaturesBySubFeatureId(item.Id).ToList();

                var allProductIds = DistinctByExtension.DistinctBy(allProducts, p => p.ProductId).Select(p => p.ProductId).ToList();

                viewModel.ProductCount = allProductIds.Count();

                model.Add(viewModel);
            }

            var minPrice = _productMainFeaturesRepo.GetMinPrice();

            var maxPrice = _productMainFeaturesRepo.GetMaxPrice();

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            if (id != null)
                ViewBag.Id = id;
            if (searchString != null)
                ViewBag.SearchString = searchString;

            return View(model);
        }

        private List<Product> FilteringByPrice(int minFilterPrice, int maxFilterPrice, List<Product> allTargetProducts)
        {
            var targetProducts = new List<Product>();

            foreach (var product in allTargetProducts)
            {
                product.ProductMainFeatures = new List<ProductMainFeature>();

                product.ProductMainFeatures = (_productMainFeaturesRepo.GetProductMainFeatures(product.Id));

                var targetProductId = product.ProductMainFeatures.Where(pmf => pmf.Price >= minFilterPrice && pmf.Price <= maxFilterPrice).Select(pmf => pmf.ProductId).FirstOrDefault();

                if (targetProductId != 0)
                {
                    targetProducts.Add(_productsRepo.GetProduct(targetProductId));
                }
            }

            return targetProducts;
        }

        [HttpPost]
        [Route("FilterProducts/")]
        public ActionResult SizeSearchSection(FilterModel model)
        {
            var allSearchedTargetProducts = new List<Product>();

            var targetProductsPriceFilted = new List<Product>();

            var productListVm = new List<ProductListViewModel>();

            var allTargetProducts = _productsRepo.GetAllProducts();

            var maxLimitPrice = _productMainFeaturesRepo.GetMaxPrice();
            var minLimitPrice = _productMainFeaturesRepo.GetMinPrice();

            //search by group id
            if (model.GroupId != null)
            {
                var category = _productGroupsRepo.GetProductGroup(model.GroupId.Value);
                if (category != null)
                {
                    ViewBag.GroupId = model.GroupId.Value;
                    ViewBag.BreadCrumb = category.Title;
                    allSearchedTargetProducts = _productsRepo.getProductsByGroupId(model.GroupId.Value);
                }

                if (model.MinPrice != minLimitPrice || model.MaxPrice != maxLimitPrice)
                {
                    targetProductsPriceFilted = FilteringByPrice(model.MinPrice, model.MaxPrice, allSearchedTargetProducts);

                    allTargetProducts = targetProductsPriceFilted;
                }
                else
                {
                    allTargetProducts = allSearchedTargetProducts;
                }

                foreach (var item in allTargetProducts)
                {
                    var vm = new ProductListViewModel(item);

                    if (item.ProductComments != null)
                    {
                        vm.CommentCounter = item.ProductComments.Count();
                    }
                    productListVm.Add(vm);
                }

                return PartialView(productListVm);
            }

            //search by string
            else if (!string.IsNullOrEmpty(model.SearchString))
            {
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    ViewBag.BreadCrumb = $"جستجو {model.SearchString}";
                    allSearchedTargetProducts = allTargetProducts
                        .Where(p => p.Title != null && p.Title.ToLower().Trim().Contains(model.SearchString.ToLower().Trim()) ||
                            p.ShortDescription != null && p.ShortDescription.ToLower().Trim().Contains(model.SearchString.ToLower().Trim()) ||
                            p.Description != null && p.Description.ToLower().Trim().Contains(model.SearchString.ToLower().Trim())).ToList();
                }

                allTargetProducts = allSearchedTargetProducts;

                foreach (var item in allTargetProducts)
                {
                    var vm = new ProductListViewModel(item);

                    if (item.ProductComments != null)
                    {
                        vm.CommentCounter = item.ProductComments.Count();
                    }
                    productListVm.Add(vm);
                }

                return PartialView(productListVm);
            }

            //search by size id
            else
            {
                //if size id == 0 show all products
                if (model.SizeId == 0)
                {

                    if (model.MinPrice != minLimitPrice || model.MaxPrice != maxLimitPrice)
                    {
                        targetProductsPriceFilted = FilteringByPrice(model.MinPrice, model.MaxPrice, allTargetProducts);

                        allTargetProducts = targetProductsPriceFilted;
                    }
                    else
                    {
                        allTargetProducts = allTargetProducts;
                    }

                    foreach (var item in allTargetProducts)
                    {
                        var vm = new ProductListViewModel(item);

                        //vm.Role = _articlesRepo.GetAuthorRole(item.UserId);

                        if (item.ProductComments != null)
                        {
                            vm.CommentCounter = item.ProductComments.Count();
                        }
                        productListVm.Add(vm);
                    }
                }
                //search by size id
                else
                {

                    var allProductMainFeatures = _productMainFeaturesRepo.GetProductMainFeaturesBySubFeatureId(model.SizeId.Value).ToList();

                    var allProductIds = DistinctByExtension.DistinctBy(allProductMainFeatures, p => p.ProductId).Select(p => p.ProductId).ToList();

                    foreach (var id in allProductIds)
                    {
                        var product = _productsRepo.GetProduct(id);

                        if (product != null)
                        {
                            allSearchedTargetProducts.Add(product);
                        }
                    }

                    if (model.MinPrice != minLimitPrice || model.MaxPrice != maxLimitPrice)
                    {
                        targetProductsPriceFilted = FilteringByPrice(model.MinPrice, model.MaxPrice, allSearchedTargetProducts);

                        allTargetProducts = targetProductsPriceFilted;
                    }
                    else
                    {
                        allTargetProducts = allSearchedTargetProducts;
                    }

                    foreach (var item in allTargetProducts)
                    {
                        var vm = new ProductListViewModel(item);

                        //vm.Role = _articlesRepo.GetAuthorRole(item.UserId);

                        if (item.ProductComments != null)
                        {
                            vm.CommentCounter = item.ProductComments.Count();
                        }
                        productListVm.Add(vm);
                    }
                }

                ViewBag.PageCount = 0;

                return PartialView(productListVm);
            }

        }

        /*
        [Route("Shop")]
        [Route("Shop/{id}")]
        public ActionResult Index(int? id, string searchString = null, List<Product> model = null)
        {
            if (model != null)
            {
                return View(model);
            }

            var products = new List<Product>();

            if (id == null)
            {
                products = _productsRepo.GetAllProducts();
                if (!string.IsNullOrEmpty(searchString))
                {
                    ViewBag.BreadCrumb = $"جستجو {searchString}";
                    products = products
                        .Where(p => p.Title != null && p.Title.ToLower().Trim().Contains(searchString.ToLower().Trim()) ||
                            p.ShortDescription != null && p.ShortDescription.ToLower().Trim().Contains(searchString.ToLower().Trim()) ||
                            p.Description != null && p.Description.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToList();
                }
            }
            else
            {
                var category = _productGroupsRepo.GetProductGroup(id.Value);
                if (category != null)
                {
                    ViewBag.GroupId = id.Value;
                    ViewBag.BreadCrumb = category.Title;
                    products = _productsRepo.getProductsByGroupId(id.Value);
                }
            }

            foreach (var item in products)
            {

                item.ProductMainFeatures = new List<ProductMainFeature>();

                item.ProductMainFeatures = (_productMainFeaturesRepo.GetProductMainFeatures(item.Id));
            }


            var productListVm = new List<ProductListViewModel>();
            foreach (var item in products)
            {


                var vm = new ProductListViewModel(item);

                //vm.Role = _articlesRepo.GetAuthorRole(item.UserId);

                if (item.ProductComments != null)
                {
                    vm.CommentCounter = item.ProductComments.Count();
                }
                productListVm.Add(vm);
            }
            return View(productListVm);

            //var viewModel = new ProductViewModel();

            //if (id == null)
            //{
            //    viewModel.Products = _productsRepo.GetAllProducts();

            //    return PartialView(viewModel);
            //}

            //viewModel.Products = _productsRepo.getProductsByGroupId(id.Value);

            //ViewBag.CategoryTitle = _productGroupsRepo.Get(id.Value).Title;

            //return PartialView(viewModel);
        }*/

        //[HttpGet]
        //public ActionResult Search(int min, int max)
        //{

        //    return Redirect("index");
        //}

        [Route("Shop/Details/{id}")]
        public ActionResult Details(int id, bool isAddedToCart = false)
        {
            //_productsRepo.UpdateProductViewCount(id);

            var product = _productsRepo.GetProduct(id);

            if (product == null)
            {
                return new HttpNotFoundResult();
            }

            var productDetailsVm = new ProductDetailsViewModel(product);

            productDetailsVm.IsAddedToCart = isAddedToCart;

            var productComments = _productsRepo.GetProductComments(id);

            var productCommentsVm = new List<ProductCommentViewModel>();
            foreach (var item in productComments)
                productCommentsVm.Add(new ProductCommentViewModel(item));
            productDetailsVm.ProductComments = productCommentsVm;

            var productTags = _productsRepo.GetProductTags(id);
            productDetailsVm.Tags = productTags;

            return View(productDetailsVm);
        }

        public ActionResult AddToCartSection(int id)
        {
            //_productsRepo.UpdateProductViewCount(id);

            var product = _productsRepo.GetProduct(id);

            var model = new AddToCartViewModel();

            var features = new List<Feature>();

            var mainFeatures = product.ProductMainFeatures.GroupBy(pf => pf.FeatureId).Select(pf => pf.First()).ToList();

            var mainFeatureIds = mainFeatures.Select(mf => mf.FeatureId).ToList();

            foreach (var featureId in mainFeatureIds)
            {
                features.Add(_featuresRepo.GetMainFeatureWithSubFeaturesByFeatureIdAndProductId(featureId, id));
            }

            model.MainFeatures = features;

            model.ProductId = id;

            //var productDetailsVm = new ProductDetailsViewModel(product);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddToCartSection(FormCollection form)
        {
            var isAddedToCart = false;

            //var test0 = form.Keys[0];
            var productIdStr = form.GetValue("ProductId");
            var hasProductId = int.TryParse(productIdStr.AttemptedValue, out int productId);

            //var test1 = form.Keys[1];
            var featureValueIdStr = form.GetValue("feature");
            var hasFeatureValueId = int.TryParse(featureValueIdStr.AttemptedValue, out int featureValueId);

            //var test3 = form.Keys[2];
            var quantityStr = form.GetValue("quantity");
            var Hasquantity = int.TryParse(quantityStr.AttemptedValue, out int quantity);

            if (hasProductId && hasFeatureValueId && Hasquantity)
            {
                for (int i = 0; i < quantity; i++)
                {

                }

                isAddedToCart = true;
            }

            return RedirectToAction("Details", new { id = productId, isAddedToCart = isAddedToCart });
        }

        [HttpPost]
        public ActionResult PostComment(ProductCommentFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var comment = new ProductComment()
                {
                    ProductId = form.ProductId,
                    ParentId = form.ParentId,
                    Name = form.Name,
                    Email = form.Email,
                    Message = form.Message,
                    AddedDate = DateTime.Now,
                };
                _productsRepo.AddComment(comment);
                return RedirectToAction("ContactUsSummary", "Home");
            }

            var postTitle = _productsRepo.Get(form.ProductId).Title;

            return RedirectToAction("Details", new { id = form.ProductId });
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
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage).Image;

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


        //[HttpPost]
        ////[Route("SearchSection")]
        ////[Route("Shop/test/test")]
        //public ActionResult Search(FormCollection form)
        //{
        //    var model = new List<Product>();

        //    var allMainFeatures = _productMainFeaturesRepo.GetAll().OrderBy(mf => mf.Price);

        //    var maxLimitStr = form.GetValue("maxLimit").ToString();
        //    var hasMaxLimit = long.TryParse(maxLimitStr, out long maxLimit);
        //    if (!hasMaxLimit)
        //    {
        //        maxLimit = allMainFeatures.Last().Price;
        //    }

        //    var minLimitStr = form.GetValue("minLimit").ToString();
        //    var hasMinLimit = long.TryParse(minLimitStr, out long minLimit);
        //    if (!hasMinLimit)
        //    {
        //        minLimit = allMainFeatures.First().Price;
        //    }

        //    //var limitetMainFeatures = allMainFeatures.Where(mf => mf.Price >= minLimit && mf.Price <= maxLimit).ToList();

        //    //foreach (var mainFeature in limitetMainFeatures)
        //    //{
        //    //    var product = allProducts.First(p => p.Id == mainFeature.ProductId);

        //    //    model.Add(product);
        //    //}
            

        //    var allKeys = form.AllKeys;

        //    var colorKey = "color";

        //    var sizeKey = "size";

        //    var categoryKey = "cate";

        //    var colorIdsList = new List<int>();
        //    var sizeIdsList = new List<int>();
        //    var groupIdsList = new List<int>();

        //    foreach (var key in allKeys)
        //    {
        //        if (key == colorKey)
        //        {
        //            //creating color Ids list
        //            var colorIdsStr = form.GetValue(colorKey).AttemptedValue;
        //            var colorIdsArray = colorIdsStr.Split(',');
        //            if (colorIdsArray.Count() > 0)
        //            {
                        
        //                foreach (var idStr in colorIdsArray)
        //                {
        //                    var hasId = int.TryParse(idStr, out int id);
        //                    if (hasId)
        //                    {
        //                        colorIdsList.Add(id);
        //                    }
        //                }
        //            }
        //        }
        //        else if (key == sizeKey)
        //        {
        //            //creating size Ids list
        //            var sizeIdsStr = form.GetValue(sizeKey).AttemptedValue;
        //            var sizeIdsArray = sizeIdsStr.Split(',');
        //            if (sizeIdsArray.Count() > 0)
        //            {
                        
        //                foreach (var idStr in sizeIdsArray)
        //                {
        //                    var hasId = int.TryParse(idStr, out int id);
        //                    if (hasId)
        //                    {
        //                        sizeIdsList.Add(id);
        //                    }
        //                }
        //            }
        //        }
        //        else if (key == categoryKey)
        //        {
        //            //creating group Ids list
        //            var groupIdsStr = form.GetValue(categoryKey).AttemptedValue;
        //            var groupIdsArray = groupIdsStr.Split(',');
        //            if (groupIdsArray.Count() > 0)
        //            {
                        
        //                foreach (var idStr in groupIdsArray)
        //                {
        //                    var hasId = int.TryParse(idStr, out int id);
        //                    if (hasId)
        //                    {
        //                        groupIdsList.Add(id);
        //                    }
        //                }
        //            }
        //        }
        //    }



        //    var allProducts = _productsRepo.GetAllProducts().ToList();

        //    //searching all main features based on values of color and values of sizes and limits of price
        //    var SearchedMainFeatures = allMainFeatures.Where(mf => (mf.FeatureId == (int)ProductFeatures.Color && colorIdsList.Contains(mf.SubFeatureId.Value)) ||
        //                                                   (mf.FeatureId == (int)ProductFeatures.Size && sizeIdsList.Contains(mf.SubFeatureId.Value)) ||
        //                                                   (mf.Price >= minLimit && mf.Price <= maxLimit)).ToList();
        //    //adding searched products to model
        //    if (SearchedMainFeatures.Count() > 0)
        //    {
        //        foreach (var mainFeature in SearchedMainFeatures)
        //        {
        //            var product = allProducts.Where(p => p.Id == mainFeature.ProductId).FirstOrDefault();

        //            model.Add(product);
        //        }
        //    }
            

        //    //seaching all products in a group
        //    var allProductsByGroup = new List<Product>();

        //    foreach (var id in groupIdsList)
        //    {
        //        var products = _productsRepo.getProductsByGroupId(id);

        //        allProductsByGroup.AddRange(products);
        //    }

        //    //checking if that product alredy is in the model do noting and else add it
        //    foreach (var product in allProductsByGroup)
        //    {
        //        if (!model.Contains(product))
        //        {
        //            model.Add(product);
        //        }
        //    }

        //    return RedirectToAction("Index", new { model = model });
        //}

        //public ActionResult SearchSection()
        //{
        //    //var model = CreatingColor_SizeSearchViewModel((int)ProductFeatures.Color);
        //    //model = CreatingColor_SizeSearchViewModel((int)ProductFeatures.Size);

        //    var model = new SearchViewModel();

        //    #region SizeViewModels
        //    var allProductGroups = _productGroupsRepo.GetAllProductGroups();

        //    foreach (var item in allProductGroups)
        //    {
        //        var vm = new ProductCategoriesViewModel();
        //        vm.Id = item.Id;
        //        vm.Title = item.Title;
        //        vm.ProductCount = _productsRepo.getProductsByGroupId(item.Id).Count();

        //        model.ProductCategoriesViewModels.Add(vm);
        //    }
        //    #endregion

        //    #region SizeViewModels
        //    var sizeSubFeatures = _productsRepo.GetSubFeaturesByFeatureId((int)ProductFeatures.Size);

        //    foreach (var item in sizeSubFeatures)
        //    {
        //        var viewModel = new SizeViewModel()
        //        {
        //            Id = item.Id,
        //            Value = item.Value,
        //            OtherInfo = item.OtherInfo,
        //            ProductCount = _productFeatureValuesRepo.GetProductsCountBySubFeatureId(item.Id)
        //        };

        //        model.SizeViewModels.Add(viewModel);
        //    }
        //    #endregion

        //    #region ColorViewModels
        //    var colorSubFeatures = _productsRepo.GetSubFeaturesByFeatureId((int)ProductFeatures.Color);

        //    foreach (var item in colorSubFeatures)
        //    {
        //        var viewModel = new ColorViewModel()
        //        {
        //            Id = item.Id,
        //            Value = item.Value,
        //            OtherInfo = item.OtherInfo,
        //            ProductCount = _productFeatureValuesRepo.GetProductsCountBySubFeatureId(item.Id)
        //        };

        //        model.ColorViewModels.Add(viewModel);
        //    }
        //    #endregion

        //    #region priceLimits

        //    var allMainFeatures = _productMainFeaturesRepo.GetAll();

        //    var allPrices = allMainFeatures.OrderBy(mf => mf.Price).Select(mf => mf.Price).ToList();

        //    model.MinPrice = allPrices.First();
        //    model.MinPrice = allPrices.Last();

        //    #endregion

        //    return PartialView(model);

        //}

        public ActionResult PriceSearchSection()
        {
            var model = new SearchViewModel();

            model.MaxPrice = _productMainFeaturesRepo.GetMaxPrice();
            model.MinPrice = _productMainFeaturesRepo.GetMinPrice();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PriceSearchSection(int min, int Max)
        {
            var model = new List<Product>();
            return View("Index", model);
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

        /*
        public ActionResult SizeSearchSection()
        {
            var model = new List<Color_SizeSearchViewModel>();

            var subFeatures = _productsRepo.GetSubFeaturesByFeatureId((int)ProductFeatures.Size);

            foreach (var item in subFeatures)
            {
                var viewModel = new Color_SizeSearchViewModel()
                {
                    Id = item.Id,
                    Value = item.Value,
                    OtherInfo = item.OtherInfo,
                };

                var allProducts = _productMainFeaturesRepo.GetProductMainFeaturesBySubFeatureId(item.Id).ToList();

                var allProductIds = DistinctByExtension.DistinctBy(allProducts, p => p.ProductId).Select(p => p.ProductId).ToList();

                viewModel.ProductCount = allProductIds.Count();

                model.Add(viewModel);
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SizeSearchSection(int sizeId)
        {

            var allProducts = new List<Product>();

            var allProductMainFeatures = _productMainFeaturesRepo.GetProductMainFeaturesBySubFeatureId(sizeId).ToList();

            var allProductIds = DistinctByExtension.DistinctBy(allProductMainFeatures, p => p.ProductId).Select(p => p.ProductId).ToList();

            foreach (var id in allProductIds)
            {
                var product = _productsRepo.GetProduct(id);

                allProducts.Add(product);
            }

            //filling productmainFeatures
            foreach (var item in allProducts)
            {

                item.ProductMainFeatures = new List<ProductMainFeature>();

                item.ProductMainFeatures = (_productMainFeaturesRepo.GetProductMainFeatures(item.Id));
            }

            //creating productListViewModel
            var productListVm = new List<ProductListViewModel>();
            foreach (var item in allProducts)
            {


                var vm = new ProductListViewModel(item);

                //vm.Role = _articlesRepo.GetAuthorRole(item.UserId);

                if (item.ProductComments != null)
                {
                    vm.CommentCounter = item.ProductComments.Count();
                }
                productListVm.Add(vm);
            }

            //model = _productsRepo

            ViewBag.PageCount = 0;
            return View("Index", productListVm);
        }*/

        //private List<Color_SizeSearchViewModel> CreatingColor_SizeSearchViewModel(int featureId)
        //{
        //    var model = new List<Color_SizeSearchViewModel>();

        //    var subFeatures = _productsRepo.GetSubFeaturesByFeatureId(featureId);

        //    foreach (var item in subFeatures)
        //    {
        //        var viewModel = new Color_SizeSearchViewModel()
        //        {
        //            Id = item.Id,
        //            Value = item.Value,
        //            OtherInfo = item.OtherInfo,
        //            ProductCount = _productFeatureValuesRepo.GetProductsCountBySubFeatureId(item.Id)
        //        };

        //        model.Add(viewModel);
        //    }

        //    return model;
        //}



        public ActionResult TopProductGroupSection()
        {
            ViewBag.BackImage = _staticContentDetailsRepo.GetStaticContentDetail((int)StaticContents.BackGroundImage).Image;

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
            var product = _productsRepo.GetProduct(productId);

            var descriptionVM = new ProductDescriptionViewModel
            {
                DescriptionOneTitle = product.DescriptionOneTitle,
                DescriptionTwoTitle = product.DescriptionTwoTitle,
                DescrriptionOneShortDescription = product.DescriptionOneShortDescription,
                DescrriptionTwoShortDescription = product.DescriptionTwoShortDescription,
                DescrriptionOneImage = product.DescriptionOneImage,
                DescrriptionTwoImage = product.DescriptionTwoImage
            };

            return PartialView(descriptionVM);
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

        public ActionResult RelatedProductsSection(int productId)
        {
            //its better to use product tags but for this site i use product groups for relatedProducs
            var allGroupProducts = new List<Product>();

            var product = _productsRepo.GetProduct(productId);

            if (product.ProductGroupId.HasValue)
            {
                allGroupProducts = _productsRepo.getProductsByGroupId(product.ProductGroupId.Value);
            }

            allGroupProducts.Remove(product);

            foreach (var item in allGroupProducts)
            {
                ImageResizer image = new ImageResizer(850, 400, true);
            }

            return PartialView(allGroupProducts);
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


    }
}