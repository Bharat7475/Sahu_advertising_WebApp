$(document).ready(function () {
   
    $('#saveVendorButton').click(function () {
        event.preventDefault(); // Prevent the default form submission
        // Clear previous error messages
        $('.error-message').remove();

        // Validate required fields     businessName VendorPersonName email gstn contactNumber
        var isValid = true;

        // Check Business Name
        if ($('#businessName').val().trim() === '') {
            $('#businessName').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check Contact Person Name
        if ($('#VendorPersonName').val().trim() === '') {
            $('#VendorPersonName').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check Email
        if ($('#email').val().trim() === '') {
            $('#email').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check Contact Number
        if ($('#contactNumber').val().trim() === '') {
            $('#contactNumber').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check if GST Number (if filled) is exactly 15 characters long
        var gstNumber = $('#gstn').val().trim();
        if (gstNumber !== '' && gstNumber.length !== 15) {
            $('#gstn').after('<span class="error-message text-danger">GST Number must be exactly 15 characters.</span>');
            isValid = false;
        }

        // If the form is valid, proceed with the AJAX submission
        if (isValid) {
            var formData = $('#addVendorForm').serialize();
            console.log(formData); // Check serialized data in console

            $.ajax({
                type: "POST",
                url: '/Vendor/AddVendor', // Match the controller action name
                data: formData,
                success: function (response) {
                    console.log("success" + response);
                    if (response.message == "Vendor Add Successfully. ") {
                        toastr.success('Vendor added successfully.');
                       
                        location.reload(); // Optionally reload the page
                    }
                    else {
                        toastr.error('An error occurred while adding the customer.');
                    }
                   
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText); // Log any errors
                    toastr.error('An error occurred while adding the vendor.');
                }
            });
        } else {
            toastr.error('Please fill all the required fields.');
        }


    });


    // Update Vendor

    $('#updateVendorButton').click(function () {

        event.preventDefault(); // Prevent the default form submission
        // Clear previous error messages
        $('.error-message').remove();

        // Validate required fields
        var isValid = true;

        // Check Business Name
        if ($('#editBusinessName').val().trim() === '') {
            $('#editBusinessName').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check Contact Person Name
        if ($('#editVendorPersonName').val().trim() === '') {
            $('#editVendorPersonName').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check Email
        if ($('#editEmail').val().trim() === '') {
            $('#editEmail').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check Contact Number
        if ($('#editContactNo').val().trim() === '') {
            $('#editContactNo').after('<span class="error-message text-danger">This field is required.</span>');
            isValid = false;
        }

        // Check if GST Number (if filled) is exactly 15 characters long
        var gstNumber = $('#editGstNo').val().trim();
        if (gstNumber !== '' && gstNumber.length !== 15) {
            $('#editGstNo').after('<span class="error-message text-danger">GST Number must be exactly 15 characters.</span>');
            isValid = false;
        }

        // If the form is valid, proceed with the AJAX submission
        if (isValid) {

            var vendorId = $('#editVendorId').val(); // Get vendor ID from hidden input
            var formData = $('#editVendorForm').serialize(); // Serialize form data

            console.log("Updating Vendor with ID: " + formData);

            $.ajax({
                type: "PUT",
                url: '/Vendor/EditVendor', // Update URL as needed
                data: formData,
                success: function (response) {
                    if (response.success) {
                        toastr.success('Vendor updated successfully.');
                        document.getElementById("editVendorModal").style.display = 'none'
                        location.reload(); // Reload the page after successful update
                    } else {
                        toastr.error('Error updating vendor.');
                    }
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText); // Log any errors
                    toastr.error('An error occurred while updating the vendor.');
                }
            });

        } else {
            toastr.error('Please fill all the required fields.');
        }

      
    });
});


// <!-- using ajax call -->

//function openEditModal(vendorId) {

//    $.ajax({
//        type: "GET",
//        url: '/Vendor/EditVendor', // Adjust this to match your controller action
//        data: { id: vendorId },
//        success: function (response) {
//            // Populate form fields with the vendor data
//            $('#editVendorId').val(response.id);
//            $('#editBusinessName').val(response.businessName);
//            $('#editVendorPersonName').val(response.vendorName);
//            $('#editEmail').val(response.email);
//            $('#editGstNo').val(response.gstNo);
//            $('#editContactNo').val(response.contactNo);
//            $('#editContactNo2').val(response.contactNo2);
//            $('#editAddress').val(response.address);
//            $('#editState').val(response.state);

//            // Open the modal
//            $('#editVendorModal').modal('show');
//        },
//        error: function (xhr, status, error) {
//            console.error(xhr.responseText); // Log any errors
//            toastr.error('An error occurred while retrieving the vendor details.');
//        }
//    });
//}

function openEditmodal(id, businessName, vendorName, email, gstNo, contactNo, alternateNumber, address, state) {
    // Set the values of the input fields in the modal
    $('#editVendorId').val(id);
    $('#editBusinessName').val(businessName);
    $('#editVendorPersonName').val(vendorName);
    $('#editEmail').val(email);
    $('#editGstNo').val(gstNo);
    $('#editContactNo').val(contactNo);
    $('#editContactNo2').val(alternateNumber);
    $('#editAddress').val(address);
    $('#editState').val(state);

    // Show the modal using Bootstrap's modal method
    document.getElementById("editVendorModal").style.display = 'block'
}
//function openDeleteModal(id) {
//   // alert("ID: " + id);
//    $('#deleteVendorId').val(id); // Set the vendor ID in the hidden input

//    $('#deleteConfirmButton').click(function () {

//        var vendorId = $('#deleteVendorId').val(); // Get vendor ID from hidden input

//        $.ajax({
//            type: "DELETE",
//            url: '/Vendor/DeleteVendor/' + vendorId, // Update URL to include vendor ID
//            success: function (response) {
//                if (response.success) {
//                    toastr.success('Delete successfully.');
//                    location.reload(); 
//                } else {
//                    toastr.error('Error deleting vendor.');
//                }
//            },
//            error: function (xhr, status, error) {
//                console.error(xhr.responseText); // Log any errors
//                toastr.error('An error occurred while deleting the vendor.');
//            }
//        });
//    });
//}

function openDeleteModalforvendor(id) {

    $('#deleteVendorId').val(id); // Set the customer ID in the hidden input
    $('#deleteConfirmButton').click(function () {

        var id = $('#deleteVendorId').val(); // Get customer ID from hidden input

        $.ajax({
            type: "DELETE",
            url: '/Vendor/Delete/' + id, // Update URL to include customer ID
            success: function (response) {
                if (response.success) {
                    toastr.success('Delete successfully.');
                    location.reload();
                } else {
                    toastr.error('Error deleting .');
                }
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText); // Log any errors
                toastr.error('An error occurred while deleting the vendor.');
            }
        });
    });
}
