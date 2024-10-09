using System;
using System.Collections.Generic;

namespace Hoarding_managment.Models
{
    public partial class TblInventory
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; }
        public string? Location { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public int? Type { get; set; }
        public ulong? BookingStatus { get; set; }
        public string? Rate { get; set; }
        public string? VendorAmt { get; set; }
        public int? FkVendorId { get; set; }
        public ulong? IsDelete { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual TblVendor? FkVendor { get; set; }
    }
}
