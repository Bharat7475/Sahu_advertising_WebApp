
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;

namespace Hoarding_managment.Interface
{
    public interface IDashboard
    {
        //public Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(int pageNumber, int pageSize);
        public Task<int> GetAllHoarldingInvenrotyCountAsync();

        public Task<TblInventory> GetInvetroyByIdAsync(int id);
        public Task<int> DeleteInventryAsync(int id);
        public Task<TblInventoryitem> CreateInventryItemsAsync(TblInventoryitem model);

        public Task<TblInventory> AddNewInventoryAsync(TblInventory model);
        public  Task<TblInventory> UpdateInventoryItemAsync(TblInventory model);
        public Task<TblInventory> GetInventryByIdAsync(int id);
        public  Task<TblInventoryitem> GetInventryItemsByIdAsync(int id);
        public Task<int> InventryItemscountAsync();
        public  Task<int> InventoryCountAsync();
        public Task<int> DeleteInventryItemsAsync(int id);
        public Task<List<InventoryitemViewmodel>> GetInventryItemsAsync();
        public Task<List<InventoryitemViewmodel>> get();


        //public  Task<QuotationItemListViewModel> addCampaign(QuotationItemListViewModel selectedItems);
        public  Task<TblInventory> UploadExcelAsync(TblInventory inventory);

     //   public Task<int> GetAllHoarldingInvenrotyCountAsync(string searchQuery);
        public Task<int> GetAllHoarldingInvenrotyCountAsync(string searchQuery, string amount, string vendor, string City, string Area, string Width, string Height);
        public Task<InventoryViewModel> SearchByInventoryNameAsync(string name);

        //public Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(
        //          string searchQuery, int pageNumber, int pageSize,
        //          string? city, string? area, string? minRate, string? maxRate,
        //          string? width, string? height, int? vendorId);

          // public Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(string searchQuery,int pageNumber, int pageSize);
           public Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(string searchQuery, string amount, string vendor, string City, string Area, string Width, string Height, int pageNumber, int pageSize);


       // public Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(string searchQuery, int pageNumber, int pageSize,string? city = null, string? area = null,  string? minRate = null,string? maxRate = null, string? width = null,  string? height = null,  int? vendorId = null);

       
     
    }
}
