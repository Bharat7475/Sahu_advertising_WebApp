namespace Hoarding_management.Data
{
    public class CampaignPagedViewModel
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchQuery { get; set; }
        public List<CampaignViewModel>? CampaignsViewModel { get; set; }
        public List<InventoryViewModel> InventoryViewModel { get; internal set; }
    }

   
}
