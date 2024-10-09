
namespace Hoarding_managment
{
    public class AutocompleteService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutocompleteService> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public readonly db_hoarding_managementContext _context;

        public AutocompleteService(ILogger<AutocompleteService> logger, IWebHostEnvironment hostingEnvironment, db_hoarding_managementContext context, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public IEnumerable<TblVendor> Getvendorname(string vendorname)
        {
            return _context.TblVendors.Where(s => s.VendorName.Contains(vendorname) && s.IsDelete == 0).ToList();
        }

        public IEnumerable<TblCustomer> Getbusinessname(string businessname)
        {
            return _context.TblCustomers.Where(s => s.BusinessName.Contains(businessname) && s.IsDelete == 0).ToList();
        }
    }
}
