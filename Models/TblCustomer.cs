using System;
using System.Collections.Generic;

namespace Hoarding_managment.Models
{
    public partial class TblCustomer
    {
        public TblCustomer()
        {
            TblCampaigns = new HashSet<TblCampaign>();
        }

        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? BusinessName { get; set; }
        public string? Email { get; set; }
        public string? GstNo { get; set; }
        public string? ContactNo { get; set; }
        public string? AlternateNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public ulong? IsDelete { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<TblCampaign> TblCampaigns { get; set; }
    }
}
