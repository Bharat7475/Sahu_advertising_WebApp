using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Hoarding_managment.Data;
using Microsoft.EntityFrameworkCore;

namespace Hoarding_managment.Repository
{
    public class DashboardRepository : IDashboard
    {
        private readonly db_hoarding_managementContext _context;
        public DashboardRepository(db_hoarding_managementContext db_hoarding_managementContext)
        {
            _context = db_hoarding_managementContext;
        }

        //public async Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(
        //         string searchQuery, int pageNumber, int pageSize,
        //         string? city, string? area, string? minRate, string? maxRate,
        //         string? width, string? height, int? vendorId)
        // {

        //    return null;

        //}


        //  public async Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(
        //string searchQuery,
        //int pageNumber,
        //int pageSize,
        //string? city = null,
        //string? area = null,
        //string? minRate = null,
        //string? maxRate = null,
        //string? width = null,
        //string? height = null,
        //int? vendorId = null)
        //  {
        //      // Normalize the search query
        //      searchQuery = searchQuery?.ToLower().Trim();

        //      // Start the base query (without any filters yet)
        //      var query = _context.TblInventories
        //          .Where(x => x.IsDelete == 0 &&
        //                      (string.IsNullOrEmpty(searchQuery) ||
        //                       x.City.ToLower().Contains(searchQuery) ||
        //                       x.Area.ToLower().Contains(searchQuery) ||
        //                       x.Location.ToLower().Contains(searchQuery)));

        //      // Apply filters only if they are provided (not null or empty)
        //      if (!string.IsNullOrEmpty(city))
        //          query = query.Where(x => x.City.ToLower() == city.ToLower());

        //      if (!string.IsNullOrEmpty(area))
        //          query = query.Where(x => x.Area.ToLower() == area.ToLower());

        //      // Handle minRate and maxRate as strings for comparison
        //      if (!string.IsNullOrEmpty(minRate))
        //          query = query.Where(x => string.Compare(x.Rate, minRate) >= 0);

        //      if (!string.IsNullOrEmpty(maxRate))
        //          query = query.Where(x => string.Compare(x.Rate, maxRate) <= 0);

        //      if (!string.IsNullOrEmpty(width))
        //          query = query.Where(x => x.Width == width);

        //      if (!string.IsNullOrEmpty(height))
        //          query = query.Where(x => x.Height == height);

        //      if (vendorId.HasValue)
        //          query = query.Where(x => x.FkVendorId == vendorId.Value);

        //      // Paginate and order by creation date
        //      var lst = await query
        //          .OrderByDescending(x => x.CreatedAt)
        //          .Skip((pageNumber - 1) * pageSize)  // Apply pagination
        //          .Take(pageSize)
        //          .ToListAsync();

        //      // Map the results to view models
        //      var inventory = new List<InventoryViewModel>();
        //      foreach (var item in lst)
        //      {
        //          // Check if there are active campaigns for the inventory item
        //          var campaignsCheck = await _context.TblCampaigns
        //              .Where(x => x.FkInventoryId == item.Id && x.IsDelete == 0 && x.ToDate >= DateTime.Today)
        //              .ToListAsync();

        //          // Set booking status based on campaign check
        //          var bookStatus = campaignsCheck.Any() ? 0 : 1; // 0 if campaigns exist, else 1

        //          // Get the associated vendor (if any)
        //          var vendor = await _context.TblVendors.FindAsync(item.FkVendorId);

        //          // Create the view model
        //          var model = new InventoryViewModel
        //          {
        //              Id = item.Id,
        //              Image = item.Image,
        //              City = item.City,
        //              Area = item.Area,
        //              location = item.Location,
        //              Width = item.Width,
        //              Height = item.Height,
        //              Rate = item.Rate, // Rate as a string
        //              vendoramt = item.VendorAmt, // VendorAmt as a string
        //              BookingStatus = (ulong?)bookStatus,
        //              CreatedAt = item.CreatedAt,
        //              UpdatedAt = item.UpdatedAt,
        //              Type = item.Type,
        //              VendorName = vendor?.VendorName,
        //              FkVendorId = item.FkVendorId
        //          };

        //          inventory.Add(model);
        //      }

        //      return inventory;
        //  }


