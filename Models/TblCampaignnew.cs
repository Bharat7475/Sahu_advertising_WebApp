using System;
using System.Collections.Generic;

namespace Hoarding_managment.Models
{
    public partial class TblCampaignnew
    {
        public int Id { get; set; }
        public string? CampaignNumber { get; set; }
        public int FkCustomerId { get; set; }
        public ulong? IsDelete { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
