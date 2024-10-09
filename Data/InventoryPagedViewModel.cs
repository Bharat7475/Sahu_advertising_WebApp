namespace Hoarding_managment.Data
{
    public class InventoryPagedViewModel
    {
        public List<InventoryViewModel>? InventoryViewModel { get; set; }
        public List<TblVendor>? vendorlist { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } // Add this property
        public string? SearchQuery { get; set; }
        public string? amount { get; set; }
        public string? vendor { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
    }
}
