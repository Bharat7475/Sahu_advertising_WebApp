
using Hoarding_management.Data;

namespace Hoarding_managment.Repository
{
    public class OngoingCampainRepository : IOngoingCampain
    {
        private readonly db_hoarding_managementContext _context;

        public OngoingCampainRepository( db_hoarding_managementContext db_Hoarding_Management)
        {
            _context = db_Hoarding_Management;  
        }

        public async Task<List<CampaignViewModel>> GetallOngoingCampaignAsync(int pageNumber, int pageSize)
        {
            var campaigns = await _context.TblCampaigns.Where(x=>x.IsDelete==0)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(item => new CampaignViewModel
                {
                    Id = item.Id,
                    ToDate = item.ToDate,
                    FromDate = item.FromDate,
                    BookingAmt = item.BookingAmt,
                    UpdatedBy = item.UpdatedBy,
                    CreatedAt = item.CreatedAt,
                    IsDelete = item.IsDelete,
                    CustomerName = _context.TblCustomers
                        .Where(c => c.Id == item.FkCustomerId)
                        .Select(c => c.CustomerName)
                        .FirstOrDefault(),
                    City = _context.TblInventories
                        .Where(v => v.Id == item.FkInventoryId)
                        .Select(v => v.City)
                        .FirstOrDefault(),
                    BusinessName = _context.TblVendors
                        .Where(v => v.Id == item.FkInventoryId)
                        .Select(v => v.BusinessName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return campaigns;
        }

        public async Task<CampaigneditViewModel> UpdateCampaignAsync(CampaigneditViewModel model)  
        {
            try
            {
                var campaigndetails =await _context.TblCampaigns.FirstOrDefaultAsync(x => x.Id == model.Id);
                if(campaigndetails != null)
                {
                    campaigndetails.FromDate = model.FromDate;
                    campaigndetails.ToDate = model.ToDate;
                    campaigndetails.BookingAmt = model.BookingAmt;
                    _context.TblCampaigns.UpdateRange();
                  await  _context.SaveChangesAsync();

                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        

            return model;
        }

   

        public async Task<TblCampaign> GetCampaingnByIdAsync(int id)
        {
            return await _context.TblCampaigns.FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);
        }
        public async Task<int> DeleteCampaignAsync(int id)
        {
            var campaign = await _context.TblCampaigns
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (campaign == null)
            {
                return 0;
            }
            // Mark the campaign as deleted
            campaign.IsDelete = 1;

            // Update the campaign in the database
            _context.TblCampaigns.Update(campaign);
            await _context.SaveChangesAsync();

            // Return 1 (indicating a successful deletion)
            return 1;
        }
        public async Task<int> GetOngoingCampaignCountAsync()
        {
            return await _context.TblCampaigns
                                 .CountAsync(c => c.FromDate >= DateTime.Today && c.IsDelete == 0);
        }


        //public async Task<QuotationItemListViewModel> addCampaign(QuotationItemListViewModel selectedItems)
        //{
        //    // Determine the financial year part
        //    int currentYear = DateTime.Now.Year;
        //    int nextYear = DateTime.Now.Month >= 4 ? currentYear + 1 : currentYear;
        //    string financialYearCode = $"{currentYear % 100}{nextYear % 100}"; // For example: "2425"


        //    if (selectedItems.CustomerId != null && selectedItems.SelectedItems != null)
        //    {
        //        foreach (var item in selectedItems.SelectedItems)
        //        {
        //            var inventoryitems = _context.TblInventoryitems.Where(x => x.Id == item.Id).FirstOrDefault();
        //            var inventory = _context.TblInventories.Where(x => x.Id == inventoryitems.FkInventoryId).FirstOrDefault();


        //            var lastcampaign = await _context.TblCampaignnews
        //     .Where(x => x.IsDelete == 0 && x.CampaignNumber.Contains(financialYearCode))
        //     .OrderByDescending(x => x.CampaignNumber)
        //     .FirstOrDefaultAsync();

        //            // Extract the sequence number
        //            string lastNumberPart = lastcampaign != null
        //                ? lastcampaign.CampaignNumber.Substring(6) // Extracts the last three digits (sequence)
        //                : "000";

        //            int nextNumber = int.Parse(lastNumberPart) + 1;

        //            // Generate the next quotation number
        //            string nextCampNumber = $"Q{financialYearCode}{nextNumber:D3}"; // Example: "Q2425001"


        //            // Create and add the new quotation
        //            var data = new TblCampaignnew
        //            {
        //                CampaignNumber = nextCampNumber,
        //                CreatedAt = DateTime.Now,
        //                CreatedBy = "admin",
        //                FkCustomerId = selectedItems.CustomerId,
        //                IsDelete = 0
        //            };

        //            await _context.AddRangeAsync(data);
        //            await _context.SaveChangesAsync();

        //            var lastCampId = data.Id;

        //            var newdata = new TblCampaingitem
        //            {
        //                FkCampaignId = lastCampId,
        //                FkInventoryId = item.FkInventoryId,
        //                FromDate = item.FromDate,
        //                ToDate = item.ToDate,
        //                BookingAmt = item.Rate,
        //                CreatedAt = DateTime.Now,
        //                CreatedBy = "Admin",
        //                IsDelete = 0,
        //            };

        //            await _context.TblCampaingitems.AddRangeAsync(newdata);
        //            await _context.SaveChangesAsync();

        //        }
        //    }

        //    return null;
        //}




        public async Task<QuotationItemListViewModel> addCampaign(QuotationItemListViewModel selectedItems)
        {
            // Determine the financial year part
            int currentYear = DateTime.Now.Year;
            int nextYear = DateTime.Now.Month >= 4 ? currentYear + 1 : currentYear;
            string financialYearCode = $"{currentYear % 100}{nextYear % 100}"; // For example: "2425"



            if (selectedItems.CustomerId != null && selectedItems.SelectedItems != null)
            {


                        var lastcampaign = await _context.TblCampaignnews
                 .Where(x => x.IsDelete == 0 && x.CampaignNumber.Contains(financialYearCode))
                 .OrderByDescending(x => x.CampaignNumber)
                 .FirstOrDefaultAsync();

                // Extract the sequence number
                string lastNumberPart = lastcampaign != null
                    ? lastcampaign.CampaignNumber.Substring(6) // Extracts the last three digits (sequence)
                    : "000";

                int nextNumber = int.Parse(lastNumberPart) + 1;

                // Generate the next quotation number
                string nextCampNumber = $"camp{financialYearCode}{nextNumber:D3}"; // Example: "Q2425001"

                // Create and add the new quotation
                var data = new TblCampaignnew
                {
                    CampaignNumber = nextCampNumber,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "admin",
                    FkCustomerId = selectedItems.CustomerId,
                    IsDelete = 0
                };

                await _context.AddRangeAsync(data);
                await _context.SaveChangesAsync();

                var lastCampId = data.Id;


                foreach (var item in selectedItems.SelectedItems)
                {
                   
                    var   inventory = _context.TblInventories.Where(x => x.Id ==item.Id).FirstOrDefault();

                    var newdata = new TblCampaingitem
                    {
                        FkCampaignId = lastCampId,
                        FkInventoryId = item.FkInventoryId,
                        FromDate = item.FromDate,
                        ToDate = item.ToDate,
                        BookingAmt = item.Rate,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "Admin",
                        IsDelete = 0,
                    };

                    await _context.TblCampaingitems.AddRangeAsync(newdata);
                    await _context.SaveChangesAsync();

                }
            }

            return null;
        }







        //public async Task<QuotationItemListViewModel> addCampaign(QuotationItemListViewModel selectedItems)
        //{
        //    if (selectedItems.CustomerId != null && selectedItems.SelectedItems != null)
        //    {
        //        foreach (var item in selectedItems.SelectedItems)
        //        {
        //            var newdata = new TblCampaign
        //            {
        //                FkCustomerId = selectedItems.CustomerId,
        //                FkInventoryId = item.Id,

        //                FromDate = item.FromDate,
        //                ToDate = item.ToDate,
        //                BookingAmt = item.Rate,
        //                CreatedAt = DateTime.Now,
        //                UpdatedAt = DateTime.Now,
        //                IsDelete = 0,
        //                UpdatedBy = "admin",
        //                CreatedBy = "Admin"
        //            };

        //            _context.TblCampaigns.Add(newdata);
        //            var recordsAffected = await _context.SaveChangesAsync();

        //            if (recordsAffected > 0)
        //            {
        //                // Remove associated inventory items if the quotation item was saved successfully.
        //                var itemsToDelete = _context.TblInventoryitems
        //                    .Where(x => x.FkInventoryId == item.FkInventoryId);
        //                _context.TblInventoryitems.RemoveRange(itemsToDelete);
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //    }

        //    return null;
        //}
        public async Task<List<CampaignViewModel>> GetallOngoingCampaignAsync(string searchQuery, int pageNumber, int pageSize)
        {
            // Retrieve all campaigns that are not deleted
            var campaigns = await _context.TblCampaigns
                .Where(x => x.IsDelete == 0 && x.ToDate >= DateTime.Today)
                .ToListAsync();

            // Filter by CustomerName, City, or BusinessName if a search query is provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                campaigns = campaigns.Where(x =>
                    _context.TblCustomers.Any(c => c.Id == x.FkCustomerId && c.CustomerName.Contains(searchQuery)) ||
                    _context.TblInventories.Any(v => v.Id == x.FkInventoryId && v.City.Contains(searchQuery)) ||
                    _context.TblVendors.Any(v => v.Id == x.FkInventoryId && v.BusinessName.Contains(searchQuery))
                ).ToList();
            }

            // Group campaigns by CustomerId and VendorId, selecting the last entry for each group
            var groupedCampaigns = campaigns
                .GroupBy(c => new { c.FkCustomerId, c.FkInventoryId })
                .Select(g => g.OrderBy(c => c.CreatedAt).FirstOrDefault())
                .ToList();

            // Apply pagination (Skip and Take)
            var pagedCampaigns = groupedCampaigns
                .OrderByDescending(x=>x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map the paged campaigns to the CampaignViewModel
            var result = pagedCampaigns.Select(item => new CampaignViewModel
            {
                Id = item.Id,
                ToDate = item.ToDate,
                FromDate = item.FromDate,
                BookingAmt = item.BookingAmt,
                UpdatedBy = item.UpdatedBy,
                CreatedAt = item.CreatedAt,
                IsDelete = item.IsDelete,

                CustomerName = _context.TblCustomers
                    .Where(c => c.Id == item.FkCustomerId)
                    .Select(c => c.CustomerName)
                    .FirstOrDefault(),
                BusinessName = _context.TblVendors
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.BusinessName)
                    .FirstOrDefault(),
                VendorName = _context.TblVendors
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.VendorName)
                    .FirstOrDefault(),
                City = _context.TblInventories
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.City)
                    .FirstOrDefault(),
                Location = _context.TblInventories
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.Location)
                    .FirstOrDefault(),
                Image = _context.TblInventories
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.Image)
                    .FirstOrDefault(),
            }).ToList();

