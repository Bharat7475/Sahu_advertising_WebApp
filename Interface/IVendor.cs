
namespace Hoarding_managment.Interface
{
    public interface IVendor
    {

     //  public Task<IEnumerable<TblVendor>> GetAllVendorsAsync(int pageNumber, int pageSize);
       public Task<IEnumerable<TblVendor>> GetAllVendorsAsync(string searchQuery,int pageNumber, int pageSize);
        public Task<TblVendor> GetVendorByIdAsync(int id);
        public Task<TblVendor> AddVendorAsync(TblVendor vendor);
        public Task<TblVendor> UpdateVendorAsync(int id,TblVendor vendor);
        public TblVendor DeleteVendor(int id);

       // public Task<int> GetVendorCountAsync();
        public Task<int> GetVendorCountAsync(string searchQuery);

        public List<TblVendor> getvendorlist();

        public InventoryFilterViewModel ApplyFilters(InventoryFilterViewModel filter);
    }
       
    }
