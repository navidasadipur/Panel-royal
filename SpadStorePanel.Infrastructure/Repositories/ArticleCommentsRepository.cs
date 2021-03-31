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
    public class ArticleTagsRepository : BaseRepository<ArticleTag, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ArticleTagsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<ArticleTag> GetArticleTags(int articleId)
        {
            return _context.ArticleTags.Where(h => h.ArticleId == articleId & h.IsDeleted == false).ToList();
        }

        public string GetArticleName(int articleId)
        {
            return _context.Articles.Find(articleId).Title;
        }

        public ArticleTag DeleteTag(int id)
        {
            var tag = _context.ArticleTags.Find(id);
            
            //foreach (var child in children)
            //{
            //    child.IsDeleted = true;
            //    _context.Entry(child).State = EntityState.Modified;
            //    _context.SaveChanges();
            //}

            tag.IsDeleted = true;
            _context.Entry(tag).State = EntityState.Modified;
            _context.SaveChanges();
            _logger.LogEvent(tag.GetType().Name, tag.Id, "Delete");
            return tag;
        }
    }
}