            return result;
        } 
        public async Task<List<CampaignViewModel>> GetallCompletedCampaignAsync(string searchQuery, int pageNumber, int pageSize)
        {
            // Retrieve all campaigns that are not deleted
            var campaigns = await _context.TblCampaigns
                .Where(x => x.IsDelete == 0 && x.ToDate < DateTime.Today)
                .ToListAsync();

            // Filter by CustomerName, City, or BusinessName if a search query is provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                campaigns = campaigns.Where(x =>
                    _context.TblCustomers.Any(c => c.Id == x.FkCustomerId && c.CustomerName.Contains(searchQuery)) ||
                    _context.TblInventories.Any(v => v.Id == x.FkInventoryId && v.City.Contains(searchQuery)) ||
                    _context.TblVendors.Any(v => v.Id == x.FkInventoryId && v.BusinessName.Contains(searchQuery))
                ).ToList();
            }

            // Group campaigns by CustomerId and VendorId, selecting the last entry for each group
            var groupedCampaigns = campaigns
                .GroupBy(c => new { c.FkCustomerId, c.FkInventoryId })
                .Select(g => g.OrderBy(c => c.CreatedAt).FirstOrDefault())
                .ToList();

            // Apply pagination (Skip and Take)
            var pagedCampaigns = groupedCampaigns
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map the paged campaigns to the CampaignViewModel
            var result = pagedCampaigns.Select(item => new CampaignViewModel
            {
                Id = item.Id,
                ToDate = item.ToDate,
                FromDate = item.FromDate,
                BookingAmt = item.BookingAmt,
                UpdatedBy = item.UpdatedBy,
                CreatedAt = item.CreatedAt,
                IsDelete = item.IsDelete,

                CustomerName = _context.TblCustomers
                    .Where(c => c.Id == item.FkCustomerId)
                    .Select(c => c.CustomerName)
                    .FirstOrDefault(),
                BusinessName = _context.TblVendors
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.BusinessName)
                    .FirstOrDefault(),
                VendorName = _context.TblVendors
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.VendorName)
                    .FirstOrDefault(),
                City = _context.TblInventories
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.City)
                    .FirstOrDefault(),
                Location = _context.TblInventories
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.Location)
                    .FirstOrDefault(),
                Image = _context.TblInventories
                    .Where(v => v.Id == item.FkInventoryId)
                    .Select(v => v.Image)
                    .FirstOrDefault(),
            }).ToList();

            return result;
        }

        public async Task<int> GetOngoingCampaignCountAsync(string searchQuery)
        {
            // Initialize the query for campaigns
            var query = _context.TblCampaigns.Where(x => x.IsDelete == 0 && x.ToDate >= DateTime.Today).AsQueryable();

            // If a search query is provided, filter by CustomerName, City, or BusinessName
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(x =>
                    _context.TblCustomers.Any(c => c.Id == x.FkCustomerId && c.CustomerName.Contains(searchQuery)) ||
                    _context.TblInventories.Any(v => v.Id == x.FkInventoryId && v.City.Contains(searchQuery)) ||
                    _context.TblVendors.Any(v => v.Id == x.FkInventoryId && v.BusinessName.Contains(searchQuery)));
            }

            // Return the count of campaigns that match the query
            return await query.CountAsync();
        } 
        
        public async Task<int> GetCompletedCampaignCountAsync(string searchQuery)
        {
            // Initialize the query for campaigns
            var query = _context.TblCampaigns.Where(x => x.IsDelete == 0 && x.ToDate < DateTime.Today).AsQueryable();

            // If a search query is provided, filter by CustomerName, City, or BusinessName
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(x =>
                    _context.TblCustomers.Any(c => c.Id == x.FkCustomerId && c.CustomerName.Contains(searchQuery)) ||
                    _context.TblInventories.Any(v => v.Id == x.FkInventoryId && v.City.Contains(searchQuery)) ||
                    _context.TblVendors.Any(v => v.Id == x.FkInventoryId && v.BusinessName.Contains(searchQuery)));
            }

            // Return the count of campaigns that match the query
            return await query.CountAsync();
        }

        public async Task<CampaignViewModel> SearchByCampaignNameAsync(string name)
        {
            var lowerCaseName = name.ToLower();

            // Find the first campaign that matches the name based on the customer, inventory, or vendor name
            var campaign = await _context.TblCampaigns
                .Where(x => x.IsDelete == 0 &&
                       (_context.TblCustomers.Any(c => c.Id == x.FkCustomerId && c.CustomerName.ToLower().Contains(lowerCaseName)) ||
                        _context.TblInventories.Any(v => v.Id == x.FkInventoryId && v.City.ToLower().Contains(lowerCaseName)) ||
                        _context.TblVendors.Any(v => v.Id == x.FkInventoryId && v.BusinessName.ToLower().Contains(lowerCaseName))))
                .Select(item => new CampaignViewModel
                {
                    Id = item.Id,
                    ToDate = item.ToDate,
                    FromDate = item.FromDate,
                    BookingAmt = item.BookingAmt,
                    UpdatedBy = item.UpdatedBy,
                    CreatedAt = item.CreatedAt,
                    IsDelete = item.IsDelete,
                    CustomerName = _context.TblCustomers
                        .Where(c => c.Id == item.FkCustomerId)
                        .Select(c => c.CustomerName)
                        .FirstOrDefault(),
                    City = _context.TblInventories
                        .Where(v => v.Id == item.FkInventoryId)
                        .Select(v => v.City)
                        .FirstOrDefault(),
                    BusinessName = _context.TblVendors
                        .Where(v => v.Id == item.FkInventoryId)
                        .Select(v => v.BusinessName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return campaign;
        }

        public async Task<TblCampaign> IsCampaignBooked(int id, DateTime fromDate)
        {
            var existingCampaign = _context.TblCampaigns
        .Where(c => c.FkInventoryId == id &&  c.ToDate < fromDate)
        .FirstOrDefault();

            // No conflict found, return false
            return existingCampaign;
        }
    }
}
