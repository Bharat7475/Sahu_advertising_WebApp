using HoardingManagement.Interface;
using Microsoft.EntityFrameworkCore;

namespace Hoarding_managment.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboard _context;
        private readonly db_hoarding_managementContext _dbContext;
        private readonly AutocompleteService _autocompleteService;
        private readonly IQuotation _Quoatations_context;
        IWebHostEnvironment hostingenvironment;
        private readonly ICustomer _customer;

        public DashboardController(IDashboard dashboard, IWebHostEnvironment hc, IQuotation quotation, db_hoarding_managementContext dbContext,ICustomer customer, AutocompleteService autocompleteService)
        {
            _context = dashboard;
            hostingenvironment = hc;
            _Quoatations_context = quotation;
            _dbContext = dbContext;
            _customer = customer;
            _autocompleteService = autocompleteService;

        }
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> FilterInventory(
        //    string? searchQuery = "",
        //    int pageSize = 9,
        //    int pageNumber = 1,
        //    string? city = null,
        //    string? area = null,
        //    string? minRate = null,
        //    string? maxRate = null,
        //    string? width = null,
        //    string? height = null,
        //    int? vendorId = null)
        //{
        //    // Call the async method to get filtered data
        //    var filteredData = await _context.GetAllHoarldingInvenrotyAsync(searchQuery="", pageSize, pageNumber, city, area, minRate, maxRate,
        //             width, height, vendorId);

        //    // Return the filtered data along with a success message
        //    return Json(new { success = true, message = "success", data = filteredData });
        //}


        //[HttpGet]
        //public async Task<IActionResult> HoardingInventory(string searchQuery = "", int pageSize = 9, int pageNumber = 1,
        //    string? city = null,
        //    string? area = null,
        //    string? minRate = null,
        //    string? maxRate = null,
        //    string? width = null,
        //    string? height = null,
        //    int? vendorId = null)
        //{
        //    var inventory = await _context.GetAllHoarldingInvenrotyAsync(searchQuery = "", pageSize, pageNumber, city, area, minRate, maxRate,
        //             width, height, vendorId);
        //    var totalItems = await _context.GetAllHoarldingInvenrotyCountAsync(searchQuery);
        //    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        //    InventoryPagedViewModel? viewModel = new InventoryPagedViewModel
        //    {
        //        InventoryViewModel = inventory,
        //        CurrentPage = pageNumber,
        //        TotalPages = totalPages,
        //        PageSize = pageSize,
        //        SearchQuery = searchQuery
        //    };

        //    return View(viewModel);
        //}

        [HttpGet]
        public async Task<IActionResult> HoardingInventory(string searchQuery = "", string amount = "", string vendor = "", string City = "", string Area = "", string Width = "", string Height = "", int pageSize = 9, int pageNumber = 1)
        {
            var inventory = await _context.GetAllHoarldingInvenrotyAsync(searchQuery,amount,vendor,City,Area,Width,Height, pageNumber, pageSize);
            var totalItems = await _context.GetAllHoarldingInvenrotyCountAsync(searchQuery, amount, vendor, City, Area, Width, Height);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var vendorlist = await _dbContext.TblVendors.Where(x => x.IsDelete == 0).ToListAsync();

            if (string.IsNullOrEmpty(amount))
            {
                amount = "0";
            }

            InventoryPagedViewModel? viewModel = new InventoryPagedViewModel
            {
                InventoryViewModel = inventory,
                vendorlist= vendorlist,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery,
                amount=amount,
                vendor=vendor,
                City=City,
                Area=Area,
                Width=Width,
                Height=Height

            };

            return View(viewModel);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByInventoryName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name cannot be empty.");
            }

            var customers = await _context.SearchByInventoryNameAsync(name);
            return Ok(customers);
        }



        [HttpDelete]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.GetInvetroyByIdAsync(id);
            if (inventory == null)
            {
                return Json(new { success = false, message = "Inventory not found." });
            }

            try
            {
                await _context.DeleteInventryAsync(id);
                return Json(new
                {
                    success = true,
                    message = "Deleted Successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventoryItems(int Id, string Image, string Area, string City, string width, string height, string Rate,int FkVendorId  )
        {
            var existingItem = _dbContext.TblInventoryitems.FirstOrDefault(x => x.FkInventoryId == Id && x.IsDelete==0); // Use await to get the result

            if (existingItem == null)
            {
                if (ModelState.IsValid)
            {

                    var inventoryItem = new TblInventoryitem
                    {
                        FkInventoryId = Id,
                        Image = Image,
                        Area = Area,
                        City = City,
                        Width = width,
                        Height = height,
                        Rate = Rate,
                        BookingStatus = 0,
                        FkVendorId = FkVendorId,
                        IsDelete = 0,
                        Type = 0,
                        Fkcustomer = 1,
                        CreatedAt = DateTime.Now
                    };

                    _dbContext.TblInventoryitems.Add(inventoryItem);
                    _dbContext.SaveChanges();

                    return Json(new { success = true, Message = "Create Quotation successfully." });
                }
                else
                {
                    return Json(new { success = false, Message = "Allready Added" });

                }

                //}
                //else
                //{
                //    return Json(new { success = false, Message = "Please select another item." });
                //}
            }
            return Json(new { success = false, Message = "Creating error." });
        }

        public JsonResult GetVendorName(string query)
        {
            var subproduct = _autocompleteService.Getvendorname(query);

            var result = subproduct
              .Where(f => f.VendorName.ToLower().Contains(query.ToLower())).Select(s => new { Name = s.VendorName, Id = s.Id })
              .ToList();
            return Json(result);
        }
        public JsonResult GetBusinessName(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<object>());
            }

            var subproduct = _autocompleteService.Getbusinessname(query);
            if (subproduct == null)
            {
                return Json(new List<object>());
            }

            var result = subproduct
                .Where(f => f.BusinessName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(s => new { Name = s.BusinessName, Id = s.Id })
                .ToList();

            return Json(result);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateInventoryItems(int id, string city, string area, string width, string height, string rate, string VendorName,int vendorid,string Image,string location,string vendoramt,int st)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model state is invalid." });
            }

            var existingItem = await _context.GetInventryByIdAsync(id);
            if (existingItem == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }

            // Update the properties of the existing item
            existingItem.Id = id;
            existingItem.City = city;
            existingItem.Area = area;
            existingItem.Width = width;
            existingItem.Height = height;
            existingItem.Rate = rate;
            existingItem.FkVendorId = vendorid;
            existingItem.Image = Image;
            existingItem.Location = location;
            existingItem.VendorAmt = vendoramt;
            existingItem.Type = st;

            // Save changes to the database
            this._dbContext.SaveChanges();

            return Json(new { success = true, message = "Inventory item updated successfully." });
        }



        [HttpDelete]
        public async Task<IActionResult> DeletedSelectedInventryHoarding(int id)
        {

            var existingItem = await _context.GetInventryItemsByIdAsync(id);
            if (existingItem == null)
            {
                return Json(new { success = false, Message = "Deleteing Error seleced hoarding", model = 0 });
            }
            var deletecnf = await _context.DeleteInventryItemsAsync(id);

            Console.WriteLine("deleted id" + existingItem);

            return Json(new { success = true, Message = "Delete seleced hoarding", model = deletecnf });


        }

        [HttpGet]
        public async Task<IActionResult> getallCount()
        {
            try
            {
                int itemCount = await _context.InventryItemscountAsync();
                return Json(new { success = true, count = itemCount });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Json(new { success = false, message = "An error occurred while fetching the count." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> InventoryallCount()
        {
            try
            {
            
                int itemCount = await _context.InventoryCountAsync();

                return Json(new { success = true, count = itemCount });
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return Json(new { success = false, message = "An error occurred while fetching the count." });
            }
        }

        public async Task<IActionResult> getSelectedHoarding()
        {
            var data = _context.GetInventryItemsAsync();

            if (data == null)
            {
                return Json(new { success = false, Message = " Data not Found " });

            }

            return Json(new { success = true, Message = "send ", model = data });
        }

        public async Task<IActionResult> GetAllInventoryItems()
        {
            var model = await _context.GetInventryItemsAsync();
            return View();
        }

        [HttpGet("SelectedHoarding")]
        public async Task<IActionResult> SelectedHoarding()
        {
            var model = await _context.GetInventryItemsAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> SelectedHoardingJ()
        {
            var quotations = await _context.GetInventryItemsAsync();
            return Json(new { success = true, message = "Success", model = quotations });
        }
        [HttpPost]
        public  IActionResult GetHordingdata(int id)
        {
            if (id != null)
            {
                var getHordingItemsAgainstId = this._dbContext.TblQuotationitems.Where(x => x.FkQuotationId == id && x.IsDelete == 0).Select(item => new InventoryitemViewmodel
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
                    FkInventoryId = 1,
                    Fkcustomer = item.FkCustomerId,
                    FkVendorId = (int)item.FkVendorId,
                    VendorName = _dbContext.TblVendors
                               .Where(v => v.Id == item.FkVendorId && v.IsDelete == 0)
                               .Select(v => v.VendorName)
                               .FirstOrDefault(),
                    CustomerName = _dbContext.TblCustomers
                               .Where(c => c.Id == item.FkCustomerId && c.IsDelete == 0)
                               .Select(c => c.CustomerName)
                               .FirstOrDefault(),
                    BusinessName = _dbContext.TblCustomers
                               .Where(c => c.Id == item.FkCustomerId && c.IsDelete == 0)
                               .Select(c => c.BusinessName)
                               .FirstOrDefault(),
                })
               .ToList();
                return Json(new { success = true, message = "Success", model = getHordingItemsAgainstId });
            }

            return Json(new { success = false, Message = " Data not Found " });
        }
        [HttpGet]
        public async Task<IActionResult> SelectedHoarding(int pageNumber = 1, int pageSize = 5)
        {
            var quotations = await _context.GetInventryItemsAsync();

            return Json(new { success = true, Message = "", model = quotations });
        }
        [HttpPost]
        public async Task<IActionResult> DeletedSelectInventoryHoarding(int id)
        {
            var existingItem = await _context.GetInventryItemsByIdAsync(id);
            if (existingItem == null)
            {
                return Json(new { success = false, message = "Error Deleting Hoarding", model = 0 });
            }

            var deleteCnf = await _context.DeleteInventryItemsAsync(id);
            Console.WriteLine("Deleted ID: " + existingItem.Id);

            return Json(new { success = true, message = "Hoarding Deleted successfully", model = deleteCnf });
        }

        [HttpGet]
        public async Task<IActionResult> getSelectInventoryHoarding()
        {

            var model = _context.get();
            return Json(new { success = true, message = "Selected hoarding deleted successfully", model = model });
        }

        
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid Excel file.");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new ClosedXML.Excel.XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1); // Use the first worksheet
                        var rowCount = worksheet.LastRowUsed().RowNumber(); // Get the last used row

                        // Ensure the worksheet has at least 7 columns
                        if (worksheet.FirstRowUsed().Cells().Count() < 7)
                        {
                            return BadRequest("The uploaded Excel file must have at least 7 columns.");
                        }

                        var records = new List<TblInventory>();

                        for (int row = 2; row <= rowCount; row++) // Adjust if first row is headers
                        {
                            var cityCell = worksheet.Cell(row, 1).GetString()?.Trim();
                            var areaCell = worksheet.Cell(row, 2).GetString()?.Trim();
                            var locationCell = worksheet.Cell(row, 3).GetString()?.Trim();
                            var widthCell = worksheet.Cell(row, 4).GetString()?.Trim();
                            var heightCell = worksheet.Cell(row, 5).GetString()?.Trim();
                            var typeCell = worksheet.Cell(row, 6).GetString()?.Trim().ToLower();
                            var rateCell = worksheet.Cell(row, 7).GetString()?.Trim();

                            // Skip rows with empty mandatory fields like City or Area
                            if (string.IsNullOrWhiteSpace(cityCell) || string.IsNullOrWhiteSpace(areaCell))
                            {
                                continue;
                            }

                            var record = new TblInventory
                            {
                                Image = "/9j/4QAYRXhpZgAASUkqAAgAAAAAAAAAAAAAAP/sABFEdWNreQABAAQAAABkAAD/4QMvaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA3LjEtYzAwMCA3OS5lZGEyYjNmYWMsIDIwMjEvMTEvMTctMTc6MjM6MTkgICAgICAgICI+IDxyZGY6UkRGIHhtbG5zOnJkZj0iaHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyI+IDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSIiIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCAyMy4xIChXaW5kb3dzKSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo1OUVEQjI1NzQ5MjcxMUVEODFFQkQyOTg3QTYwRTE2RSIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDo1OUVEQjI1ODQ5MjcxMUVEODFFQkQyOTg3QTYwRTE2RSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjU5RURCMjU1NDkyNzExRUQ4MUVCRDI5ODdBNjBFMTZFIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjU5RURCMjU2NDkyNzExRUQ4MUVCRDI5ODdBNjBFMTZFIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+/+IMWElDQ19QUk9GSUxFAAEBAAAMSExpbm8CEAAAbW50clJH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Y0TxorImNRlUNkVkJwpzg5NGdMLS4vJVZXVWN4SFo7PD0+PzKRqUpLTE1OT0laW1xdXl9ShHV2Y4doaWprbG1ub2Z3eHl6e3x9fn90hYaHiImKi4yNjo+DlJWWl5iZmpucnZ6fkqOkpaanqKmqq6ytrq+v/aAAwDAQACEQMRAD8A2yPfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+690Dnd/e3X/x+2lBu7f9TkGhyFf/AArB4PB00ddn8/kRCaiWnx1NLNBBHDSU6+SeomdIYVIuSWAPuvdE9/4dG6M/59x3Hf8AP7OzP/r97917r3/Do3Rn/PuO4/8AqTsz/wCv3v3Xuvf8OjdGf8+47j/6k7M/+v3v3Xuvf8OjdGf8+47j/wCpOzP/AK/e/de69/w6N0Z/z7juP/qTsz/6/e/de69/w6N0Z/z7juP/AKk7M/8Ar97917r3/Do3Rn/PuO4/+pOzP/r97917r3/Do3Rn/PuO4/8AqTsz/wCv3v3Xuvf8OjdGf8+47j/6k7M/+v3v3Xuvf8OjdGf8+47j/wCpOzP/AK/e/de69/w6N0Z/z7juP/qTsz/6/e/de69/w6N0Z/z7juP/AKk7M/8Ar97917r3/Do3Rn/PuO4/+pOzP/r97917r3/Do3Rn/PuO4/8AqTsz/wCv3v3Xup+L/md9AV+Ro6PIbS7W25RVM8cVTnMljdu12PxcTsFasraXE5ioyMlLDe7mJHdVBNja3v3XurD6Wqpa6kpK+hqYK2gr6WmrqCtpZBNS1tFWQpU0lXTSr6ZIKmCRXQ/lSPfuvdZ/fuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691Uf/NSv4ui7E/5zsDi50n07W+o+l7e/de6K78XfhvJ8ltp7q3QnY8eyv7r7op9tNQSbafOGvNThqfL/AH33C5OhEAQT+Lx6W/Te/NvfuvdGZ/4aqn/5/wAwf+gBL/8AZB7917r3/DVU/wDz/mD/ANACX/7IPfuvde/4aqn/AOf8wf8AoAS//ZB7917r3/DVU/8Az/mD/wBACX/7IPfuvde/4aqn/wCf8wf+gBL/APZB7917r3/DVU//AD/mD/0AJf8A7IPfuvde/wCGqp/+f8wf+gBL/wDZB7917r3/AA1VP/z/AJg/9ACX/wCyD37r3Xv+Gqp/+f8AMH/oAS//AGQe/de69/w1VP8A8/5g/wDQAl/+yD37r3Xv+Gqp/wDn/MH/AKAEv/2Qe/de69/w1VP/AM/5g/8AQAl/+yD37r3Xv+Gqp/8An/MH/oAS/wD2Qe/de6rf7t60PTnaO9OsWzQ3G20K2noDmxQHGLkTU4uiyXlFA09UacJ9747eRr6b/m3v3Xutjr49G/QPR55P/GJ9i8klj/xY6b6k8k+/de6F/wB+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3VR/81L/NdF/8tOwP+hdre/de6XX8rf8A5lP2t/4lDGf+8bQe/de6s29+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691rcfNn/sqbun/tf47/AN5fB+/de6vn+PP/ADIHo/8A8RPsb/3R03v3Xuhg9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3VR/wDNS/zXRf8Ay07A/wChdre/de6XX8rf/mU/a3/iUMZ/7xtB7917qzb37r3Xvfuvde9+691737r3QXdt9z9b9HbaXdPZG4Y8PSVMr02IxlNC+Q3BuGtjXW9Hg8PCVqK14wR5JCUghuPJItxf3Xuq/wCr/mn7NTIGPH9LbsqcQHIFXWbrwtHlZYg1vIuPhoaqjjdl5CNUG1wCffuvdHO6L+TvU3yEpqlNj5WrodyY+nFXldkbkhioNz0NJq0PXU8MUs1HmsbHIQrVFJJIq3GtUuPfuvdGD9+691737r3Xvfuvde9+691737r3Xvfuvda3HzZ/7Km7p/7X+O/95fB+/de6vn+PP/Mgej//ABE+xv8A3R03v3Xuhg9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3VR/81L/NdF/8tOwP+hdre/de6XX8rf8A5lP2t/4lDGf+8bQe/de6s29+691737r3Xvfuvdc408kiJqCBmAZzyEW/rkP+CLcn/Ae/de61kPkp2znO8+7N2bnqWqqqjp85VbN2HhkLSLjtvY3JSYvFY/H099CVWYqlNTOwAM0892NgAPde6Pftz+VtVVWzqefdfa8uD7Cq6GKplxON2/Bk9q4Stmi8qYqvrJKqDJ5M0rEJUzwCNQ4bxq4ALe691XHP/pD+OXb0xjlOC7I6o3PInlo5melmqqFg0kaygJ95gdxY5wrKygS0s9mF+PfuvdbO+z9z0W9tn7T3njU8VBu7beF3LSwlizU8WZx8Fa1MxP1NNLK0YP1svPPv3XulF7917r3v3Xuve/de697917r3v3Xutbj5s/8AZU3dP/a/x3/vL4P37r3V8/x5/wCZA9H/APiJ9jf+6Om9+690MHv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de6qP8A5qX+a6L/AOWnYH/Qu1vfuvdLr+Vv/wAyn7W/8ShjP/eNoPfuvdWbe/de697917r3v3XuskTKsiFwTGTpkC/qMbjRJp/2rQxt/j7917rVo7b2VuHpjubd22K1DS5raG9p8zhqiSPVDVUL5b+P7Xy8MZ4noq2jaJrg2Yhl4INvde6uM29/Mr6Dr9oQ5zdVLvLCbzio4/4psmgwE2UNZl0hH3AwOcSWPFNjKypBMUlS8Lwo1pFJXn3XuqbOzt67j747c3NvCPDn+8nZO6IYcLtvHlqqSKWuanxG3sHBIqq1VNDTxwo8ulQz63sF9+691s0dc7TbYXXewtjSSrPNs/Z23tu1MyMGSWtxmMp4K542H1jNYH0nkEC4+vv3Xull7917pBdj9obA6i2427Ox9zUO18EalKGmnqxNNVZLISKXSgxOOpY5q7JVfjBdkiRvGg1OVHPv3XuuXXPZ2we3NuLu3rjc1DunBGpehnqKQTQ1OOr41DvQZXHVUcNbjazQQypLGutCGUsOffuvdLv37r3Xvfuvda3HzZ/7Km7p/wC1/jv/AHl8H7917q+f48/8yB6P/wDET7G/90dN7917oYPfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691Uf/ADUv810X/wAtOwP+hdre/de6XX8rf/mU/a3/AIlDGf8AvG0Hv3XurNvfuvde9+691737r3XvfuvdFm+RvxW68+R+Oo5s7JVbb3zhKR6Pb2+sRDFPX09C0jTNh81QStHDnMH521rGzpNAxJiddTA+691WjV/yv+8o8kaeg331VXYoyELl5q3cOPnEd+GkwzYiplWTT9VWZhf8+/de6PX8Zvg/sjoTJQb2z2WHYHZtNDLFjc49A2P29tUVMbRVUm2sTM89Q2RmiYxmtqmMqJcRJHqJPuvdHc9+690GPcHcGx+jNj1+/d+1zw46nJpcXiaIxvmtz5l4y9LgsFSyEeesnteWQjxUsV5JCFHPuvda6vc3c3Y3yV7Hp87n46irrKuqXB7D2LhEqa2jwNHX1CR0OAwdHGplyGVrpWX7mp0+ern5NkCqvuvdd9K91dh/GvsSpz2AiqIJ6eqfBb+2JmlqKKiz9JQ1JjrcJmaR0E2NzOOlVjTVOgTUkw/KMyt7r3WxX1H29sfvDZFBv7YWQaqxdUwpcljarxx5nbGaRFaq2/n6WNj9tX0xN0cftVMVpIyVPHuvdCZ7917rW4+bP/ZU3dP/AGv8d/7y+D9+691fP8ef+ZA9H/8AiJ9jf+6Om9+690MHv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de6qP/mpf5rov/lp2B/0Ltb37r3S6/lb/APMp+1v/ABKGM/8AeNoPfuvdWbe/de6SW+997S6z2lmd875zVPgdsYGATV9fMC8ssrcUuNxtMtpchlsjNaKnp47vI5/Cgke691Rl2J/ML793HvyTcPX+eHX+zMfWA4HZL4zE5aGsx8b2Em8qipp5ZsrX5KIXnSGSKKnDaIiCus+691bX8afkps/5H7QfJYpIcDvbBQwR732PJU+WpxVRJaNcziHktLktq5GXmCotqgY+GazgFvde6Mf7917r3v3Xuve/de6DDuHuHY/Rex6/fu/a5oMdTsaTFYmjMT5zc+ZeNnpsFgaR2BnrJyLySEeGlivLIQo5917rXT7p7o7E+SPYkW4dxRVVXV1VSuD2JsXBR1FbSYCjrKlUosBgqGMGXIZWvmK/cVBXzVk3Jsiqq+691cH8N/hvRdH0dL2J2JT0mT7jyFK32dMHSqoetcfWxWlxuPkGqGp3XURNpra1LiAEwQG2t2917rF8yfhtSd2UlV2N1vR0mO7gx9KDkKHVHTUHZOPo4rR0NY5CRU27qeFdNHWNYVACwzH9Dr7r3VQfSXdnYXxu7Cl3Bt2KphkhqWwm+9i5paiio8/R0NQyVmEzlHIolx2Zx0isaap0+akm55jLK3uvdbFvUXb2yO8dkUG/dhV7VOLqWFLksbVmKPNbazSRrJVYHPUsbH7evpwbo4/aqYrSRkqePde6oE+bX/ZU/dP/AGv8d/7y+D9+691fP8ef+ZA9H/8AiJ9jf+6Om9+690MHv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de6qP/mpf5rov/lp2B/0Ltb37r3S6/lb/APMp+1v/ABKGM/8AeNoPfuvdWFb733tPrHaeY3zvnMQYHbGCg81fXTWeWWV7rS43G0t/LkcvkZbR09PGC8jn6BQWHuvda6/ya+Te7Pkdu1a+uSqwOwsHUyrsbYySmZKBJj4BmcwILplN25VCBI6hlgVhBCLAl/de6Nh1L/Lfz27upcvuLsDOVWx+yNx0NNXdd7fqIg1FtyFLVET9gwBWqTUbjjIjNPERJjYWEjapLxj3XuiJUdZ2x8a+1zNCMjsHs/Ylc0M8EwWWKWCbiWmnh4pM/tTcNIAQRqgqYGDKQ4BX3XutgH40/JbaHyP2e+TxiQ4De+Cigj3tseWo8lViqhwsS5nDtIfLktqZKYEwT8tAx8M1nALe690ZD37r3QYdwdw7H6L2PX7937WvDj6djSYrEUbRNnNz5pkZ6bBYGlkYGoq5gt5JD+1TRXkkIUc+691rpd0d0di/JHsRNxbgiqqurq6gYTYmxcGlTWUmAoqypVaHAYGijUy5DLV0pU1NRp89XN6jZAqp7r3Vwfw3+HFF0hRUnYvYdNR5PuLJUjGlpgyVdB1vj62IiTGY+T1RVO654m01tatxACYYCAHdvde6Pz7917r3v3XuiBfMn4bUndlJVdjdb0tJje38dShshQ3jpKDsugo47JQ1r+iKm3bTwrpo6trCpAEMx/Q6+691UH0p3Z2H8b+wpdw7djqYZIahsLvzYmaWooaPPUVDUFKzC5uikUS47M4+UN9tU6RNSTf1RmVvde6x/JHsDb/avdO/+xdrrWx4Ld9VispQ0+Sh8GQoiNu4qkrKCsRWaNp6KuppIi6EpKEDrww9+691sO/Hn/mQPR//AIifY3/ujpvfuvdDB7917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuqj/5qX+a6L/5adgf9C7W9+691N/l5772l1l8eu798b5zMGB2zg+x8ZPXV0/qlllk2dQLSY3G0yny5DL5GX9unp4/XI5/Cgke690Q35NfJrdvyN3YldXx1OC2HgqmVNjbFSUzpQLMfAuYy/gBXKbtyasA7gMsCsIYBa5f3XurEvhV8Kf7nfwnuTuXEJJvCRIMlsXYmShWSLaEUiCSl3JuSlcMku6pEbXS0rgrj1IdwZyAnuvdWiElizMSWYksxNyWYkliTe7Em9/6+/de6K78oPi9tb5IbXjjeSl2/2Rt+llGy96tAW8KkmU7c3EIR5q/bFdP+OZaKVvLF/bR/de6oOo6vtj419riaIZHYXZ2wsg0U8M4E0E1NOB5YKiPil3BtTP0g4/VBUwMGUq4BX3Xurs9k/PbprPdL5TtPdNSNt7l2uKXHbk6zgnSfcOQ3HVxuMdDsyOcq2WweelhZoqpuKFAwqSpQF/de6pi7n7n7F+SPYkW4twxVNXW1dQuD2NsXBx1NZSYCirqhVosDgaGNWlr8tXSsv3NTpM9XNdjZFVV917q4T4cfDii6PpKXsXsOno8n3JkaRjS0waOroOt8fWRaZMZQSXeGo3XURNpra1eIReCA21u3uvdH4+nv3Xuve/de697917r3++/31/fuvdEC+ZHw3o+66Wr7H65paXHdwY+lD19ADHSY/sqho4rJR1rHTDTbup4l00lY1hUgCGY/odfde6oZrqSsx9TW0GRo6vHZChnqaKvx+Qp5aOvoayld4amkrKSdVlp6mnmQq6MAQR7917raD+PP/Mgej/8AxE+xv/dHTe/de6GD37r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3XvfuvdVH/wA1L/NdF/8ALTsD/oXa3v3XuqmHzWWOFi222Rrn2/HmXz8ODSSU0LbgqKOLFfxT7KPUtRlGoYkp43IZ1T0pbUb+691cn8KfhQNnDEdy9y4pX3jIkOR2NsPIwpLFs9HVZKXcm5KaQMku6pY2DUtK4ZMepDuDOQE917q0UkszMxLMzFmYklmZjcsxPJYn6n37r3XXv3Xuve/de6K/8ofi9tX5I7WSOSSm2/2Tt+llTZW9TAWEK3aX+7m4lhUzV+2a+Y8j1SUcreWL+2r+691r0bn653zs7fFT13uPaOZo9+UeR/hEe3YcfUV2Qr6qodY6RsH9tE/8YocnqRqeenLRSowNxyB7r3V33w3+HFF0fRUvYnYUFJku5MjSN9pTKyVWP62x9ZEBJjMdJ64qndVRC2itrV/zAJhgNtbt7r3R+ffuvde9+691737r3Xvfuvde9+691737r3Wtx82uflR3UxtdtwY9iQALsdr4O7GwF2Y8k/Unk+/de6vn+PP/ADIHo/8A8RPsb/3R03v3Xuhg9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3VR/wDNS/zXRf8Ay07A/wChdre/de6S/wDLa6N2Ru+o3R3Luim/jmb2Hueh2/tHC1sUUuExeRnw8OYfdM8DahXZilE4ipFkBipmBlAZ9JX3XurmWYszMxLMxLMzElmZjdmYnkkk8n37r3XXv3Xuve/de697917r3v3XusbQ071NPWvS0kldSRtFS18lJTyV9LE99UdNWvE1VTxtqPpR1HPv3Xusnv3Xuve/de697917r3v3Xuve/de697917r3v3Xutbj5s/wDZU3dP/a/x3/vL4P37r3V8/wAef+ZA9H/+In2N/wC6Om9+690MHv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de6qP/mpf5rov/lp2B/0Ltb37r3S+/laxSP1N2sUjkcf6T8ZcqjMA39zaD03APOnn37r3VnH29R/xwm/6lP/ANG+/de699vUf8cJv+pT/wDRvv3Xuvfb1H/HCb/qU/8A0b7917r329R/xwm/6lP/ANG+/de699vUf8cJv+pT/wDRvv3Xuvfb1H/HCb/qU/8A0b7917r329R/xwm/6lP/ANG+/de699vUf8cJv+pT/wDRvv3Xuvfb1H/HCb/qU/8A0b7917r329R/xwm/6lP/ANG+/de699vUf8cJv+pT/wDRvv3Xuvfb1H/HCb/qU/8A0b7917r329R/xwm/6lP/ANG+/de61tPm2rL8qO6lZWVhn8bcMrKRfa+DtcMBa/v3Xur5fjz/AMyB6P8A/ET7G/8AdHTe/de6GD37r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3XvfuvdVhfzOOvt3bl2b1pvTb2FyGaw+ysluWk3QMVST19XiYNww4g4/K1NHTJJUHFifGPFLKqkRO6arKb+/de6p7w+4t8begmptvZvemBpqmYVFRT4TIbhxNPUVCxiJaieHHy08U04iAUOwLaRa9vfuvdO3+kHtb/nt+zf/Ql3h/8AVvv3Xuvf6Qe1v+e37N/9CXeH/wBW+/de69/pB7W/57fs3/0Jd4f/AFb7917r3+kHtb/nt+zf/Ql3h/8AVvv3Xuvf6Qe1v+e37N/9CXeH/wBW+/de69/pB7W/57fs3/0Jd4f/AFb7917r3+kHtb/nt+zf/Ql3h/8AVvv3Xuvf6Qe1v+e37N/9CXeH/wBW+/de69/pB7W/57fs3/0Jd4f/AFb7917r3+kHtb/nt+zf/Ql3h/8AVvv3Xuvf6Qe1v+e37N/9CXeH/wBW+/de69/pB7W/57fs3/0Jd4f/AFb7917r3+kHtb/nt+zf/Ql3h/8AVvv3XumVMbvXe2cSnpsZuzd+6s7UxU8EX2eXzOby9bIEp4I3nnSapmcgKuuR9MaC7Mqi4917raG6n2zktl9V9Z7OzPhGY2psHam38stPIJYEyWMxFNBWxxSr6ZUhnDJqBIJUkEix9+690v8A37r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3XvfuvdclZkN0YqbFbg2urCzKf6qw+oPB9+691i8UP/KvSf8AnFR/9ePfuvde8UP/ACr0n/nFR/8AXj37r3XvFD/yr0n/AJxUf/Xj37r3XvFD/wAq9J/5xUf/AF49+6917xQ/8q9J/wCcVH/149+6917xQ/8AKvSf+cVH/wBePfuvde8UP/KvSf8AnFR/9ePfuvde8UP/ACr0n/nFR/8AXj37r3XvFD/yr0n/AJxUf/Xj37r3XvFD/wAq9J/5xUf/AF49+6917xQ/8q9J/wCcVH/149+6917xQ/8AKvSf+cVH/wBePfuvde8UP/KvSf8AnFR/9ePfuvde8UP/ACr0n/nFR/8AXj37r3WSO0Ta4o4YnIK64YIIXswsw1xRo4DDgi9iPfuvdde/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917qFk6mSixWXrogjTUGIyuQhWQExtNQ4+pq4lkAILRtJCAwBBt7917qj/GfzNO/K2uxVNLtfq8JXZTG0MzR4bMKyRVmQgpJWj1ZllEixSkrcEavfuvdXpTII5GRbkLp+v9SoJ/3k+/de6qe+TPzo7c6Z7u3n1rtbA7CrcDtxcEaGqzWNydTk5Tk8LSZGo+5mp8nTQvpnnYJZBZbX59+690Pnwx+VuV+RlHvfD71ocDiN87UqKHKU1Jt+KopqDI7QyI+0WtWnqqmql+6x2XjMU5D6Ss0ZsPfuvdHg9+691737r3XvfuvdV1/Mf5n7l6C3rtvYHXeM2xmM6+CO4d4PuOmrKyHGQZKUx7dx1NHRVdIYayqp4JamXWTaJo7D1H37r3TX8PPmN2f8gu1MvsfemF2XjsRQbGye5oajb2PyFLXtX0WTxtHFFJJVZCqjNK0VYxYab6gOR7917qyj37r3XvfuvdFC+ZfyLzHx26929lNow4as3vu3c6YrDUudpXr8ZFh8bSvW7hyNRSQ1FNJI0SvBBCdekSzc/T37r3VcuP/ma98w5DHTZnB9bVGGiyFDJmoKHbVbT102IWqiOUjoag5aQQVhofJ4mKtZ7cH37r3V51DXUOVoaDLYuoWrxeWoaLKYyqQhkqcdkaWKtop1ZSVYS086ng2v7917qT7917r3v3XumnPZ/BbVwuS3HubMY3b238PTNWZbN5irjosbjqZf8AdtTUSkKC7WVEW8kjkKgZiB7917qr7tj+Z1gcbU1OK6U2QdziF3i/vjvaSqxOIqJFYr5MRtujK5WrprC6y1U1NrH+67e/de6KlV/zGPlLUTtNFndi42JzeOmo9iY4wRr9QFarqKidmUcElr+/de6W+z/5mvd+KqYRvba+wt7Y8MonFJQVm0MqYyBfxVuPnrqPyjkgvTMp+lh9ffuvdWa9B/LHqX5Cx/w/bNbV7c3vBTvU1vX25zTwZ5oIQPuKvCVMDNQbkoYb6mamPmjT1SRIOffuvdGY9+690Wv5bdvbr6M6TyfYmy4MNUZ+j3LtjEQx5+ikyGNNJmaySCraSmiqKV2mVEBRg40n8H37r3VWJ/mY/IofXE9Vjm3/AB6lb9f9jmfz7917r3/DmPyJ/wCdT1X/AOgrWf8A159+691im/mafIuOMsuK6rBBQc7UrbcuqngZn8A+/de6vfopnqKDHVUmkSVeNx1ZKEBVBLV0MFTKEU3KoJJTYc2Hv3XupHv3Xuve/de6Id82vkz2N8dT1r/cGk2rVDd/95/4qNy4moymj+DjFfaGj8NZRiG/3z676tXH0t7917oGvi98/wDc/Y/aFB193JSbQw9Fu2JcdtHPbfx82Hgpd1hy9Lis19xW1cb0uej/AGKeUFfFVBAbiTj3XurVyCpswIIJBBFiCPqLH8j37r3XXv3Xuq/Pm18o+y/jrmetqDYFHtKqp93YrclblDuXET5SVJsRW4+npRRtDXUYgjZKttYIa5A+nv3XulB8Ivkh2D8jMZ2hWdgUm16WXZuR2rSYgbZxc+LSSPN0mXmrDWrNWVnndXoU0EadIv8AW/v3Xujx+/de6D3s3tfr3pvbM27uyNyUe3cQjmCjjkWSqy2arQuoY/A4in1V2WrdPLLGuiNTqkZF59+691Vd2N/NC3PVVM1L1H11h8LjULJBm+wJJczmagD9E5wGLqKXFUIb6iN6iob8Eg+/de6Af/hxb5TmXyf3l2SFLl/t/wC4mJ8QW5PjsZjKE/FtWq359+690NvXf80LelFVU9P2z13gtxY1ii1OV2JJNt7ORKbBp4sRkp6zDVui9zH5Kcn8Ncj37r3VqHVPcfXPdu213T1tuOnzlBEYosrQSRtRZ/b1ZKupaHcGFnP3WOmYA6H9UEtrxu3v3XuhO/1vfuvdVRfJn50dudMd3bz622tg9hVuC26MAaKpzeNydRk5P4pt3GZao+4lp8nTQtpqa1glkFkAB559+690BH/DnPff/PMdW/8AnnzP/wBevfuvdd/8Ob9+fX+6/V1h9f8AcNmv/r17917rr/hznvv/AJ5jq3/zz5n/AOvXv3Xuj5fCv5Mb7+RtJ2RPvjGbXxr7PqttQYwbao6ykWZMzBkJao1oq62rMjI1IugrpsCb39+690eL37r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde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a3DH37r3Qh+/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de6as//wAe7uT/AMNrcX/ulrvfuvdam23/APi67e/8ODB/+7qk9+691tzVX+fk/wCQf+hF9+691rmfPP8A7Kv7S/4JtH/3lsd7917oD94dZ5DbXXvUvY37tRt7tPFblEcrIGjotw7Uz1Vicrii4ABSfHmnqow3JV3/AAvHuvdXg/AfuE9o9EY/A5arM+7uq6iLZeX8smuoq8EsTT7Qyraj5CsmKjakZjf10h/rb37r3R2vfuvdVq/zK+3v7s9a4Dp/D1XizXZdaMpuIxtaWn2Rt6qR/tmAOpUz2eWOP+jRU0g+nv3XuqkYetMj/oSyvc9V5IMXH2hgetMEmi0WSrajA5jPbhqQxH7keLjpqWIFfpLKyn6ce690cD+WR/2ULuX/AMRLnv8A3e4P37r3V7Hv3Xukh2FvbHda7D3j2DlSpodmbcyu4GjYhfuamhpXbHUYJ+rVmRaKIDm5b37r3WrrgMTubtvsHE4RXkq92dmbygpp6k6nb+Kbpy5myNa/1YR0i1Usp+ulY/zb37r3VxX8xDpjFw/H7Y24NrUQSLouqxG3IxHGfIuxMpS0eAm4UElaPL0lJUN9beVzwCx9+690Ur+XB2X/AHQ71qdiV1T4sN2rgKjEwq76Yo917fE2X2/ILkJrrKYVdN+SfIoHv3Xur5PfuvdFP+b++6zr/wCMvYWQxtQ9NlNyDFbDx80TFJohuusFJlJo3UqQ6YaKoXg3AckfT37r3VHvxk6uou4u9Ouuv8lH5MDV5STMbigFwJttbYppMzlKT0g6Y6yKlSma39iU/wBPfuvdbOXpARI444Yoo44YIIUWOCnghRYoKeCNQFjggiRURQLKqge/de6yJDLIuqOKSQatN0Rm5sDa6gjgH37r3XP7Wp/5V5/+pUn/AEb7917pDbg6l673bk3ze6usto7mzUkEFLLl87tagyWRkpaRClJTyVlTTPPJFTRnTGpJCLwOPfuvdStudc7K2O9bUbP2LtzaD5FIIcjLt/BU2GNfHTvI9NHVtSQRCoWB5WKBr6Sxt9T7917ooX8xn/sljPf+HzsP/wB2VR7917qtv+XhlcThfkvQ12by+JwdAOv97QmvzeUoMPQCaaChEMJrclUUtKJpiDoTXqa3APv3Xur2/wDSF11/z8jrn/0Ptof/AF59+691Kod57JylXBj8VvjZOVyFUxSlx2K3htrJZCqdUaRkpaChyk9XUusaMxVEYhVJ+gJ9+690pPfuvde9+691737r3VRf81L/AJoZ/wCT5/8AK97917oCvht0dhvkF1t8jthZD7ejza0vXeY2Rn5UBfb27aOXd4oKhpbaxjcirGlrUHDwSk/VFI917ot/VHZG+PjF3PT7kbG1dFndn5Wv2xvvaFQfEctiVqRT7j23VKbRyOyxeajm5VZkilU6Tz7r3WyztLdWA31tfAb02pXpldtboxdLmcNXx2vLR1SXMVQg5graOYNDURnmOaNlP09+691Uh/NS/wCPm6N/8N7fH/u0wvv3Xulh/Kp/4sPff/a769/9124/fuvdOH80bfdVj9o9XdaUk7xQbpzOW3hmo0vapptrpBQYiCaxF44slk3mCm4Lop/s+/de6AL+Wn1lR7q7e3F2LlKVamn6rwlPJg1mAaKLdu55JqKkrwjAhp8ViKepeK/6JZUccj37r3V5n+++t/fuvde9+6917j8/77/H/G3v3XuqS/5m/WdFt/sbZXaWLpVp17FxNbiNzeMaUqNzbVFKtLk3CiwqMjgaqOOQ/V3pbn8k+690Jn8rXfFTNQ9tdY1M7yUuPmw2/sNC5JWlfIl8HuBIb+lY6mSmpZWUf2wzH6+/de6to9+690FHfP8AzI7uL/xGW9f/AHQ1vv3Xutcr44/8z36P/wDElbL/APdpTe/de62kJv8AOy/8tH/6GPv3Xusfv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de6as//AMe7uT/w2txf+6Wu9+691qbbf/4uu3v/AA4MH/7uqT37r3W3NVf5+T/kH/oRffuvda5nzz/7Kv7S/wCCbR/95bHe/de6Ojsfp5u6/wCWxtzb2Pp1n3Vtqfee+NlkLqnbN7f3Fl5qrGwH9X+53DtUUrKP1syXvpHv3XuiW/BvuOPqXvnbxydS9NtDsiCLYm5xK/jgpZcnUxvtrLVGvSI3xOf0KzG2mKeQH/D3Xuti2TRTmY1UqU8VMs0lVPMRHFTQ0yvJVSzOSBHFTxRszMeAqkm3v3XutZH5C9m5X5C977n3XhoKnIQ57OUOz+vcWgZ3bA0NUuF2vSU0IvpbK1EhqXsOXqCT7917qwj5qdYY/pj4T9MdaY4xSNtnsvb0eXq4l0rkdy1+2Nz1248kT9XNVlZ5ACf91oo+gHv3XugM/lkf9lC7l/8AES57/wB3uD9+691ex7917qtL+Zp2YMF1ftPqqhqNGQ7Gz38ZzMaMQ42rtF4p1icDnxZHcM8AN+GFOf6e/de6Kx/LW61O6O6Mx2NW0/lxfVuAkagd1Jjbd+6VmxuOKtyokx+HSrn5vYsh4Nj7917q63e20Mb2Fsvdmw8wEbG7x25lduVDuARAcnSSQU1YL8BqKsMcwP1BS/v3XutWimqNzdV77gqkWSg3h1pvBZDG143izm0MuRLGwIuI6qWiZTcf5uT/AG/uvdbT20t0YvfG1Ns71wjq+I3bgMVuLH6WDeOny1JHVfbsRf10srtE1+dUZvzf37r3RG/5mVNUz/G3EzwavDQ9sbTqK7lgPt5sZnqOHVYWKisnT9XF/pz7917ogv8ALjqaWD5R4iKpsJa7YO/aXHsW06atMfS1MiqPozy0kLqB+ffuvdX/AHv3XuqiP5gcHe83c20W6th7ck2+Or6AV56/TdTYb+Nf3mzvkFWcEDRjJ/Z+LVr/AHfFpvxb37r3RGfs/mB/yrfJX/qV2L/xT37r3TLn8z8nNqUKZPdOY762zjJKmOjjyO4a/e+HoHrJVZ4qSOrr5YIXqZVjYqgOohSQLD37r3Rk/gX2F2BuH5QbMxO4d+bzz2JqNv74kmxea3NmMnj55afb08lPJLR1lXLTyPBINSEqSrC459+690f/APmM/wDZLGe/8PnYf/uyqPfuvdUZdfdYb07g3GmydhbeG6dxS0Nbl0xBqsZRhqDGLG9ZVGbL1NJRf5Msq8F9Zv6QffuvdDh/shPye/58vH/5/tif/Xv37r3Rgfir8QO++t/kR1Vvnd3VyYDbW287kKzL5lcxtGpOPp59t5zHxzeDHZWorpNVVWRpaNGI1XIsCffuvdXgj6D/AAAH+2Hv3Xuve/de697917qov+al/wA0M/8AJ8/+V737r3WP+VaP8o70/wCoXrv/ANyN2n/iPfuvdP38xv45jKYwfIjaNFqyeGgo8Z2jQUkWqXIYSMrTYnenjQXeowl1pa5xy1M0cjf5tj7917oKP5dnyOGz9yf6B931/h2pvTISVewa+rmC0+396VPqqcG0kj2p8ZvAIDEL6UyKi1vMffuvdKP+amCNzdHAggjb++AQeCCMrhQQR+CD7917pX/yqf8Aiw99/wDa769/9124/fuvdB3/ADT4Zh2H01UsrfbzbE3JBC3JUzU24YHqFUH0hgk6Xt9QffuvdCN/Ksqaf+7/AHpREL97/H9i1o+ms0X8JzFNf8No+5X/AFr+/de6td9+691737r3XvfuvdVcfzTaimHX/TlGSv3sm+txVUYJ9f2tPtxYp9I+ukTVMV/xci/v3XugL/leQTP3D2ROqt4IOs4lnYXsr1G5KEU4IFgSzRtb/W49+691d3/r/X37r3QUd8/8yO7i/wDEZb1/90Nb7917rXK+OP8AzPfo/wD8SVsv/wB2lN7917raQm/zsv8Ay0f/AKGPv3Xusfv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de6as/wD8e7uT/wANrcX/ALpa737r3Wptt/8A4uu3v/Dgwf8A7uqT37r3W3NVf5+T/kH/AKEX37r3WuZ88/8Asq/tL/gm0f8A3lsd7917q234DMyfFLq50NmSs3kyn+jLuvIEG1jfke/de6qB+anTydOd9bmx+LpmpNpb1X+/mz3hDJDS0+YqJHy+Lp2HCS4LPrKqhTdI3jItx7917o6/ZfzA/jvwUwGRpsmB2p2F5unM/DFKEyFDUYGkhXem45QreSKLL7cenMb8apa+wNwffuvdAD/Lk6g/vv3JUdh5WkEu2uoqGLI0mtD4avfGXjlpNuwLxpJxFIk9aR9VZIj+R7917o5v8z3/ALJ72cP6dx4Mf+uvub/XPv3Xuie/yyP+yhdy/wDiJc9/7vcH7917q9pVZ2VEBLuwVFH1Z2NlUf4ljb37r3WuH82ezk7T+Re9a6jqBU4LZhh6727okDw/a7baSPL1UTLwRkNwTVL3H1Crz7917qD0f8uey/j5tfKbT2Dh9iS0WZzkm4MnX5/D19flqqualhooYXqqfK0UYoqKmh0wx6PSWYkm/v3Xuhm/4cx+Q/8AzqOrf/Qcy3/2Qe/de6Jf2Rv7K9pb63J2FnaHDY3N7rrUyWXpdv0s1FiWyH28NNPV09LPU1ckMlZ4A8v7hBkJItf37r3V0X8tjssbp6UyvXVdOZMt1ZuCSChWR7yNtHc5myeK0qefHQZRKun/AMBoHAt7917oznyY6xqO4eiuxdg4+ITZnIYcZfbiN9X3JtqdM3iIUv8A7srJaVqdP9ql/wAPfuvda5vUvY2T6j7M2T2TjaaaSt2buGnr6zGOpgqKyhRpaHcGFkR7GOoqcdNUQWb9Mtr/AE9+691tDbT3Xtzfe2MHvPaGUp8ztjcuPgyeHyNM6sHgmUF6WpRSTT5Cgl1Q1MTWaKZGUge/de6UiTTRgrHNNGpNyscsiAn6XIVgCbe/de65/dVX/KzU/wCwnm+v0A/X+T7917qmH+Zn3XSbi3DtTpHC5M5KPZNVPune8kVS1RTUm6K+i+xxOAch2jauxOKllmqF+sUlQimzA2917qP/ACwusauu3pvnuSrp2GK2zh5diYGoeM6KncGfNPV5x4HYaWOKwtPGjsOUeqAv9R7917o3X8xn/sljPf8Ah87D/wDdlUe/de6rq/lvf9lQ0P8A4jvfX/uPQe/de6v69+691737r3Xvfuvde9+691737r3VRf8ANS/5oZ/5Pn/yve/de6x/yrf+BHev/UL13/7kbu9+691blU0tHXU1TQZGkpshj6+mqKDIUFbGstJXUNZC9NWUdVE11lp6mnkZHU/UH37r3Wtv8rfj9XfHbtapwmPkrW2TuAybn61zaPKlRT4xatXfCtWgh1ze0a3RFquHaIQzD9Xv3Xup3yL+QMvf+zeiazOMTv8A2Pg91bY3y5TSmVqfucPJh90Qsq+Jv4/Qwlp1BvHVxy3FmU+/de6PD/Kp/wCLD33/ANrvr3/3Xbj9+690Jn8yjrKq3d01hOwsXTvUV/VWdeqyqQq0kq7Q3IkOPydYEUEmLE5OKlmkP0WJnY/Tj3Xuq9Pgz3djOme64Itz1sWO2V2Njk2dn8hM+ikw+Qarjqts5urc+hKGlyV4J3NhHDUs54U+/de62IHUqxDfXggghgysAysri6ujqQVYcEG449+691x9+691yRWkdUQFnc6VUfUmxP8AsBYcn8D37r3Wvx8+u7cV273HDhNrV0eR2f1ZQVe2KHJU8qyUWZ3LWVST7qytHKp0TUcU9NFRQyLcSLTMwuCp9+690df+WV1pVbd6x3j2fk6d4ZuyczS4zbwkTQ0m1tpmoifIpcXMOUzlTMEI4ZKYH+l/de6su9+690FHfP8AzI7uL/xGW9f/AHQ1vv3Xutcr44/8z36P/wDElbL/APdpTe/de62kJv8AOy/8tH/6GPv3Xusfv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve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91737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691GraZa2hr6F2ZI8hQV+OldLa446+kmpHkS/p1okxIvxce/de6rcov5X3UNDU0NTH2V2XI9BW0ddEskG29MklFVR1aJJpowQjyRAG3Njxz7917qy+RzI5cgAtbgfTgAfk/4e/de6I13J8DOuO6uyNw9mZ/fO+MPltyLjBVY3DRYNsbTfwvHwY6H7dqymkqSZIYAzaifUTbj37r3RlOmuq8N0n1vt/rLAZPJ5nFbckyslLkswKVcjUHLZGfJzipWjjjprRSzlU0gem1/fuvdCVNDBUwT0tVBFVUtXBPSVdLOoeCqo6uF6aqpZkPpaGpp5GRgf7LH37r3VfG2f5bPSu2d44Pdabo3xlaTA7jpdwUm1MmuFbC1Ix9b/EKDE1skdItXNi6aaONbatckcYBPJ9+691Ya7tIzO51O7F3Y/lmJLH8fUn37r3QGfILoXbfyK2TjNjbpzmd2/jsVuik3ZDW7eSheulraLH1+NjpJVyEUsIpXiyLMxADalHNr+/de6C/oD4YbD+O+96/fW194bwz+RyG26zbEtDuCPDpRR0dbV0dbJUxtQU8U/wByklEoFzp0sbj6e/de6FDv/onDfIbZ2O2NuLdG49s4Si3BTbjqRtuPHvPlqyhpqiDH09e2QilT7SikqWmVVAJlAJ+nv3Xugi6L+EHWvQu/4exsHubdu5s3R4fKYfG0+4o8QlFjWzEcdPWZOAUFNFKa77JXhQklUWVj+ffuvdHO9+691737r3TRuDB43dG389tjMwCqw+5MNk8DlaZrfvY/L0c1DVKpsbSrFNqQ/QMoPv3Xuq66L+WF1VjqihrKLtPtGnrsbVUdbQ1ccW2lngrMfPFVUlSpWiA8sVRArfgEj37r3VlYaRlUyv5ZdEYmmKqpnlCKss7KvoV55AXYDgFjbj37r3XJWZGV0ZkdGDKykqysOQQRyD7917or/bXw66A7krKnM7i2gdv7pq2Mk+7Nj1S7bytXKbXkyVJDDNhsrK1rl5qYynkl78j3Xuin1/8AKy2RJOzYvuPedJTMWKwV228FXTIt7hBUQ1FIraV/JQE/X37r3Qh7J/lp9C7dqYazduZ3v2M8Thxj8lX023MJKQbqtXSYGOOuqYl+hT7lVcEhgffuvdHy27tzb20MJRbb2ng8Ttrb2OBFDhcHQ0+NxtNcAM6U1OiK8zgeqR9Ur/VmJ59+6908+/de6CLvPpvA999d1nW25Mvl8Hiq3L4bMyZHBLSNkUnwtQ9RTwoK6OWn8UzvZ7i9vp7917oln/DXPUH/AD8vsz/qRts/7z9kL+/de69/w111B/z8vsz/AKkbb/8AqP37r3XCT+Vv0/IpQ9l9mgEi5EG2r8MGH1oiObe/de6sup4lp6akpkJZKSjo6NGb9TpR00VMjtbjW6xAm3Fz7917rL7917r3v3Xuiz/Ir4u7Q+Sn90P717l3Nt3+5v8AGfsP7uR4x/u/419j9wa05GGYjw/Yro0W+pv+Pfuvdcfjr8W9n/Gt94PtTc25txHeceDjr/7xpjUFGMC+TemNH/DoIbmc5R9eu/6Vt+ffuvdGa9+691737r3RMu8/g71X3tvh+wstm9z7R3BWY6loM4drx4o0m4J6EeKkzGQiyFNIyZVaIJBJIhAljjQsCwufde6X3xz+Mu0vjVR7yotp7j3JuKPetXhazIPuNMaj0b4KCup6daMY6KEFJ1yDF9dzdRa3Pv3XujH+/de64TRQ1EFRS1MEFTSVkElLWUlTDFU0tXSzArLTVVNOkkFRTyqbMjqVI/Hv3XuiRdj/AMvf4678qKjJYXGZvrDL1DNJK+x62NcFLM1yZTtnKx1eOpmYm5FM0CE/j8e/de6Ag/yr9qGU6e690iDyGyttHDeYRXvpMv34TyW/Om3+Hv3Xuhs68/l2/HfZNTBkc/S7i7PyNOVdI9510MG3xMlissm3MLHSU1ZYi+iollj4/SeffuvdHkpKWlx9HR47H0tLQY7H08dJQY+hp4aOhoqSIWjpqSjpkipqaBB9FRVHv3Xus/v3Xuve/de6Tm8ds0m9do7o2bX1NTRUO69v5Xb1ZWUYiNXTUuXo5aKeelEytCaiGOUsusFSbX9+690RjYv8uLqzYO8do70xvYXYVdX7Oz2K3DQ0dfDt8UdXU4ioSphp6xoKRZlglaOzlCGt9PfuvdWGM2tmfgamLcfT1G/H+HPv3XuuPv3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917oFex/kZ0h1FnabbHZHYON2tn6zFwZqnxc+PzVfUPiqmaanp6xzisdWxQxzzU0gUOysdJNrWPv3XusnW/yH6U7fzNbt7rXf+O3VnMdjHzNZjaegzNBURYuOohpZa1P4rjqGOeKGeoRXEZZkDAkAc+/de6Gb37r3Xv6f4n/bf6/v3Xui87t+WPx22HuXMbO3h2bQ4Hc+36v7HM4erwW6HnoaoxRzrG8tPhZqaVZIJkdXjd0dGBBI9+690K+xN/bN7O2xRbz2Dn6Xc218hUV1LR5ekiqoIparG1Bpa+nanroKargmpahdLLJGp5BHBBPuvdK/37r3XvfuvdJreG8dr9f7Zy+896Zmm2/tfAwR1OXzFWk8sFFDNURUkJaKlinqZnmqZ0jVI0d2ZgAPfuvdAvtv5dfG7eG4MNtXbHaePzO4tw5GnxOFxNLgt1Cor8jVX8FNG02EjhjLWJLOyogBLEAe/de6Mb+SP6Gx/wBf8/69vfuvde9+690EnZffPT/TdXh6Ds7fOP2lW5+kqq7D0tVRZaunraOinSmqakJiqCuMMMdQ4QGTTra4W+k2917pm2B8mehe09yw7O6/7Ixe490VNFXZClw8WOztBUVNJjIlnr5IJcpjKKmkamhbWyB9ZW5AIB9+690Onv3Xuve/de6Se+N9bR602xkd6b7zcO3drYl6NMjmKinrKqGlevqoqGjVoMfT1dW/nq50QaYzYtc2HPv3Xugv2P8AKT4/dlbqxmyNjdlY7cO68195/C8NBiNxUstZ9hRz5Cs0VFfiaWjj8FFTSSHXItwthc2Hv3Xuh9UFiAvJP0/21/z7917orFR83PilSvUx1HcmJjaklqIKgHAbuPilpJZIahDpwJuYpYmBtcG3F/fuvdGjikSeGCoibXDU09PVQOAQJKeqhjqaeQAgMBJBKrWIBF7Hn37r3XP37r3Xvfuvde9+690huw+y9h9TbeXdfY25aTau3nyVJh48lWQVtUsuUr1lelo4abHU1XVyySRwO5KxlURCzEDn37r3QQYj5k/GDPZbF4LFdvYepyubyNHicXSvh9z0q1WRyE6U1FTfc1WFhpoDUVEioGkdUBIuR7917ozLKysVYFWUlWU2urD6g2uLj37r3XXv3Xuo9XV01BR1mQrJRT0WPpKqvrahgzLBR0UElVVTlUDO4hp4mYhQWIHAJ9+690V5fnB8UHVXXuXEMrqrqwwG7rMrAMpF8ADyD7917rv/AGd74o/8/kxP/ng3d/8AWH37r3TpjPmP8W8vMsFL3ZtKnkdxGGy8eZwkGpvpepyuLpYFU/1LW9+690YbF5PF5zH0+WweUxubxVWoalymGr6TKY6pUgH9itoZZ6eQ2PKhtS/kA+/de6ne/de6B7sn5A9M9PZXG4Ts3fdDtLK5jGvl8ZSVeNzda1XjY6lqN6pZMXja6JFWpQrpZlbi9rc+/de6e+te3Otu4sXk811luql3Zi8NkY8TlKylosnQpSZGWmWsjpXjytFQyyM1Kwe6Ky2Nr349+690I3v3Xuve/de6ATfPyj+P/Wm6MjsvffZWO29unErRvksPPiNxVU1IuQo4a+iLz0GJqqR/PR1CSDTI1g1jY3Hv3Xukl/s73xR/5/Jif/PBu7/6w+/de69/s73xR/5/Jif/ADwbu/8ArD7917oQOuPkT0n29m6zbnWu/qHdWcx+KkzdZjqbGZyikgxUVVTUMlY0uUxlFTsi1dZEmkOXJf6WuffuvdDR7917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de65IFLAOwReS8jEBI41BeSV2JsqRRqWY/hQffuvdawPyP7O/wBMPd/Y3YCTGTGZDOS4vbuo+mLa+3R/B8Isf1CxzU9KZ/8AEzE/4+/de6nfF/s4dQd89c71nmaDD/xhNubmcEmN9tbp04bJNMoYBoqR546jngGEN9R7917rZwddDsmoNpYjUpBVhfhlYcMjLYgjgg39+691x9+691S3/M66vOI3tsnuGgpwtFvTGHaG45o1so3HtmIzYiqnI4EuRwEviueXNJfn8e690r/5XfZfr7J6ZrZ7XFP2PtiGRxYkeLDbqp6cMbk2+0qGUf0ZrfU+/de6t19+691737r3VW/8z7ssYzZewuoaKo01W7crJvPcMKtZmwG2malwtNOA2oRVudqDLY8MKX82Fvde6AH+Wj1idydtbg7Rr6YSYvrLCmjxDyJeN94brjlpYJFY8NLjMFHUy25s0yH+h9+691eN7917rkiNIyov6nZUX/gzEKP95Pv3Xutb75p9np2n8jN9ZGiqTPgdoSw9ebd0vqgFFtdpYMnUw/grkM/LVSlv7QK3+nv3XugZ6i7Gq+puz9h9j0TORtPcmPyVdHGTepwskn2edpfSQWFTh6mdbfQm39Lj3XutqGGopaynpq6gnSqx9fS0uQx9VGQ0dTQV0EdXRVCMpKlZqaZGBHHPv3Xusnv3Xuic/Pz/ALJP7L/6jNmf+9bivfuvdVOfAn/srjqb/X3p/wC8Tn/fuvdbFkH60/1v+iT7917rUZ3HxV7l/wC1tuM/7E5SvH++/p7917rbVxX/ABZsD/4bu3f/AHRY/wB+691O9+691737r3Xj9D7917qlr+Z52YcrvrYnUVDOXo9m4aTeG4o1YeMbh3Qpp8TTyAX/AH6HAQM/P0FV+Df37r3VXgkljZJaaZ4amCWKemniNpKeqgkSammQjkPDMiuP9b37r3W0h0V2VB2/0/172LGyNVbh25SDNIjBvBuTFr/C9xwmxJUjK0kj2POmQfS/v3XuhY9+690mt6/8eTvb/wAMzdn/ALz2S9+691qgYWjlyU+DxkLxpPlKnDYqnkl1CGOoyc9LQU8kxRWcQxzVClyASFBsD7917qyGX+Vx31FLLE2/umy0UjxMVyW7bFo2KEi+3gbXHv3XukXvb+XL8itnYSrzlAdk9gihglqqrD7Ly2QfPeCBWeVqLG5rH47+KSrGpbxQu0rAWVSeD7r3ReOi+/N//HrdcGf2bX1hwrVaDduxaiWVMFuegjl019DV4yT9qhzMaKwgqo1SennAuSNSn3XutmHa25sNvXbG3d47dqGqcFurB4vcGJma2s0GVpUq4ElA4Wog8hjkH4kQ+/de6po/mkj/AIy31b/4jOsP+xG56r37r3Q+/wArb/mVHa//AIkvGf8AvLQ/7b37r3Vm/v3Xuve/de612/n1/wBlYdm/9Quyv8P+YMwnv3Xunrpb4Idjd49c4Tsvb2+th4TE5yoy1NT4zNxZ1slA2JyM+OmaoahpJaYiWWnLJpY+ki/Pv3XuhU/4a27h/wCfn9W/9Sd0f/UHv3XujTfEb4Z78+OnY2f3runeOzNxY/MbJqdrQUW3I8yldDWT5vEZVaqY5GlhgNKIcc6EKS2tl4tf37r3ViH+3/2P1/2P19+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3XvfuvdFl+YfZ79T/Hjf+do6gU+d3BRJsXbTBtM38X3YJaCWeCxVtdBiBVTkjlNAPFx7917qjb4mdXR9q9/dbbPnpvucDj8om6tzRSIHp221tJUylXT1a8gwV9RFDSlTw5msfr7917qB8oesv8ART3x2bsdITDiRnJ87t1QhSFts7qDZnErTn0hoqNKp6f08K0JF7iw917q+n4ndnnt34+9d7rqagVGboMYdn7oZmvMNw7T0Yqoln/teSuo4oKi5+olv7917oxPv3Xui7fLDqxu4egd/wC06WBZ83j8eu8dqjSDINxbUEmTghha2oPkqFJ6Ugct5QOfp7917qgX46donqLunrjsEyvFjKDO02P3ABdde2dwf7h89DOt1AWGlq/MQfo8IJ+lvfuvdbQrqqsQkiSxmzRTRkGOaJwHimQjgpLGwYf4H37r3XkRpHWNf1Oyov45YgC5/Aufr7917rWm+XvaMXbHyC7C3RT1RqMFha3+5W2mB1QpgdoeTHtPB6iNFfk0qakkfq8gP9ffuvdXVfCnq1uqPjtsugrqbwbi3mJewty60KTrV7kjhlxVFNqAYHHYFKePT/ZZm/qffuvdGt9+690EHf3ZkfT3TPYfYmtErcHt+pgwQY8y7lzNsTt+JB/akGSq0kt/qYz7917rXF6U67rO2+2uv+vUkllfdW6aNMvVHU8gxVPM2X3NkJmXlb4+nnZm+mtxyL+/de6GH5t9WUfVfyI3jjMTj4sftTd0FFvfatHBEsdJTYvNoYK7GQBVEenF5ejniYAWAK/k+/de6tv+BvZr9kfHTbVLW1Hnz3XFXUde5jWxedqXGItXtmsk1EtpqcBUxRgm/qgbm9wPde6OT7917onPz9/7JP7L/wCozZn/AL1uK9+691U58Cf+yuOpv9fen/vE5/37r3WxZB+pP9b/AKJPv3XutRncn/Azcv8A2tdxf+7Sv9+691tq4r/izYH/AMN3bv8A7osf7917qd7917r3v3XusNTV0ePpazJZKdKXG4yjq8lkaqRgkdNj8fTy1lbO7HhVipoWN/fuvdarvaW/Mr272dvTf0kM1RkN87qrKzE0Prkm+1rKtcdtnFxIuo/tY9KaJVW/P0v+fde6OB81/jbTdKbX6AzWJpUSM7Gpdgb4qaeIrFWb6xETZ6TJ1Eg/VUZKLI1UQLXJSkUDhR7917ow38r3swTYjsXpuvqC0mMq4ew9sRO/K0OSaLE7npYEuDohr46aoIAsBMxP5Pv3XurYvfuvdJrev/Hk72/8Mzdn/vPZL37r3Wqjsz/j4tjf+HPsz/3eYr37r3W25W/8Daz/AKi6n/rfJ7917rFCsryxLCGMzSIItF9fl1AoUI+jBhcH8fX37r3Wr18j6zbtZ3/3RW7XkpG29L2NuaWjnoiv8PkZawjI1FIy/tmnfIpM2pfSeSOPfuvdX9/EvFZHC/GjpSgysUsFb/cahrTDMGWWKmylVW5KgR0f1Rt9jVxnT+AQPpb37r3VaX80n/mbfVv/AIjKt/8AenqvfuvdD5/K1/5lR2v/AOJLxn/vLQe/de6s39+691737r3Wu38+v+ysOzf+oXZX/vGYT37r3VrH8v8A/wCyVdgf9rTef/vUZH37r3RzPfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3XvfuvdeH++/33Pv3XuqWf5nnZv8V35sbqGhn1UmysM+79wxRt6V3FupPBiqeUXt56Lb9OXFxdRVH+p9+690Jv8r3rT7TbvYncVbTgS56vp+v9uTSKQxxmEKZTcdRTlhYx1OVmp4Sy/X7dl/qPfuvdNv8AND60D0nXHctHT+unlqOuNyVCr9IanzZjas81hbSk0dZTqxP9pR7917pJfywOyzQ7s3/09WzkU+5cZFvnbcTtZTmdvotBn6aBCbeasw08UxA5P2pNvqffuvdXMe/de65xuYpEkUXKMG0/UNY30kfkMOCPyPfuvdaznyz6qj6j767D2hT05g2/lK3+922Aq6Yn27u7y5KGKD6C2OrZKilNv0tDbi9vfuvdXhfDns9u1/jxsHN1lT9zndu0kmw9zMzXlGW2oI6KnnmHBByOF+1nUkerWf8AY+690q/kt2d/of6L7H3zBKIstT4OTBbaOoLI25tzE4bE+EHlpqdql6iwudMLH8e/de617/jp1bL2/wB0dd9dusk2OyechyO5pfUSu2cD/uZ3BPO/1UVNPTeEsfq84H59+691tCHRe0UaxRKFSGJFCJDDGojhhRBYKkUShVA4AHv3XuuvfuvdVK/zQezRHRdddM0U/qq55ux9zQo3P29KZsPtWmqFDcCSoarqVU8ERq39Le690k/5X3Wf3u6uwu4a2nDU+28ZDsLbkjobLmNwKmR3BUQMeC9PhoIYmINwKm30JHv3Xuho/mb9arnerto9pUNPryHXm4Rhc3IiXc7W3e0cEcr2FzDQZ+mhvfhROTx+fde6Kz/LW7LO1e6Mz13XVPjxPaW3pI6FJHtEm7tqrPlMbYWH72QxLVcC/wBWCj37r3V6gNxf37r3ROfn7/2Sf2X/ANRmzP8A3rcV7917qpz4E/8AZXHU3+vvT/3ic/7917rYsg/Un+t/0SffuvdajO5P+Bm5f+1ruL/3aV/v3XuttXFf8WbA/wDhu7d/90WP9+691O9+691737r3RLvnx2Y3XXx23DjqKo8Ge7LrabYOKCPomXH1iNX7prEsdWmHCUzw6rW11Kj629+691VB8G+sU7K+RuzUq6bzbf2DHUdh5wMheFk2+YkwNE5tpD1m4ainCgmxWNvfuvdXPfMDrSTtr489jYClp/uc9iKBd87bCreU5zaZkyZiisGYvXY37mCw/V5B9ePfuvdUP/F3tAdR97da75lnaHD/AMXTb+5PVaN9s7qUYfJef/VR0hqYqj/BoQfqPfuvdbOjrod0uG0sQCCCGH4YEXBVhyD9CPfuvdJjev8Ax5O9v/DM3Z/7z2S9+691qfYmqmoJMPkaYqtVjZsVkqVnUSRrVY6WmraZpIzYSRrPApZTwwuPfuvdHkk/mRfJ+WSSV8716Xld5HtsTFAapGLtYfc8C59+690jd6fOj5L7+wtZt6u39R4PFZCnkpckmytv4zbdfW0k4Mc1NNmaQS5OGnmRirLDJFqX+1yffuvdKr4W/HHrfu3d1PUb87C2wtNgqgVsfTlNVVNPvLeKUTCYGqeqhpqVdtq0YaoSikqaqaO6sIlLH37r3WwSqpHGkcSRxQxJHDBDCgigghhjWOGCGNAsccMMSqqKBZVAA+nv3XuqTf5pP/M2+rf/ABGVb/709V7917ofP5Wv/MqO1/8AxJeM/wDeWg9+691Zv7917r3v3Xutdv59f9lYdm/9Quyv/eMwnv3XuovU/wA3e5el9iYrrnZ9HsKbb+FnyVRRyZzB1lbk/Jla6bIVQnqYctSxyKKic6AEXSvHP19+690I3/Dl/wAiv+VDq3/0F8h/9fvfuvdWa/DLvDeXyA6ozW9t9Q4CHM0G+sltunTblDNj6D+H0eLxtZEZIJ6usd6oy1j6nDgFbcfn37r3Rs/fuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvde9+691737r3Xvfuvd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697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3XuiW/Pvs5uuvjrnsZRVJgzvZ1dT7Axao2mZcdVg5DdFYlvV44MLStCSONVSovewPuvdUndDdGbr+QW+W2DtCtxOIqqXAZLcNfmc4la2HxmOxpp4UFWMfFNVGauqqlIYAq2Zz/AEHv3Xujk/8ADXXcP/Pzerf/ADl3d/8AW/37r3Xv+Guu4f8An5vVv/nLu7/63+/de6Jd3n0tujoPsCs653dV4zKV0eIxeapMxhVqxh8vi8xAzxVFB99HFVD7eeKWCVXUFZYz+LE+691eR8E+zj2V8ctqQVs/n3B13PUdeZvUxaVosPHHPt2re5J01m36qEAn6tE30+nv3Xujh+/de6RPZOwsb2l19vTrrLaRR7y27kcGsjAH7avqIvLia0XvY0GVhhmuORo9+691rM9c7pz/AEr2/tTc1RDLR5zrnfUEWeowrrKY8XkZcRuih0EqWWqx7VKBf7QYDm/v3XunXvnsaq7p7q35vyihqahN17mFFtOgdGNUMJTvDhNo4wQm7pI9HFFdLXWSVuPx7917rY16R61p+n+pdg9bwBPNtnb9LFl5EAAqdx198juKqYj9Ty5aqlF7/pRR+PfuvdCl7917otfy87PPUnx77B3HS1H2+dzOPXZG2CraZjm92iTG+eCxDGSgxjVNRcfp8d/9f3XutffpjqXcXdfYe3+sNpT0NFlczDXy/wASyi1T4rEY7D0MlZWZHI/ZpLU/awxxqg0As0kige/de6PF/wANddw/8/N6t/8AOXdx/wDkf7917r3/AA113D/z83q3/wA5d3f/AFv9+690Uj5A/H3ePxy3bh9qbuyWFzhz2313Di83t5K9MTVQLWzUFZQj+JQwziux9REvlW1tMiMPr7917q3D+XF2YN5dE1Gx66pWXNdU56bCxRu95pNp50zZjb81j6vHR1L1VL+RaNRwLAe690u/n7/2Sf2X/wBRmzP/AHrcV7917qpz4E/9lcdTf6+9P/eJz/v3XutiyD9Sf63/AESffuvdajO5P+Be5fr/AMXXcX4v/wAvSv8Ar/Qe/de621MUy/wbA3kiH+/d27/u2P8A50WP/wBq+vv3Xup2pP8AjpF/1Nj/AOjvfuvdM25ty4rZm29w7xzkix4bamEym4si7Mqq1JiaOWsaMObqDUNEIxzyzgD37r3Wq1lMpuLs/feQzFRFLkN19jbxlqxTrdpJsxurMWpaRfqwjgkrEjH10on+Hv3XurCH/lb9xxuUfszq0MtgwFLu4gNb1LcY8g6GuLjg249+691w/wCGuu4f+fm9W/8AnLu7/wCt/v3XugG+QXw67E+Om18Ju/c24tq7nw2Zz393Xl2xFmUbE18lFLW0RyP8UpoFEOQWnkjiKknyJY/Ue/de6M1/LA7MNBuzf/T1dUhafdGNh3ztyGRuGze3kFBn4KcH/dtXhZ4pio+v2xNj9R7r3Vum9P8Ajyd6/wDhmbs+n/hvZL37r3Wqjsz/AI+LY3/hz7M/93mK9+691tt1rJ97WfuRf8C6n/dsf/HeT/a/fuvdJ7N7k2ztjH1WW3Pubb228VQxNNWZHO5rHY2kpo1GotI9TUI5OkXCqrMfwD7917rXM+X3cmF7z7zz28Nr+Z9qYzF4naO2aueF6eXKY7BJN9xmft5QJoIcrkKmWSJHAYQhCQCSB7r3Vtv8vHa2T2z8ZMHU5WGWnk3lurc28MfBKHRv4LVy0+OxlUqPbTFkI8e0yWFmVg35B9+690Sv+aT/AMzb6t/8RlWf+9PVe/de6Hz+VuVHVHa93Qf8ZLxn6nRSf9+tDyAzAke/de6s21J/x0i/6mx/9He/de674P0ZSPwVZX/1wdJOn/Y+/de612/n1/2Vh2b/ANQuyv8A3jMJ7917q1j4AaR8VdgXeNT/ABTenDSIrf8AH0ZH6gsCPfuvdHL1J/x0i/6mx/8AR3v3Xuu7qb2dD9f0ujkf4kK1wP8AH37r3Xj7917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuqgv5gOxu+O3e09vYfY/Ve+NybF2DtmOKizGLxnnxeT3LuGRK/O1FK7TJ5loKaGmpS2n9cbgce/de6G3+Xj0NubqnZ++93dhbZyO1d5bxzdJhaTD5mFafKUm09vRGdJpYlaTxw5bMVbyKLgssCk/j37r3ViHv3Xuve/de6rb/AJiHQe6+z8L15vfrrauT3bu3bmQr9rZfE4SnFTk59sZhTkqSu8WtDJT4jL0zhzf0rVH37r3QZ/y+dld69S9jbu29vzqze+2Nj7622Khsxl8aIMXjN1balafFtUyCd/D/ABPHVVRTg25cID/Ue691bh7917r3P4JH+I9+691SN82Pit2fkO+c1vLqzrvcW8Nt9gY6h3JkJNuUKVMWI3Uqfw/cFHVL5IvE+Rlpo6xOLHzNze/v3Xuk98R/iZ2qe+9mZztHrTc21Nn7Lep3lUVW4qBKakyOYwyA7dxMNpZDLUTZiaOYixAjga9uPfuvdXtlmYlmN2Yksf6sTcn/AGJPv3Xuur25/p7917qrb+YhtTurtHMde7G67603luzae2qGv3VmMvhMd9zjJt05djjqKhErTIJajEYmncuLWRqm3v3Xusv8u7487064yXYnYvZW0cvtDP1NHQbL2vjs9SrS5A4udly24MrBGJJLU1VNDTUwbi5jcW/J917q0T37r3XvfuvdEO/mAdI7g7d6s27mNk7erty742FueOajxGKiWbJ5PbW4Y1oM3T0kZePyGgqYaaqK3uURrC/v3XuinfBbrv5AdO94RSbr6k35gtjb5wGQ2xuXJ1+KCY/FVUA/i23MvWOtQ/iipslTNAzWYhKk/i59+690f35p7S3Tvr417/2vszAZPdG5MhU7UkoMHiIRUZGrSj3NjaqqaCEugcU9NGztzwqn37r3Va3wy6B7w2P8mutd1by6p3ntjbOL/vWclncvjVp8dQ/d7SzVHS/cTCZyn3FXOka8cuwHv3Xur0YeGW/Fhzf/AIKffuvdazeS+LXyUlymWmi6N7EeKbMZiaJ1w6FJYZspVywyLeo5SWJww/qD7917rr/ZZvlUOB1B25YcAaK6wA4AA/iXAA+nv3XusFV8ZflY1NOqdP8AbpZoZAoCV1yxUgAf7kvrf37r3VvXy3xvauS+LOzOteu9j7r3Rufd+K2Lgt4UuFozVVOCwG38JjshmqfJs0q+KfI5ijhpSCSW0yc/W/uvdEs+GHxX7Toe/dtbs7R633JtHbGxKLIbrgqNx0CUtNldzQRrRbdx1KVlkMs1NW1ZrHFhZacH68H3Xurwrk/Ukn6kn8k/U/659+691737r3QIfJLrD/TF0b2JsCCFZsvkMK2V2zcetN1bfcZbBCEkgJLVVEBpwbjicgmxPv3XuqSemOmvlH1T2p172PSdFdkhtrbjxtdkII8Mpeow1Sxoc/R6TVAMs+IqpgQeCwHHHv3Xutgfd0EtbtTdtJQRS1U9ftXclLQU6JaepnrMFXRUlOkZtaeaSVUCm1nNvfuvda1VN8VfkylPTA9Fdjo6QQA/7h1VkdI0+hFTdWVh9R9D7917px/2Wb5Vf8+h7c/5Jrv/AK5e/de67h+JfyizVTHA/SfYlVLrUo+XjhSGNv0+Q1OUySwxaB9Te4A9+690croz+WtueuylBnvkBX47C7dpZ46iXr3b+QXJ57PLGQ60ebzlJ/uOweMlI0zR07T1MiXUNHcke691chS0tNQ0tJj6Glp6Kgx9JTUGPoaOJKejoaGihSmpKOkgQBIaanp41REAsqgD37r3VTX8xPp3tjsvszrrKdedd7p3njcZ1/VY7IV2AoVq6eiyD7hqalKOocyoUnanYOBb9J9+690QSL4v/KKnBWm6X7TpVY6mSlo56VGYCwZ0p66NWYDi5BNvfuvdZf8AZZvlV/z6Dtz/AJIrv/rl7917q0v+XX1z2d11trtim7N2puralVltx7YqMNFukTLLXU1Niq2GqlovPUVF4opnVXsRyR7917opvzR6C7w3x8kt/wC6NmdVby3PtvJU+01oM3h8clTj6xqPamIo6tYJjOhY09VC8bccMp9+690WOP4xfKWJRHD032tBGL6YqenqYIlubnTFDkEjW55Nhyffuvdc/wDZZvlV/wA+g7c/5Irv/rl7917o8/8AL86h7q6+7i3ZmOyNi752vhKvrStxlHX7nWpWhnysm59v1MdHAZqudfu2pIJXFgDoVuffuvdW9+/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917rsMw+jMB/QMQP6/QH8+/de68STySSf6nk/0Hv3Xuuvfuvde9+6912CQbgkf63+9e/de68WZhYsxB+ouf8Aivv3Xuuvfuvde9+6912CR9GI5vwSPxb8e/de68ST9ST/AK5NuL/4/wCPv3Xuuvfuvde9+6912GYCwYgD+ht/vVh7917rxJPJJPFuT+L3/wB79+69117917r3v3XuvAkEEEgj+n+9f7H37r3Xepj/AGmt+Rc2P+uP9h7917rrn8Ej/WNv95+vv3XuuRZjwWYj+hJPv3XuuPv3XuuWt/8AVt+B+o/j37r3Xtb/AOqb/ko/8V9+6917W/8Aqm/5KP8AxX37r3XQLAkgkX/px/vXv3XuvFmP1YkA3sTfn6X/AK/Q+/de669+691737r3Xvfuvdctb/6tv9e/++Hv3XuuP5uOP9b/AH1/fuvdcgzD+03/ACUf+K+/de69rf8A1Tf8lH/ivv3XuuizH6s3+3/3j/W9+69117917r3v3XuuwSPoSP8AWJH+9H37r3Xet/8AVN/yUf8Aivv3Xuva3/1Tf8lH/ivv3XuuiSbXJNv6kn/Yc+/de68GYcBmH+xI/wB4HHv3Xuu9b/6pv+Sj/wAV9+6917W/+qb/AJKP/FffuvdeLMfqSf6XJNj/AFH9D7917rj7917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuve/de697917r3v3Xuv/Z",
                                City = cityCell,
                                Area = areaCell,
                                Location = locationCell,
                                BookingStatus = 0,

                                Width = widthCell,
                                Height = heightCell,
                                Type = typeCell == "fl" ? 0 :
                                       typeCell == "lit" ? 1 :
                                       typeCell == "nl" ? 2 : 3,
                                Rate = rateCell,
                                IsDelete = 0,
                                CreatedAt = DateTime.Now,
                                CreatedBy = "Admin",
                            };

                            records.Add(record);
                        }

                        // Save records to the database
                        await _dbContext.TblInventories.AddRangeAsync(records);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                return Ok(new { message = "File uploaded and data saved." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. " + ex.Message);
            }
        }


        [HttpPost]
                       
        public async Task<IActionResult> AddNewInventory(string city, string area, string location, string width, string height, string rate, string vendoramt, string Image,int vendorid,int stype)
        {
            
                var data = new TblInventory
                {
                    City = city,
                    Area = area,
                    Location = location,
                    Width = width,
                    Height = height,
                    Rate = rate,
                    VendorAmt = vendoramt,
                    Image = Image,
                    FkVendorId= vendorid,
                   Type=stype,
                    IsDelete = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Admin",
                };

                await _context.AddNewInventoryAsync(data);
                return Ok(new { Success = true, Message = "Customer  Add Successfully." });
           
        }


        [HttpGet]
        public IActionResult SearchCustomersByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name cannot be empty.");
            }

            var customers = _customer.SearchCustomersByNameAsync(name);
            return Json(new {success=true,data= customers });
        }
    }
}

