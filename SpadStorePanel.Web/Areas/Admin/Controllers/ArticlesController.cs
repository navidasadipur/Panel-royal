using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure;
using SpadStorePanel.Infrastructure.Helpers;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;

namespace SpadStorePanel.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        private readonly ArticlesRepository _repo;
        public ArticlesController(ArticlesRepository repo)
        {
            _repo = repo;
        }
        // GET: Admin/Articles
        public ActionResult Index()
        {
            var articles = _repo.GetArticles();
            var articlesListVm = new List<ArticleInfoViewModel>();
            foreach (var article in articles)
            {
                var articleVm = new ArticleInfoViewModel(article);
                articlesListVm.Add(articleVm);
            }
            return View(articlesListVm);
        }
        // GET: Admin/Articles/Create
        public ActionResult Create()
        {
            ViewBag.ArticleCategoryId = new SelectList(_repo.GetArticleCategories(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article, HttpPostedFileBase ArticleMainImage, HttpPostedFileBase ArticleRightImage, HttpPostedFileBase ArticleLeftImage, string Tags)
        {
            if (ModelState.IsValid)
            {

                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    ViewBag.Message = "کاربر وارد کننده پیدا نشد.";
                    return View(article);
                }


                #region Upload Main Image
                if (ArticleMainImage != null)
                {
                    var newFileName = Guid.NewGuid() + Path.GetExtension(ArticleMainImage.FileName);
                    ArticleMainImage.SaveAs(Server.MapPath("/Files/ArticleImages/Image/" + newFileName));

                    ImageResizer thumb = new ImageResizer();
                    thumb.Resize(Server.MapPath("/Files/ArticleImages/Image/" + newFileName),
                        Server.MapPath("/Files/ArticleImages/Thumb/" + newFileName));

                    article.MainImage = newFileName;
                }
                #endregion

                #region Upload Right Image
                if (ArticleRightImage != null)
                {
                    var newFileName = Guid.NewGuid() + Path.GetExtension(ArticleRightImage.FileName);
                    ArticleRightImage.SaveAs(Server.MapPath("/Files/ArticleImages/Image/" + newFileName));

                    ImageResizer thumb = new ImageResizer();
                    thumb.Resize(Server.MapPath("/Files/ArticleImages/Image/" + newFileName),
                        Server.MapPath("/Files/ArticleImages/Thumb/" + newFileName));

                    article.RightImage = newFileName;
                }
                #endregion

                #region Upload Left Image
                if (ArticleLeftImage != null)
                {
                    var newFileName = Guid.NewGuid() + Path.GetExtension(ArticleLeftImage.FileName);
                    ArticleLeftImage.SaveAs(Server.MapPath("/Files/ArticleImages/Image/" + newFileName));

                    ImageResizer thumb = new ImageResizer();
                    thumb.Resize(Server.MapPath("/Files/ArticleImages/Image/" + newFileName),
                        Server.MapPath("/Files/ArticleImages/Thumb/" + newFileName));

                    article.LeftImage = newFileName;
                }
                #endregion

                _repo.AddArticle(article);

                if (!string.IsNullOrEmpty(Tags))
                    _repo.AddArticleTags(article.Id, Tags);

                return RedirectToAction("Index");
            }
            ViewBag.Tags = Tags;
            ViewBag.ArticleCategoryId = new SelectList(_repo.GetArticleCategories(), "Id", "Title", article.ArticleCategoryId);
            return View(article);
        }

        // GET: Admin/Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = _repo.GetArticle(id.Value);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tags = _repo.GetArticleTagsStr(id.Value);
            ViewBag.ArticleCategoryId = new SelectList(_repo.GetArticleCategories(), "Id", "Title", article.ArticleCategoryId);
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Article article, HttpPostedFileBase ArticleMainImage, HttpPostedFileBase ArticleRightImage, HttpPostedFileBase ArticleLeftImage, string Tags)
        {
            if (ModelState.IsValid)
            {
                #region Upload Main Image
                if (ArticleMainImage != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Image/" + article.MainImage)))
                        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Image/" + article.MainImage));

                    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Thumb/" + article.MainImage)))
                        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Thumb/" + article.MainImage));

                    var newFileName = Guid.NewGuid() + Path.GetExtension(ArticleMainImage.FileName);
                    ArticleMainImage.SaveAs(Server.MapPath("/Files/ArticleImages/Image/" + newFileName));

                    ImageResizer thumb = new ImageResizer();
                    thumb.Resize(Server.MapPath("/Files/ArticleImages/Image/" + newFileName), Server.MapPath("/Files/ArticleImages/Thumb/" + newFileName));
                    article.MainImage = newFileName;
                }
                #endregion

                #region Upload Right Image
                if (ArticleRightImage != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Image/" + article. RightImage)))
                        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Image/" + article.RightImage));

                    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Thumb/" + article.RightImage)))
                        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Thumb/" + article.RightImage));

                    var newFileName = Guid.NewGuid() + Path.GetExtension(ArticleRightImage.FileName);
                    ArticleRightImage.SaveAs(Server.MapPath("/Files/ArticleImages/Image/" + newFileName));

                    ImageResizer thumb = new ImageResizer();
                    thumb.Resize(Server.MapPath("/Files/ArticleImages/Image/" + newFileName), Server.MapPath("/Files/ArticleImages/Thumb/" + newFileName));
                    article.RightImage = newFileName;
                }
                #endregion

                #region Upload Left Image
                if (ArticleLeftImage != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Image/" + article.LeftImage)))
                        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Image/" + article.LeftImage));

                    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Thumb/" + article.LeftImage)))
                        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Thumb/" + article.LeftImage));

                    var newFileName = Guid.NewGuid() + Path.GetExtension(ArticleLeftImage.FileName);
                    ArticleLeftImage.SaveAs(Server.MapPath("/Files/ArticleImages/Image/" + newFileName));

                    ImageResizer thumb = new ImageResizer();
                    thumb.Resize(Server.MapPath("/Files/ArticleImages/Image/" + newFileName), Server.MapPath("/Files/ArticleImages/Thumb/" + newFileName));
                    article.LeftImage = newFileName;
                }
                #endregion

                _repo.Update(article);

                if (!string.IsNullOrEmpty(Tags))
                    _repo.AddArticleTags(article.Id, Tags);

                return RedirectToAction("Index");
            }
            ViewBag.Tags = Tags;
            ViewBag.ArticleCategoryId = new SelectList(_repo.GetArticleCategories(), "Id", "Title", article.ArticleCategoryId);
            return View(article);
        }

        [HttpPost]
        public ActionResult FileUpload()
        {
            var files = HttpContext.Request.Files;
            foreach (var fileName in files)
            {
                HttpPostedFileBase file = Request.Files[fileName.ToString()];
                var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                file.SaveAs(Server.MapPath("/Files/ArticleImages/" + newFileName));
                TempData["UploadedFile"] = newFileName;
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        // GET: Admin/Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = _repo.GetArticle(id.Value);
            if (article == null)
            {
                return HttpNotFound();
            }
            return PartialView(article);
        }

        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var article = _repo.Get(id);

            //#region Delete Article Image
            //if (article.Image != null)
            //{
            //    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Image/" + article.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Image/" + article.Image));

            //    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Thumb/" + article.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Thumb/" + article.Image));
            //}
            //#endregion

            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