        public async Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(string searchQuery, string amount, string vendor, string City, string Area, string Width, string Height, int pageNumber, int pageSize)
        {
            List<InventoryViewModel> inventory = new List<InventoryViewModel>();

            // Ensure the search query is not null or empty
            searchQuery = searchQuery?.ToLower().Trim();
            vendor = vendor?.Trim();
            City = City?.ToLower().Trim();
            Area = Area?.ToLower().Trim();
            Width = Width?.Trim();
            Height = Height?.Trim();

            var lst = _context.TblInventories.ToList();



            if (!string.IsNullOrEmpty(vendor) && vendor != "0")
            {
                var newvendor = Convert.ToInt32(vendor);

                lst = lst
                      .Where(x => x.IsDelete == 0 && x.FkVendorId == newvendor)
                      .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order
                     
                      .ToList();
            }

            if (!string.IsNullOrEmpty(amount) && amount != "0")
            {
                var newamount = Convert.ToInt32(amount);

                lst = lst
                      .Where(x => x.IsDelete == 0 && Convert.ToInt32(!string.IsNullOrEmpty(x.Rate) ? x.Rate : "0") <= newamount)
                      .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order

                      .ToList();
            }


            if (!string.IsNullOrEmpty(City))
            {
                lst = lst
                      .Where(x => x.IsDelete == 0 &&
                                  x.City.ToLower().Contains(City))
                      .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order
                     
                      .ToList();
            }

            if (!string.IsNullOrEmpty(Area))
            {
                lst = lst
                     .Where(x => x.IsDelete == 0 &&
                                 x.Area.ToLower().Contains(Area))
                     .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order
                    
                     .ToList();

            }

            if (!string.IsNullOrEmpty(Width))
            {

                lst = lst.Where(x => x.IsDelete == 0 && x.Width == Width).OrderByDescending(x => x.CreatedAt)
                     .ToList();
            }

            if (!string.IsNullOrEmpty(Height))
            {
                lst = lst.Where(x => x.IsDelete == 0 && x.Height == Height).OrderByDescending(x => x.CreatedAt)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                lst = lst
                     .Where(x => x.IsDelete == 0 &&
                                 (string.IsNullOrEmpty(searchQuery) ||
                                 x.City.ToLower().Contains(searchQuery) ||
                                 x.Area.ToLower().Contains(searchQuery) ||
                                 x.Location.ToLower().Contains(searchQuery)))
                     .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order                   
                     .ToList();
            }

                lst = lst
                       // Ordering by CreatedAt in descending order
                      .Skip((pageNumber - 1) * pageSize)
                      .Take(pageSize)
                      .ToList();

            foreach (var item in lst)
            {
                var campaignscheck = _context.TblCampaigns
                    .Where(x => x.FkInventoryId == item.Id
                                && x.IsDelete == 0
                                && x.ToDate >= DateTime.Today)
                    .ToList();
                int bookstatus = 1;
                if (campaignscheck.Count > 0)
                {
                    bookstatus = 0;
                }

                InventoryViewModel model = new InventoryViewModel
                {

                    Id = item.Id,
                    Image = !string.IsNullOrEmpty(item.Image) ? item.Image : "N/A",
                    City = !string.IsNullOrEmpty(item.City) ? item.City : "N/A",
                    Area = !string.IsNullOrEmpty(item.Area) ? item.Area : "N/A",
                    location = !string.IsNullOrEmpty(item.Location) ? item.Location : "N/A",
                    Rate = item.Rate != null ? item.Rate : "N/A", // Assuming Rate is a nullable type, otherwise you can skip this check
                    vendoramt = item.VendorAmt != null ? item.VendorAmt : "N/A", // Same for VendorAmt
                    Width = item.Width != null ? item.Width : "N/A", // Same for Width
                    Height = item.Height != null ? item.Height : "N/A", // Same for Height
                    BookingStatus = (ulong?)bookstatus,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    Type = item.Type,

                    VendorName = _context.TblVendors.FirstOrDefault(x => x.Id == item.FkVendorId) != null ?
                                     _context.TblVendors.FirstOrDefault(x => x.Id == item.FkVendorId).VendorName :
                                     "N/A",

                    FkVendorId = item.FkVendorId != null ? item.FkVendorId : 0 // Assuming FkVendorId is nullable, adjust accordingly

                };
                inventory.Add(model);
            }

            return inventory;
        }



        //public async Task<List<InventoryViewModel>> GetAllHoarldingInvenrotyAsync(string searchQuery, int pageNumber, int pageSize)
        //{
        //    List<InventoryViewModel> inventory = new List<InventoryViewModel>();

        //    // Ensure the search query is not null or empty
        //    searchQuery = searchQuery?.ToLower().Trim();

