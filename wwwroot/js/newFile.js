$(document).ready(function() {
    $('#saveCustomerButton').click(function() {
        var formData = $('#addCustomerForm').serialize();
        console.log("Serialized Form Data: ", formData); // For debugging

        $.ajax({
            type: "POST",
            url: '/Customer/AddNewCustomer',
            data: formData,
            success: function(response) {

                if (response.Success) {
                    toastr.success('Customer added successfully.');
                    document.getElementById('addCustomerModal').style.display = 'none';
                    location.reload(); // Optionally reload the page
                } else {
                    toastr.error('Error adding customer.');
                }
            },
            error: function(xhr, status, error) {
                console.error("AJAX Error: ", xhr.responseText);
                toastr.error('An error occurred while adding the customer.');
            }
        });
    });


    $('#editCustomerForm').on('submit', function(event) {
        event.preventDefault(); // Prevent the default form submission


        // Get the form data
        var formData = {
            Id: $('#editCustomerId').val(),
            BusinessName: $('#editBusinessName').val(),
            CustomerName: $('#editCustomerName').val(),
            Email: $('#editEmail').val(),
            GstNo: $('#editGstn').val(),
            ContactNo: $('#editContactNumber').val(),
            AlternateNumber: $('#editAlternateNumber').val(),
            Address: $('#editAddress').val(),
            State: $('#editState').val()
        };


        $.ajax({
            url: '/Customer/UpdateCustomer',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                if (response.success) {
                    toastr.success('Customer added successfully.');
                    location.reload(); // Reload the page on success
                } else {
                    toastr.error('Error adding Customer.');
                }
            },
            error: function(error) {
                // Handle error response
                alert('An error occurred while updating customer details.');
            }
        });
    });


});


function openDeleteModal(id) {

    alert("id: " + Id);

    $('#customerId').val(Id); // Set the customer ID in the hidden input

    $('#deleteConfirmButton').click(function () {

        var id = $('#customerId').val(); // Get customer ID from hidden input

        $.ajax({
            type: "DELETE",
            url: '/Customer/Delete/' + id, // Update URL to include customer ID
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

