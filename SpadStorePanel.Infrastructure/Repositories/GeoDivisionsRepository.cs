using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class GeoDivisionsRepository : BaseRepository<GeoDivision, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public GeoDivisionsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<GeoDivision> GetGeoDivisionsByType(int type)
        {
            return _context.GeoDivisions.Where(g => g.IsDeleted == false && g.GeoDivisionType == type).ToList();
        }
    }
}
