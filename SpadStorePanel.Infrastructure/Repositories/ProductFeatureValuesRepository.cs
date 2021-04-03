using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public int GetProductsCountBySubFeatureId(int id)
        {
            return _context.ProductFeatureValues
                .Count(pfv => pfv.IsDeleted == false && pfv.SubFeatureId == id);
        }

    }
}
