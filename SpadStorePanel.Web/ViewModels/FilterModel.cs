using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpadStorePanel.Web.ViewModels
{
    public class FilterModel
    {
        public int? SizeId { get; set; }
        public int? GroupId { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string SearchString { get; set; }
        public string SearchedGroupIds { get; set; }
        public string SearchedProductIds { get; set; }
    }
}