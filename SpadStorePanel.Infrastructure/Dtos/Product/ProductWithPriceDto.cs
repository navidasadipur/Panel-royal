using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpadStorePanel.Infrastructure.Dtos.Product
{
    public class ProductGridDto
    {
        public List<ProductWithPriceDto> Products { get; set; }
        public int Count { get; set; }
    }
    public class ProductWithPriceDto
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string ShortTitle { get; set; }
        public int ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public long Price { get; set; }
        public long PriceAfterDiscount { get; set; }
        public int DiscountPercentage { get; set; }
        public int Rate { get; set; }
    }

    public class ProductGroupWithProductCountDto
    {
        public ProductGroup ProductGroup { get; set; }
        public int ProductCount { get; set; }
    }
}
