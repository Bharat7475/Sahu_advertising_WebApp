









$('StartDate_${itemId}').on('change', function () {

     // Get the selected start date for the inventory item
    var selectedStartDate = document.getElementById(`StartDate_${itemId}`).value;

    // AJAX request to check if the selected date conflicts with an ongoing campaign
    $.ajax({
        url: '/Campaign/CheckCampaignDates',  // Your backend endpoint to check campaign dates
        type: 'POST',
        data: { inventoryId: itemId, startDate: selectedStartDate },
        success: function (response) {
            if (response.isConflict) {
                // Show message to user if dates conflict
                toastr.error('The campaign is ongoing. Please select a date later than the existing campaign.');
            } else {
                // Continue with your logic if no conflict
                toastr.success('The date is available for selection.');
            }
        },
        error: function () {
            toastr.error('An error occurred while checking the campaign dates.');
        }
    });

});












//function updateDisplay() {
//  const actionButtons = document.querySelector(".action-buttons");
//  const campaignFields = document.getElementById("campaign-fields");
//  const saveQuotationButton = document.getElementById("saveQuotation");
//  const saveCampaignButton = document.getElementById("save-campaign");
//  const openCustomerModalButton = document.getElementById("openCustomerModal");

//  if (document.getElementById("create-quotation").checked) {
//    actionButtons.style.display = "block";
//    saveQuotationButton.style.display = "inline-block";
//    saveCampaignButton.style.display = "none";
//    openCustomerModalButton.style.display = "inline-block";
//  } else if (document.getElementById("add-campaign").checked) {
//    actionButtons.style.display = "block";
//    saveQuotationButton.style.display = "none";
//    saveCampaignButton.style.display = "inline-block";
//    openCustomerModalButton.style.display = "block";
//  } else {
//    actionButtons.style.display = "none";
//  }
//}

//document.querySelectorAll('input[name="quotation"]').forEach((radio) => {
//  radio.addEventListener("change", updateDisplay);
//});

//// Set default selection and update display accordingly
//document.getElementById("create-quotation").checked = true;
//updateDisplay();

//$(document).ready(function () {
//  // Initially hide the date fields
//  $("#campDate").hide();
//  updateFileName;
//  // Event listener for the radio buttons
//  $('input[name="quotation"]').change(function () {
//    if ($(this).val() === "campaign") {
//      $("#campDate").show();
//    } else {
//      $("#campDate").hide();
//    }
//  });
//});

//$(document).ready(function () {
//  // Load customer details when the document is ready
//  $.ajax({
//    url: "/Customer/GetCustomerinfo", // Corrected endpoint for getting all customer info
//    type: "GET",
//    success: function (response) {
//      if (response.success) {
//        var customerDropdown = $("#customerName");
//        customerDropdown.empty(); // Clear existing options
//        customerDropdown.append('<option value="">Select Customer</option>'); // Default option

//        $.each(response.model, function (index, data) {
//          customerDropdown.append(
//            '<option value="' + data.id + '">' + data.customerName + "</option>"
//          );
//        });

//        // Open the Add Quotation modal after loading customer details
//        $("#addQuotationModal").modal("show");
//      } else {
//        console.error("Failed to load customer details:", response.Message);
//      }
//    },
//    error: function (xhr, status, error) {
//      console.error("Error fetching customer details:", error);
//    },
//  });

//  // Fetch and display customer details when a customer is selected
//  $("#customerName").change(function () {
//    var selectedCustomerId = $(this).val();
//    if (selectedCustomerId) {
//      $.ajax({
//        url: "/Customer/GetCustomerinfoById", // Correct endpoint for getting customer by ID
//        type: "GET",
//        data: { id: selectedCustomerId },
//        success: function (response) {
//          console.log("Model Data:", response.model.result.address);
//          if (response.success) {
//            // Accessing properties from response.model.result
//            $("#businessName").text(response.model.result.businessName);
//            $("#address").text(response.model.result.address);
//            $("#city").text(response.model.result.city);
//            $("#gstNo").text(response.model.result.gstNo);
//            $("#contactNo").text(response.model.result.contactNo);
//            $("#alternateNumber").text(response.model.result.alternateNumber);
//            $("#state").text(response.model.result.state);

//            toastr.success("Customer info loaded successfully.");
//          } else {
//            console.error("Failed to load customer info:", response.message);
//          }
//        },
//        error: function (xhr, status, error) {
//          console.error("Error fetching customer info:", error);
//        },
//      });
//    } else {
//      // Clear customer info if no customer is selected
//      $("#businessName").text("huiii");
//      $("#city").text("");
//      $("#area").text("");
//      $("#gstNo").text("");
//      $("#contactNo").text("");
//      $("#alternateNumber").text("");
//      $("#address").text("");
//      $("#state").text("");
//    }
//  });
//});

