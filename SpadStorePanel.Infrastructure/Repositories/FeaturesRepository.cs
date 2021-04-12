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

            var subFeatures = new List<SubFeature>();

            //finding feature ids of a specific product
            var productFeatureValues = _context.ProductFeatureValues.Where(pfv => pfv.IsDeleted == false && pfv.ProductId == productId).ToList();

            var featureIds = productFeatureValues.Where(pfv => pfv.FeatureId != null).Select(pfv => pfv.FeatureId).ToList();

            //foreach (var id in featureIds)
            //{
            //    var feature = _context.Features.Where(f => f.IsDeleted == false && f.Id == id).ToList();

            //    features.AddRange(feature);
            //}

            //foreach (var feature in features)
            //{

            //    var subFeatures = _context.SubFeatures.Where(sf => sf.IsDeleted == false && sf.FeatureId == feature.Id).ToList();

            //    feature.SubFeatures = subFeatures;
            //}

            //finding and getting all features of that specific product
            foreach (var featureId in featureIds)
            {
                var feature = _context.Features.Where(f => f.IsDeleted == false && f.Id == featureId).FirstOrDefault();

                if (feature != null)
                {
                    features.Add(feature);
                }
            }

            //finding and getting all subfeatures of all features of specific product
            foreach (var feature in features)
            {
                var subFeatureIds = _context.ProductFeatureValues.Where(pfv => pfv.FeatureId == feature.Id && pfv.SubFeatureId != null).Select(pfv => pfv.SubFeatureId).ToList();

                foreach (var subFeatureId in subFeatureIds)
                {
                    var subFeature = _context.SubFeatures.Where(sf => sf.IsDeleted == false && sf.Id == subFeatureId).FirstOrDefault();

                    if (subFeature != null)
                    {
                        feature.SubFeatures.Add(subFeature);
                    }
                }
            }

            return features;
        }
    }
}
