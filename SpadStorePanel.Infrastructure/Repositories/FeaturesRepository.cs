using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class FeaturesRepository : BaseRepository<Feature, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public FeaturesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Feature> GetAllFeaturesWithSubFeatures()
        {
            var allFeatures = _context.Features.Where(f => f.IsDeleted == false).ToList();

            foreach (var feature in allFeatures)
            {
                 
                var subFeatures = _context.SubFeatures.Where(fv => fv.IsDeleted == false && fv.FeatureId == feature.Id).ToList();

                feature.SubFeatures = subFeatures;
            }

            return allFeatures;
        }

        public List<Feature> GetFeaturesWithSubFeaturesByProductId(int productId)
        {
            var features = new List<Feature>();

            var productFeatureValues = _context.ProductFeatureValues.Where(pfv => pfv.IsDeleted == false && pfv.ProductId == productId).ToList();

            var featureIds = productFeatureValues.Select(pfv => pfv.FeatureId).ToList();

            foreach (var id in featureIds)
            {
                var feature = _context.Features.Where(f => f.IsDeleted == false && f.Id == id).ToList();

                features.AddRange(feature);
            }

            foreach (var feature in features)
            {

                var subFeatures = _context.SubFeatures.Where(sf => sf.IsDeleted == false && sf.FeatureId == feature.Id).ToList();

                feature.SubFeatures = subFeatures;
            }

            return features;
        }
    }
}
