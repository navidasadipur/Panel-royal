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
    }
}
