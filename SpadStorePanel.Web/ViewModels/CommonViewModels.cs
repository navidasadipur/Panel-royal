using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Web.ViewModels
{
    public class DiscountFormViewModel
    {
        [DisplayName("عنوان تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} باید کمتر از 500 کارکتر باشد")]
        public string Title { get; set; }
        public int DiscountType { get; set; }
        [DisplayName("میزان تخفیف")]
        [Required(ErrorMessage = "لطفا میزان تخفیف را وارد کنید")]
        public long Amount { get; set; }
        public List<int> BrandIds { get; set; }
        public List<int> ProductIds { get; set; }
        public List<int> ProductGroupIds { get; set; }
        public bool IsOffer { get; set; }
        public int? OfferId { get; set; }

        // Edit Props
        public string GroupIdentifier { get; set; }
        public List<Discount> PreviousDiscounts { get; set; }
    }
}