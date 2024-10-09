using System.Drawing;

namespace Hoarding_managment.Data
{
    public class InventoryViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? location { get; set; }
        public ulong? BookingStatus { get; set; }
        public string? Rate { get; set; }
        public string? vendoramt { get; set; }
        public ulong? Isdelete { get; set; }
   
        public int? FkVendorId { get; set; }
        public int? Type { get; set; }
       
        public string? VendorName { get; set; } // Added vendor name
        public double Size {  get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }


    }



}


   

