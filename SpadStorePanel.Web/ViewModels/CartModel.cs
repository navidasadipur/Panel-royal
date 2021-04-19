using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpadStorePanel.Web.ViewModels
{
    public class CartModel
    {
        public CartModel()
        {
        }

        public CartModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            var jItems = jObject["CartItems"].ToList();
            var cartItems = new List<CartItemModel>();
            foreach (var item in jItems)
            {
                cartItems.Add(new CartItemModel()
                {
                    Id = Convert.ToInt32(item["Id"]),
                    ProductName = (string)item["ProductName"],
                    Image = (string)item["Image"],
                    Price = Convert.ToInt64(item["Price"]),
                    MainFeatureId = Convert.ToInt32(item["MainFeatureId"]),
                    Quantity = Convert.ToInt32(item["Quantity"])
                });
            }

            this.CartItems = cartItems;
            this.TotalPrice = Convert.ToInt64(jObject["TotalPrice"]);
        }
        public List<CartItemModel> CartItems { get; set; }
        public long TotalPrice { get; set; }
    }
}