        //    var lst = _context.TblInventories
        //              .Where(x => x.IsDelete == 0 &&
        //                          (string.IsNullOrEmpty(searchQuery) ||
        //                          x.City.ToLower().Contains(searchQuery) ||
        //                          x.Area.ToLower().Contains(searchQuery) ||
        //                          x.Location.ToLower().Contains(searchQuery)))
        //              .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order
        //              .Skip((pageNumber - 1) * pageSize)
        //              .Take(pageSize)
        //              .ToList();

        //    foreach (var item in lst)
        //    {
        //        var campaignscheck = _context.TblCampaigns
        //            .Where(x => x.FkInventoryId == item.Id
        //                        && x.IsDelete == 0
        //                        && x.ToDate >= DateTime.Today)
        //            .ToList();
        //        int bookstatus = 1;
        //        if (campaignscheck.Count > 0)
        //        {
        //            bookstatus = 0;
        //        }

        //        InventoryViewModel model = new InventoryViewModel
        //        {

        //            Id = item.Id,
        //            Image = !string.IsNullOrEmpty(item.Image) ? item.Image : "N/A",
        //            City = !string.IsNullOrEmpty(item.City) ? item.City : "N/A",
        //            Area = !string.IsNullOrEmpty(item.Area) ? item.Area : "N/A",
        //            location = !string.IsNullOrEmpty(item.Location) ? item.Location : "N/A",
        //            Rate = item.Rate != null ? item.Rate : "N/A", // Assuming Rate is a nullable type, otherwise you can skip this check
        //            vendoramt = item.VendorAmt != null ? item.VendorAmt : "N/A", // Same for VendorAmt
        //            Width = item.Width != null ? item.Width : "N/A", // Same for Width
        //            Height = item.Height != null ? item.Height : "N/A", // Same for Height
        //            BookingStatus = (ulong?)bookstatus,
        //            CreatedAt = item.CreatedAt,
        //            UpdatedAt = item.UpdatedAt,
        //            Type = item.Type,

        //            VendorName = _context.TblVendors.FirstOrDefault(x => x.Id == item.FkVendorId) != null ?
        //                             _context.TblVendors.FirstOrDefault(x => x.Id == item.FkVendorId).VendorName :
        //                             "N/A",

        //            FkVendorId = item.FkVendorId != null ? item.FkVendorId : 0 // Assuming FkVendorId is nullable, adjust accordingly

        //        };
        //        inventory.Add(model);
        //    }

        //    return inventory;
        //}


        public async Task<int> GetAllHoarldingInvenrotyCountAsync(string searchQuery, string amount, string vendor, string City, string Area, string Width, string Height)
        {
            searchQuery = searchQuery?.ToLower().Trim();
            vendor = vendor?.ToLower().Trim();
            City = City?.ToLower().Trim();
            Area = Area?.ToLower().Trim();
            Width = Width?.Trim();
            Height = Height?.Trim();

            var lst = _context.TblInventories
                      .Where(x => x.IsDelete == 0 &&
                                  (string.IsNullOrEmpty(searchQuery) ||
                                  x.City.ToLower().Contains(searchQuery) ||
                                  x.Area.ToLower().Contains(searchQuery) ||
                                  x.Location.ToLower().Contains(searchQuery)))
                      .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order
                      
                      .ToList();

            if (!string.IsNullOrEmpty(vendor) && vendor != "0")
            {
                var newvendor = Convert.ToInt32(vendor);

                lst = lst
                      .Where(x => x.IsDelete == 0 && x.FkVendorId == newvendor)
                      .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order

                      .ToList();
            }

            if (!string.IsNullOrEmpty(amount) && amount != "0")
            {
                var newamount = Convert.ToInt32(amount);

                lst = lst
                      .Where(x => x.IsDelete == 0 && Convert.ToInt32(!string.IsNullOrEmpty(x.Rate) ? x.Rate : "0") <= newamount)
                      .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order
                     
                      .ToList();
            }


            if (!string.IsNullOrEmpty(City))
            {
                lst = lst
                      .Where(x => x.IsDelete == 0 &&
                                  x.City.ToLower().Contains(City))
                      .OrderByDescending(x => x.CreatedAt) // Ordering by CreatedAt in descending order
                     
                      .ToList();
            }

            if (!string.IsNullOrEmpty(Area))
            {
                lst = lst.Where(x => x.IsDelete == 0 && x.Area.ToLower().Contains(Area)).OrderByDescending(x => x.CreatedAt)
                     .ToList();

            }

            if (!string.IsNullOrEmpty(Width))
            {

                lst = lst.Where(x => x.IsDelete == 0 && x.Width == Width).OrderByDescending(x => x.CreatedAt)
                     .ToList();
            }

            if (!string.IsNullOrEmpty(Height))
            {
                lst = lst.Where(x => x.IsDelete == 0 && x.Height == Height).OrderByDescending(x => x.CreatedAt)
                    .ToList();
            }
            //// Ensure the search query is not null or empty
            //searchQuery = searchQuery?.ToLower().Trim();

            //// Count the total number of matching hoardings
            //var count = await _context.TblInventories
            //             .Where(x => x.IsDelete == 0 &&
            //                         (string.IsNullOrEmpty(searchQuery) ||
            //                         x.City.ToLower().Contains(searchQuery) ||
            //                         x.Area.ToLower().Contains(searchQuery) ||
            //                         x.Location.ToLower().Contains(searchQuery)))
            //             .CountAsync();

            var count = lst.Count();

            return count;
        }

