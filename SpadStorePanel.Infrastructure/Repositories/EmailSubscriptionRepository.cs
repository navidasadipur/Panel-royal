using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Identity;
using SpadStorePanel.Infrastructure.Filters;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Net.Mail;
using SpadStorePanel.Infrastructure;

namespace SpadStorePanel.Infratructure.Repositories
{
    public class EmailSubscriptionRepository : IDisposable
    {
        private readonly MyDbContext _context;

        public EmailSubscriptionRepository()
        {
            _context = new MyDbContext();
        }

        public List<EmailSubscription> GetAll()
        {
            return _context.EmailSubscriptions.Where(e => e.IsDeleted == false).ToList();
        }

        public EmailSubscription Get(int id)
        {
            return _context.EmailSubscriptions.FirstOrDefault(e => e.IsDeleted == false && e.Id==id);
        }

        public void Create(EmailSubscription emailSubscription)
        {
            _context.EmailSubscriptions.Add(emailSubscription);
            _context.SaveChanges();
        }

        public void Delete(EmailSubscription emailSubscription)
        {
            emailSubscription = _context.EmailSubscriptions.FirstOrDefault(s => s.Id == emailSubscription.Id);
            _context.EmailSubscriptions.Remove(emailSubscription);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var emailSubscription = _context.EmailSubscriptions.FirstOrDefault(s => s.Id == id);
            emailSubscription.IsDeleted = true;
            _context.SaveChanges();
        }

        protected void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}
