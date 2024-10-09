using System;
using System.Collections.Generic;

namespace Hoarding_managment.Models
{
    public partial class TblCampaingitem
    {
        public int Id { get; set; }
        public int? FkCampaignId { get; set; }
        public int? FkInventoryId { get; set; }
        public string? BookingAmt { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public ulong? IsDelete { get; set; }
    }
}
