using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Web.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public Customer Customer { get; set; }
        public List<CustomerInvoiceViewModel> Invoices { get; set; }
    }
    public class CustomerInvoiceViewModel
    {
        public CustomerInvoiceViewModel()
        {

        }

        public CustomerInvoiceViewModel(Invoice invoice)
        {
            this.Id = invoice.Id;
            this.InvoiceNumber = invoice.InvoiceNumber;
            this.IsPayed = invoice.IsPayed;
            this.RegisteredDate = new PersianDateTime(invoice.AddedDate).ToString("dddd d MMMM yyyy");
            this.AddedDate = invoice.AddedDate;
            this.Price = invoice.TotalPrice.ToString("##,###");
        }
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string RegisteredDate { get; set; }
        public string Price { get; set; }
        public bool IsPayed { get; set; }
        public DateTime AddedDate { get; set; }
    }
    public class RegisterCustomerViewModel
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public string LastName { get; set; }
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public string UserName { get; set; }
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [Phone(ErrorMessage = "شماره موبایل وارد شده معتبر نیست")]
        public string Mobile { get; set; }
    }
}