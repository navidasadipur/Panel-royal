using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class InvoicesRepository : BaseRepository<Invoice, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public InvoicesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Invoice> GetInvoices()
        {
            return _context.Invoices.Include(i => i.Customer.User).Where(i => i.IsDeleted == false).ToList();
        }
        public Invoice GetInvoice(int invoiceId)
        {
            return _context.Invoices.Include(i => i.Customer.User).Where(i => i.IsDeleted == false).Include(i => i.InvoiceItems).FirstOrDefault(i => i.Id == invoiceId);
        }

        public string GetInvoiceItemsMainFeature(int invoiceItemId)
        {
            var invoiceItem = _context.InvoiceItems.Find(invoiceItemId);
            var mainFeature = _context.ProductMainFeatures.Include(m => m.SubFeature).FirstOrDefault(m => m.Id == invoiceItem.MainFeatureId);
            return mainFeature.SubFeature.Value;
        }

        public List<Product> GertTopSoldProducts(int take)
        {
            List<Product> products = new List<Product>();
            var productIds = _context.InvoiceItems.GroupBy(i => i.ProductId)
                .OrderByDescending(pi => pi.Count())
                .Select(g => g.Key).ToList();
            foreach (var id in productIds)
            {
                if (products.Count < take)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == id);
                    if (product != null && product.IsDeleted == false)
                    {
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        public List<Invoice> GetCustomerInvoices(int customerId)
        {
            return _context.Invoices.Where(i => i.IsDeleted == false && i.CustomerId == customerId).ToList();
        }

        public InvoiceItem AddInvoiceItem(InvoiceItem invoiceItem)
        {
            var user = GetCurrentUser();
            invoiceItem.InsertDate = DateTime.Now;
            if (user != null)
                invoiceItem.InsertUser = user.UserName;

            _context.InvoiceItems.Add(invoiceItem);
            _context.SaveChanges();

            return invoiceItem;
        }
        public List<InvoiceItem> GetInvoiceItemsByInvoiceId(int id)
        {
            return _context.InvoiceItems.Include(i => i.Product).Where(i => i.IsDeleted == false && i.InvoiceId == id)
                .ToList();
        }
        public Invoice GetInvoiceWithGeo(int id)
        {
            return _context.Invoices.Include(i => i.GeoDivision).FirstOrDefault(i => i.Id == id);
        }
    }
}
