
namespace Hoarding_managment.Repository
{
    public class VendorRepository:IVendor
    {
        private readonly db_hoarding_managementContext _context;

        public VendorRepository(db_hoarding_managementContext db_hoarding_managementContext)
        {
            _context = db_hoarding_managementContext;
        }


            public  List<TblVendor> getvendorlist()
            {

                return _context.TblVendors.ToList();

            }

            public InventoryFilterViewModel ApplyFilters(InventoryFilterViewModel filter)
            {
                // Start with the base query
                IQueryable<TblInventory> query = _context.TblInventories;

                // Apply filters based on the filter object
                if (!string.IsNullOrWhiteSpace(filter.City))
                {
                    query = query.Where(i => i.City.ToLower() == filter.City.ToLower());
                }

                if (!string.IsNullOrWhiteSpace(filter.Area))
                {
                    query = query.Where(i => i.Area.ToLower() == filter.Area.ToLower());
                }

                if (!string.IsNullOrWhiteSpace(filter.MinRate))
                {
                    query = query.Where(i => string.Compare(i.Rate, filter.MinRate) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(filter.MaxRate))
                {
                    query = query.Where(i => string.Compare(i.Rate, filter.MaxRate) <= 0);
                }

                // Assuming Width and Height are string properties in the model
                if (!string.IsNullOrWhiteSpace(filter.Width))
                {
                    query = query.Where(i => i.Width == filter.Width);
                }

                if (!string.IsNullOrWhiteSpace(filter.Height))
                {
                    query = query.Where(i => i.Height == filter.Height);
                }

                if (filter.VendorId.HasValue)
                {
                    query = query.Where(i => i.FkVendorId == filter.VendorId);
                }

                // Filter based on BookingStatus (0 or null)
                if (filter.BookingStatus.HasValue)
                {
                    if (filter.BookingStatus.Value == 0)
                    {
                        query = query.Where(i => i.BookingStatus == 0 || i.BookingStatus == null);
                    }
                    else
                    {
                        query = query.Where(i => i.BookingStatus == filter.BookingStatus.Value);
                    }
                }

                // Execute the query and get the filtered results
                List<TblInventory> filteredResults = query.ToList();

                // Update the filter model with the filtered results or relevant data
                filter.FilteredResults = filteredResults; // Assuming InventoryFilterViewModel has this property

                // Return the updated filter view model
                return filter;
            }
      

        public async Task<IEnumerable<TblVendor>> GetAllVendorsAsync(string searchQuery, int pageNumber, int pageSize)
        {
            var query = _context.TblVendors.Where(v => v.IsDelete == 0);

            // If search query is not empty, filter by BusinessName, CustomerName, or Email
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(v => v.VendorName.Contains(searchQuery) ||
                                         v.BusinessName.Contains(searchQuery) ||
                                         v.ContactNo.Contains(searchQuery) ||
                                         v.Address.Contains(searchQuery) ||
                                         v.GstNo.Contains(searchQuery) ||
                                         v.Email.Contains(searchQuery));
            }

            // Apply pagination
            return await query
                        .OrderByDescending(q => q.CreatedAt)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
        }
        public async Task<TblVendor> GetVendorByIdAsync(int id)
        {
            return await _context.TblVendors.FindAsync(id);
        }
        public async Task<TblVendor> AddVendorAsync(TblVendor vendor)
        {
            // Add the vendor entity to the context
            _context.TblVendors.Add(vendor);
            // Save the changes asynchronously to the database
            await _context.SaveChangesAsync();

            return vendor;
        }
        public async Task<TblVendor> UpdateVendorAsync(int id, TblVendor vendor)
        {
            _context.TblVendors.Update(vendor);
            await _context.SaveChangesAsync();

            return vendor;
        }
        public TblVendor DeleteVendor(int id)
        {
            var vendor =  _context.TblVendors.FirstOrDefault(x=>x.Id==id);
            if (vendor != null)
            {
                  vendor.IsDelete = 1; // Set the Isdelete column to 1
                _context.TblVendors.Update(vendor); // Update the entity
                 _context.SaveChanges(); // Save changes to the database
            }
            return vendor;
        }
        //public async Task<int> GetVendorCountAsync()
        //{
        //    return await _context.TblVendors.CountAsync(v => v.IsDelete == 0);

        //}
        public async Task<int> GetVendorCountAsync(string searchQuery = "")
        {
            var query = _context.TblVendors.Where(v => v.IsDelete == 0);

            // Apply search filter if a search query is provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(v => v.VendorName.Contains(searchQuery) ||
                                         v.BusinessName.Contains(searchQuery) ||
                                         v.ContactNo.Contains(searchQuery) ||
                                         v.Address.Contains(searchQuery) ||
                                         v.GstNo.Contains(searchQuery) ||
                                         v.Email.Contains(searchQuery));
            }

            return await query.CountAsync();
        }



    }

}