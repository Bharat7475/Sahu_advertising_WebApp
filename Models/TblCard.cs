using System;
using System.Collections.Generic;

namespace Hoarding_managment.Models
{
    public partial class TblCard
    {
        public int Id { get; set; }
        public int? FkVendorId { get; set; }
        public int? FkCustomer { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public ulong? IsDelete { get; set; }

        public virtual TblCustomer? FkCustomerNavigation { get; set; }
        public virtual TblVendor? FkVendor { get; set; }
    }
}