//function cnfDeleteselecedInvetry(Id) {
//  $("#selectedId").val(Id);

//  $("#deleteCustomer").on("click", function () {
//    var selectedCustomerId = $("#selectedId").val();
//    console.log("id" + selectedCustomerId);
//    $.ajax({
//      url: "/Dashboard/DeletedSelecedInventryHoarding", // Correct endpoint for getting customer by ID
//      type: "Post",
//      data: { id: selectedCustomerId },
//      success: function (response) {
//        if (response.success) {
//          toastr.success(response.message);

//          location.reload();
//        } else {
//          toastr.error(response.message);
//        }
//      },
//      error: function (xhr, status, error) {
//        console.error("Error fetching customer info:", error);
//      },
//    });
//  });
//}

//$(document).ready(function () {
//  var selectedItems = []; // Declare the selectedItems array in a higher scope

//  // Function to fetch data and populate cards
//  function fetchData(id) {
//    $.ajax({
//      url: "/Dashboard/SelectedHoardingJ", // Replace with your controller name
//      type: "POST",
//      dataType: "json",
//      success: function (data) {
//        // Clear existing cards
//        $("#selectedCard").empty();

//        // Reset the selectedItems array
//        selectedItems = [];

//        // Loop through the data and create HTML for each item
//        $.each(data, function (index, item) {
//          var isLight = item.isLight == 1 ? "Yes" : "No";
//          var cardHtml = `
//                                          <div class="hoarding-item my-1">
//                                              <img src='/images/favicon.png' alt="Hoarding Image">
//                                              <div class="hoarding-info">
//                                                  <h5 class="ml-4">${
//                                                    item.vendorName
//                                                  }</h5>
//                                                  <p>
//                                                      ${item.area}<br>
//                                                      ${item.size}<br>
//                                                      ${isLight}<br>
//                                                      ${item.city}
//                                                  </p>
//                                              </div>
//                                              <div class="hoarding-actions">
//                                                  <button type="button" class="btn btn-danger rounded-circle p-2" data-toggle="modal" data-target="#deleteModal" onclick="cnfDeleteselecedInvetry(${
//                                                    item.id
//                                                  })">
//                                                      <i class="mdi mdi-delete"></i>
//                                                  </button>
//                                                  <h4 class="my-2"><span style='font-size:20px;'>&#8377; </span>${item.rate.toLocaleString()}</h4>
//                                                  <div class="camp-date" style="display:none;">
//                                                      <input type="date" class="form-control" id="StartDate_${
//                                                        item.id
//                                                      }" name="StartDate">
//                                                      <input type="date" class="form-control" id="EndDate_${
//                                                        item.id
//                                                      }" name="EndDate">
//                                                  </div>
//                                                  <br>
//                                              </div>
//                                          </div>
//                                      `;
//          $("#selectedCard").append(cardHtml);

//          // Add the item to the selectedItems array and include the selectedCustomerId
//          selectedItems.push({
//            id: item.id,
//            vendorName: item.vendorName,
//            area: item.area,
//            size: item.size,
//            isLight: item.isLight,
//            city: item.city,
//            rate: item.rate,
//            customerId: $("#customerName").val(), // Add the selectedCustomerId here
//          });
//        });
//      },
//      error: function (xhr, status, error) {
//        console.error("AJAX Error: " + status + error);
//      },
//    });
//  }

//  // Fetch data on page load
//  fetchData();
//  var customerId;
//  $("CustomerName").on("change", function () {
//    customerId = $(this).val();
//    alert("id customer:  " + customerId);
//  });

//  // Handle save button click event
//  $("#saveQuotation").on("click", function () {
//    $.ajax({
//      url: "/Dashboard/SaveSelectedHoardings", // Replace with your controller action
//      type: "POST",
//      contentType: "application/json",
//      data: JSON.stringify(selectedItems),
//      success: function (response) {
//        console.log(response);
//        toastr.success("Selected hoardings saved successfully!");
//        window.location.href = "/Dashboard/DeleteQuotation";
//      },
//      error: function (xhr, status, error) {
//        console.error("Error saving selected hoardings: " + status + error);
//        toastr.error("Failed to save selected hoardings.");
//        // Redirect to the same page on failure
//        window.location.href = window.location.href; // Reload the current page
//      },
//    });
//  });

//  // Handle radio button change event
//  $('input[name="quotation"]').change(function () {
//    if ($(this).val() === "campaign") {
//      $(".camp-date").show();
//      $(".camp-date").css("display", "flex");
//    } else {
//      $(".camp-date").hide();
//    }
//  });

//  // Trigger change event to set initial state
//  $('input[name="quotation"]:checked').trigger("change");
//});
