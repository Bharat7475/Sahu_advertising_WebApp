namespace Hoarding_managment.Data
{
    public class VendorViewModel
    {
        public int Id { get; set; }
        public string? VendorName { get; set; }
        public string? BusinessName { get; set; }
     
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? GstNo { get; set; }
        public string? AlternateNumber { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public int? Isdelete  { get; set; }
        public DateTime? CreatedAt { get; set; } 
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }




    }
}
