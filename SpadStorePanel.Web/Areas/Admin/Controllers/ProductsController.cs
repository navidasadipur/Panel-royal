﻿using System;
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
            return View();
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
        public bool UploadImage(int id, HttpPostedFileBase File)
        {
            #region Upload Image
            if (File != null)
            {
                var product = _productRepo.Get(id);
                if (product.Image != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Image/" + product.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Image/" + product.Image));

                    if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Thumb/" + product.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Thumb/" + product.Image));
                }
                // Saving Temp Image
                var newFileName = Guid.NewGuid() + Path.GetExtension(File.FileName);
                File.SaveAs(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));
                // Resize Image
                ImageResizer image = new ImageResizer(850, 400, true);
                image.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileName),
                    Server.MapPath("/Files/ProductImages/Image/" + newFileName));

                ImageResizer thumb = new ImageResizer(200, 200, true);
                thumb.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileName),
                    Server.MapPath("/Files/ProductImages/Thumb/" + newFileName));

                // Deleting Temp Image
                System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));
                product.Image = newFileName;
                _productRepo.Update(product);
                return true;

            }
            #endregion

            return false;
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