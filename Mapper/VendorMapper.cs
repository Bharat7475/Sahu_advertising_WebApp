
namespace Hoarding_managment.Mapper
{
    public class VendorMapper
    {
        public static VendorViewModel ToViewModel(TblVendor vendor) => new VendorViewModel
        {
            Id = vendor.Id,
            VendorName = vendor.VendorName,
            BusinessName = vendor.BusinessName,
            Email = vendor.Email,
            ContactNo = vendor.ContactNo,
            GstNo = vendor.GstNo,
            AlternateNumber = vendor.AlternateNumber,
            Address = vendor.Address,
            State = vendor.State,
            Isdelete = vendor.IsDelete as int?,
            CreatedAt = vendor.CreatedAt,
            CreatedBy = vendor.CreatedBy,
            UpdatedAt = vendor.UpdatedAt,
            UpdatedBy = vendor.UpdatedBy
        };

        public static TblVendor ToModel(VendorViewModel viewModel)
        {
            return new TblVendor
            {
                Id = viewModel.Id,
                VendorName = viewModel.VendorName,
                BusinessName = viewModel.BusinessName,
                Email = viewModel.Email,
                ContactNo = viewModel.ContactNo,
                GstNo = viewModel.GstNo,
                AlternateNumber = viewModel.AlternateNumber,
                Address = viewModel.Address,
                State = viewModel.State,
                IsDelete =(ulong?) viewModel.Isdelete ,
                CreatedAt = viewModel.CreatedAt,
                CreatedBy = viewModel.CreatedBy,
                UpdatedAt = viewModel.UpdatedAt,
                UpdatedBy = viewModel.UpdatedBy
            };
        }
    }
}
