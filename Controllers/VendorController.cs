
namespace Hoarding_managment.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendor _context;

        public VendorController(IVendor vendor)
        {
            _context = vendor;
        }

        [HttpGet]
        public async Task<IActionResult> GetVendorList()
        {

            var listofvendor = _context.getvendorlist();
            return Json(new { success = true, Message = "success", data = listofvendor });
        }
            [HttpGet]
        public async Task<IActionResult> Index(string searchQuery = "", int pageSize = 10, int pageNumber = 1)
        {
            // Get filtered customers based on the search query
            var vendors = await _context.GetAllVendorsAsync(searchQuery, pageNumber, pageSize);

            // Get the total number of customers (taking the search query into account)
            var totalVendors = await _context.GetVendorCountAsync(searchQuery);

            // Calculate total pages based on filtered results
            var totalPages = (int)Math.Ceiling(totalVendors / (double)pageSize);

            // Convert to view models
            var viewModels = vendors.Select(v => VendorMapper.ToViewModel(v)).ToList();

            // Pass search query, pagination info, and customers to the view
            var model = new VendorViewModelWithpagingnation
            {
                Vendor = viewModels,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(model);
        }


        //VendorViewModelWithpagingnation

        [HttpGet]   
        public async Task<IActionResult> Details(int id)
        {
            var vendor = await _context.GetVendorByIdAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }
            var viewModel = VendorMapper.ToViewModel(vendor);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddVendor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddVendor(string businessName, string vendorName, string email, string gstNo, string contactNo, string alternateNumber, string address, string state)
        {
            try
            {
                var data = new TblVendor
                {
                    BusinessName = businessName,
                    VendorName = vendorName,
                    Email = email,
                    GstNo = gstNo,
                    ContactNo = contactNo,
                    AlternateNumber = alternateNumber,
                    Address = address,
                    State = state,
                    IsDelete = 0,
                    CreatedBy = vendorName,
                    CreatedAt = DateTime.Now
                };

              var newresult = await _context.AddVendorAsync(data);
                if (newresult.Id != 0)
                {
                    return Ok(new { success = true, message = "Vendor Add Successfully. " });
                }
                else
                {
                    return Json(new { success = true, message = "Invalid data." });
                }


            }
            catch
            {
                return Json(new { success = true, message = "Invalid data." });
            }

           
        }

        [HttpGet]
        public async Task<IActionResult> EditVendor(int id)
        {
            var vendor = await _context.GetVendorByIdAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            var viewModel = VendorMapper.ToViewModel(vendor);
            return Json(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> EditVendor(int id, VendorViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var vendor = VendorMapper.ToModel(viewModel);
                vendor.Id = viewModel.Id;
                vendor.VendorName = viewModel.VendorName;
                vendor.BusinessName = viewModel.BusinessName;
                vendor.Address = viewModel.Address;
                vendor.State = viewModel.State;
                vendor.IsDelete = 0;
                vendor.Email = viewModel.Email;
                vendor.ContactNo = viewModel.ContactNo;
                vendor.AlternateNumber = viewModel.AlternateNumber;

                await _context.UpdateVendorAsync(id,vendor);
                return Json(new { success = true,  }); // Return success as JSON
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) }); // Return errors if any
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = _context.DeleteVendor(id);
            if (customer == null)
            {
                return NotFound();
            }
            try
            {
                 _context.DeleteVendor(id);
                return Json(new
                {
                    success = true,
                    Message = "Delete Successfully",
                    id = id
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPost]
        public IActionResult DeleteVendor(int id)
        {
            var vendor = _context.DeleteVendor(id);
            if (vendor != null)
            {
                _context.DeleteVendor(id);
                return Json(new
                {
                    success = true,
                    Message = "Delete Successfully",
                    id = id
                });

            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error"
                });
            }
        }
    }
}