        public async Task<InventoryViewModel> SearchByInventoryNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null; // Return null if the name is empty or null
            }

            var lowerCaseName = name.ToLower();

            // Search the inventory by matching the name with vendor name, city, or area
            var inventory = await _context.TblInventories
                .Where(x => x.IsDelete == 0 &&
                            (_context.TblVendors
                                .Where(v => v.Id == x.FkVendorId && v.VendorName.ToLower().Contains(lowerCaseName))
                                .Any() ||
                             x.City.ToLower().Contains(lowerCaseName) ||
                             x.Area.ToLower().Contains(lowerCaseName)))
                .Select(item => new InventoryViewModel
                {
                    Id = item.Id,
                  
                    Image = item.Image,
                    City = item.City,
                    Area = item.Area,
                    location = item.Location,
                    Rate = item.Rate,
                    vendoramt = item.VendorAmt,
                    Width = item.Width,
                    Height = item.Height,
                    BookingStatus = item.BookingStatus,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    FkVendorId = item.FkVendorId,
                    VendorName = _context.TblVendors
                        .Where(v => v.Id == item.FkVendorId)
                        .Select(v => v.VendorName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return inventory;
        }

        public async Task<TblInventory> GetInvetroyByIdAsync(int id)
        {
            return await _context.TblInventories.FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);
        }
        public async Task<int> DeleteInventryAsync(int id)
        {
            var existcustomer = _context.TblInventories.FirstOrDefault(x => x.Id == id);
            if (existcustomer != null)
            {

                existcustomer.IsDelete = 1;
                await _context.SaveChangesAsync();
                return 0;
            }
            return 1;
        }
        public async Task<TblInventoryitem> CreateInventryItemsAsync(TblInventoryitem model)
        {
            _context.TblInventoryitems.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<TblInventory> UpdateInventoryItemAsync(TblInventory model)
        {
            try
            {
                //_context.TblInventoryitems.Update();
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                // Optionally, log the exception and handle it as needed
                throw new Exception("An error occurred while updating the inventory item.", ex);
            }
        }

        // Method to get TblInventory by Id
        public async Task<TblInventory> GetInventryByIdAsync(int id)
        {
            return await _context.TblInventories
              .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);
        }

        // Method to get TblInventoryitem by Id
        public async Task<TblInventoryitem> GetInventryItemsByIdAsync(int id)
        {
            return await _context.TblInventoryitems
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);
        }


