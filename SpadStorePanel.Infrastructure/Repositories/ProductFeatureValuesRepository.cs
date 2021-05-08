using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class ProductFeatureValuesRepository : BaseRepository<ProductFeatureValue, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductFeatureValuesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        //public List<Product> GetProductsBySubFeatureId(int id)
        //{
        //    var productIds = _context.ProductFeatureValues.Where(pfv => pfv.SubFeatureId == id && pfv.IsDeleted == false);

        //        _context.ProductFeatureValues.Where(p => p.ProductId == id && p.IsDeleted == false).ToList();
        //}

        public List<ProductFeatureValue> GetProductFeatures(int productId)
        {
            return _context.ProductFeatureValues.Include(f => f.Feature).Include(f => f.SubFeature)
                .Where(f => f.IsDeleted == false && f.ProductId == productId)/*.OrderBy(f => f.Feature.OrderPriority)*/.ToList();
        }


        public int GetProductsCountBySubFeatureId(int id)
        {
            var allProductFeatureValues = _context.ProductFeatureValues.Where(pfv => pfv.IsDeleted == false && pfv.SubFeatureId == id);

            var allProductIds = allProductFeatureValues.Select(pfv => pfv.ProductId).ToList();
            //distinct product ids
            allProductIds = allProductIds.GroupBy(i => i.Value).Select(i => i.First()).ToList();

            return allProductIds.Count();
        }

    }
}
