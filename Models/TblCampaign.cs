using System;
using System.Collections.Generic;

namespace Hoarding_managment.Models
{
    public partial class TblCampaign
    {
        public int Id { get; set; }
        public int? FkInventoryId { get; set; }
        public int? FkCustomerId { get; set; }
        public string? BookingAmt { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ulong? IsDelete { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual TblCustomer? FkCustomer { get; set; }
    }
}
