using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpadStorePanel.Web.ViewModels
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public long Price { get; set; }
        public int Quantity { get; set; }
        public int MainFeatureId { get; set; }
    }
}