using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SpadStorePanel.Core.Models
{
    public class ProductTag : IBaseEntity
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
