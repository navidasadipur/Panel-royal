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

    public class NavbarViewModel
    {
        public NavbarViewModel()
        {
            ProductCategories = new List<ProductGroup>();
        }
        public List<ProductGroup> ProductCategories { get; set; }
        public List<Product> Products { get; set; }
    }

    public class AboutViewModel
    {
        [DataType(DataType.MultilineText)]
        public string AboutDescription { get; set; }
    }
    public class FooterViewModel
    {
        public StaticContentDetail Phone { get; set; }
        //public StaticContentDetail Email { get; set; }
        public StaticContentDetail Address { get; set; }
        public StaticContentDetail Youtube { get; set; }
        public StaticContentDetail Instagram { get; set; }
        public StaticContentDetail Twitter { get; set; }
        public StaticContentDetail Facebook { get; set; }
        public StaticContentDetail Pinterest { get; set; }
        public StaticContentDetail SiteTitle { get; set; }
        public StaticContentDetail CopyRightShortDescrioption { get; set; }
        public StaticContentDetail ImplementationShortDescription { get; set; }
        public StaticContentDetail CompanyServices { get; set; }
    }
    public class ContactUsViewModel
    {
        public StaticContentDetail Map { get; set; }
        public StaticContentDetail ContactInfo { get; set; }
        public StaticContentDetail Phone { get; set; }
        public StaticContentDetail Email { get; set; }
        public StaticContentDetail Address { get; set; }
        public StaticContentDetail WorkingHours { get; set; }
        public StaticContentDetail Youtube { get; set; }
        public StaticContentDetail Instagram { get; set; }
        public StaticContentDetail Twitter { get; set; }
        public StaticContentDetail Facebook { get; set; }
        public StaticContentDetail Pinterest { get; set; }

        [MaxLength(600)]
        [Display(Name = "نام *")]
        [Required(ErrorMessage = "لطفا {0} خود را وارد کنید")]
        public string Name { get; set; }

        [Display(Name = "ایمیل *")]
        [Required(ErrorMessage = "لطفا {0} خود را وارد کنید")]
        [EmailAddress(ErrorMessage = "{0} وارد شده معتبر نیست")]
        [MaxLength(600)]
        public string CustomerEmail { get; set; }

        [Display(Name = "پیام شما *")]
        [Required(ErrorMessage = "لطفا {0} خود را وارد کنید")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }

    public class GalleryPageViewModel
    {
        public List<ProductGallery> Images { get; set; }
        //public List<ProductGalleryVideo> Videos { get; set; }
    }

    public class InstaGalleryViewModel
    {
        public InstaGalleryViewModel()
        {
            this.Images = new List<StaticContentDetail>();
        }

        public List<StaticContentDetail> Images { get; set; }
        //public List<ProductGalleryVideo> Videos { get; set; }
    }

    public class HomeTopSliderViewModel
    {
        public HomeTopSliderViewModel()
        {
            this.Slides = new List<StaticContentDetail>();
            this.LogoAndButton = new StaticContentDetail();
        }

        public List<StaticContentDetail> Slides { get; set; }

        public StaticContentDetail LogoAndButton { get; set; }
    }

}