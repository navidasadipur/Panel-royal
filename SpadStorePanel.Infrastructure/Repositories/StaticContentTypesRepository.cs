using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class StaticContentTypesRepository : BaseRepository<StaticContentType, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public StaticContentTypesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<StaticContentType> GetStaticContentTypes()
        {
            return _context.StaticContentTypes.Where(e => e.IsDeleted == false).ToList();
        }

        public StaticContentType GetStaticContentType(int Id)
        {
            return _context.StaticContentTypes.FirstOrDefault(e => e.IsDeleted == false && e.Id == Id);
        }
    }
}