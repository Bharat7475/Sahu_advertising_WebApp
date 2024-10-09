
namespace Holding_management.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuth _auth;
        private readonly db_hoarding_managementContext _context;

        public AuthController(IAuth auth, db_hoarding_managementContext db_hoarding_managementContext)
        {
            _auth = auth;
            _context = db_hoarding_managementContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _auth.Login(model.Username, model.Password);

                //if (user != null)
                //{
                //    HttpContext.Session.SetInt32("Id", user.Id);

                //    return RedirectToAction("Index", "Dashboard");
                //    // Redirect based on user role
                //    switch (user.FkUserrole)
                //    {
                //        case 4:
                //            return RedirectToAction("Index", "Supervisor");
                //        case 5:
                //            return RedirectToAction("Index", "DeliveryBoy");
                //        case 6:
                //            return RedirectToAction("Index", "Purchase");
                //        case 7:
                //            return RedirectToAction("Index", "Billing");
                //        default:
                //            return RedirectToAction("Index", "Home");
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Invalid username or password");
                //}
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }
      
    }
}
