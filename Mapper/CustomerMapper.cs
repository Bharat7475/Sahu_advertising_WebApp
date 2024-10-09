
namespace Hoarding_managment.Mapper
{
    public class CustomerMapper
    {
       
        public static CustomerViewModel ToCustomerViewModel(TblCustomer customer) => new CustomerViewModel
        {
            Id = customer.Id,
            CustomerName = customer.CustomerName,
            BusinessName = customer.BusinessName,
            Email = customer.Email,
            ContactNo = customer.ContactNo,
            GstNo = customer.GstNo,
            AlternateNumber = customer.AlternateNumber,
            Address = customer.Address,
            State = customer.State,
            IsDelete = customer.IsDelete ,
            CreatedAt = customer.CreatedAt,
            CreatedBy = customer.CreatedBy,
            UpdatedAt = customer.UpdatedAt,
            UpdatedBy = customer.UpdatedBy
        };


        public static TblCustomer ToCustomerModel(CustomerViewModel viewModel)
        {
            return new TblCustomer
            {
                Id = viewModel.Id,
                CustomerName = viewModel.CustomerName,
                BusinessName = viewModel.BusinessName,
                Email = viewModel.Email,
                ContactNo = viewModel.ContactNo,
                GstNo = viewModel.GstNo,
                AlternateNumber = viewModel.AlternateNumber,
                Address = viewModel.Address,
                State = viewModel.State,
                IsDelete = viewModel.IsDelete,
                CreatedAt = viewModel.CreatedAt,
                CreatedBy = viewModel.CreatedBy,
                UpdatedAt = viewModel.UpdatedAt,
                UpdatedBy = viewModel.UpdatedBy
            };
        }
    }
}
