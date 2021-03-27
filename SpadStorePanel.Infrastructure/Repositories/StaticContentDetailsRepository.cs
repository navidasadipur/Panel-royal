using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class StaticContentDetailsRepository : BaseRepository<StaticContentDetail, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public StaticContentDetailsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public StaticContentDetail GetStaticContentDetail(int id)
        {
            return _context.StaticContentDetails.Where(a => a.IsDeleted == false).Include(a => a.StaticContentType).SingleOrDefault();
        }
        public List<StaticContentDetail> GetStaticContentDetails()
        {
            return _context.StaticContentDetails.Where(e=>e.IsDeleted == false).Include(a => a.StaticContentType).ToList();
        }

        public List<StaticContentDetail> GetStaticContentDetailsByStaticContentTypeId(int id)
        {
            return _context.StaticContentDetails.Where(e => e.IsDeleted == false && e.StaticContentTypeId == id).Include(a => a.StaticContentType).ToList();
        }

    }
}
