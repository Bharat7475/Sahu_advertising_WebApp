namespace Hoarding_managment.Data
{
    public class CustomerViewModelwithPagignation
    {
        public List<CustomerViewModel> Customers { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; }
    }
}
