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
}