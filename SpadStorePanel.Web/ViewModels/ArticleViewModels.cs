using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SpadStorePanel.Web.ViewModels
{
    public class ArticleFormViewModel
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
        public int ArticleCategoryId { get; set; }
        public HttpPostedFileBase ArticleImage { get; set; }

        public List<ArticleHeadLineViewModel> ArticleHeadLines { get; set; }
    }

    public class ArticleHeadLineViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Description { get; set; }
    }

    public class ArticleInfoViewModel
    {
        public ArticleInfoViewModel()
        {
            
        }

        public ArticleInfoViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.Author = article.User != null? $"{article.User.FirstName} {article.User.LastName}" : "-";
            this.ArticleCategory = article.ArticleCategory != null? article.ArticleCategory.Title : "-";
            this.PersianAddedDate = article.AddedDate != null? new PersianDateTime(article.AddedDate.Value).ToString() : "-";
            this.AddedDate = article.AddedDate;
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

    public class CommentWithPersianDateViewModel : ArticleComment
    {
        public CommentWithPersianDateViewModel(ArticleComment comment)
        {
            this.Comment = comment;
            this.PersianDate = comment.AddedDate != null ? new PersianDateTime(comment.AddedDate.Value).ToString() : "-";
        }
        public ArticleComment Comment { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public string PersianDate { get; set; }
    }









    public class TopArticlesViewModel
    {
        public TopArticlesViewModel()
        {
        }

        public TopArticlesViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.MainImage = article.MainImage;
            this.PersianDate = article.AddedDate != null ? new PersianDateTime(article.AddedDate.Value).ToString("d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string PersianDate { get; set; }
    }
    public class ArticleListViewModel
    {
        public ArticleListViewModel()
        {
        }

        public ArticleListViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.ShortDescription = article.ShortDescription;
            this.Author = article.User != null ? $"{article.User.FirstName} {article.User.LastName}" : "-";
            this.MainImage = article.MainImage;
            //this.AuthorAvatar = article.User.Avatar ?? "user-avatar.png";
            this.PersianDate = article.AddedDate != null ? new PersianDateTime(article.AddedDate.Value).ToString("d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string MainImage { get; set; }
        public string PersianDate { get; set; }
        public string Author { get; set; }
        //public string AuthorAvatar { get; set; }
        public string Role { get; set; }
        public int CommentCounter { get; set; }
    }

    public class ArticleCategoriesViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ArticleCount { get; set; }
    }

    public class ArticleDetailsViewModel
    {
        public ArticleDetailsViewModel()
        {

        }
        public ArticleDetailsViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.MainImage = article.MainImage;
            this.RightImage = article.RightImage;
            this.LeftImage = article.LeftImage;
            this.ShortDescription = article.ShortDescription;
            this.Description = article.Description;
            this.Author = article.User != null ? $"{article.User.FirstName} {article.User.LastName}" : "-";
            this.PersianDate = article.AddedDate != null ? new PersianDateTime(article.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string RightImage { get; set; }
        public string LeftImage { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string PersianDate { get; set; }
        public string SubTitles { get; set; }
        public List<ArticleHeadLine> HeadLines { get; set; }
        public List<ArticleTag> Tags { get; set; }
        public List<ArticleCommentViewModel> ArticleComments { get; set; }
        public CommentFormViewModel CommentForm { get; set; }
    }

    public class ArticleCommentViewModel
    {
        public ArticleCommentViewModel()
        {

        }

        public ArticleCommentViewModel(ArticleComment comment)
        {
            this.Id = comment.Id;
            this.ParentId = comment.ParentId;
            this.Name = comment.Name;
            this.Email = comment.Email;
            this.Message = comment.Message;
            this.AddedDate = comment.AddedDate != null ? new PersianDateTime(comment.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string AddedDate { get; set; }
    }

    public class CommentFormViewModel
    {
        public int? ParentId { get; set; }
        public int ArticleId { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} باید کمتر از 300 کارکتر باشد")]
        public string Name { get; set; }
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نا معتبر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Email { get; set; }
        [Display(Name = "کامنت")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800, ErrorMessage = "{0} باید کمتر از 800 کارکتر باشد")]
        public string Message { get; set; }

    }
    public class LatestArticlesViewModel
    {
        public LatestArticlesViewModel()
        {
        }

        public LatestArticlesViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.MainImage = article.MainImage;
            this.PersianDate = article.AddedDate != null ? new PersianDateTime(article.AddedDate.Value).ToString("d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string PersianDate { get; set; }
    }

}