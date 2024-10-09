
namespace Hoarding_managment.Data
{
    public class QuatationViewModel
    {

        public int Id { get; set; }
        public string? QuotationNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? BusinessName { get; set; }
        public string? VendorName { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; }
        public string? Location { get; set; }
        public ulong? type { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public ulong? BookingStatus { get; set; }
        public string? Rate { get; set; }
        public ulong? IsDelete { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
     
        public int? FkVendorId { get; set; }
        public int? FkInventoryitemsId { get; set; }
        public int? FkCustomerId { get; set; }
        public int? Totalhoarding { get; set; }

        public string? SearchQuery { get; set; }
        public string? Address { get;  set; }
    }
}
