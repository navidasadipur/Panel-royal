using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure.Helpers;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;

namespace SpadStorePanel.Web.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductsRepository _productRepo;
        private readonly ProductGroupsRepository _productGroupRepo;
        private readonly ProductFeatureValuesRepository _featureRepo;
        private readonly ProductMainFeaturesRepository _mainFeatureRepo;

        public ProductsController(ProductsRepository productRepo, ProductGroupsRepository groupRepo, ProductFeatureValuesRepository featureRepo, ProductMainFeaturesRepository mainFeatureRepo)
        {
            _productRepo = productRepo;
            _productGroupRepo = groupRepo;
            _featureRepo = featureRepo;
            _mainFeatureRepo = mainFeatureRepo;
        }

        // GET: Admin/Products
        public ActionResult Index()
        {
            return View(_productRepo.GetAllProducts());
        }
        public ActionResult Create()
        {
            ViewBag.ProductGroups = _productGroupRepo.GetAllProductGroups();
            var product = new Product();
            return View(product);
        }
        [HttpPost]
        public int? Create(NewProductViewModel product)
        {
            if (!ModelState.IsValid) return null;
            var prod = new Product();
            prod.Title = product.Title;
            prod.ShortDescription = product.ShortDescription;
            prod.Description = HttpUtility.UrlDecode(product.Description, System.Text.Encoding.Default);
            prod.BrandId = product.Brand;
            prod.ProductGroupId = product.ProductGroup;
            prod.Rate = product.Rate;
            prod.ShortDescription = product.ShortDescription;
            prod.DescriptionOneTitle = product.DescriptionOneTitle;
            prod.DescriptionOneShortDescription = product.DescriptionOneShortDescription;
            prod.DescriptionTwoTitle = product.DescriptionTwoTitle;
            prod.DescriptionTwoShortDescription = product.DescriptionTwoShortDescription;
            var addProduct = _productRepo.Add(prod);
            #region Adding Product Features

            foreach (var feature in product.ProductFeatures)
            {
                if (feature.IsMain)
                {
                    var model = new ProductMainFeature();
                    model.ProductId = addProduct.Id;
                    model.FeatureId = feature.FeatureId;
                    model.SubFeatureId = feature.SubFeatureId;
                    model.Value = feature.Value;
                    model.Quantity = feature.Quantity??0;
                    model.Price = feature.Price ?? 0;
                    _productRepo.AddProductMainFeature(model);
                }
                else
                {
                    var model = new ProductFeatureValue();
                    model.ProductId = addProduct.Id;
                    model.FeatureId = feature.FeatureId;
                    model.SubFeatureId = feature.SubFeatureId;
                    model.Value = feature.Value;
                    _productRepo.AddProductFeature(model);
                }
            }
            #endregion
            return addProduct.Id;

        }
        public ActionResult Edit(int id)
        {
            ViewBag.ProductGroups = _productGroupRepo.GetAllProductGroups();
            var product = _productRepo.GetProduct(id);
            return View(product);
        }
        [HttpPost]
        public int? Edit(NewProductViewModel product)
        {
            if (!ModelState.IsValid) return null;

            var prod = _productRepo.Get(product.ProductId.Value);
            prod.Title = product.Title;
            prod.ShortDescription = product.ShortDescription;
            prod.Description = HttpUtility.UrlDecode(product.Description, System.Text.Encoding.Default);
            prod.BrandId = product.Brand;
            prod.ProductGroupId = product.ProductGroup;
            prod.Rate = product.Rate;
            prod.DescriptionOneTitle = product.DescriptionOneTitle;
            prod.DescriptionOneShortDescription = product.DescriptionOneShortDescription;
            prod.DescriptionTwoTitle = product.DescriptionTwoTitle;
            prod.DescriptionTwoShortDescription = product.DescriptionTwoShortDescription;
            var updateProduct = _productRepo.Update(prod);

            #region Removing Previous Product Features
            var productMainFeatures = _productRepo.GetProductMainFeatures(updateProduct.Id);
            foreach (var mainFeature in productMainFeatures)
                _mainFeatureRepo.Delete(mainFeature.Id);

            var productFeatures = _productRepo.GetProductFeatures(updateProduct.Id);
            foreach (var feature in productFeatures)
                _featureRepo.Delete(feature.Id);
            #endregion

            #region Adding Product Features

            foreach (var feature in product.ProductFeatures)
            {
                if (feature.IsMain)
                {
                    var model = new ProductMainFeature();
                    model.ProductId = updateProduct.Id;
                    model.FeatureId = feature.FeatureId;
                    model.SubFeatureId = feature.SubFeatureId;
                    model.Value = feature.Value;
                    model.Quantity = feature.Quantity ?? 0;
                    model.Price = feature.Price ?? 0;
                    _productRepo.AddProductMainFeature(model);
                }
                else
                {
                    var model = new ProductFeatureValue();
                    model.ProductId = updateProduct.Id;
                    model.FeatureId = feature.FeatureId;
                    model.SubFeatureId = feature.SubFeatureId;
                    model.Value = feature.Value;
                    _productRepo.AddProductFeature(model);
                }
            }
            #endregion
            return updateProduct.Id;

        }
        [HttpPost]
        public bool UploadImage(int id, HttpPostedFileBase ProductImage, HttpPostedFileBase ProductDesOneImage, HttpPostedFileBase ProductDesTwoImage)
        {
            #region Upload Images
            if (ProductImage != null || ProductDesOneImage != null || ProductDesTwoImage != null)
            {
                var product = _productRepo.Get(id);

                #region Upload Product Image

                if (ProductImage != null)
                {
                    var productImage = SaveImage(ProductImage, product.Image);
                    product.Image = productImage;
                }

                #endregion

                #region Upload ProductDesOneImage

                if (ProductDesOneImage != null)
                {
                    var DescriptionOneImage = SaveImage(ProductDesOneImage, product.DescriptionOneImage);
                    product.DescriptionOneImage = DescriptionOneImage;
                }

                //if (ProductDesOneImage != null)
                //{
                //    if (product.DescriptionOneImage != null)
                //    {
                //        if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Image/" + product.DescriptionOneImage)))
                //            System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Image/" + product.DescriptionOneImage));

                //        if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Thumb/" + product.DescriptionOneImage)))
                //            System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Thumb/" + product.DescriptionOneImage));
                //    }
                //    // Saving Temp Image
                //    var newFileNameDesOne = Guid.NewGuid() + Path.GetExtension(ProductDesOneImage.FileName);
                //    ProductDesOneImage.SaveAs(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesOne));
                //    // Resize Image
                //    ImageResizer imageDesOne = new ImageResizer(850, 400, true);
                //    imageDesOne.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesOne),
                //        Server.MapPath("/Files/ProductImages/Image/" + newFileNameDesOne));

                //    ImageResizer thumbDesOne = new ImageResizer(200, 200, true);
                //    thumbDesOne.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesOne),
                //        Server.MapPath("/Files/ProductImages/Thumb/" + newFileNameDesOne));

                //    // Deleting Temp Image
                //    System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesOne));
                //    product.DescriptionOneImage = newFileNameDesOne;
                //}

                #endregion

                #region Upload ProductDesTwoImage

                if (ProductDesTwoImage != null)
                {
                    var DescriptionTwoImage = SaveImage(ProductDesTwoImage, product.DescriptionTwoImage);
                    product.DescriptionTwoImage = DescriptionTwoImage;
                }

                //if (ProductDesTwoImage != null)
                //{
                //    if (product.DescriptionTwoImage != null)
                //    {
                //        if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Image/" + product.DescriptionTwoImage)))
                //            System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Image/" + product.DescriptionTwoImage));

                //        if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Thumb/" + product.DescriptionTwoImage)))
                //            System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Thumb/" + product.DescriptionTwoImage));
                //    }
                //    // Saving Temp Image
                //    var newFileNameDesTwo = Guid.NewGuid() + Path.GetExtension(ProductDesTwoImage.FileName);
                //    ProductDesTwoImage.SaveAs(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesTwo));
                //    // Resize Image
                //    ImageResizer imageDesTwo = new ImageResizer(850, 400, true);
                //    imageDesTwo.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesTwo),
                //        Server.MapPath("/Files/ProductImages/Image/" + newFileNameDesTwo));

                //    ImageResizer thumbDesTwo = new ImageResizer(200, 200, true);
                //    thumbDesTwo.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesTwo),
                //        Server.MapPath("/Files/ProductImages/Thumb/" + newFileNameDesTwo));

                //    // Deleting Temp Image
                //    System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Temp/" + newFileNameDesTwo));
                //    product.DescriptionTwoImage = newFileNameDesTwo;
                //}

                #endregion

                _productRepo.Update(product);
                return true;
            }

            #endregion

            return false;
        }

        private string SaveImage(HttpPostedFileBase ProductImage, string imageName)
        {
                if (imageName != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/ProductImages/Image/" + imageName)))
                        System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Image/" + imageName));

                    if (System.IO.File.Exists(Server.MapPath("/Files/ProductImages/Thumb/" + imageName)))
                        System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Thumb/" + imageName));
                }
                // Saving Temp Image
                var newFileName = Guid.NewGuid() + Path.GetExtension(ProductImage.FileName);
                ProductImage.SaveAs(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));
                // Resize Image
                ImageResizer image = new ImageResizer(850, 400, true);
                image.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileName),
                    Server.MapPath("/Files/ProductImages/Image/" + newFileName));

                ImageResizer thumb = new ImageResizer();
                thumb.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileName),
                    Server.MapPath("/Files/ProductImages/Thumb/" + newFileName));

                // Deleting Temp Image
                System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));
                return newFileName;
        }

        public JsonResult GetProductGroupFeatures(int id)
        {
            var features = _productGroupRepo.GetProductGroupFeatures(id);
            var obj = features.Select(item => new FeaturesObjViewModel() {Id = item.Id, Title = item.Title}).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductFeatures(int id)
        {
            var mainFeatures = _productRepo.GetProductMainFeatures(id);
            var features = _productRepo.GetProductFeatures(id);
            var obj = mainFeatures.Select(mainFeature => new ProductFeaturesViewModel()
                {
                    ProductId = mainFeature.ProductId,
                    FeatureId = mainFeature.FeatureId,
                    SubFeatureId = mainFeature.SubFeatureId,
                    IsMain = true,
                    Value = mainFeature.Value,
                    Quantity = mainFeature.Quantity,
                    Price = mainFeature.Price
                })
                .ToList();
            obj.AddRange(features.Select(feature => new ProductFeaturesViewModel() 
                {
                    ProductId = feature.ProductId, 
                    FeatureId = feature.FeatureId.Value, 
                    Value = feature.Value,
                    IsMain = false,
                    SubFeatureId = feature.SubFeatureId
                }));

            return Json(obj.GroupBy(a=>a.FeatureId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductGroupBrands(int id)
        {
            var brands = _productGroupRepo.GetProductGroupBrands(id);
            var obj = brands.Select(item => new BrandsObjViewModel() { Id = item.Id, Name = item.Name }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFeatureSubFeatures(int id)
        {
            var subFeatures = _productRepo.GetSubFeaturesByFeatureId(id);
            var obj = subFeatures.Select(item => new SubFeaturesObjViewModel() {Id = item.Id, Value = item.Value}).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = _productRepo.Get(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            return PartialView(product);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = _productRepo.Get(id);
            #region Deleting Product Features
            var productMainFeatures = _productRepo.GetProductMainFeatures(product.Id);
            foreach (var mainFeature in productMainFeatures)
                _mainFeatureRepo.Delete(mainFeature.Id);

            var productFeatures = _productRepo.GetProductFeatures(product.Id);
            foreach (var feature in productFeatures)
                _featureRepo.Delete(feature.Id);
            #endregion
            _productRepo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}