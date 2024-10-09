namespace Hoarding_managment.Data
{
    public class QuotationItemListViewModel
    {
        public int CustomerId { get; set; }

        public List<SaveSelectedHoardingsViewModel>? SelectedItems { get; set; }

    }
    public class SaveSelectedHoardingsViewModel
    {
        public int Id { get; set; }
        public string? Rate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int FkInventoryId { get; set; }
    }
}
