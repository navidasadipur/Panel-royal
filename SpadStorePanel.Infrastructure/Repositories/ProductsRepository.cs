using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class ProductsRepository : BaseRepository<Product, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Where(p => p.IsDeleted == false).Include(a => a.ProductGroup).ToList();
        }

        public Product GetProduct(int id)
        {
            var product = _context.Products.Include(p => p.ProductGroup).Include(p => p.ProductMainFeatures).Include(p => p.ProductFeatureValues)
                .FirstOrDefault(p => p.Id == id);
            product.ProductMainFeatures = product.ProductMainFeatures.Where(f => f.IsDeleted == false).ToList();
            product.ProductFeatureValues = product.ProductFeatureValues.Where(f => f.IsDeleted == false).ToList();
            return product;
        }
        public List<ProductMainFeature> GetProductMainFeatures(int id)
        {
            return _context.ProductMainFeatures.Where(p=>p.ProductId == id && p.IsDeleted == false).ToList();
        }
        public List<ProductFeatureValue> GetProductFeatures(int id)
        {
            return _context.ProductFeatureValues.Where(p => p.ProductId == id && p.IsDeleted == false).ToList();
        }
        public List<SubFeature> GetSubFeaturesByFeatureId(int id)
        {
            return _context.SubFeatures.Where(p => p.IsDeleted == false && p.FeatureId == id).ToList();
        }

        public ProductMainFeature AddProductMainFeature(ProductMainFeature mainFeature)
        {
            var user = GetCurrentUser();
            mainFeature.InsertDate = DateTime.Now;
            mainFeature.InsertUser = user.UserName;
            _context.ProductMainFeatures.Add(mainFeature);
            _context.SaveChanges();

            _logger.LogEvent(mainFeature.GetType().Name, mainFeature.Id, "Add");
            return mainFeature;
        }
        public ProductFeatureValue AddProductFeature(ProductFeatureValue feature)
        {
            var user = GetCurrentUser();
            feature.InsertDate = DateTime.Now;
            feature.InsertUser = user.UserName;
            _context.ProductFeatureValues.Add(feature);
            _context.SaveChanges();

            _logger.LogEvent(feature.GetType().Name, feature.Id, "Add");
            return feature;
        }

        public List<Product> getProductsByGroupId(int id)
        {
            var allProducts = _context.Products.Where(g => g.IsDeleted == false).Include(g => g.ProductGroup);

            var ProductIdCategory = allProducts.Where(g => g.ProductGroup.Id == id).OrderByDescending(g => g.Id).ToList();

            return ProductIdCategory;
        }

        //public void UpdateProductViewCount(int productId)
        //{
        //    var product = _context.Products.Find(productId);
        //    product. ViewCount++;
        //    _context.Entry(product).State = EntityState.Modified;
        //    _context.SaveChanges();
        //}

        public List<ProductComment> GetProductComments(int productId)
        {
            return _context.ProductComments.Where(c => c.IsDeleted == false && c.ProductId == productId).ToList();
        }

        public void AddComment(ProductComment comment)
        {
            _context.ProductComments.Add(comment);
            _context.SaveChanges();
        }

        public List<ProductTag> GetProductTags(int productId)
        {
            return _context.ProductTags.Where(c => c.IsDeleted == false && c.ProductId == productId).ToList();
        }
    }
}
