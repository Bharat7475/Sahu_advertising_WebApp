

$(document).ready(function () {
    $('#saveCustomerButton').click(function () {

        var formData = $('#addCustomerForm').serialize();
        console.log("Serialized Form Data: ", formData); // For debugging

        $.ajax({
            type: "POST",
            url: '/Customer/AddNewCustomer', // Update the URL to match your controller action
            data: formData,
            success: function (response) {
                console.log("Response: ", response);

                if (response.Success) {
                    toastr.success('Customer added successfully.');
                    $('#addCustomerModal').modal('hide'); // Close the modal
                    location.reload(); // Optionally reload the page
                } else {
                    toastr.error('Error adding customer.');
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error: ", xhr.responseText);
                toastr.error('An error occurred while adding the customer.');
            }
        });
    });
});


