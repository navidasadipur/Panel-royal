using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpadStorePanel.Web.ViewModels
{
    public class WishListModel
    {
        public WishListModel()
        {
        }

        public WishListModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            var jItems = jObject["WishListItems"].ToList();
            var wishListItems = new List<WishListItemModel>();
            foreach (var item in jItems)
            {
                wishListItems.Add(new WishListItemModel()
                {
                    Id = Convert.ToInt32(item["Id"]),
                    ProductName = (string)item["ProductName"],
                    Image = (string)item["Image"],
                });
            }

            this.WishListItems = wishListItems;
        }
        public List<WishListItemModel> WishListItems { get; set; }
    }
}