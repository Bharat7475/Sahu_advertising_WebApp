
<script>
    $(document).ready(function () {
            var updatedRates = [];
    var addCamign = [];
    var changedateids = [];
    var newRate;
    var originalRates = { };
    var id = sessionStorage.getItem('selectedHoardingId');
    let idString = "";  // Step 1: Initialize an empty string

    function fetchData() {
                var id = sessionStorage.getItem('selectedHoardingId');

    // Define ajaxOptions based on default GET request
    var ajaxOptions = {
        url: '/Dashboard/SelectedHoardingJ', // Default URL
    type: 'GET', // Default method
    dataType: 'json',
    success: function (data) {
                        if (data.success) {
                            // document.getElementById('create-quotation').checked = true;
                            if (id !== null) {
        // Set the checkbox to checked
        document.getElementById('add-campaign').checked = true;
                            }


    $('#selectedCard').empty();

    var custidnew = 0;


    // Loop through the data and create HTML for each item
    $.each(data.model, function (index, item) {
        // Save the original rates to detect changes
        originalRates[ item.id ] = { rate: item.rate, FkInventoryId: item.fkInventoryId };

    updatedRates.push({id: item.id, rate: item.rate, FkInventoryId: item.fkInventoryId });
    addCamign.push({id: item.id, rate: item.rate, FkInventoryId: item.fkInventoryId });



    if (index === 0) {
        $("#idforalldatechange").val("" + item.id + "");
                                }
    else {
        changedateids.push({ id: item.id });
    // Step 2: Append item.id to the idString, separating with a comma
    idString += item.id + ",";
                                }

    var isLight = item.isLight == 1 ? "Yes" : "No";
    var cardHtml = `
    <div class="hoarding-item my-2">
        <img src="data:image/png;base64,${item.image}" alt="Image" class="rounded">
            <div class="hoarding-info">
                <p class="font-size-4"><b>Vendor Name: ${item.vendorName}</b></p>
                <p>
                    <span style="display:none;" > ${item.fkInventoryId}</span>
                    Area : ${item.area}<br>
                        Size: ${item.width}FT x ${item.height}FT <br>
                            LED  : ${isLight}<br>
                                City : ${item.city}
                            </p>
                        </div>
                        <div class="hoarding-actions" style="margin-right: 15px;">
                            <button type="button" class="btn btn-danger rounded-circle p-2" data-toggle="modal" data-target="#deleteModal" onclick="cnfDeleteselecedInvetry(${item.id})">
                                <i class="mdi mdi-delete"></i>
                            </button>

                            <button type="button" style="display: none;" id="deleteQuteHoarding" class="btn btn-danger rounded-circle p-2" data-toggle="modal" data-target="#deleteModalquate" onclick="cnfDeleteselecedInvetry(${item.id})">
                                <i class="mdi mdi-delete"></i>
                            </button>

                            <div id="rate-section" class="rate-section my-2">
                                <h4 class="my-2" id='id_h4' >
                                    <span style='font-size:20px;'>&#8377; </span>
                                    <span id="rateDisplay_${item.id}">${item.rate}</span>
                                    <button type="button" class="btn btn-primary btn-sm ml-2" id="btn-pencil" onclick="editRate(${item.id}, ${item.rate})">
                                        <i class="mdi mdi-pencil"></i>
                                    </button>
                                </h4>
                                <div class="rate-edit" id="rateEdit_${item.id}" style="display: none;">
                                    <input type="number" class="form-control mb-2 rateInput_rate" id="rateInput_${item.id}" value="${item.rate}">
                                        <button type="button" class="btn btn-success btn-sm" onclick="saveRate(${item.id})">Save</button>
                                        <button type="button" class="btn btn-secondary btn-sm" onclick="cancelEdit(${item.id})">Cancel</button>
                                </div>
                            </div>

                            <div class="camp-date" style="display:none;">
                                <input type="date" class="form-control StartDate" id="StartDate_${item.id}" name="StartDate"
                                    onchange="validateDates(${item.id})"
                                    value="${new Date().toISOString().split('T')[ 0 ]}">
                                    <input type="date" class="form-control EndDate" id="EndDate_${item.id}" name="EndDate" onchange="validateDates(${item.id})">
                                    </div>

                            </div>
                        </div>
                        `;
                        $('#selectedCard').append(cardHtml);

                        if (custidnew == 0 && id !== null) {
                            $("#businessName").val("" + item.businessName + "");
                        custidnew = item.fkcustomer;
                        $("#businessNameid").val(item.fkcustomer);
                                }
                            });

                            // <div class="rate-section my-2">
                            //     <h4 class="my-2">
                            //         <span style='font-size:20px;'>&#8377; </span>
                            //         <span id="rateDisplay_${item.id}" class="rate-display" ondblclick="editRate(${item.id}, ${item.rate})">${item.rate}</span>
                            //     </h4>
                            //     <div class="rate-edit" id="rateEdit_${item.id}" style="display: none;">
                            //         <input type="number" class="form-control mb-2 rate-input" id="rateInput_${item.id}" value="${item.rate}">
                            //     </div>
                            // </div>



                            if (idString.length > 0) {
                            idString = idString.slice(0, -1);  // Remove trailing comma
                            }


                        $("#idforalldatechangestring").val("");
                        $("#idforalldatechangestring").val("" + idString + "");
                        if (id !== null) {
                            $.ajax({
                                url: '/Customer/GetCustomerinfoById',
                                type: 'GET',
                                data: { id: custidnew },
                                success: function (response) {
                                    if (response.success) {
                                        const customer = response.model.result;
                                        $('#customerName').text(customer.customerName);
                                        $('#address').text(customer.address);
                                        $('#city').text(customer.city);
                                        $('#gstNo').text(customer.gstNo);
                                        $('#contactNo').text(customer.contactNo);
                                        $('#alternateNumber').text(customer.alternateNumber);
                                        $('#state').text(customer.state);

                                        toastr.success('Customer loaded successfully.');
                                    } else {
                                        console.error('Failed to load customer info:', response.message);
                                    }
                                },
                                error: function (xhr, status, error) {
                                    console.error('Error fetching customer info:', error);
                                }
                            });
                            }



                        window.editRate = function (id, currentRate) {
                            $('#rateDisplay_' + id).hide();

                        $('#rateEdit_' + id).show();
                            };

                        // Function to save the updated rate
                        window.saveRate = function (id) {
                            newRate = $('#rateInput_' + id).val();
                        $('#rateDisplay_' + id).text(newRate);
                        $('#rateEdit_' + id).hide();
                        $('#rateDisplay_' + id).show();

                        // Check if the rate was actually changed
                        let isRateChanged = newRate != originalRates[ id ].rate;

                                // Remove the existing entry, if any, to avoid duplicates
                                updatedRates = updatedRates.filter(item => item.id !== id);
                                addCamign = addCamign.filter(item => item.id !== id);

                        // Add the rate regardless of whether it has changed
                        updatedRates.push({id: id, rate: newRate, FkInventoryId: originalRates[ id ].FkInventoryId });
                        addCamign.push({id: id, rate: newRate, FkInventoryId: originalRates[ id ].FkInventoryId });

                        // Log changes for debugging
                        console.log("Updated rates: ", updatedRates);
                        console.log("Campaign rates: ", addCamign);

                        // Optionally, show a message if a change was detected
                        if (isRateChanged) {
                            console.log(`Rate for item ${id} was changed.`);
                                }
                            };

                            // Function to handle the double-click event and enable editing
                            // window.editRate = function (id, currentRate) {
                            //     // Hide the rate display and show the input field
                            //     $('#rateDisplay_' + id).hide();
                            //     $('#rateEdit_' + id).show();

                            //     // Get the input field
                            //     var inputField = $('#rateInput_' + id);

                            //     // Set focus to the input field
                            //     inputField.focus();

                            //     // Move the caret to the end of the input value
                            //     var value = inputField.val();
                            //     inputField.val('').val(value); // This resets the value to move the caret to the end

                            //     // Clear previous keydown event handlers to avoid multiple bindings
                            //     inputField.off('keydown').on('keydown', function (e) {
                            //         if (e.key === 'Enter') {
                            //             e.preventDefault(); // Prevent the default action (form submission, etc.)
                            //             saveRate(id); // Call the save function to update the rate
                            //         }
                            //     });
                            // };

                            // The rest of your code remains the same...



                            // Function to save the rate when the user clicks outside the input field (blur event)
                            // $(document).on('blur', '.rate-input', function () {
                            //     var id = $(this).attr('id').split('_')[1]; // Extract the item ID from the input field ID
                            //     saveRate(id); // Call the save function
                            // });


                            //window.saveRate = function (id) {
                            //    var newRate = $('#rateInput_' + id).val();
                            //    $('#rateDisplay_' + id).text(newRate);
                            //    $('#rateEdit_' + id).hide();
                            //    $('#rateDisplay_' + id).show();

                            //    // Check if the rate was actually changed
                            //    let isRateChanged = newRate != originalRates[id].rate;

                            //    // Remove the existing entry, if any, to avoid duplicates
                            //    updatedRates = updatedRates.filter(item => item.id !== id);
                            //    addCamign = addCamign.filter(item => item.id !== id);

                            //    // Add the rate regardless of whether it has changed
                            //    updatedRates.push({ id: id, rate: newRate, FkInventoryId: originalRates[id].FkInventoryId });
                            //    addCamign.push({ id: id, rate: newRate, FkInventoryId: originalRates[id].FkInventoryId });

                            //    // Optionally, show a message if a change was detected
                            //    if (isRateChanged) {
                            //        console.log(`Rate for item ${id} was changed.`);
                            //    }
                            //};

                            window.cancelEdit = function (id) {
                                $('#rateEdit_' + id).hide();
                                $('#rateDisplay_' + id).show();
                            };

                        if (id !== null) {
                                // Set the checkbox to checked
                                if (document.getElementById('add-campaign').checked) {
                            $('.camp-date').show();
                        $('.camp-date').css('display', 'flex');
                        $('#saveCampaign').show();
                        $('#saveQuotation').hide();
                        $('#deleteQuteHoarding').hide();
                                } else {
                            $('.camp-date').hide();
                                }
                            }
                        } else {
                            console.error('Error: ' + data.message);
                        }
                    },
                        error: function (xhr, status, error) {
                            console.error('AJAX Error: ' + status + error);
                    }
                };

                        // Check if id exists and modify ajaxOptions accordingly
                        if (id) {
                            ajaxOptions.url = '/Dashboard/GetHordingdata'; // Change URL
                        ajaxOptions.type = 'POST'; // Change request type
                        ajaxOptions.data = {id: id }; // Pass id as data
                        sessionStorage.removeItem('selectedHoardingId');
                }

                        // Make the AJAX request
                        $.ajax(ajaxOptions);
            }

                        fetchData();

                        function updateradio() {
                            $('input[name="quotation"]').change(function () {
                                if ($(this).val() === 'campaign') {
                                    $('.camp-date').show();
                                    $('.camp-date').css('display', 'flex');
                                } else {
                                    $('.camp-date').hide();
                                }
                            });
            }
                        updateradio();


                        $('#saveQuotation').on('click', function () {


                var customerId = document.getElementById('businessNameid').value;

                        if (updatedRates.length === 0) {  // Check if updatedRates is empty or undefined
                            toastr.error('Please select first Hoarding.');
                        return;
                }

                        //  alert(customerId)
                        if (!customerId) {
                            toastr.error('Please select Bussness Name.');
                        return;
                }


                        $.ajax({
                            url: '/Dashboard/SaveSelectedHoardings',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({selectedItems: updatedRates, customerId: customerId }),
                        success: function (response) {

                        //setTimeout(function () {
                        //    window.location.href = '/Quatation/LastQuotation?id=' + response.id; // Corrected line

                        //}, 1000);

                        if (response.success) {
                            window.location.href = '/Quatation/ViewQuotation?id=' + response.id; // Corrected line
                        } else {
                            toastr.error(response.message);
                        }

                        // setTimeout(function () {


                            // }, 2000);


                        },
                        error: function (xhr, status, error) {
                            toastr.error('Error saving the quotation.');
                        console.error('Error saving selected hoardings:', status, error);
                    }
                });
            });


                        $('#saveCampaign').on('click', function () {
                var customerId = document.getElementById('businessNameid').value;

                        // Check if customerId is valid (not null or zero)
                        if (!customerId || customerId === '0') {
                            toastr.error('Please select a valid Business Name.');
                        return;
                }

                        let updatedArray1 = [];
                        let isDataValid = true;

                        // Check if there are items to save and validate their dates
                        if (!addCamign || addCamign.length === 0) {
                            toastr.error('No campaigns to save. Please add items.');
                        return;
                }

                addCamign.forEach(item => {
                            let startDate = document.getElementById(`StartDate_${item.id}`).value;
                        let endDate = document.getElementById(`EndDate_${item.id}`).value;

                        // Validate startDate and endDate
                        if (!startDate || !endDate) {
                            toastr.error('Please provide valid start and end dates for all items.');
                        isDataValid = false; // Set flag to false if any date is missing
                        return; // Stop processing further for this item
                    }

                        // Create the new item object
                        let newItem = {
                            id: item.id,
                        rate: item.rate,
                        FkInventoryId: item.FkInventoryId,
                        FromDate: startDate,
                        ToDate: endDate
                    };
                        updatedArray1.push(newItem);
                });

                        // If validation failed, stop further execution
                        if (!isDataValid) {
                    return;
                }

                        // AJAX call to save campaign data
                        $.ajax({
                            url: '/OngoingCampain/Addcampaingn',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({selectedItems: updatedArray1, customerId: customerId }),
                        success: function (response) {
                            toastr.success('Campaign saved successfully!');
                        setTimeout(function () {
                            window.location.href = '/OngoingCampain/Index';
                        }, 2000); // 2000 milliseconds = 2 seconds
                    },
                        error: function (xhr, status, error) {
                            toastr.error('Error saving the campaign.');
                        console.error('Error saving campaign:', status, error);
                    }
                });
            });


        });
                    </script>
                    <script>
        // Set today's date as default for start date
                        document.addEventListener('DOMContentLoaded', function () {
            const today = new Date().toISOString().split('T')[ 0 ]; // Get today's date in yyyy-mm-dd format
                        document.getElementById(`StartDate_${item.id}`).value = today;
        });

                        // Function to validate date inputs  $("#idforalldatechangestring").val("" + idString + "");

                        function validateDates(id) {
            const startDateInput = document.getElementById(`StartDate_${id}`);
                        const endDateInput = document.getElementById(`EndDate_${id}`);

                        const startDate = new Date(startDateInput.value);
                        const endDate = new Date(endDateInput.value);
                        const today = new Date();

                        var newid = $("#idforalldatechange").val();

                        var newidsaary = [];
                        var idString = $("#idforalldatechangestring").val();

                        newidsaary = idString.split(',');

                        if (newid == id) {
                            $.each(newidsaary, function (index, item) {
                                // console.log("ID at index " + index + ": " + item.id);
                                // Perform any other actions with item.id
                                document.getElementById(`StartDate_${item}`).value = startDateInput.value;
                                document.getElementById(`EndDate_${item}`).value = endDateInput.value;
                            });

            }

                        // Check if start date is before today
                        if (startDate < today.setHours(0, 0, 0, 0)) {  // Compare with today's date at 00:00
                            alert("Start date cannot be in the past.");
                        startDateInput.value = today.toISOString().split('T')[ 0 ]; // Reset to today's date
            }

                        // Check if end date is less than start date
                        if (endDate < startDate) {
                            alert("End date cannot be less than the start date.");
                        endDateInput.value = startDateInput.value; // Reset to start date
            }
        }


                    </script>
                    <script>
                        document.querySelectorAll('.autocomplete').forEach(function (input) {
                            let suggestionsContainer;
                        let activeSuggestionIndex = -1;
                        let debounceTimeout;
                        let selectedCustomerId = null;  // Store the selected customer ID

                        // Debounce function
                        function debounce(func, delay) {
                return function () {
                            clearTimeout(debounceTimeout);
                        debounceTimeout = setTimeout(func, delay);
                };
            }

                        // Input event with debounce
                        input.addEventListener('input', debounce(function () {
                const query = input.value;
                        const id = input.getAttribute('data-id');
                        let urls = "";

                        if (id === "businessName") {
                            urls = `/Dashboard/GetBusinessName?query=${query}`;
                }

                        if (query.length < 1) {
                    if (suggestionsContainer) {
                            suggestionsContainer.innerHTML = '';
                        suggestionsContainer.style.display = 'none';
                    }
                        return;
                }

                        // Fetch suggestions from the server
                        fetch(urls)
                    .then(response => response.json())
                    .then(data => {
                        if (!suggestionsContainer) {
                            suggestionsContainer = document.createElement('div');
                        suggestionsContainer.className = 'autocomplete-suggestions';
                        input.parentNode.appendChild(suggestionsContainer);
                        }

                        suggestionsContainer.innerHTML = '';
                        suggestionsContainer.style.display = 'block';
                        activeSuggestionIndex = -1;

                        if (data.length === 0) {
                            // If no matches are found, show a message
                            suggestionsContainer.innerHTML = `<div class="no-results">No matches found</div>`;
                        return;
                        }

                        // Populate suggestions
                        data.forEach((item) => {
                            const suggestionItem = document.createElement('div');
                        suggestionItem.className = 'autocomplete-suggestion';
                        suggestionItem.textContent = item.name;
                        suggestionItem.dataset.key = item.id;
                        suggestionItem.dataset.value = item.name;

                        // Handle suggestion click
                        suggestionItem.addEventListener('click', function () {
                            selectSuggestion(item, input, suggestionsContainer);
                            });
                        suggestionsContainer.appendChild(suggestionItem);
                        });
                    });
            }, 300)); // Debounce delay

                        // Handle keyboard navigation
                        input.addEventListener('keydown', function (e) {
                const suggestions = suggestionsContainer?.querySelectorAll('.autocomplete-suggestion');
                        if (!suggestions) return;

                        if (e.key === 'ArrowDown') {
                            activeSuggestionIndex = (activeSuggestionIndex + 1) % suggestions.length;
                        updateActiveSuggestion(suggestions);
                } else if (e.key === 'ArrowUp') {
                            activeSuggestionIndex = (activeSuggestionIndex - 1 + suggestions.length) % suggestions.length;
                        updateActiveSuggestion(suggestions);
                } else if (e.key === 'Enter') {
                            e.preventDefault();
                    if (activeSuggestionIndex > -1) {
                        const activeSuggestion = suggestions[ activeSuggestionIndex ];
                        const item = {
                            id: activeSuggestion.dataset.key,
                        name: activeSuggestion.dataset.value
                        };
                        selectSuggestion(item, input, suggestionsContainer);
                    }
                }
            });

                        // Function to select a suggestion
                        function selectSuggestion(item, input, suggestionsContainer) {
                            input.value = item.name;
                        selectedCustomerId = item.id;
                        suggestionsContainer.style.display = 'none';
                        $("#businessNameid").val(item.id);  // Set the hidden field for ID

                        // Fetch customer info after selection
                        fetchCustomerInfo(item.id);
            }

                        // Fetch customer info
                        function fetchCustomerInfo(id) {
                            $.ajax({
                                url: '/Customer/GetCustomerinfoById',
                                type: 'GET',
                                data: { id: id },
                                success: function (response) {
                                    if (response.success) {
                                        const customer = response.model.result;
                                        if (customer) {
                                            $('#customerName').text(customer.customerName);
                                            $('#address').text(customer.address);
                                            $('#city').text(customer.city);
                                            $('#gstNo').text(customer.gstNo);
                                            $('#contactNo').text(customer.contactNo);
                                            $('#alternateNumber').text(customer.alternateNumber);
                                            $('#state').text(customer.state);
                                        } else {
                                            clearCustomerInfo();
                                            toastr.error('No customer data found.');
                                        }
                                    } else {
                                        clearCustomerInfo();
                                        toastr.error('Failed to load customer info.');
                                    }
                                },
                                error: function (xhr, status, error) {
                                    clearCustomerInfo();
                                    toastr.error('Error fetching customer info.');
                                }
                            });
            }

                        // Handle form submission to ensure valid customer is selected
                        document.querySelector('#saveQuotation').addEventListener('click', function (e) {
                if (!selectedCustomerId) {
                            e.preventDefault();
                }
            });

                        // Clear customer info on invalid selection
                        input.addEventListener('blur', function () {
                const query = input.value.trim();

                        // Check if the current input value matches any suggestion
                        fetch(`/Dashboard/GetBusinessName?query=${query}`)
                    .then(response => response.json())
                    .then(data => {
                        const match = data.find(item => item.name.toLowerCase() === query.toLowerCase());
                        if (!match) {
                            clearCustomerInfo();
                        selectedCustomerId = null;
                        $("#businessNameid").val('');  // Clear hidden field for ID
                            // setTimeout(function () {
                            //     toastr.error('Please select a valid business name.');

                            // }, 1000); // Adjust the timeout duration (3000 milliseconds = 3 seconds)
                            toastr.clear(); // This will clear all toastr notifications

                        }
                    });
            });

                        // Clear customer info function
                        function clearCustomerInfo() {
                            $('#customerName').text('');
                        $('#address').text('');
                        $('#city').text('');
                        $('#gstNo').text('');
                        $('#contactNo').text('');
                        $('#alternateNumber').text('');
                        $('#state').text('');
            }
        });


                    </script>

                    <script>


                        function updateDisplay() {
            const actionButtons = document.querySelector('.action-buttons');
                        const campaignFields = document.getElementById('campaign-fields');
                        const saveQuotationButton = document.getElementById('saveQuotation');
                        const saveCampaignButton = document.getElementById('saveCampaign');
                        const openCustomerModalButton = document.getElementById('openCustomerModal');


                        if (document.getElementById('create-quotation').checked) {
                            actionButtons.style.display = 'block';
                        saveQuotationButton.style.display = 'inline-block';
                        saveCampaignButton.style.display = 'none';
                        openCustomerModalButton.style.display = 'inline-block';

            }
                        else if (document.getElementById('add-campaign').checked) {
                            actionButtons.style.display = 'block';
                        $('#campDate').show();
                        saveQuotationButton.style.display = 'none';
                        saveCampaignButton.style.display = 'inline-block';
                        openCustomerModalButton.style.display = 'block';
            }
                        else {
                            actionButtons.style.display = 'none';
            }
        }

        document.querySelectorAll('input[name="quotation"]').forEach((radio) => {
                            radio.addEventListener('change', updateDisplay);
        });

                        // Set default selection and update display accordingly
                        document.getElementById('create-quotation').checked = true;
                        updateDisplay();

                        $(document).ready(function () {
                            // Initially hide the date fields
                            $('#campDate').hide();

                        // Event listener for the radio buttons
                        $('input[name="quotation"]').change(function () {
                if ($(this).val() === 'campaign') {
                            $('#campDate').show();
                } else {
                            $('#campDate').hide();
                }
            });


        });

                        $(document).ready(function () {
                            // Load customer details when the document is ready
                            $.ajax({
                                url: '/Customer/GetCustomerinfo', // Corrected endpoint for getting all customer info
                                type: 'GET',
                                success: function (response) {
                                    if (response.success) {
                                        var customerDropdown = $('#businessName');
                                        customerDropdown.empty(); // Clear existing options
                                        customerDropdown.append('<option value="">Select Customer</option>'); // Default option

                                        $.each(response.model, function (index, data) {
                                            customerDropdown.append('<option value="' + data.id + '">' + data.businessName + '</option>');
                                        });

                                        // Open the Add Quotation modal after loading customer details
                                        $('#addQuotationModal').modal('show');

                                    } else {
                                        console.error('Failed to load customer details:', response.Message);
                                    }
                                },
                                error: function (xhr, status, error) {
                                    console.error('Error fetching customer details:', error);
                                }
                            });

                        // Fetch and display customer details when a customer is selected
                        $('#businessName').change(function () {

                var selectedCustomerId = document.getElementById('businessNameid').value;


                        if (selectedCustomerId) {
                            $.ajax({
                                url: '/Customer/GetCustomerinfoById', // Correct endpoint for getting customer by ID
                                type: 'GET',
                                data: { id: selectedCustomerId },
                                success: function (response) {
                                    console.log("Selected Customer :", response.model.result.id);
                                    if (response.success) {
                                        // Accessing properties from response.model.result
                                        //  $('#businessName').text(response.model.result.businessName);
                                        $('#customerName').text(response.model.result.customerName);
                                        $('#address').text(response.model.result.address);
                                        $('#city').text(response.model.result.city);
                                        $('#gstNo').text(response.model.result.gstNo);
                                        $('#contactNo').text(response.model.result.contactNo);
                                        $('#alternateNumber').text(response.model.result.alternateNumber);
                                        $('#state').text(response.model.result.state);

                                        // setTimeout(function () {
                                        //     toastr.success('Customer loaded successfully.');
                                        // },);

                                    } else {
                                        console.error('Failed to load customer info:', response.message);
                                    }

                                },
                                error: function (xhr, status, error) {
                                    console.error('Error fetching customer info:', error);
                                }
                            });
                }
                        else {
                            // Clear customer info if no customer is selected
                            // $('#businessName').text('');
                            $('#customerName').text('');
                        $('#city').text('');
                        $('#area').text('');
                        $('#gstNo').text('');
                        $('#contactNo').text('');
                        $('#alternateNumber').text('');
                        $('#address').text('');
                        $('#state').text('');
                }
            });

        });

                        function cnfDeleteselecedInvetry(id) {


                            $('#selectedId').val(id);

                        // var selectedId = document.getElementById("selectedId").value();

                        $('#deleteHoarding').on('click', function () {
                var selectedCustomerId = $('#selectedId').val();
                        console.log("id" + selectedCustomerId);
                        $.ajax({
                            url: '/Dashboard/DeletedSelectInventoryHoarding', // Correct endpoint for getting customer by ID
                        type: 'Post',
                        data: {id: selectedCustomerId },
                        success: function (response) {

                        if (response.success) {
                            toastr.success(response.message);
                        location.reload();
                        }
                        else {
                            toastr.error(response.message);
                        }

                    },
                        error: function (xhr, status, error) {
                            console.error('Error fetching customer info:', error);
                    }

                });
            });

                        $('#deleteQuteHoarding').on('click', function () {
                var selectedCustomerId = $('#selectedId').val();
                        console.log("id" + selectedCustomerId);
                        $.ajax({
                            url: '/Dashboard/DeletedSelectInventoryHoarding',
                        type: 'Post',
                        data: {id: selectedCustomerId },
                        success: function (response) {

                        if (response.success) {
                            toastr.success(response.message);
                        location.reload();
                        }
                        else {
                            toastr.error(response.message);
                        }

                    },
                        error: function (xhr, status, error) {
                            console.error('Error fetching customer info:', error);
                    }

                });
            });
        }


                    </script>

