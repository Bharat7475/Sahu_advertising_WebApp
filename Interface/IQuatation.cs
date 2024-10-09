using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace HoardingManagement.Interface
{
    public interface IQuotation
    { public Task<List<TblQuotation>> AddQuotationsAsync(List<TblQuotation> models); 
        public Task<IEnumerable<TblQuotation>> GetAllQuotationsAsync(int pageNumber, int pageSize);
        public Task<List<QuatationViewModel>> GetAllQuotationsListAsync(int pageNumber, int pageSize); // Added alternative name
        public Task<TblQuotation> GetQuotationByIdAsync(int id); // Renamed for clarity
        public Task<TblQuotation> GetQuotationByNumberAsync(string quotationNumber); // Added quotation number parameter
        public Task<int> GetQuotationCountAsync();
        public Task<TblQuotation> UpdateQuotationAsync(TblQuotation model); // Renamed for clarity
        public Task<int> DeleteQuotationAsync(int id); // Renamed for clarity
        public Task<QuatationDetaileViewModel> GetQuotationByIdDetailAsync(int id);
        public Task<QuatationDetaileViewModel> GetLatestQuotationById(int id);


        public Task<List<QuatationViewModel>> GetAllQuotationsListAsync(string searchQuery,int pageNumber,int pageSize);
        public Task<int>  GetQuotationCountAsync(string searchQuery);

        public Task<List<TblQuotation>> SearchByCustomerNameAsync(string name);


        public Task<int> findQuotationitemByIdAsync(int id);
        public Task<int> DeleteQuotationitemByIdAsync(int id);

        public  Task<int> AddQuatationsAsync(QuotationItemListViewModel selectedItems);
    }
}
