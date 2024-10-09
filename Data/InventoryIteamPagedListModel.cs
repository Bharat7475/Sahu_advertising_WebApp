namespace Hoarding_managment.Data
{
    public class InventoryIteamPagedListModel
    {
        public List<InventoryitemViewmodel> Inventoryitems { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } // Add this property
        public string? SearchQuery { get; set; }
    }
}
