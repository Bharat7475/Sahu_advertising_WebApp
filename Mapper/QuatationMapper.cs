//namespace Hoarding_managment.Mapper
//{
//    class QuatationMapper
//    {
//        public static QuatationViewModel ToQuatationViewModel(TblQuotation quotation) => new QuatationViewModel
//        {
//            Id = quotation.Id,
//            QuotationNumber = quotation.QuotationNumber,
//            FkCustomerId = quotation.FkCustomerId,
//            City = quotation.City,
//            Area = quotation.Area,
//            Location = quotation.Location,
//            IsLight = quotation.IsLight,
//            Width = quotation.Width,
//            Height = quotation.Height,

//            BookingStatus = quotation.BookingStatus,
//            Rate = quotation.Rate,
//            VendorAmt = quotation.VendorAmt,
//            MarginAmt = quotation.MarginAmt,
//            FkVendorId = quotation.FkVendorId,
//            Image = quotation.Image,
//            LocationDescription = quotation.LocationDescription,
//            IsDelete = quotation.IsDelete,
//            CreatedAt = quotation.CreatedAt,
//            CreatedBy
//               = quotation.CreatedBy,
//            UpdatedAt = quotation.UpdatedAt,
//            UpdatedBy = quotation.UpdatedBy,
//        };


//        public static TblQuotation ToQuatationModel(QuatationViewModel viewModel)
//        {
//            return new TblQuotation
//            {
//                Id = viewModel.Id,
//                QuotationNumber = viewModel.QuotationNumber,
//                FkCustomerId = viewModel.FkCustomerId,
//                City = viewModel.City,
//                Area = viewModel.Area,
//                Location = viewModel.Location,
//                IsLight = viewModel.IsLight,
//                Width = viewModel.Width,
//                Height = viewModel.Height,
//                BookingStatus = viewModel.BookingStatus,
//                Rate = viewModel.Rate,
//                VendorAmt = viewModel.VendorAmt,
//                MarginAmt = viewModel.MarginAmt,
//                FkVendorId = viewModel.FkVendorId,
//                Image = viewModel.Image,
//                LocationDescription = viewModel.LocationDescription,
//                IsDelete = viewModel.IsDelete,

//            };

//        }
//    }
//}
