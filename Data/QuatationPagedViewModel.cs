
namespace Hoarding_managment.Data
{
    public class QuatationPagedViewModel
    {
        public IEnumerable<QuatationViewModel> ? QuatationViewModel { get; set; }
        public List<TblQuotation> tblQuotations { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } // Add this property
        public string? SearchQuery { get; set; }
    }
}
