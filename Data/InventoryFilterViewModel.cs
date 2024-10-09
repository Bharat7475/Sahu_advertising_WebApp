namespace Hoarding_managment.Data
{
    public class InventoryFilterViewModel
    {
        public string? City { get; set; }
        public string? Area { get; set; }
        public string? MinRate { get; set; }
        public string? MaxRate { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public int? VendorId { get; set; }

        public ulong? BookingStatus { get; set; }
        // Add a property to store the filtered results
        public List<TblInventory> FilteredResults { get; set; } = new List<TblInventory>();


    }
}