using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;

namespace SpadStorePanel.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class DiscountsController : Controller
    {
        private readonly DiscountsRepository _repo;
        private readonly OffersRepository _offerRepo;
        private readonly BrandsRepository _brandRepo;
        private readonly ProductGroupsRepository _productGroupRepo;
        private readonly ProductsRepository _productRepo;

        public DiscountsController(DiscountsRepository repo, OffersRepository offerRepo, BrandsRepository brandRepo, ProductGroupsRepository productGroupRepo, ProductsRepository productRepo)
        {
            _repo = repo;
            _offerRepo = offerRepo;
            _brandRepo = brandRepo;
            _productGroupRepo = productGroupRepo;
            _productRepo = productRepo;
        }
        // GET: Admin/Discounts
        public ActionResult Index()
        {
            return View(_repo.GetDistinctedDiscounts());
        }
        public ActionResult Create()
        {
            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();
            return View();
        }
        [HttpPost]
        public ActionResult Create(DiscountFormViewModel newDiscount)
        {
            if (ModelState.IsValid)
            {
                var groupIdentifier = Guid.NewGuid().ToString();
                #region Adding Brands Discounts
                if (newDiscount.BrandIds != null)
                {
                    foreach (var item in newDiscount.BrandIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            BrandId = item,
                            GroupIdentifier = groupIdentifier
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion
                #region Adding ProductGroups Discounts
                if (newDiscount.ProductGroupIds != null)
                {
                    foreach (var item in newDiscount.ProductGroupIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductGroupId = item,
                            GroupIdentifier = groupIdentifier
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion

                #region Adding Products Discounts
                if (newDiscount.ProductIds != null)
                {
                    foreach (var item in newDiscount.ProductIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductId = item,
                            GroupIdentifier = groupIdentifier
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion

                return RedirectToAction("Index");
            }
            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();
            return View();
        }
        public ActionResult Edit(int id)
        {
            #region Edit Props

            var vm = new DiscountFormViewModel();
            var discountGroup = _repo.GetDiscountGroup(id);
            var groupIdentifier = discountGroup.FirstOrDefault().GroupIdentifier;
            vm.PreviousDiscounts = discountGroup;
            vm.GroupIdentifier = groupIdentifier;
            vm.Title = discountGroup.FirstOrDefault().Title;
            vm.OfferId = discountGroup.FirstOrDefault().OfferId;
            vm.DiscountType = discountGroup.FirstOrDefault().DiscountType;
            vm.Amount = discountGroup.FirstOrDefault().Amount;

            #endregion

            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();
            return View(vm);
        }
        [HttpPost]
        public ActionResult Edit(DiscountFormViewModel newDiscount)
        {
            if (ModelState.IsValid)
            {
                #region Removing All Previous Discounts
                var prevDicounts = _repo.GetDiscountsByGroupIdentifier(newDiscount.GroupIdentifier);

                foreach (var item in prevDicounts)
                    _repo.Delete(item.Id);

                #endregion

                var groupIdentifier = Guid.NewGuid().ToString();
                #region Adding Brands Discounts
                if (newDiscount.BrandIds != null)
                {
                    foreach (var item in newDiscount.BrandIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            BrandId = item,
                            GroupIdentifier = groupIdentifier
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion
                #region Adding ProductGroups Discounts
                if (newDiscount.ProductGroupIds != null)
                {
                    foreach (var item in newDiscount.ProductGroupIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductGroupId = item,
                            GroupIdentifier = groupIdentifier
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion

                #region Adding Products Discounts
                if (newDiscount.ProductIds != null)
                {
                    foreach (var item in newDiscount.ProductIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductId = item,
                            GroupIdentifier = groupIdentifier
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion

                return RedirectToAction("Index");
            }

            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();
            return View();
        }
        [HttpPost]
        public string ValidateDuplicateDiscount(DiscountFormViewModel newDiscount,string groupIdentifier = null)
        {
            var discounts = _repo.GetAll();
            #region Check for Duplicate Brands
            if (newDiscount.BrandIds!= null)
            {
                foreach (var item in newDiscount.BrandIds)
                {
                    if (discounts.Any(d => d.BrandId == item))
                    {
                        var brandName = _brandRepo.GetAll().FirstOrDefault(b => b.Id == item).Name;
                        var discountName = discounts.FirstOrDefault(d => d.BrandId == item).Title;
                        if (groupIdentifier != null)
                        {
                            if (discounts.FirstOrDefault(d => d.BrandId == item).GroupIdentifier != groupIdentifier)
                            {
                                return $"برای برند {brandName} قبلا تخفیف ثبت شده ( {discountName} )";
                            }
                        }
                        else
                        {
                            return $"برای برند {brandName} قبلا تخفیف ثبت شده ( {discountName} )";
                        }
                    }
                }
            }
            #endregion
            #region Check for Duplicate ProductGroups
            if (newDiscount.ProductGroupIds != null)
            {
                foreach (var item in newDiscount.ProductGroupIds)
                {
                    if (discounts.Any(d => d.ProductGroupId == item))
                    {
                        var productGroupName = _productGroupRepo.GetAll().FirstOrDefault(b => b.Id == item).Title;
                        var discountName = discounts.FirstOrDefault(d => d.ProductGroupId == item).Title;
                        if (groupIdentifier != null)
                        {
                            if (discounts.FirstOrDefault(d => d.ProductGroupId == item).GroupIdentifier != groupIdentifier)
                            {
                                return $"برای دسته {productGroupName} قبلا تخفیف ثبت شده ( {discountName} )";
                            }
                        }
                        else
                        {
                            return $"برای دسته {productGroupName} قبلا تخفیف ثبت شده ( {discountName} )";
                        }
                    }
                }
            }
            #endregion

            #region Check for Duplicate Products
            if (newDiscount.ProductIds != null)
            {
                foreach (var item in newDiscount.ProductIds)
                {
                    if (discounts.Any(d => d.ProductId == item))
                    {
                        var productName = _productRepo.GetAll().FirstOrDefault(b => b.Id == item).Title;
                        var discountName = discounts.FirstOrDefault(d => d.ProductId == item).Title;
                        if (groupIdentifier != null)
                        {
                            if (discounts.FirstOrDefault(d => d.ProductId == item).GroupIdentifier != groupIdentifier)
                            {
                                return $"برای محصول {productName} قبلا تخفیف ثبت شده ( {discountName} )";
                            }
                        }
                        else
                        {
                            return $"برای محصول {productName} قبلا تخفیف ثبت شده ( {discountName} )";
                        }

                    }
                }
            }
            #endregion
            return "valid";
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var discount = _repo.Get(id.Value);
            if (discount == null)
            {
                return HttpNotFound();
            }
            return PartialView(discount);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var discount = _repo.Get(id);
            var discountGroup = _repo.GetDiscountGroup(id);
            foreach (var item in discountGroup)
                _repo.Delete(item.Id);

            return RedirectToAction("Index");
        }
    }
}