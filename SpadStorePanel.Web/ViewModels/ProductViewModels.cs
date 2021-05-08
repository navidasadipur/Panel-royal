using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpadStorePanel.Web.ViewModels
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; }
    }

    public class NewProductViewModel
    {
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string DescriptionOneTitle { get; set; }
        public string DescriptionOneShortDescription { get; set; }
        public string DescriptionTwoTitle { get; set; }
        public string DescriptionTwoShortDescription { get; set; }
        public int Brand { get; set; }
        public int Rate { get; set; }
        public int ProductGroup { get; set; }
        public List<ProductFeaturesViewModel> ProductFeatures { get; set; }

    }

    public class ProductFeatures_SubFeaturesViewModel
    {
        public ProductFeatures_SubFeaturesViewModel()
        {
            SubFeatures = new List<string>();
        }

        public string Title { get; set; }
        public List<string> SubFeatures { get; set; }
    }

    public class ProductFeaturesViewModel
    {
        public int? ProductId { get; set; }
        public int FeatureId { get; set; }
        public int? SubFeatureId { get; set; }
        public string Value { get; set; }
        public bool IsMain { get; set; }
        public int? Quantity { get; set; }
        public long? Price { get; set; }
    }
    public class ProductCommentWithPersianDateViewModel : ProductComment
    {
        public ProductCommentWithPersianDateViewModel()
        {
        }
        public ProductCommentWithPersianDateViewModel(ProductComment comment)
        {
            Comment = comment;
            PersianDate = comment.AddedDate != null ? new PersianDateTime(comment.AddedDate.Value).ToString() : "-";
        }
        public ProductComment Comment { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public string PersianDate { get; set; }
    }


    public class NewProductGroupViewModel
    {
        public string Title { get; set; }
        public List<int> BrandIds { get; set; }
        public int ParentGroupId { get; set; }
        public List<int> ProductGroupFeatureIds { get; set; }
    }
    public class UpdateProductGroupViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> BrandIds { get; set; }
        public int ParentGroupId { get; set; }
        public List<int> ProductGroupFeatureIds { get; set; }
    }

    public class FeaturesObjViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class BrandsObjViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SubFeaturesObjViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }


    public class ProductFormViewModel
    {
        public int Id { get; set; }
        [Display(Name = "عنوان مقاله")]
        [MaxLength(600, ErrorMessage = "{0} باید از 600 کارکتر کمتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        [Display(Name = "توضیح")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Description { get; set; }
        public int ProductCategoryId { get; set; }
        public HttpPostedFileBase ArticleImage { get; set; }

        public List<ArticleHeadLineViewModel> ArticleHeadLines { get; set; }
    }

    public class ProductInfoViewModel
    {
        public ProductInfoViewModel()
        {

        }

        public ProductInfoViewModel(Product product)
        {
            Id = product.Id;
            Title = product.Title;
            //this.Author = product.User != null ? $"{product.User.FirstName} {product.User.LastName}" : "-";
            //this.ArticleCategory = product.ArticleCategory != null ? product.ArticleCategory.Title : "-";
            //this.PersianAddedDate = product.AddedDate != null ? new PersianDateTime(product.AddedDate.Value).ToString() : "-";
            //this.AddedDate = product.AddedDate;
        }
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        public string Title { get; set; }
        [Display(Name = "نویسنده")]
        public string Author { get; set; }
        [Display(Name = "دسته بندی")]
        public string ArticleCategory { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public string PersianAddedDate { get; set; }
        public DateTime? AddedDate { get; set; }
    }

    public class TopProductsViewModel
    {
        public TopProductsViewModel()
        {
        }

        public TopProductsViewModel(Product product)
        {
            Id = product.Id;
            Title = product.Title;
            //this.MainImage = product.MainImage;
            //this.PersianDate = product.AddedDate != null ? new PersianDateTime(product.AddedDate.Value).ToString("d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string PersianDate { get; set; }
    }

    //public class ProductDetailsViewModel
    //{
    //    public ProductDetailsViewModel()
    //    {

    //    }
    //    public ProductDetailsViewModel(Product product)
    //    {
    //        Id = product.Id;
    //        Title = product.Title;
    //        Image = product.Image;
    //        ShortDescription = product.ShortDescription;
    //        Description = product.Description;
    //        ProductGroup = product.ProductGroup;
    //        ProductMainFeatures = product.ProductMainFeatures;
    //        //this.Author = product.User != null ? $"{product.User.FirstName} {product.User.LastName}" : "-";
    //        //this.PersianDate = product.AddedDate != null ? new PersianDateTime(product.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
    //    }
    //    public int Id { get; set; }
    //    public string Title { get; set; }
    //    public string Image { get; set; }
    //    public string ShortDescription { get; set; }
    //    public string Description { get; set; }
    //    public string Author { get; set; }
    //    public string PersianDate { get; set; }
    //    public string SubTitles { get; set; }
    //    public bool IsAddedToCart { get; set; }

    //    public ICollection<ProductMainFeature> ProductMainFeatures { get; set; }
    //    public ProductGroup ProductGroup { get; set; }
    //    public List<ProductTag> Tags { get; set; }
    //    public List<ProductCommentViewModel> ProductComments { get; set; }
    //    public ProductCommentFormViewModel CommentForm { get; set; }
    //}

    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public List<ProductGallery> ProductGalleries { get; set; }
        public List<ProductMainFeature> ProductMainFeatures { get; set; }
        public List<ProductFeatureValue> ProductFeatureValues { get; set; }
        public long Price { get; set; }
        public long PriceAfterDiscount { get; set; }
        public List<ProductCommentViewModel> ProductComments { get; set; }
        public CommentFormViewModel CommentForm { get; set; }
    }

    public class AddToCartViewModel
    {
        public AddToCartViewModel()
        {
            MainFeatures = new List<Feature>();
            SubFeatures = new List<SubFeature>();
        }

        public int ProductId { get; set; }
        public List<Feature> MainFeatures { get; set; }
        public List<SubFeature> SubFeatures { get; set; }

    }


    public class ProductListViewModel
    {
        public ProductListViewModel()
        {
        }

        public ProductListViewModel(Product product)
        {
            Id = product.Id;
            Title = product.Title;
            ShortDescription = product.ShortDescription;
            //this.Author = product.User != null ? $"{product.User.FirstName} {product.User.LastName}" : "-";
            Image = product.Image;
            Rate = product.Rate;
            ProductMainFeatures = product.ProductMainFeatures.ToList();
            //this.AuthorAvatar = product.User.Avatar ?? "user-avatar.png";
            //this.PersianDate = product.AddedDate != null ? new PersianDateTime(product.AddedDate.Value).ToString("d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Image { get; set; }
        public string PersianDate { get; set; }
        public string Author { get; set; }
        //public string AuthorAvatar { get; set; }
        public string Role { get; set; }
        public int CommentCounter { get; set; }
        public int Rate { get; set; }

        public List<ProductMainFeature> ProductMainFeatures { get; set; }
    }

    public class ProductCategoriesViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int ProductCount { get; set; }
    }

    //public class ProductFeaturesSearchViewModel
    //{
    //    public ProductFeaturesSearchViewModel()
    //    {
    //        SubFeatures = new List<SubFeatureViewModel>();
    //    }
    //    public int Id { get; set; }
    //    public string Title { get; set; }
    //    public ICollection<SubFeatureViewModel> SubFeatures { get; set; }
    //}

    public class Color_SizeSearchViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string OtherInfo { get; set; }
        public int ProductCount { get; set; }
    }

    public class ColorViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string OtherInfo { get; set; }
        public int ProductCount { get; set; }
    }

    public class SizeViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string OtherInfo { get; set; }
        public int ProductCount { get; set; }
    }

    public class SearchViewModel
    {
        public SearchViewModel()
        {
            //ColorViewModels = new List<ColorViewModel>();
            //SizeViewModels = new List<SizeViewModel>();
            //ProductCategoriesViewModels = new List<ProductCategoriesViewModel>();
        }

        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        //public List<ColorViewModel> ColorViewModels { get; set; }
        //public List<SizeViewModel> SizeViewModels { get; set; }
        //public List<ProductCategoriesViewModel> ProductCategoriesViewModels { get; set; }
    }



    public class ProductCommentViewModel
    {
        public ProductCommentViewModel()
        {

        }

        public ProductCommentViewModel(ProductComment comment)
        {
            Id = comment.Id;
            ParentId = comment.ParentId;
            Name = comment.Name;
            Email = comment.Email;
            Message = comment.Message;
            AddedDate = comment.AddedDate != null ? new PersianDateTime(comment.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string AddedDate { get; set; }
    }

    public class ProductCommentFormViewModel
    {
        public int? ParentId { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} باید کمتر از 300 کارکتر باشد")]
        public string Name { get; set; }
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نا معتبر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Email { get; set; }
        [Display(Name = "پیام")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800, ErrorMessage = "{0} باید کمتر از 800 کارکتر باشد")]
        public string Message { get; set; }
    }

    public class LatestProductsViewModel
    {
        public LatestProductsViewModel()
        {
        }

        public LatestProductsViewModel(Product product)
        {
            Id = product.Id;
            Title = product.Title;
            //this.MainImage = product.MainImage;
            //this.PersianDate = product.AddedDate != null ? new PersianDateTime(product.AddedDate.Value).ToString("d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string PersianDate { get; set; }
    }

    public class ProductDescriptionViewModel
    {
        public string DescriptionOneTitle { get; set; }
        public string DescrriptionOneShortDescription { get; set; }
        public string DescrriptionOneImage { get; set; }

        public string DescriptionTwoTitle { get; set; }
        public string DescrriptionTwoShortDescription { get; set; }
        public string DescrriptionTwoImage { get; set; }
    }

    public class ProductFeatureViewModel
    {
        public string FeatureTitle { get; set; }
        public List<string> FeatureValues { get; set; }
    }
}