using Hoarding_management.Data;

namespace Hoarding_managment.Controllers
{
    public class OngoingCampainController : Controller
    {
        private readonly IOngoingCampain _context;
        public OngoingCampainController(IOngoingCampain ongoingCampain)
        {
            _context = ongoingCampain;
        }

        //public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 9)
        //{
        //    var campaign = await _context.GetallOngoingCampaignAsync(pageNumber, pageSize);
        //    var totalItems = await _context.GetOngoingCampaignCountAsync();
        //    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        //    CampaignPagedViewModel? viewModel = new CampaignPagedViewModel
        //    {
        //        Campaigns = campaign,
        //        CurrentPage = pageNumber,
        //        TotalPages = totalPages,

        //    };


        //    return View(viewModel);
        //}

        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery = "", int pageSize = 10, int pageNumber = 1)
        {
            var campaign = await _context.GetallOngoingCampaignAsync(searchQuery, pageNumber, pageSize);
            var totalItems = await _context.GetOngoingCampaignCountAsync(searchQuery);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            CampaignPagedViewModel? viewModel = new CampaignPagedViewModel
            {
                CampaignsViewModel = campaign,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CompletedCampain(string searchQuery = "", int pageSize = 10, int pageNumber = 1)
        {
            var campaign = await _context.GetallCompletedCampaignAsync(searchQuery, pageNumber, pageSize);
            var totalItems = await _context.GetCompletedCampaignCountAsync(searchQuery);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            CampaignPagedViewModel? viewModel = new CampaignPagedViewModel
            {
                CampaignsViewModel = campaign,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }



        [HttpGet("search")]
        public async Task<IActionResult> SearchByCampaignName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name cannot be empty.");
            }

            var customers = await _context.SearchByCampaignNameAsync(name);
            return Ok(customers);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            var Campaign = await _context.GetCampaingnByIdAsync(id);
            if (Campaign != null)
            {
                var deleteCampaign = await _context.DeleteCampaignAsync(id);


                return Json(new
                {
                    success = true,
                    Message = "Delete Campaign Successfully",
                    id = id
                });

            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Deleting Error..."
                });
            }
        }


        [ HttpPost]
        public async Task<IActionResult> UpdateCampaign(CampaigneditViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, Meassage = "Invalid data" });
            }

            var Campaign = _context.GetCampaingnByIdAsync(model.Id);
            if (Campaign == null)
            {
                return Json(new { success = false, Meassage = "Campaign not found" });
            }


            await _context.UpdateCampaignAsync(model);

            return Json(new { success = true, Meassage = "Updated successfully" });
        }



        [HttpPost]
        public async Task<IActionResult> Addcampaingn([FromBody] QuotationItemListViewModel model)
        {
            if (model == null || model.SelectedItems == null || !model.SelectedItems.Any())
            {
                return BadRequest(new { success = false, message = "No items selected." });
            }

            try
            {

                await _context.addCampaign(model);


                return Ok(new { success = true, message = "Campaign Created saved successfully." });
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                return StatusCode(500, new { success = false, message = "Campaign saved error." });
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> addCampaign([FromBody] QuotationItemListViewModel model)
        //{
        //    if (model == null || model.SelectedItems == null || !model.SelectedItems.Any())
        //    {
        //        return BadRequest(new { success = false, message = "No items selected." });
        //    }

        //    try
        //    {

        //        await _context.addCampaign(model);


        //        return Ok(new { success = true, message = "Campaign Created saved successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error (optional)
        //        return StatusCode(500, new { success = false, message = "Campaign saved error." });
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> getallCampaignCount()
        {
            // Assuming GetOngoingCampaignCountAsync() returns an integer count value
            var upcomingeventCount = await _context.GetOngoingCampaignCountAsync();
            return Json(new { success = true, count = upcomingeventCount });
        }

        [HttpPost]
        public IActionResult ValidateDatesCampaign(int id, DateTime fromDate)
        {

            var finddata = _context.GetCampaingnByIdAsync(id);
            if (finddata == null )
            {
                return Json(new { success = false, message = "data not found"});
            }
            var exitsdate = _context.IsCampaignBooked(id, fromDate);


            return Json(new { success=true, message="success", date = exitsdate });
        }

    }
}
