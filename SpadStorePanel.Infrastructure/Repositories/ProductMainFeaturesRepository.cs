using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class ProductMainFeaturesRepository : BaseRepository<ProductMainFeature, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductMainFeaturesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public ProductMainFeature GetByProductId(int productId)
        {
            return _context.ProductMainFeatures.FirstOrDefault(f => f.IsDeleted == false && f.ProductId == productId );
        }
        public ProductMainFeature GetByProductId(int productId, int mainFeatureId)
        {
            return _context.ProductMainFeatures.FirstOrDefault(f => f.IsDeleted == false && f.ProductId == productId && f.Id == mainFeatureId);
        }
        public List<ProductMainFeature> GetProductMainFeatures(int productId)
        {
            return _context.ProductMainFeatures.Include(f => f.Feature).Include(f => f.SubFeature).Where(f => f.IsDeleted == false && f.ProductId == productId).ToList();
        }

        public List<ProductMainFeature> GetProductMainFeaturesBySubFeatureId(int subFeatureId)
        {
            return _context.ProductMainFeatures.Where(f => f.IsDeleted == false && f.SubFeatureId == subFeatureId).ToList();
        }

        public long GetMinPrice()
        {
            var minPrice = _context.ProductMainFeatures.Where(f => f.IsDeleted == false && f.SubFeatureId != null).Min(f => f.Price);
            return minPrice;
        }

        public long GetMaxPrice()
        {
            var maxPrice = _context.ProductMainFeatures.Where(f => f.IsDeleted == false && f.SubFeatureId != null).Max(f => f.Price);
            return maxPrice;
        }
    }
}
