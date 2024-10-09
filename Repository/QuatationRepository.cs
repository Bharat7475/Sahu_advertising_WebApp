using AngleSharp.Dom;
using HoardingManagement.Interface;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static iTextSharp.text.pdf.AcroFields;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HoardingManagement.Repository
{
    public class QuotationRepository : IQuotation
    {
        private readonly db_hoarding_managementContext _context;

        public QuotationRepository(db_hoarding_managementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TblQuotation>> GetAllQuotationsAsync(int pageNumber, int pageSize)
        {
            return await _context.TblQuotations
                .Where(x => x.IsDelete == 0)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<QuatationViewModel>> GetAllQuotationsListAsync(int pageNumber, int pageSize)
        {
            List<QuatationViewModel> lstquotation = new List<QuatationViewModel>();

            var quotations = await _context.TblQuotations
                .Where(x => x.IsDelete == 0)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            foreach (var item in quotations)
            {


                var vendorid = _context.TblQuotationitems.Where(x=>x.FkQuotationId==item.Id).Select(x=>x.FkVendorId).FirstOrDefault();
                var name = _context.TblVendors.FirstOrDefault(x => x.Id == vendorid)?.VendorName;
                QuatationViewModel model = new QuatationViewModel
                {
                    Id = item.Id,
                    QuotationNumber = item.QuotationNumber,
                    CreatedAt = item.CreatedAt,
                    CreatedBy = item.CreatedBy,
                    UpdatedBy = item.UpdatedBy,
                    FkCustomerId = item.FkCustomerId,
                    VendorName = name,
                    CustomerName = await _context.TblCustomers
                        .Where(x => x.Id == item.FkCustomerId && x.IsDelete == 0)
                        .Select(x => x.CustomerName)
                        .FirstOrDefaultAsync(),
                    Totalhoarding = _context.TblQuotationitems.Where(x => x.FkQuotationId == item.Id).Count(),
                };
                lstquotation.Add(model);
            }
            return lstquotation;
        }


        public async Task<List<QuatationViewModel>> GetAllQuotationsListAsync(string searchQuery, int pageNumber, int pageSize)
        {
            // Start with the base query for quotations
            var query = _context.TblQuotations
                .Where(q => q.IsDelete == 0)
                .AsQueryable();

            // If search query is provided, filter by QuotationNumber, CreatedBy, or CustomerName
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var customerIds = await _context.TblCustomers
                    .Where(c => c.IsDelete == 0 && c.CustomerName.Contains(searchQuery))
                    .Select(c => c.Id)
                    .ToListAsync();

                query = query.Where(q => q.QuotationNumber.Contains(searchQuery) ||
                                         q.CreatedBy.Contains(searchQuery) ||
                                         customerIds.Contains((int)q.FkCustomerId));
            }

            // Apply ordering by CreatedAt in descending order and pagination
            List<QuatationViewModel>? quotations = await query
                .OrderByDescending(q => q.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new QuatationViewModel
                {
                    Id = q.Id,
                    QuotationNumber = q.QuotationNumber,
                    CreatedAt = q.CreatedAt,
                    CreatedBy = q.CreatedBy,
                    UpdatedBy = q.UpdatedBy,
                    FkCustomerId = q.FkCustomerId,
                    VendorName = _context.TblVendors
                        .Where(v => v.Id == _context.TblQuotationitems
                                    .Where(qi => qi.FkQuotationId == q.Id)
                                    .Select(qi => qi.FkVendorId)
                                    .FirstOrDefault())
                        .Select(v => v.VendorName)
                        .FirstOrDefault(),
                    CustomerName = _context.TblCustomers
                        .Where(c => c.Id == q.FkCustomerId && c.IsDelete == 0)
                        .Select(c => c.CustomerName)
                        .FirstOrDefault(),
                    BusinessName = _context.TblCustomers
                        .Where(c => c.Id == q.FkCustomerId && c.IsDelete == 0)
                        .Select(c => c.BusinessName)
                        .FirstOrDefault(),
                    Totalhoarding = _context.TblQuotationitems
                        .Count(qi => qi.FkQuotationId == q.Id && qi.IsDelete==0)
                })
                .ToListAsync();

            return quotations;
        }

        public async Task<int> GetQuotationCountAsync(string searchQuery = "")
        {
            // Initialize the query for quotations
            var query = _context.TblQuotations
                .Where(x => x.IsDelete == 0);

            // If search query is provided, filter by QuotationNumber, CreatedBy, or CustomerName
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(x => x.QuotationNumber.Contains(searchQuery) ||
                                         x.CreatedBy.Contains(searchQuery) ||
                                         _context.TblCustomers
                                             .Where(c => c.IsDelete == 0)
                                             .Any(c => c.Id == x.FkCustomerId && c.CustomerName.Contains(searchQuery)));
            }

            // Return the count of matching quotations
            return await query.CountAsync();
        }

        public async Task<List<TblQuotation>> SearchByCustomerNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<TblQuotation>(); // Return an empty list if name is null or empty
            }

            var lowerCaseName = name.ToLower();

            // Join Quotations with QuotationItems and Customers
            var query = from q in _context.TblQuotations
                        join qi in _context.TblQuotationitems on q.Id equals qi.FkQuotationId
                        join c in _context.TblCustomers on q.FkCustomerId equals c.Id
                        where q.IsDelete == 0 && c.CustomerName.ToLower().Contains(lowerCaseName)
                        select q;

            return await query.ToListAsync();
        }

        public async Task<List<TblCustomer>> SearchCustomersByNameAsync(string name)
        {
            var lowerCaseName = name.ToLower();
            return await _context.TblCustomers
                                 .Where(c => c.IsDelete == 0 && c.CustomerName.ToLower().Contains(lowerCaseName))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<QuatationViewModel>> GetCustomerDetailsAsync()
        {
            return (IEnumerable<QuatationViewModel>)_context.TblQuotations.Where(x => x.IsDelete == 0).ToList();
        }

        public async Task<TblQuotation> GetQuotationByIdAsync(int id)
        {
            return await _context.TblQuotations
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);
        }

        public async Task<TblQuotation> GetQuotationByNumberAsync(string quotationNumber)
        {
            return await _context.TblQuotations
                .Where(x => x.IsDelete == 0 && x.QuotationNumber == quotationNumber)
                .OrderByDescending(x => x.QuotationNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetQuotationCountAsync()
        {
            return await _context.TblQuotations.CountAsync();
        }

       

        public async Task<TblQuotation> UpdateQuotationAsync(TblQuotation model)
        {
            _context.TblQuotations.Update(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<int> DeleteQuotationAsync(int id)
        {
            var quotation = await _context.TblQuotations
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (quotation != null)
            {
                quotation.IsDelete = 1;
                _context.TblQuotations.Update(quotation);
                await _context.SaveChangesAsync();
                return 1;
            }

            return 0;
        }

        public async Task<List<InventoryitemViewmodel>> GetInventoryItemsAsync()
        {
            return await _context.TblInventoryitems
                .Where(x => x.IsDelete == 0)
                .Select(item => new InventoryitemViewmodel
                {
                    Id = item.Id,
                    Image = item.Image,
                    City = item.City,
                    Area = item.Area,
                    Width = item.Width,
                    Height = item.Height,
                    Rate = item.Rate,
                    BookingStatus = (ulong?)item.Type,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    FkInventoryId = item.FkInventoryId
                })
                .ToListAsync();
        }


        public async Task<QuatationDetaileViewModel> GetQuotationByIdDetailAsync(int id)
        {
            // Retrieve the quotation details
            var quotation = await _context.TblQuotations
                .Where(q => q.Id == id && q.IsDelete == 0) // Using `0` for `IsDelete`
                .Select(q => new
                {
                    q.Id,
                    q.FkCustomerId,
                    q.QuotationNumber,
                    q.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (quotation == null)
            {
                return null; // Handle case when the quotation is not found
            }

            // Retrieve the quotation items
            var quotationItems = await _context.TblQuotationitems
                .Where(x => x.FkQuotationId == id && x.IsDelete == 0)
                .Select(x => new
                {
                    x.City,
                    x.Area,
                    x.Location,
                    x.Type,
                    x.Width,
                    x.Height,
                    x.BookingStatus,
                    x.Rate,
                    x.VendorAmt,
                    x.MarginAmt,
                    x.Image,
                    x.LocationDescription,
                   
                })
                .ToListAsync();

            // Retrieve customer details
            var customer = await _context.TblCustomers
                .Where(c => c.Id == quotation.FkCustomerId)
                .Select(c => new { c.CustomerName, c.City, c.BusinessName, c.Address })
                .FirstOrDefaultAsync();

            // Retrieve vendor ID
            var vendorId = await _context.TblQuotationitems
                .Where(x => x.FkQuotationId == id)
                .Select(x => x.FkVendorId)
                .FirstOrDefaultAsync();

            var vendorName = await _context.TblVendors
                .Where(v => v.Id == vendorId)
                .Select(v => v.VendorName)
                .FirstOrDefaultAsync();

            // Retrieve inventory ID
            var inventoryId = await _context.TblQuotationitems
                .Where(x => x.FkQuotationId == id)
                .Select(x => x.FkInventory)
                .FirstOrDefaultAsync();

            // Retrieve inventory details
            var inventory = await _context.TblInventories
                .Where(i => i.Id == inventoryId)
                .Select(i => new { i.Location, i.Width, i.Height })
                .FirstOrDefaultAsync();

            // Map to ViewModel
            var viewModel = new QuatationDetaileViewModel
            {
                Items = quotationItems.Select(item => new QuotationItemViewModel
                {
                    City = item.City,
                    Area = item.Area,
                    Location = item.Location,
                    Width = item.Width,
                    Height = item.Height,
                    Rate = item.Rate,
                    VendorAmt = item.VendorAmt,
                    MarginAmt = item.MarginAmt,
                    Image = item.Image,
                    LocationDescription = item.LocationDescription
                }).ToList(),

                Id = quotation.Id,
                CreatedAt = quotation.CreatedAt,
                QuotationNumber = quotation.QuotationNumber,
                CustomerName = customer?.CustomerName,
                BusinessName = customer?.BusinessName,
                Address =customer.Address,
                VendorName = vendorName,
                Width = inventory?.Width,
                Height = inventory?.Height
            };

            return viewModel;
        }

        public async Task<QuatationDetaileViewModel> GetLatestQuotationById(int id)
            {
            // Retrieve the quotation by ID
            var quotation = await _context.TblQuotations
                .Where(q => q.Id == id && q.IsDelete == 0)
                .Select(q => new
                {
                    q.Id,
                    q.FkCustomerId,
                    q.QuotationNumber,
                    q.CreatedAt,
                })
                .FirstOrDefaultAsync();

            // If no quotation is found, return null
            if (quotation == null)
            {
                return null;
            }

            // Retrieve the related quotation items
            var quotationItems = await _context.TblQuotationitems
                .Where(x => x.FkQuotationId == quotation.Id && x.IsDelete == 0)
                .Select(x => new QuotationItemViewModel
                {
                    City = x.City,
                    Area = x.Area,
                    Location = x.Location,
                    type = (ulong)x.Type,
                    Width = x.Width,
                    Height = x.Height,
                    BookingStatus = (ulong)x.BookingStatus,
                    Rate = x.Rate,
                    VendorAmt = x.VendorAmt,
                    MarginAmt = x.MarginAmt,
                    Image = x.Image,
                    LocationDescription = x.LocationDescription,
                    FkVendorId = x.FkVendorId,
                })
                .ToListAsync();

            // Retrieve the customer details
            var customer = await _context.TblCustomers
                .Where(c => c.Id == quotation.FkCustomerId)
                .Select(c => new { c.CustomerName, c.City, c.BusinessName })
                .FirstOrDefaultAsync();

            // Get the first vendor ID from the quotation items, if available
            var vendorId = quotationItems.FirstOrDefault()?.FkVendorId;

            // Retrieve the vendor details if a vendor ID exists
            var vendor = await _context.TblVendors
                .Where(v => v.Id == vendorId)
                .Select(v => new { v.VendorName })
                .FirstOrDefaultAsync();

            // Fetch the last added item from the quotation items
            var lastItem = quotationItems.LastOrDefault();

            // Create the ViewModel
            var viewModel = new QuatationDetaileViewModel
            {
                Items = quotationItems,
                Id = quotation.Id,
                CreatedAt = quotation.CreatedAt,
                QuotationNumber = quotation.QuotationNumber,
                CustomerName = customer?.CustomerName,
                VendorName = vendor?.VendorName,
                Width = lastItem?.Width ?? "0",  // Use the last added item's width if available
                Height = lastItem?.Height ?? "0", // Use the last added item's height if available
                LastAddedItem = lastItem // Assign the last added item
            };

            return viewModel;
        }

        public async Task<int> DeleteQuotationitemByIdAsync(int id)
        {
            var existQuotationItem = await _context.TblQuotationitems.FirstOrDefaultAsync(x => x.Id == id);
            if (existQuotationItem != null)
            {
                existQuotationItem.IsDelete = 1;
                _context.TblQuotationitems.Update(existQuotationItem);
                await _context.SaveChangesAsync(); // Save the changes to the database
                return 0;
            }
            return 1; // Item not found
        }

        public async Task<int> findQuotationitemByIdAsync(int id)
        {
            var finditem = await _context.TblQuotationitems
                                         .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (finditem == null)
            {
                return 0; 
            }

            return finditem.Id; 
        }


        public async Task<int> AddQuatationsAsync(QuotationItemListViewModel selectedItems)
        {
            // Determine the financial year part
            int currentYear = DateTime.Now.Year;
            int nextYear = DateTime.Now.Month >= 4 ? currentYear + 1 : currentYear;
            string financialYearCode = $"{currentYear % 100}{nextYear % 100}"; // For example: "2425"

            // Get the last quotation number
            var lastQuotation = await _context.TblQuotations
                .Where(x => x.IsDelete == 0 && x.QuotationNumber.Contains(financialYearCode))
                .OrderByDescending(x => x.QuotationNumber)
                .FirstOrDefaultAsync();

            // Extract the sequence number
            string lastNumberPart = lastQuotation != null
                ? lastQuotation.QuotationNumber.Substring(6) // Extracts the last three digits (sequence)
                : "000";

            int nextNumber = int.Parse(lastNumberPart) + 1;

            // Generate the next quotation number
            string nextQuotationNumber = $"Q{financialYearCode}{nextNumber:D3}"; // Example: "Q2425001"

            // Create and add the new quotation
            var data = new TblQuotation
            {
                QuotationNumber = nextQuotationNumber,
                CreatedAt = DateTime.Now,
                CreatedBy = "admin",
                FkCustomerId = selectedItems.CustomerId,
                IsDelete = 0
            };

            await _context.TblQuotations.AddAsync(data);
            await _context.SaveChangesAsync();

            int qid = data.Id;

            // Proceed with saving the quotation items
            if (qid != 0)
            {
                foreach (var item in selectedItems.SelectedItems)
                {
                    var inventoryitems = _context.TblInventoryitems.FirstOrDefault(x => x.Id == item.Id);
                    var inventory = _context.TblInventories.FirstOrDefault(x => x.Id == inventoryitems.FkInventoryId);

                    var newdata = new TblQuotationitem
                    {
                        FkCustomerId = selectedItems.CustomerId,
                        Rate = item.Rate,
                        FkQuotationId = qid,
                        City = inventoryitems.City,
                        Area = inventoryitems.Area,
                        Location = inventory.Location,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsDelete = 0,
                        Height = inventoryitems.Height,
                        Width = inventoryitems.Width,
                        BookingStatus = inventoryitems.BookingStatus,
                        Type = inventory.Type,
                        VendorAmt = inventory.VendorAmt,
                        Image = inventoryitems.Image,
                        LocationDescription = inventory.Location,
                        FkVendorId = inventoryitems.FkVendorId,
                        UpdatedBy = "admin",
                        CreatedBy = "Admin",
                        FkInventory = item.FkInventoryId
                    };

                    await _context.TblQuotationitems.AddAsync(newdata);
                    var recor = await _context.SaveChangesAsync();

                    if (recor > 0)
                    {
                        _context.TblInventoryitems.Remove(inventoryitems);  // Remove the record from the database
                        await _context.SaveChangesAsync();  // Save the changes

                    }
                }
            }

            return qid;
        }



        public async Task<List<TblQuotation>> AddQuotationsAsync(List<TblQuotation> models)
        {
            await _context.TblQuotations.AddRangeAsync(models);
            await _context.SaveChangesAsync();
            return models;
        }
    }
}
