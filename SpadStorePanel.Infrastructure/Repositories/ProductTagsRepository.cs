using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class ProductTagsRepository : BaseRepository<ProductComment, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductTagsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<ProductComment> GetProductComments(int articleId)
        {
            return _context.ProductComments.Where(h => h.ProductId == articleId & h.IsDeleted == false).ToList();
        }
        public string GetProductName(int articleId)
        {
            return _context.Products.Find(articleId).Title;
        }
        public ProductComment DeleteComment(int id)
        {
            var comment = _context.ProductComments.Find(id);
            var children = _context.ProductComments.Where(c=>c.ParentId == id).ToList();
            foreach (var child in children)
            {
                child.IsDeleted = true;
                _context.Entry(child).State = EntityState.Modified;
                _context.SaveChanges();
            }
            comment.IsDeleted = true;
            _context.Entry(comment).State = EntityState.Modified;
            _context.SaveChanges();
            _logger.LogEvent(comment.GetType().Name, comment.Id, "Delete");
            return comment;
        }
    }
}
