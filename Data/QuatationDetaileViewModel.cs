namespace Hoarding_managment.Data
{
    public class QuatationDetaileViewModel
    {
        public int Id { get; set; }
        public string QuotationNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? BusinessName { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        
        public string? VendorName { get; set; }
        public string? InventoryLocation { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public List<QuotationItemViewModel> Items { get; set; } = new List<QuotationItemViewModel>();
        public QuotationItemViewModel? LastAddedItem { get; set; }

       
    }

    public class QuotationItemViewModel
    {
        public string City { get; set; }
        public string Area { get; set; }
        public string Location { get; set; }
        public ulong type { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public ulong BookingStatus { get; set; }
        public string Rate { get; set; }
        public string VendorAmt { get; set; }
        public string MarginAmt { get; set; }
        public string Image { get; set; }
        public string LocationDescription { get; set; }
    
        public int? FkVendorId { get; set; }
        public int? FkInventory { get; set; }
    }

}
