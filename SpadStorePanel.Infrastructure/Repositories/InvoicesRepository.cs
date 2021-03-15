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
            return _context.Invoices.Include(i => i.Customer.User).ToList();
        }
        public Invoice GetInvoice(int invoiceId)
        {
            return _context.Invoices.Include(i => i.Customer.User).Include(i=>i.InvoiceItems).FirstOrDefault(i=>i.Id == invoiceId);
        }

        public string GetInvoiceItemsMainFeature(int invoiceItemId)
        {
            var invoiceItem = _context.InvoiceItems.Find(invoiceItemId);
            var mainFeature = _context.ProductMainFeatures.Include(m=>m.SubFeature).FirstOrDefault(m=>m.Id == invoiceItem.MainFeatureId);
            return mainFeature.SubFeature.Value;
        }
    }
}