        public async Task<int> InventryItemscountAsync()
        {
            return await _context.TblInventoryitems
                                 .Where(x => x.IsDelete == 0)
                                 .CountAsync();
        }
        public async Task<int> InventoryCountAsync()
        {
            return await _context.TblInventories
                                 .Where(x => x.IsDelete == 0)
                                 .CountAsync();
        }
        public async Task<int> DeleteInventryItemsAsync(int id)
        {
            var existcustomer = _context.TblInventoryitems.FirstOrDefault(x => x.Id == id);
            if (existcustomer != null)
            {

                _context.TblInventoryitems.Remove(existcustomer);
                await _context.SaveChangesAsync();
                return 0;
            }
            return 1;
        }
        public async Task<TblInventory> AddInvenrotyAsync(TblInventory model)
        {
            _context.TblInventories.AddAsync(model);
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<TblInventory> UpdateInvenrotyAsync(int id, TblInventory model)
        {
            _context.TblInventories.Update(model);
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<TblInventory> GetInventoryByIdAsync(int id)
        {
            return await _context.TblInventories.FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);
        }
        public async Task<int> GetAllHoarldingInvenrotyCountAsync()
        {
            return await _context.TblInventories.CountAsync();
        }
        public async Task<int> DeleteInvenrotyAsync(int id)
        {
            var exitinventory = await _context.TblInventories
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (exitinventory == null)
            {
                return 0;
            }
            // Mark the campaign as deleted
            exitinventory.IsDelete = 1;

            // Update the campaign in the database
            _context.TblInventories.Update(exitinventory);
            await _context.SaveChangesAsync();

            // Return 1 (indicating a successful deletion)
            return exitinventory.Id;
        }

        public async Task<List<InventoryitemViewmodel>> GetInventryItemsAsync()
        {
            var inventoryItems = await _context.TblInventoryitems
                .Where(x => x.IsDelete == 0)
                .OrderByDescending(item => item.CreatedAt)  // Order by CreatedAt in descending order
                .Select(item => new InventoryitemViewmodel
                {
                    Id = item.Id,
                    Image = item.Image,
                    City = item.City,
                    Area = item.Area,
                    Width = item.Width,
                    Height = item.Height,
                    Rate = item.Rate,
                    BookingStatus = item.BookingStatus,
                    type = (int?)item.Type,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    FkInventoryId = item.FkInventoryId,
                    Fkcustomer = item.Fkcustomer,
                    FkVendorId = (int)item.FkVendorId,
                    VendorName = _context.TblVendors
                        .Where(v => v.Id == item.FkVendorId && v.IsDelete == 0)
                        .Select(v => v.VendorName)
                        .FirstOrDefault(),
                    CustomerName = _context.TblCustomers
                        .Where(c => c.Id == item.Fkcustomer && c.IsDelete == 0)
                        .Select(c => c.CustomerName)
                        .FirstOrDefault(),
                    BusinessName = _context.TblCustomers
                        .Where(c => c.Id == item.Fkcustomer && c.IsDelete == 0)
                        .Select(c => c.BusinessName)
                        .FirstOrDefault(),
                })
                .ToListAsync();

            return inventoryItems;
        }

        public Task<List<InventoryitemViewmodel>> get()
        {
            var selectedInvert = _context.TblInventoryitems
                .Where(x => x.IsDelete == 0)  // Filtering only non-deleted items
                .Select(x => new InventoryitemViewmodel
                {
                    Id = x.Id,
                    Image = x.Image,
                    Area = x.Area,
                    City = x.City,
                    Width = x.Width,
                    Height = x.Height,
                    Rate = x.Rate,
                    BookingStatus = x.BookingStatus,
                    type = (int?)x.Type,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    IsDelete = x.IsDelete,
                    FkInventoryId = x.FkInventoryId,
                    Fkcustomer = x.Fkcustomer,
                    FkVendorId = (int)x.FkVendorId
                })
                .ToListAsync(); // Use ToListAsync to return a Task<List<InventoryitemViewmodel>>

            return selectedInvert;
        }


     

        //public async Task<int> AddQuatationsAsync(QuotationItemListViewModel selectedItems)
        //{
        //    var lastQuotation = await _context.TblQuotations
        //        .Where(x => x.IsDelete == 0)
        //        .OrderByDescending(x => x.QuotationNumber)
        //        .FirstOrDefaultAsync();

        //    string lastNumberPart = lastQuotation != null ? lastQuotation.QuotationNumber.Replace("Q", "") : "000";
        //    int nextNumber = int.Parse(lastNumberPart) + 1;
        //    string nextQuotationNumber = $"Q{nextNumber:D3}";

        //    var data = new TblQuotation
        //    {
        //        QuotationNumber = nextQuotationNumber,
        //        CreatedAt = DateTime.Now,
        //        CreatedBy = "admin",
        //        FkCustomerId = selectedItems.CustomerId,
        //        IsDelete = 0,

        //    };

        //    await _context.TblQuotations.AddAsync(data);
        //    await _context.SaveChangesAsync();

        //    int qid = data.Id;
        //    if (qid != 0)
        //    {
        //        foreach (var item in selectedItems.SelectedItems)
        //        {
        //            var inventoryitems = _context.TblInventoryitems.Where(x => x.Id == item.Id).FirstOrDefault();
        //            var inventory = _context.TblInventories.Where(x => x.Id == inventoryitems.FkInventoryId).FirstOrDefault();

        //            var newdata = new TblQuotationitem
        //            {
        //                FkCustomerId = selectedItems.CustomerId,
        //                Rate = item.Rate,
        //                FkQuotationId = qid,
        //                City = inventoryitems.City,
        //                Area = inventoryitems.Area,
        //                Location = inventory.Location,
        //                CreatedAt = DateTime.Now,
        //                UpdatedAt = DateTime.Now,
        //                IsDelete = 0,
        //                Height = inventoryitems.Height,
        //                Width = inventoryitems.Width,
        //                BookingStatus = inventoryitems.BookingStatus,
        //                type = inventory.type,
        //                VendorAmt = inventory.VendorAmt,
        //                Image = inventoryitems.Image,
        //                LocationDescription = inventory.Location,
        //                FkVendorId = inventoryitems.FkVendorId,
        //                UpdatedBy = "admin",
        //                CreatedBy = "Admin",
        //                FkInventory = item.FkInventoryId
        //            };

        //               _context.TblQuotationitems.AddAsync(newdata);
        //            var recor = await _context.SaveChangesAsync();

        //            if (recor > 0)
        //            {
        //                inventoryitems.IsDelete = 1;
        //               _context.Update(inventoryitems);
        //                _context.SaveChanges();
        //            }



        //        }
        //    }

        //    return qid;
        //}

        //public async Task<QuotationItemListViewModel> addCampaign(QuotationItemListViewModel selectedItems)
        //{


        //    if (selectedItems.CustomerId != null && selectedItems.SelectedItems != null)
        //    {
        //        foreach (var item in selectedItems.SelectedItems)
        //        {
        //            var inventoryitems = _context.TblInventoryitems.Where(x => x.Id == item.Id).FirstOrDefault();
        //            var inventory = _context.TblInventories.Where(x => x.Id == inventoryitems.FkInventoryId).FirstOrDefault();

        //            var newdata = new TblCampaign
        //            {
        //                FkCustomerId = selectedItems.CustomerId,
        //                FkInventoryId = item.FkInventoryId,
        //                FromDate = DateTime.Now,
        //                ToDate = DateTime.Now,
        //                BookingAmt = item.Rate,
        //                CreatedAt = DateTime.Now,
        //                UpdatedAt = DateTime.Now,
        //                IsDelete = 0,
        //                UpdatedBy = "admin",
        //                CreatedBy = "Admin"
        //            };

        //            _context.TblCampaigns.Add(newdata);
        //            await _context.SaveChangesAsync();

        //        }
        //    }

        //    return null;
        //}






        public async Task<TblInventory> UploadExcelAsync(TblInventory inventory)
        {
            return inventory;
        }
        public async Task<TblInventory> AddNewInventoryAsync(TblInventory model)
        {
            _context.TblInventories.Add(model);
            await _context.SaveChangesAsync();
            return model;

        }

        //    public async Task<TblInventory> UploadExcelAsync(TblInventory inventory)
        //    {
        //        // Set EPPlus license context
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                var worksheet = package.Workbook.Worksheets[0];
        //                var rowCount = worksheet.Dimension.Rows;

        //                var records = new List<TblInventory>();

        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    DateTime? date = null;
        //                    var dateString = worksheet.Cells[row, 4].Value?.ToString().Trim();
        //                    if (!string.IsNullOrEmpty(dateString) && DateTime.TryParse(dateString, out DateTime parsedDate))
        //                    {
        //                        date = parsedDate;
        //                    }
        //                    var record = new TblInventory
        //                    {
        //                        Image = worksheet.Cells[row, 2].Value?.ToString().Trim(),
        //                        City = worksheet.Cells[row, 3].Value?.ToString().Trim(),
        //                        Area = worksheet.Cells[row, 3].Value?.ToString().Trim(),
        //                        Location = worksheet.Cells[row, 3].Value?.ToString().Trim(),
        //                        Width = worksheet.Cells[row, 3].Value?.ToString().Trim(),
        //                        Height = worksheet.Cells[row, 3].Value?.ToString().Trim(),
        //                        Rate = worksheet.Cells[row, 3].Value?.ToString().Trim(),
        //                        VendorAmt = worksheet.Cells[row, 3].Value?.ToString().Trim(),
        //                        // Map other columns as needed
        //                    };

        //                    records.Add(record);
        //                }





        //            }
        //            return inventory;
        //    }
    }

}
