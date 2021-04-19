using Newtonsoft.Json;
using SpadStorePanel.Infrastructure.Dtos.Product;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Infrastructure.Services;
using SpadStorePanel.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpadStorePanel.Web.Controllers
{
    public class CookieController : Controller
    {
        private readonly ProductService _productService;

        private readonly ProductMainFeaturesRepository _productMainFeaturesRepo;

        private readonly ProductsRepository _productsRepository;

        public CookieController(ProductService productService,ProductMainFeaturesRepository productMainFeaturesRepository,ProductsRepository productsRepository)
        {
            _productService = productService;

            _productMainFeaturesRepo = productMainFeaturesRepository;

            _productsRepository = productsRepository;
        }

        [HttpPost]
        public void AddToCart(int productId, int? mainFeatureId)
        {
            try
            {
                var cartModel = new CartModel();
                var cartItemsModel = new List<CartItemModel>();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                    cartItemsModel = cartModel.CartItems;
                }
                #endregion

                ProductWithPriceDto product;
                int productStockCount;
                if (mainFeatureId == null)
                {
                    mainFeatureId = _productMainFeaturesRepo.GetByProductId(productId).Id;
                }
                product = _productService.CreateProductWithPriceDto(productId, mainFeatureId.Value);
                productStockCount = _productService.GetProductStockCount(productId, mainFeatureId.Value);

                if (productStockCount > 0)
                {
                    if (cartItemsModel.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value))
                    {
                        if (cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity < productStockCount)
                        {
                            cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity += 1;
                            cartModel.TotalPrice += product.PriceAfterDiscount;
                        }
                    }
                    else
                    {
                        cartItemsModel.Add(new CartItemModel()
                        {
                            Id = product.Id,
                            ProductName = product.ShortTitle,
                            Price = product.PriceAfterDiscount,
                            Quantity = 1,
                            MainFeatureId = mainFeatureId.Value,
                            Image = product.Image
                        });
                        cartModel.TotalPrice += product.PriceAfterDiscount;
                    }
                    cartModel.CartItems = cartItemsModel;
                    var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                    cartCookie.Values.Set("cart", jsonStr);

                    cartCookie.Expires = DateTime.Now.AddHours(12);
                    cartCookie.SameSite = SameSiteMode.Lax;
                    Response.Cookies.Add(cartCookie);
                }
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }

        public ActionResult CartSection()
       {
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
                return PartialView(cartModel);

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
                return PartialView(cartModel);

            }
        }

        [HttpPost]
        public void RemoveFromCart(int productId, int? mainFeatureId, string complete = null)
        {
            try
            {
                var cartModel = new CartModel();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                #endregion

                if (cartModel.CartItems.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId))
                {
                    var itemToRemove = cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId);
                    if (complete == "true" || itemToRemove.Quantity < 2)
                    {
                        cartModel.TotalPrice -= itemToRemove.Price * itemToRemove.Quantity;
                        cartModel.CartItems.Remove(itemToRemove);
                    }
                    else if (complete == "false")
                    {
                        cartModel.TotalPrice -= itemToRemove.Price;
                        cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId).Quantity -= 1;
                    }
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }


        public void AddToWishList(int productId)
        {
            try
            {
                var withListModel = new WishListModel();
                var withListItemsModel = new List<WishListItemModel>();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    withListModel = new WishListModel(cartJsonStr);
                    withListItemsModel = withListModel.WishListItems;
                }
                #endregion

                var product = _productsRepository.Get(productId);
                if (withListItemsModel.Any(i => i.Id == productId) == false)
                {
                    withListItemsModel.Add(new WishListItemModel()
                    {
                        Id = product.Id,
                        ProductName = product.Title,
                        Image = product.Image
                    });
                }
                withListModel.WishListItems = withListItemsModel;
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
                cartCookie.Values.Set("wishList", jsonStr);

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }

        public string WishListTable()
        {
            var wishListModel = new WishListModel();

            try
            {
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    wishListModel = new WishListModel(cartJsonStr);
                }

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }

            
            return JsonConvert.SerializeObject(wishListModel);
        }

        [HttpPost]
        public void RemoveFromWishList(int productId)
        {
            try
            {
                var withListModel = new WishListModel();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    withListModel = new WishListModel(cartJsonStr);
                }
                #endregion

                if (withListModel.WishListItems.Any(i => i.Id == productId))
                {
                    var itemToRemove = withListModel.WishListItems.FirstOrDefault(i => i.Id == productId);
                    withListModel.WishListItems.Remove(itemToRemove);
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
                cartCookie.Values.Set("wishList", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }

      
    }
}