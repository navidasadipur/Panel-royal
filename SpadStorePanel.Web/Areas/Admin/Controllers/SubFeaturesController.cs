using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure.Repositories;

namespace SpadStorePanel.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class SubFeaturesController : Controller
    {
        private readonly SubFeaturesRepository _repo;
        public SubFeaturesController(SubFeaturesRepository repo)
        {
            _repo = repo;
        }
        // GET: Admin/SubFeatures
        public ActionResult Index(int featureId)
        {
            ViewBag.FeatureName = _repo.GetFeatureName(featureId);
            ViewBag.FeatureId = featureId;
            return View(_repo.GetSubFeatures(featureId));
        }

        // GET: Admin/SubFeatures/Create
        public ActionResult Create(int featureId)
        {
            ViewBag.FeatureId = featureId;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubFeature subFeature)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(subFeature);
                return RedirectToAction("Index",new {featureId = subFeature.FeatureId});
            }

            return View(subFeature);
        }

        // GET: Admin/SubFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubFeature subFeature = _repo.Get(id.Value);
            if (subFeature == null)
            {
                return HttpNotFound();
            }
            return PartialView(subFeature);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubFeature subFeature)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(subFeature);
                return RedirectToAction("Index", new { featureId = subFeature.FeatureId });
            }
            return View(subFeature);
        }

        // GET: Admin/SubFeatures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubFeature subFeature = _repo.Get(id.Value);
            if (subFeature == null)
            {
                return HttpNotFound();
            }
            return PartialView(subFeature);
        }

        // POST: Admin/SubFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var featureId = _repo.Get(id).FeatureId;
            _repo.Delete(id);
            return RedirectToAction("Index", new { featureId });
        }
    }
}