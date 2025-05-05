var addressCondition = 0;
function getRegionsData() {
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refregion.csv",
        //dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].replaceAll('"', '').split(",");
            //console.log(array.length);
            //console.log(headers);
            $("#region").empty();
            $("#region").append('<option value="0" disabled selected></option>');
            $("#region").append('<option value="0" disabled >Select Region</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].replaceAll('"', '').split(",");

                $("#region").append('<option value="' + headers[2] + '" data-id="' + headers[3] + '">' + headers[2] + "</option>");
            }
        }
    });
}
function getProvinceData() {
    var region = $("#region option:selected").data('id').toString().trim();
    region = parseInt(region, 10);
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refprovince.csv",
        dataType: "text",
        success: function (data) {
            const array = data.toString().split("\n");
            $("#province").empty();
            $("#province").append('<option value="0" disabled selected></option>');
            $("#province").append('<option value="0" disabled >Select Province</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].replaceAll('"', '').split(",");
                var lookupProvince = parseInt(headers[3], 10);
                //console.log(headers[3]);
                if (lookupProvince == region) {
                    //console.log(headers[2].replaceAll('"', ''));
                    //console.log(headers[2]);
                    //console.log(lookupProvince);
                    //console.log(region);
                    $("#province").append('<option value="' + headers[2] + '"  data-id="' + headers[4] + '">' + headers[2] + "</option>");
                }

            }


        }
    });
}
function getCityData() {
    var province = $("#province option:selected").data('id').toString().trim();
    province = parseInt(province, 10);
    //console.log("province");
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refcitymun.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",");
            //console.log(array.length);
            $("#municipality").empty();
            $("#municipality").append('<option value="0" disabled selected></option>');
            $("#municipality").append('<option value="0" disabled >Select City</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].replaceAll('"', '').split(",");
                var lookUpCity = parseInt(headers[4], 10);
                //console.log(headers[2]);
                if (lookUpCity == province) {
                    $("#municipality").append('<option value="' + headers[2] + '"  data-id="' + headers[5] + '">' + headers[2] + "</option>");
                }

            }

        }
    });
}
function getBarangayData() {
    var city = $("#municipality option:selected").data('id').toString().trim();

    city = parseInt(city, 10);
    //console.log("city");
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refbrgy.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].replaceAll('"', '').split(",");
            //console.log(array.length);
            $("#barangay").empty();
            $("#barangay").append('<option value="0" disabled selected></option>');
            $("#barangay").append('<option value="0" disabled >Select Barangay</option>');
            for (var i = 0; i < array.length; i++) {
                let headers = array[i].replaceAll('"', '').split(",");
                //console.log(headers[2]);
                if (headers[5] == city) {
                    $("#barangay").append('<option value="' + headers[2] + '">' + headers[2].toUpperCase() + "</option>");
                }

            }

        }
    });
}
function addressDOM() {
    var region = document.getElementById('region');
    var province = document.getElementById('province');
    var municipality = document.getElementById('municipality');
    var barangay = document.getElementById('barangay');

    province.disabled = 'true';
    municipality.disabled = 'true';
    barangay.disabled = 'true';
    $("#region").change(function () {
        var regionValue = $("#region option:selected").data('id');
        //console.log(regionValue);

        localStorage.setItem('region', regionValue);

        getProvinceData();
        $("#province").removeAttr('disabled');
        //console.log($("#region option:selected").text());
    });
    $("#province").change(function () {
        getCityData();
        $("#municipality").removeAttr('disabled');
        //console.log($("#province option:selected").text());
    });
    $("#municipality").change(function () {
        getBarangayData();
        $("#barangay").removeAttr('disabled');
        console.log($("#municipality option:selected").text());
    });
    $("#barangay").change(function () {
        console.log($("#barangay option:selected").text());
    });
}
function GetAllDataAddress() {
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refregion.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#region").empty();
            $("#region").append('<option value="0" disabled selected>Select Region</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",");
                //console.log(headers[2]);
                $("#region").append('<option value="' + headers[2].replaceAll('"', '') + '" data-id="' + headers[3].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '') + "</option>");
            }


        }
    });
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refprovince.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#province").empty();
            $("#province").append('<option value="0" disabled selected>Select Province</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",")
                //console.log(region);
                $("#province").append('<option value="' + headers[2].replaceAll('"', '') + '"  data-id="' + headers[4].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '') + "</option>");

            }

        }
    });
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refcitymun.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#municipality").empty();
            $("#municipality").append('<option value="0" disabled selected>Select City</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",")
                //console.log(headers[2]);
                $("#municipality").append('<option value="' + headers[2].replaceAll('"', '') + '"  data-id="' + headers[5].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '') + "</option>");


            }

        }
    });
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refbrgy.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#barangay").empty();
            $("#barangay").append('<option value="0" disabled selected>Select Barangay</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",")
                //console.log(headers[2]);
                $("#barangay").append('<option value="' + headers[2].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '').toUpperCase() + "</option>");


            }

        }
    });
}

function initializeEmployeeListDataTable() {
    var tableId = '#emp-table';
    var lastSelectedRow = null;
    var img = "/img/OPTION.webp";
    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }

    // Get date values
    // var data = {
    //     datefrom: $('#datefrom_ol').val(),
    //     dateto: $('#dateto_ol').val() // Assuming this refers to the date input element
    // };
    const data = { Username: 'username', Department: 'department', page: 1 };
    // DataTable properties
    var dtProperties = {
        ajax: {
            url: '/Employee/GetDataRegistrationList',
            type: "POST",
            data: {
                data: data
            },
            dataType: "json",
            processing: true,
            serverSide: true,
            complete: function (xhr) {
                var url = new URL(window.location.href);
                var _currentPage = url.searchParams.get("page01") == null ? 1 : url.searchParams.get("page01");
                // console.log('table1', _currentPage);
                table.page(_currentPage - 1).draw('page');
            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        responsive: true,
        "columns": [
            { "title": "<input type='checkbox' id='checkAllemployee' class='checkAllemployee'>", "data": null, "orderable": false },
            {
                "title": "Profile",
                "data": "id",
                "render": function (data, type, row) {

                    // var images = "https://eportal.odeccisolutions.com/" + row['filePath'] == null ? img : "https://eportal.odeccisolutions.com/" + row['filePath'];
                    var images = "../../" + row['filePath'] == null ? img : "../../" + row['filePath'];
                    let profile = "";
                    var initial = row['fname'].charAt(0) + row['lname'].charAt(0);
                    var fullname = row['fullname'].toUpperCase();
                    var employeeId = row['employeeId'].toUpperCase();
                    initial = initial.toUpperCase()
                    if (row['filePath'] == "" || row['filePath'] == null) {
                        profile = `<div class="employeeprofile">
                                        <div class="initial"> ${initial} </div>
                                        <div class="empdetails">
                                            <strong>${fullname}</strong>
                                            <span>${employeeId}</span>
                                        </div>
                                    </div>`;
                    }
                    else {
                        profile = `<div class="data-img"> <img src='${images}' width="100%" /> </div>`;
                        profile = `<div class="employeeprofile">
                                        <div class="data-img"> <img src='${images}' width="100%" /> </div>
                                        <div class="empdetails">
                                            <strong>${fullname}</strong>
                                            <span>${employeeId}</span>
                                        </div>
                                    </div>`;
                    }
                    return profile;
                }
            },
            {
                "title": "Employee ID",
                "data": "employeeId",
                "visible": false
            },
            {
                "title": "Fullname",
                "data": "fullname",
                "visible": false
            },
            {
                "title": "Department",
                "data": "department",
                "visible": true
            },
            {
                "title": "Employee Type",
                "data": "employeeType",
                "visible": false
            },
            {
                "title": "Position Level",
                "data": "positionLevel",
                "visible": false
            },
            {
                "title": "Position",
                "data": "position"
            },
            {
                "title": "Email",
                "data": "email"
            },
            {
                "title": "Gender",
                "data": "gender"
            },
            {
                "title": "Action",
                "data": "id", "orderable": false,
                "render": function (data, type, row) {
                    var button = `<label class="popup">
                                    <input type="checkbox">
                                    <div class="burger" tabindex="0">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="5" viewBox="0 0 20 5" fill="none">
                                            <path fill-rule="evenodd" clip-rule="evenodd" d="M17.5 5C16.837 5 16.2011 4.73661 15.7322 4.26777C15.2634 3.79893 15 3.16304 15 2.5C15 1.83696 15.2634 1.20107 15.7322 0.732234C16.2011 0.263393 16.837 0 17.5 0C18.163 0 18.7989 0.263393 19.2678 0.732234C19.7366 1.20107 20 1.83696 20 2.5C20 3.16304 19.7366 3.79893 19.2678 4.26777C18.7989 4.73661 18.163 5 17.5 5ZM2.5 5C1.83696 5 1.20107 4.73661 0.732233 4.26777C0.263392 3.79893 0 3.16304 0 2.5C0 1.83696 0.263392 1.20107 0.732233 0.732234C1.20107 0.263393 1.83696 0 2.5 0C3.16304 0 3.79893 0.263393 4.26777 0.732234C4.73661 1.20107 5 1.83696 5 2.5C5 3.16304 4.73661 3.79893 4.26777 4.26777C3.79893 4.73661 3.16304 5 2.5 5ZM10 5C9.33696 5 8.70107 4.73661 8.23223 4.26777C7.76339 3.79893 7.5 3.16304 7.5 2.5C7.5 1.83696 7.76339 1.20107 8.23223 0.732234C8.70107 0.263393 9.33696 0 10 0C10.663 0 11.2989 0.263393 11.7678 0.732234C12.2366 1.20107 12.5 1.83696 12.5 2.5C12.5 3.16304 12.2366 3.79893 11.7678 4.26777C11.2989 4.73661 10.663 5 10 5Z" fill="#205375"/>
                                        </svg>
                                    </div>
                                    <nav class="popup-window">
                                        
                                        <button class="tbl-edit btn btn-info act-btn" data-id="${data}" style="width: 100px; font-size:13px; padding: 5px 5px">
                                            <i class="fa-solid fa-pen-to-square"></i>
                                                Edit
                                        </button>
                                        <button class="tbl-delete btn btn-danger act-btn" data-id="${data}" style="width: 100px; font-size:13px; padding: 5px 5px">
                                            <i class="fa-solid fa-trash"></i>
                                            Delete
                                        </button>
                                    </nav>
                                </label>`;
                    return button;
                }
            }
        ], lengthChange: false,
        dom: 'Bfrtip',
        responsive: true,
        pagingType: "simple_numbers",
        language: {
            searchPlaceholder: "Type to search...",
            search: ""
        },
        pageLength: 10,
        order: [[0, 'desc']], // Sort the second column (index 1) by descending order
        columnDefs: [
            {
                targets: [0], // Checkbox column
                searchable: false,
                width: "5%", // Adjust width
                "className": "text-center",
                render: function (data, type, row) {
                    return '<input type="checkbox" id="" class="employee-row-checkbox" value="' + row.id + '">';
                },
                orderable: false,
            }
        ],
        buttons: [
            
            {
                extend: 'excel',
                text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-file-arrow-down"></i> Export Excel File</span>',
                title: 'Employee List', // Set custom title in the file
                filename: 'Employee_List', // Custom file name
                className: 'btn btn-info',
                attr: {
                    id: 'exportEmployee',
                    style: 'display:none' // Hide button
                },
                exportOptions: {
                    columns: [2, 3, 4, 5, 6, 7, 8, 9] // Specify column indexes to export
                }
            }
        ],
    };
    $('#emp-table').on('page.dt', function () {

        var info = table.page.info();
        var url = new URL(window.location.href);
        url.searchParams.set('page01', (info.page + 1));
        window.history.replaceState(null, null, url);
    });

    var table = $(tableId).DataTable(dtProperties);
    $(tableId + '_filter input').attr('placeholder', 'Searching...');
    $(tableId + ' tbody').on('click', 'tr', function () {
        var data = table.row(this).data();
        // console.log(data);
        // Remove highlight from the previously selected row
        if (lastSelectedRow) {
            $(lastSelectedRow).removeClass('selected-row');
        }

        // Highlight the currently selected row
        $(this).addClass('selected-row');
        lastSelectedRow = this;

    });
}
$('#emp-table').on('click', '.checkAllemployee', function () {
    var checkAll = document.getElementById("checkAllemployee");

    if (checkAll.checked == true) {
        var checkboxes = document.querySelectorAll('.employee-row-checkbox');
        for (var checkbox of checkboxes) {
            checkbox.checked = this.checked;
        }
    }
    else {
        var checkboxes = document.querySelectorAll('.employee-row-checkbox');
        for (var checkbox of checkboxes) {
            checkbox.checked = false;
        }
    }
});

//Employee Type
function initializeEmployeeTypeDataTable() {
    var tableId = '#etype-table';
    var lastSelectedRow = null;

    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }

    var dtProperties = {
        ajax: {
            url: '/EmployeeType/GetETypeList',
            type: "GET",
            data: {
            },
            dataType: "json",
            processing: true,
            serverSide: true,
            complete: function (xhr) {

            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        columns: [
            { "title": "<input type='checkbox' id='checkAlletype' class='checkAlletype'>", "data": null, "orderable": false },
            {
                "title": "Type",
                "data": "title"
            },
            {
                "title": "Description",
                "data": "description"
            },

            {
                "title": "Action",
                "data": "id",
                "render": function (data, type, row) {

                    var button = `<label class="popup">
                                    <input type="checkbox">
                                    <div class="burger" tabindex="0">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="5" viewBox="0 0 20 5" fill="none">
                                            <path fill-rule="evenodd" clip-rule="evenodd" d="M17.5 5C16.837 5 16.2011 4.73661 15.7322 4.26777C15.2634 3.79893 15 3.16304 15 2.5C15 1.83696 15.2634 1.20107 15.7322 0.732234C16.2011 0.263393 16.837 0 17.5 0C18.163 0 18.7989 0.263393 19.2678 0.732234C19.7366 1.20107 20 1.83696 20 2.5C20 3.16304 19.7366 3.79893 19.2678 4.26777C18.7989 4.73661 18.163 5 17.5 5ZM2.5 5C1.83696 5 1.20107 4.73661 0.732233 4.26777C0.263392 3.79893 0 3.16304 0 2.5C0 1.83696 0.263392 1.20107 0.732233 0.732234C1.20107 0.263393 1.83696 0 2.5 0C3.16304 0 3.79893 0.263393 4.26777 0.732234C4.73661 1.20107 5 1.83696 5 2.5C5 3.16304 4.73661 3.79893 4.26777 4.26777C3.79893 4.73661 3.16304 5 2.5 5ZM10 5C9.33696 5 8.70107 4.73661 8.23223 4.26777C7.76339 3.79893 7.5 3.16304 7.5 2.5C7.5 1.83696 7.76339 1.20107 8.23223 0.732234C8.70107 0.263393 9.33696 0 10 0C10.663 0 11.2989 0.263393 11.7678 0.732234C12.2366 1.20107 12.5 1.83696 12.5 2.5C12.5 3.16304 12.2366 3.79893 11.7678 4.26777C11.2989 4.73661 10.663 5 10 5Z" fill="#205375"/>
                                        </svg>
                                    </div>
                                    <nav class="popup-window">
                                        <button class="tbl-edit btn btn-info act-btn" id="add-timeout" title="Time Out"
                                            data-id="${data}"
                                            data-title="${row.title}"
                                            data-description="${row.description}"
                                            data-schedule = "${row.scheduleId}"
                                            >
                                            <i class="fa-solid fa-pen-to-square"></i> edit
                                        </button>
                                        <button class="tbl-delete btn btn-danger act-btn" id="add-timeout" title="Time Out"
                                            data-id="${data}"
                                            data-title="${row.title}"
                                            data-description="${row.description}"
                                            data-schedule = "${row.scheduleId}"
                                            >
                                            <i class="fa-solid fa-trash"></i> delete
                                        </button>
                                    </nav>
                                </label>`;

                    return button;
                }
            }
        ], lengthChange: false,
        dom: 'frtip',
        responsive: true,
        pagingType: "simple_numbers",
        language: {
            searchPlaceholder: "Type to search...",
            search: ""
        },
        order: [[0, 'desc']], // Sort the second column (index 1) by descending order
        columnDefs: [
            {
                targets: [0], // Checkbox column
                searchable: false,
                width: "5%", // Adjust width
                "className": "text-center",
                render: function (data, type, row) {
                    return '<input type="checkbox" id="" class="etype-row-checkbox" value="' + row.id + '">';
                },
                orderable: false,
            },
            {
                targets: 0,
                orderable: false,
            },
            {
                targets: 1,
                orderable: false,
            },
            {
                targets: 2,
                orderable: false,
            },
            {
                targets: 3,
                orderable: false,
            },
        ]

    };

    var table = $(tableId).DataTable(dtProperties);

    // Attach computeTotalRenderedHours to the search event
    $(tableId + '_filter input').on('keyup', function () {
        computeTotalRenderedHours();
    });

    $(tableId).on('page.dt', function () {
        var info = table.page.info();
        var url = new URL(window.location.href);
        url.searchParams.set('page01', (info.page + 1));
        window.history.replaceState(null, null, url);
    });

    $(tableId + '_filter input').attr('placeholder', 'Search anything here...');

    $(tableId + ' tbody').on('click', 'tr', function () {
        var data = table.row(this).data();
        // console.log(data);
        // Remove highlight from the previously selected row
        if (lastSelectedRow) {
            $(lastSelectedRow).removeClass('selected-row');
        }

        // Highlight the currently selected row
        $(this).addClass('selected-row');
        lastSelectedRow = this;
    });
}
function openETypeModal() {
    document.getElementById('eTypeId').value = "";
    document.getElementById('eTypetitle').value = "";
    document.getElementById('eTypedescription').value = "";
    document.getElementById('schedule').value = "";
    etypemodal.style.display = "flex";
    actionetype.style.display = "none";
    penciletype.style.display = "none";
}
function closeETypeModal() {
   
    etypemodal.style.display = "none";
    actionetype.style.display = "none";
    penciletype.style.display = "flex";
}
$('#etype-table').on('click', '.tbl-edit', function () {


    document.getElementById('eTypeId').value = $(this).data('id');
    document.getElementById('eTypetitle').value = $(this).data('title');
    document.getElementById('eTypedescription').value = $(this).data('description');
    document.getElementById('schedule').value = $(this).data('schedule');

    //etmodal = document.getElementById('etypemodal');
    etypemodal.style.display = "flex";
});
$('#etype-table').on('click', '.tbl-delete', function () {
    var id = $(this).data('id');
    localStorage.setItem('etypeid', id);

    deletemodalEType();
    $("#alertmodal").modal('show');
});
function delete_item_EType() {
    var data = {};
    var eTypeId = localStorage.getItem('etypeid');
    data.id = eTypeId;
    data.title = "";
    $.ajax({
        url: '/EmployeeType/AddEType',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        //console.log(data);
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        $("#alertmodal").modal('hide');
        initializeEmployeeTypeDataTable();
    });
}
$("#add-eType-form").on("submit", function (event) {
    event.preventDefault();
    var eTypeId = document.getElementById('eTypeId').value;
    var eTypetitle = document.getElementById('eTypetitle').value;
    var eTypedescription = document.getElementById('eTypedescription').value;
    var schedule = document.getElementById('schedule').value;

    var data = {};
    data.id = eTypeId;
    data.title = eTypetitle;
    data.description = eTypedescription;
    data.scheduleId = schedule;
    // console.log(data);

    var etmodal = document.getElementById('etypemodal');
    $.ajax({
        url: '/EmployeeType/AddEType',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        //console.log(data);
        notifyMsg('Success!', data.status, 'green', 'fas fa-check');
        etmodal.style.display = "none";
        initializeEmployeeTypeDataTable();
        penciletype.style.display = "flex";
    });

});
$('#etype-table').on('click', '.checkAlletype', function () {

    var checkAll = document.getElementById("checkAlletype");

    if (checkAll.checked == true) {
        var checkboxes = document.querySelectorAll('.etype-row-checkbox');
        for (var checkbox of checkboxes) {
            checkbox.checked = this.checked;
        }
    }
    else {
        var checkboxes = document.querySelectorAll('.etype-row-checkbox');
        for (var checkbox of checkboxes) {
            checkbox.checked = false;
        }
    }
});
function etypeActionFunction() {
    //payrollmodal.style.display = "none";
    actionetype.style.display = "flex";
    penciletype.style.display = "none";
}
function MultiDeleteEType() {
    var checkboxes = document.querySelectorAll('.etype-row-checkbox');
    var checkedCheckboxes = Array.from(checkboxes).filter(checkbox => checkbox.checked);


    actionetype.style.display = "none";
    penciletype.style.display = "flex";
    if (checkedCheckboxes.length == 0) {
        notifyMsg('Warning!', 'Select Employee Type First', 'yellow', 'fas fa-check');
    }
    else {

        for (var checkbox of checkedCheckboxes) {
            var value = checkbox.value;

            var data = {};
            data.id = value;
            data.title = "";
            $.ajax({
                url: '/EmployeeType/AddEType',
                data: data,
                type: "POST",
                dataType: "json"
            }).done(function (data) {
                //console.log(data);
                //notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
                //$("#alertmodal").modal('hide');
                //initializeEmployeeTypeDataTable();
            });
        }


        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        window.location.reload();
    }
}

function MultiDeleteEmployee() {
    var checkboxes = document.querySelectorAll('.employee-row-checkbox');
    var checkedCheckboxes = Array.from(checkboxes).filter(checkbox => checkbox.checked);

    actionemployee.style.display = "none";
    pencilemployee.style.display = "flex";
    if (checkedCheckboxes.length == 0) {
        notifyMsg('Warning!', 'Select Employee First', 'yellow', 'fas fa-check');
    }
    else {

        for (var checkbox of checkedCheckboxes) {
            var value = checkbox.value;
            var data = {};
            data.id = value;
            data.deletedBy = defaultCreatedBy;
            $.ajax({
                url: '/Employee/deleteemployee',
                data: {
                    data: data,
                },
                type: "POST",
                datatype: "json"
            }).done(function (data) {
                //console.log(data.stats);
            });
        }
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        initializeEmployeeListDataTable();
        //window.location.reload();
    }
}
function employeeActionFunction() {
    //payrollmodal.style.display = "none";
    actionemployee.style.display = "flex";
    pencilemployee.style.display = "none";
}
function WizzardFunction() {
    var prev = document.getElementById('wizzard-prev');
    var next = document.getElementById('wizzard-next');
    var cancel = document.getElementById('employee-cancel-button');
    var save = document.getElementById('employee-add-info');
    if (wizzardPage == 1) {
        prev.style.display = "none";
        cancel.style.display = "block";
    }
    else {
        prev.style.display = "block";
        cancel.style.display = "none";
    }
    if (wizzardPage == 5) {
        next.style.display = "none";
        save.style.display = "block";
    }
    else {
        next.style.display = "block";
        save.style.display = "none";
    }
}
//Wizzard Form
$('#wizzard-prev').on('click', function () {
    if (wizzardPage == 2) {
        page2.classList.remove('active');
        page2.classList.remove('completed');
        page1.classList.remove('completed');
        page1.classList.add('active');


        pagecontainer1.style.display = "flex";
        pagecontainer2.style.display = "none";
    }
    else if (wizzardPage == 3) {
        page3.classList.remove('active');
        page3.classList.remove('completed');
        page2.classList.remove('completed');
        page2.classList.add('active');

        pagecontainer2.style.display = "flex";
        pagecontainer3.style.display = "none";
    }
    else if (wizzardPage == 4) {
        page4.classList.remove('active');
        page4.classList.remove('completed');
        page3.classList.remove('completed');
        page3.classList.add('active');

        pagecontainer3.style.display = "flex";
        pagecontainer4.style.display = "none";
    }
    else if (wizzardPage == 5) {
        page5.classList.remove('active');
        page5.classList.remove('completed');
        page4.classList.remove('completed');
        page4.classList.add('active');

        pagecontainer4.style.display = "flex";
        pagecontainer5.style.display = "none";
    }
    wizzardPage--;
    WizzardFunction();
});
$('#wizzard-next').on('click', function () {
    var department = document.getElementById('department').value;
    var position = document.getElementById('position').value;
    var poslevel = document.getElementById('poslevel').value;

    var emptype = document.getElementById('emptype').value;
    var uType = document.getElementById('uType').value;
    var manager = document.getElementById('manager').value;
    var datehired = document.getElementById('datehired').value;

    var lname = document.getElementById('lname').value;
    var fname = document.getElementById('fname').value;
    var mname = document.getElementById('mname').value;

    var gender = document.getElementById('gender').value;
    var email = document.getElementById('email').value;
    var cno = document.getElementById('cno').value;
    if (wizzardPage == 1) {
        if (department == null || department == "" ||
            position == null || position == "" ||
            poslevel == null || poslevel == "" ||
            emptype == null || emptype == "" ||
            uType == null || uType == "" ||
            manager == null || manager == "" ||
            lname == null || lname == "" ||
            fname == null || fname == "" ||
            mname == null || mname == "" ||
            gender == null || gender == "" ||
            email == null || email == "" ||
            cno == null || cno == "") {
            notifyMsg('Warning!', 'Please fill in all required fields.', 'yellow', 'fas fa-check');

        }
        else {
            page1.classList.remove('active');
            page1.classList.add('completed');
            page2.classList.add('active');

            pagecontainer1.style.display = "none";
            pagecontainer2.style.display = "flex";

            wizzardPage++;
        }

    } else if (wizzardPage == 2) {

        var sss = document.getElementById('sss-id').value;
        var pagibig = document.getElementById('pagibig-id').value;
        var philhealth = document.getElementById('philhealth-id').value;
        var tax = document.getElementById('tax-id').value;
        if (sss == null || sss == "" ||
            pagibig == null || pagibig == "" ||
            philhealth == null || philhealth == "" ||
            tax == null || tax == "") {
            notifyMsg('Warning!', 'Please fill in all required fields.', 'yellow', 'fas fa-check');

        }
        else {
            page2.classList.remove('active');
            page2.classList.add('completed');
            page3.classList.add('active');

            pagecontainer2.style.display = "none";
            pagecontainer3.style.display = "flex";

            wizzardPage++;

        }

    } else if (wizzardPage == 3) {
        var salarytype = document.getElementById('salarytype').value;
        var payrolltype = document.getElementById('payrolltype').value;
        var rate = document.getElementById('rate').value;
        var daysinmonth = document.getElementById('daysinmonth').value;
        if (salarytype == null || salarytype == "" ||
            payrolltype == null || payrolltype == "" ||
            rate == null || rate == "" ||
            daysinmonth == null || daysinmonth == "") {
            notifyMsg('Warning!', 'Please fill in all required fields.', 'yellow', 'fas fa-check');

        }
        else {
            page3.classList.remove('active');
            page3.classList.add('completed');
            page4.classList.add('active');

            pagecontainer3.style.display = "none";
            pagecontainer4.style.display = "flex";

            wizzardPage++;
        }

    } else if (wizzardPage == 4) {
        var cob = document.getElementById('cob').value;
        var region = document.getElementById('region').value;
        var province = document.getElementById('province').value;
        var municipality = document.getElementById('municipality').value;
        var barangay = document.getElementById('barangay').value;
        if (cob == null || cob == "" ||
            region == null || region == "" || region == 0 ||
            province == null || province == "" || province == 0 ||
            municipality == null || municipality == "" || municipality == 0 ||
            barangay == null || barangay == "" || barangay == 0 ) {
            notifyMsg('Warning!', 'Please fill in all required fields.', 'yellow', 'fas fa-check');

        }
        else {
            page4.classList.remove('active');
            page4.classList.add('completed');
            page5.classList.add('active');

            pagecontainer4.style.display = "none";
            pagecontainer5.style.display = "flex";

            wizzardPage++;
        }
    }
    WizzardFunction();
});


//Files Upload
function sssfilesUpload() {
    const dropArea = document.getElementById('drop-area');
    const fileList = document.getElementById('file-list');
    const sssfile = document.getElementById('sssfile');

    dropArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        dropArea.classList.add('highlight');
    });

    dropArea.addEventListener('dragleave', () => {
        dropArea.classList.remove('highlight');
    });

    dropArea.addEventListener('drop', (e) => {
        e.preventDefault();
        dropArea.classList.remove('highlight');
        handleFiles(e.dataTransfer.files);
    });

    sssfile.addEventListener('change', (e) => {
        handleFiles(e.target.files);
    });

    function handleFiles(files) {
        [...files].forEach(uploadFile);
    }
    function uploadFile(file) {
        const fileItem = document.createElement('div');
        fileItem.className = 'file-item';

        const fileName = document.createElement('div');
        fileName.innerHTML = `<strong>${file.name}</strong>`;

        const status = document.createElement('div');
        status.textContent = 'Uploading...';

        const progressBar = document.createElement('div');
        progressBar.className = 'progress-bar';

        const actions = document.createElement('div');
        actions.className = 'file-actions';

        const cancelBtn = document.createElement('button');
        cancelBtn.innerHTML = '??';
        actions.appendChild(cancelBtn);

        cancelBtn.addEventListener('click', () => {
            fileItem.remove();
        });

        fileItem.append(fileName, status, progressBar, actions);
        fileList.appendChild(fileItem);

        // Simulate upload
        let progress = 0;
        const interval = setInterval(() => {
            progress += Math.random() * 20; // random speed
            progressBar.style.width = `${Math.min(progress, 100)}%`;

            if (progress >= 100) {
                clearInterval(interval);
                status.innerHTML = '<span style="color: green;">Completed</span>';
                cancelBtn.innerHTML = '???';
                cancelBtn.onclick = () => fileItem.remove();
            }
        }, 500);
    }

}
function pagibigfilesUpload() {
    const pagibigdropArea = document.getElementById('pagibig-drop-area');
    const pagibigfileList = document.getElementById('pagibig-file-list');
    const pagibigfile = document.getElementById('pagibigfile');

    pagibigdropArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        pagibigdropArea.classList.add('highlight');
    });

    pagibigdropArea.addEventListener('dragleave', () => {
        pagibigdropArea.classList.remove('highlight');
    });

    pagibigdropArea.addEventListener('drop', (e) => {
        e.preventDefault();
        pagibigdropArea.classList.remove('highlight');
        pagibighandleFiles(e.dataTransfer.files);
    });

    pagibigfile.addEventListener('change', (e) => {
        pagibighandleFiles(e.target.files);
    });

    function pagibighandleFiles(files) {
        [...files].forEach(pagibiguploadFile);
    }
    function pagibiguploadFile(file) {
        const pagibigfileItem = document.createElement('div');
        pagibigfileItem.className = 'file-item';

        const pagibigfileName = document.createElement('div');
        pagibigfileName.innerHTML = `<strong>${file.name}</strong>`;

        const pagibigstatus = document.createElement('div');
        pagibigstatus.textContent = 'Uploading...';

        const pagibigprogressBar = document.createElement('div');
        pagibigprogressBar.className = 'progress-bar';

        const pagibigactions = document.createElement('div');
        pagibigactions.className = 'file-actions';

        const pagibigcancelBtn = document.createElement('button');
        pagibigcancelBtn.innerHTML = '??';
        pagibigactions.appendChild(pagibigcancelBtn);

        pagibigcancelBtn.addEventListener('click', () => {
            pagibigfileItem.remove();
        });

        pagibigfileItem.append(pagibigfileName, pagibigstatus, pagibigprogressBar, pagibigactions);
        pagibigfileList.appendChild(pagibigfileItem);

        // Simulate upload
        let pagibigprogress = 0;
        const pagibiginterval = setInterval(() => {
            pagibigprogress += Math.random() * 20; // random speed
            pagibigprogressBar.style.width = `${Math.min(pagibigprogress, 100)}%`;

            if (pagibigprogress >= 100) {
                clearInterval(pagibiginterval);
                pagibigstatus.innerHTML = '<span style="color: green;">Completed</span>';
                pagibigcancelBtn.innerHTML = '???';
                pagibigcancelBtn.onclick = () => fileItem.remove();
            }
        }, 500);
    }

}
function philhealthUpload() {
    const philhealthdropArea = document.getElementById('philhealth-drop-area');
    const philhealthfileList = document.getElementById('philhealth-file-list');
    const philhealthfile = document.getElementById('philhealthfile');

    philhealthdropArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        philhealthdropArea.classList.add('highlight');
    });

    philhealthdropArea.addEventListener('dragleave', () => {
        philhealthdropArea.classList.remove('highlight');
    });

    philhealthdropArea.addEventListener('drop', (e) => {
        e.preventDefault();
        philhealthdropArea.classList.remove('highlight');
        philhealthhandleFiles(e.dataTransfer.files);
    });

    philhealthfile.addEventListener('change', (e) => {
        philhealthhandleFiles(e.target.files);
    });

    function philhealthhandleFiles(files) {
        [...files].forEach(philhealthuploadFile);
    }
    function philhealthuploadFile(file) {
        const philhealthfileItem = document.createElement('div');
        philhealthfileItem.className = 'file-item';

        const philhealthfileName = document.createElement('div');
        philhealthfileName.innerHTML = `<strong>${file.name}</strong>`;

        const philhealthstatus = document.createElement('div');
        philhealthstatus.textContent = 'Uploading...';

        const philhealthprogressBar = document.createElement('div');
        philhealthprogressBar.className = 'progress-bar';

        const philhealthactions = document.createElement('div');
        philhealthactions.className = 'file-actions';

        const philhealthcancelBtn = document.createElement('button');
        philhealthcancelBtn.innerHTML = '??';
        philhealthactions.appendChild(philhealthcancelBtn);

        philhealthcancelBtn.addEventListener('click', () => {
            philhealthfileItem.remove();
        });

        philhealthfileItem.append(philhealthfileName, philhealthstatus, philhealthprogressBar, philhealthactions);
        philhealthfileList.appendChild(philhealthfileItem);

        // Simulate upload
        let philhealthprogress = 0;
        const philhealthinterval = setInterval(() => {
            philhealthprogress += Math.random() * 20; // random speed
            philhealthprogressBar.style.width = `${Math.min(philhealthprogress, 100)}%`;

            if (philhealthprogress >= 100) {
                clearInterval(philhealthinterval);
                philhealthstatus.innerHTML = '<span style="color: green;">Completed</span>';
                philhealthcancelBtn.innerHTML = '???';
                philhealthcancelBtn.onclick = () => fileItem.remove();
            }
        }, 500);
    }

}
function taxUpload() {
    const taxdropArea = document.getElementById('tax-drop-area');
    const taxfileList = document.getElementById('tax-file-list');
    const taxfile = document.getElementById('taxfile');

    taxdropArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        taxdropArea.classList.add('highlight');
    });

    taxdropArea.addEventListener('dragleave', () => {
        taxdropArea.classList.remove('highlight');
    });

    taxdropArea.addEventListener('drop', (e) => {
        e.preventDefault();
        taxdropArea.classList.remove('highlight');
        taxhandleFiles(e.dataTransfer.files);
    });

    taxfile.addEventListener('change', (e) => {
        taxhandleFiles(e.target.files);
    });

    function taxhandleFiles(files) {
        [...files].forEach(taxuploadFile);
    }
    function taxuploadFile(file) {
        const taxfileItem = document.createElement('div');
        taxfileItem.className = 'file-item';

        const taxfileName = document.createElement('div');
        taxfileName.innerHTML = `<strong>${file.name}</strong>`;

        const taxstatus = document.createElement('div');
        taxstatus.textContent = 'Uploading...';

        const taxprogressBar = document.createElement('div');
        taxprogressBar.className = 'progress-bar';

        const taxactions = document.createElement('div');
        taxactions.className = 'file-actions';

        const taxcancelBtn = document.createElement('button');
        taxcancelBtn.innerHTML = '??';
        taxactions.appendChild(taxcancelBtn);

        taxcancelBtn.addEventListener('click', () => {
            taxfileItem.remove();
        });

        taxfileItem.append(taxfileName, taxstatus, taxprogressBar, taxactions);
        taxfileList.appendChild(taxfileItem);

        // Simulate upload
        let taxprogress = 0;
        const taxinterval = setInterval(() => {
            taxprogress += Math.random() * 20; // random speed
            taxprogressBar.style.width = `${Math.min(taxprogress, 100)}%`;

            if (taxprogress >= 100) {
                clearInterval(taxinterval);
                taxstatus.innerHTML = '<span style="color: green;">Completed</span>';
                taxcancelBtn.innerHTML = '???';
                taxcancelBtn.onclick = () => fileItem.remove();
            }
        }, 500);
    }

}
function otherUpload() {
    const otherdropArea = document.getElementById('other-drop-area');
    const otherfileList = document.getElementById('other-file-list');
    const otherfile = document.getElementById('otherfile');

    otherdropArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        otherdropArea.classList.add('highlight');
    });

    otherdropArea.addEventListener('dragleave', () => {
        otherdropArea.classList.remove('highlight');
    });

    otherdropArea.addEventListener('drop', (e) => {
        e.preventDefault();
        otherdropArea.classList.remove('highlight');
        otherhandleFiles(e.dataTransfer.files);
    });

    otherfile.addEventListener('change', (e) => {
        otherhandleFiles(e.target.files);
    });

    function otherhandleFiles(files) {
        [...files].forEach(otheruploadFile);
    }
    function otheruploadFile(file) {
        const otherfileItem = document.createElement('div');
        otherfileItem.className = 'file-item';

        const otherfileName = document.createElement('div');
        otherfileName.innerHTML = `<strong>${file.name}</strong>`;

        const otherstatus = document.createElement('div');
        otherstatus.textContent = 'Uploading...';

        const otherprogressBar = document.createElement('div');
        otherprogressBar.className = 'progress-bar';

        const otheractions = document.createElement('div');
        otheractions.className = 'file-actions';

        const othercancelBtn = document.createElement('button');
        othercancelBtn.innerHTML = '??';
        otheractions.appendChild(othercancelBtn);

        othercancelBtn.addEventListener('click', () => {
            otherfileItem.remove();
        });

        otherfileItem.append(otherfileName, otherstatus, otherprogressBar, otheractions);
        otherfileList.appendChild(otherfileItem);

        // Simulate upload
        let otherprogress = 0;
        const otherinterval = setInterval(() => {
            otherprogress += Math.random() * 20; // random speed
            otherprogressBar.style.width = `${Math.min(otherprogress, 100)}%`;

            if (otherprogress >= 100) {
                clearInterval(otherinterval);
                otherstatus.innerHTML = '<span style="color: green;">Completed</span>';
                othercancelBtn.innerHTML = '???';
                othercancelBtn.onclick = () => fileItem.remove();
            }
        }, 500);
    }

}


$('#employeeForm').submit(function (e) {
    e.preventDefault();
    var Id = localStorage.getItem('id');
    var department = document.getElementById('department').value;
    var position = document.getElementById('position').value;
    var emptype = document.getElementById('emptype').value;
    var uType = document.getElementById('uType').value;
    var lname = document.getElementById('lname').value;
    var fname = document.getElementById('fname').value;
    var mname = document.getElementById('mname').value;
    var suffix = document.getElementById('suffix').value;
    var email = document.getElementById('email').value;
    var domain = document.getElementById('domain').value;
    var gender = document.getElementById('gender').value;
    var datehired = document.getElementById('datehired').value;
    var payrolltype = document.getElementById('payrolltype').value;
    var salarytype = document.getElementById('salarytype').value
    var rate = document.getElementById('rate').value;
    var daysInMonth = document.getElementById('daysinmonth').value;
    var sss = document.getElementById('sss-id').value;
    var pagibig = document.getElementById('pagibig-id').value;
    var philhealth = document.getElementById('philhealth-id').value;
    var tax = document.getElementById('tax-id').value;
    

    
    
    const taxfilerandomString = Math.random().toString(36).substring(2, 10);
    const sssfilerandomString = Math.random().toString(36).substring(2, 10);
    const pagibigfilerandomString = Math.random().toString(36).substring(2, 10);
    const philhealthfilerandomString = Math.random().toString(36).substring(2, 10);
    const otherfilerandomString = Math.random().toString(36).substring(2, 10);

    let docformData = new FormData();
    var filetypeextention = "";
    //const taxfileInput = document.getElementById('taxfile');
    // File input configuration
    const fileInputs = [
        { id: 'sssfile', label: 'sss', randomString: sssfilerandomString },
        { id: 'pagibigfile', label: 'pagibig', randomString: pagibigfilerandomString },
        { id: 'philhealthfile', label: 'philhealth', randomString: philhealthfilerandomString },
        { id: 'taxfile', label: 'tax', randomString: taxfilerandomString },
        { id: 'otherfile', label: 'other', randomString: otherfilerandomString }
    ];
   
    fileInputs.forEach(({ id, label, randomString }) => {
        const input = document.getElementById(id);
        if (!input || input.files.length === 0) return;

        Array.from(input.files).forEach((file, index) => {
            const ext = file.name.substring(file.name.lastIndexOf('.'));

            const isMultiple = input.multiple;
            const newFileName = isMultiple
                ? `${label}_${fname}_${randomString}_${index+1}${ext}`
                : `${label}_${fname}_${randomString}_${index+1}${ext}`;

            const renamedFile = new File([file], newFileName, {
                type: file.type,
                lastModified: file.lastModified
            });

            docformData.append(label.toLowerCase(), renamedFile);
        });
    });

    var documentsfiles = [];
    const otherfileInput = document.getElementById('otherfile');
    const otherfileCount = otherfileInput.files.length;

    for (let i = 0; i < otherfileCount; i++) {
        const file = otherfileInput.files[i];
        const filename = file.name;
        const fileext = filename.substring(filename.lastIndexOf('.')); // gets .pdf, .jpg, etc.
        const filepath = "/employeedocuments/" + `other_${fname}_${otherfilerandomString}_${i + 1}${fileext}`;

        documentsfiles.push({
            UserId: localStorage.getItem('id'),
            FileType: 'other',
            FileName: `other_${fname}_${otherfilerandomString}_${i}${fileext}`,
            FilePath: filepath
        });
    }
    console.log(`Files selected: ${otherfileCount}`);
    var sssfilename = $("#sssfile").val().replace(/.*(\/|\\)/, '');
    var sssfilepath = "";
    if (sssfilename == "" || sssfilename == null) {
        sssfilepath = "";
    }
    else {
        sssfilepath = "/employeedocuments/" + `sss_${fname}_${sssfilerandomString}.pdf`;
        // Create an object and push to the array
        documentsfiles.push({
            UserId: localStorage.getItem('id'),
            FileType: 'SSS',
            FileName: `sss_${fname}_${sssfilerandomString}.pdf`,
            FilePath: sssfilepath
        });
    }
    var pagibigfilename = $("#pagibigfile").val().replace(/.*(\/|\\)/, '');
    var pagibigfilepath = "";
    if (pagibigfilename == "" || pagibigfilename == null) {
        pagibigfilepath = "";
    }
    else {
        pagibigfilepath = "/employeedocuments/" + `pagibig_${fname}_${pagibigfilerandomString}.pdf`;
        // Create an object and push to the array
        documentsfiles.push({
            UserId: localStorage.getItem('id'),
            FileType: 'PagIbig',
            FileName: `pagibig_${fname}_${pagibigfilerandomString}.pdf`,
            FilePath: pagibigfilepath
        });
    }
    var philhealthfilename = $("#philhealthfile").val().replace(/.*(\/|\\)/, '');
    var philhealthfilepath = "";
    if (philhealthfilename == "" || philhealthfilename == null) {
        philhealthfilepath = "";
    }
    else {
        philhealthfilepath = "/employeedocuments/" + `philhealth_${fname}_${philhealthfilerandomString}.pdf`;
        // Create an object and push to the array
        documentsfiles.push({
            UserId: localStorage.getItem('id'),
            FileType: 'PhilHealth',
            FileName: `philhealth_${fname}_${philhealthfilerandomString}.pdf`,
            FilePath: philhealthfilepath
        });
    }
    var taxfilename = $("#taxfile").val().replace(/.*(\/|\\)/, '');
    var taxfilepath = "";
    if (taxfilename == "" || taxfilename == null) {
        taxfilepath = "";
    }
    else {
        taxfilepath = "/employeedocuments/" + `tax_${fname}_${taxfilerandomString}.pdf`;
        // Create an object and push to the array
        documentsfiles.push({
            UserId: localStorage.getItem('id'),
            FileType: 'tax',
            FileName: `tax_${fname}_${taxfilerandomString}.pdf`,
            FilePath: taxfilepath
        });
    }

    console.log(documentsfiles);
    filename = $("#img").val().replace(/.*(\/|\\)/, '');
    if (filename == "" || filename == null) {
        filename = "";
    }
    else {
        filename = "/img/" + filename;
    }


    if (Id == null) {
        var status = '1003'; //document.getElementById('status').value;
    }
    else {
        var status = '6';
    }

    var address = document.getElementById('cob').value + ", " + document.getElementById('region').value + ", "
        + document.getElementById('province').value + ", "
        + document.getElementById('municipality').value
        + ", " + document.getElementById('barangay').value;
    var cno = document.getElementById('cno').value;
    var createdby = defaultCreadedBy;
    var username = email.toLowerCase();
    var password = (Math.random() + 1).toString(36).substring(4);
    var pass = password;
    var positionlevel = document.getElementById('poslevel').value;
    var manager = document.getElementById('manager').value;
    // Emergency Contact
    var ecid = document.getElementById('ecid').value;
    var ecname = document.getElementById('ecname').value;
    var relationship = document.getElementById('relationship').value;
    var ecnumber = document.getElementById('ecnumber').value;
    // Gather other fields similarly
    // console.log(position);
    // Prepare data object to send via AJAX
    var data = {
        Department: department,
        FilePath: filename,
        Id: Id,
        UserType: emptype,
        EmployeeType: uType,
        Cno: cno,
        Lname: lname,
        Fname: fname,
        Position: position,
        Mname: mname,
        Suffix: suffix,
        Email: email + domain,
        Gender: gender,
        DateStarted: datehired,
        SalaryType: salarytype,
        Status: status,
        Address: address,
        PayrollType: payrolltype,
        CreatedBy: createdby.toString(),
        Username: username,
        Password: pass,
        PositionLevelId: positionlevel,
        ManagerId: manager,
        Rate: rate,
        DaysInMonth: daysInMonth,
        SSS_Number: sss,
        PagIbig_MID_Number: pagibig,
        PhilHealth_Number: philhealth,
        Tax_Identification_Number: tax
        // Add other fields as needed
    };
    // console.log(data);
    let formData = new FormData();
    var fileUpload = $("#img").get(0);
    var files = fileUpload.files;
    formData.append('file', files[0]);
    console.log(formData);
    $.ajax({
        url: '/Employee/SaveEmployee',
        data: {
            data: data,
        },
        type: "POST",
        datatype: "json"
    }).done(function (data) {
        // console.log(data);
        //console.log(data.status);
        //console.log(documentsfiles);
        //console.log(JSON.stringify(documentsfiles));
        if (data.status == '200') {
            //Save to database the filepath
            $.ajax({
                url: '/Employee/RequiredDocuments',
                data: {
                    data: documentsfiles,
                },
                type: "POST",
                datatype: "json",
                success: function (result) {
                    console.log("Success:", result);
                },
                error: function (xhr, status, error) {
                    console.error("Error:", xhr.responseText);
                }
            });
            $.ajax({
                url: '/Employee/UploadFile',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,

                success: function (result) {
                    // console.log(result);
                }
            });
            //Upload the file
            $.ajax({
                url: '/Employee/UploadDocuments',
                type: 'POST',
                data: docformData,
                processData: false,
                contentType: false,
                success: function (result) {
                    console.log(result);
                },
                error: function (xhr, status, error) {
                    console.error(`Error UploadDocuments: ${error}`);
                }
            });
            //Upload the other files
            //$.ajax({
            //    url: '/Employee/UploadOtherDocuments',
            //    type: 'POST',
            //    data: otherformData,
            //    processData: false,
            //    contentType: false,
            //    success: function (result) {
            //        console.log(result);
            //    },
            //    error: function (xhr, status, error) {
            //        alert(`Error UploadDocuments: ${error}`);
            //    }
            //});
            successmodal(Id);
            $("#alertmodal").modal('show');
            if (Id == null) {
                var data = {};
                data.name = fname;
                data.email = email + domain;
                data.username = username;
                data.password = pass;
                // console.log(data);
                $.ajax({
                    url: '/Employee/EmailUnregisterUser',
                    data: {
                        data: data,
                    },
                    type: "POST",
                    datatype: "json",
                    success: function (response) {

                    }
                });
            }
            var ecdata = {};

            ecdata.id = ecid;
            ecdata.userId = Id;
            ecdata.name = ecname;
            ecdata.relationship = relationship;
            ecdata.phoneNumber = ecnumber;
            // console.log(ecdata);
            $.ajax({
                url: '/Employee/SaveEmergencyContact',
                data: {
                    data: ecdata,
                },
                type: "POST",
                datatype: "json",
                success: function (response) {
                    window.location.href = '/Employee/Index/';
                }
            });
            // getempdetails();
        }
        else {
            notifyMsg('Warning!', data.status, 'red', 'fas fa-exclamation-triangle');
        }

    });


});

function fetchRequiredDocuments() {

    $("#sssuploadedfile").empty();
    $("#pagibiguploadedfile").empty();
    $("#philhealthuploadedfile").empty();
    $("#taxuploadedfile").empty();
    $("#otheruploadedfile").empty();
    var data = {};
    data.userId = userId;
    $.ajax({
        url: '/Employee/PostRequiredDocuments',
        data: {
            data
        },
        type: "POST",
        datatype: "json",
        success: function (data) {
            var checksss = "";
            var checkpagibig = "";
            var checkphilhealth = "";
            var checktax = "";

            var svgpdf = `<svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 32 32"><path fill="#909090" d="m24.1 2.072l5.564 5.8v22.056H8.879V30h20.856V7.945z"/><path fill="#f4f4f4" d="M24.031 2H8.808v27.928h20.856V7.873z"/><path fill="#7a7b7c" d="M8.655 3.5h-6.39v6.827h20.1V3.5z"/><path fill="#dd2025" d="M22.472 10.211H2.395V3.379h20.077z"/><path fill="#464648" d="M9.052 4.534H7.745v4.8h1.028V7.715L9 7.728a2 2 0 0 0 .647-.117a1.4 1.4 0 0 0 .493-.291a1.2 1.2 0 0 0 .335-.454a2.1 2.1 0 0 0 .105-.908a2.2 2.2 0 0 0-.114-.644a1.17 1.17 0 0 0-.687-.65a2 2 0 0 0-.409-.104a2 2 0 0 0-.319-.026m-.189 2.294h-.089v-1.48h.193a.57.57 0 0 1 .459.181a.92.92 0 0 1 .183.558c0 .246 0 .469-.222.626a.94.94 0 0 1-.524.114m3.671-2.306c-.111 0-.219.008-.295.011L12 4.538h-.78v4.8h.918a2.7 2.7 0 0 0 1.028-.175a1.7 1.7 0 0 0 .68-.491a1.9 1.9 0 0 0 .373-.749a3.7 3.7 0 0 0 .114-.949a4.4 4.4 0 0 0-.087-1.127a1.8 1.8 0 0 0-.4-.733a1.6 1.6 0 0 0-.535-.4a2.4 2.4 0 0 0-.549-.178a1.3 1.3 0 0 0-.228-.017m-.182 3.937h-.1V5.392h.013a1.06 1.06 0 0 1 .6.107a1.2 1.2 0 0 1 .324.4a1.3 1.3 0 0 1 .142.526c.009.22 0 .4 0 .549a3 3 0 0 1-.033.513a1.8 1.8 0 0 1-.169.5a1.1 1.1 0 0 1-.363.36a.67.67 0 0 1-.416.106m5.08-3.915H15v4.8h1.028V7.434h1.3v-.892h-1.3V5.43h1.4v-.892"/><path fill="#dd2025" d="M21.781 20.255s3.188-.578 3.188.511s-1.975.646-3.188-.511m-2.357.083a7.5 7.5 0 0 0-1.473.489l.4-.9c.4-.9.815-2.127.815-2.127a14 14 0 0 0 1.658 2.252a13 13 0 0 0-1.4.288Zm-1.262-6.5c0-.949.307-1.208.546-1.208s.508.115.517.939a10.8 10.8 0 0 1-.517 2.434a4.4 4.4 0 0 1-.547-2.162Zm-4.649 10.516c-.978-.585 2.051-2.386 2.6-2.444c-.003.001-1.576 3.056-2.6 2.444M25.9 20.895c-.01-.1-.1-1.207-2.07-1.16a14 14 0 0 0-2.453.173a12.5 12.5 0 0 1-2.012-2.655a11.8 11.8 0 0 0 .623-3.1c-.029-1.2-.316-1.888-1.236-1.878s-1.054.815-.933 2.013a9.3 9.3 0 0 0 .665 2.338s-.425 1.323-.987 2.639s-.946 2.006-.946 2.006a9.6 9.6 0 0 0-2.725 1.4c-.824.767-1.159 1.356-.725 1.945c.374.508 1.683.623 2.853-.91a23 23 0 0 0 1.7-2.492s1.784-.489 2.339-.623s1.226-.24 1.226-.24s1.629 1.639 3.2 1.581s1.495-.939 1.485-1.035"/><path fill="#909090" d="M23.954 2.077V7.95h5.633z"/><path fill="#f4f4f4" d="M24.031 2v5.873h5.633z"/><path fill="#fff" d="M8.975 4.457H7.668v4.8H8.7V7.639l.228.013a2 2 0 0 0 .647-.117a1.4 1.4 0 0 0 .493-.291a1.2 1.2 0 0 0 .332-.454a2.1 2.1 0 0 0 .105-.908a2.2 2.2 0 0 0-.114-.644a1.17 1.17 0 0 0-.687-.65a2 2 0 0 0-.411-.105a2 2 0 0 0-.319-.026m-.189 2.294h-.089v-1.48h.194a.57.57 0 0 1 .459.181a.92.92 0 0 1 .183.558c0 .246 0 .469-.222.626a.94.94 0 0 1-.524.114m3.67-2.306c-.111 0-.219.008-.295.011l-.235.006h-.78v4.8h.918a2.7 2.7 0 0 0 1.028-.175a1.7 1.7 0 0 0 .68-.491a1.9 1.9 0 0 0 .373-.749a3.7 3.7 0 0 0 .114-.949a4.4 4.4 0 0 0-.087-1.127a1.8 1.8 0 0 0-.4-.733a1.6 1.6 0 0 0-.535-.4a2.4 2.4 0 0 0-.549-.178a1.3 1.3 0 0 0-.228-.017m-.182 3.937h-.1V5.315h.013a1.06 1.06 0 0 1 .6.107a1.2 1.2 0 0 1 .324.4a1.3 1.3 0 0 1 .142.526c.009.22 0 .4 0 .549a3 3 0 0 1-.033.513a1.8 1.8 0 0 1-.169.5a1.1 1.1 0 0 1-.363.36a.67.67 0 0 1-.416.106m5.077-3.915h-2.43v4.8h1.028V7.357h1.3v-.892h-1.3V5.353h1.4v-.892"/></svg>`
            var svgtrash = `<svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 512 512"><path fill="none" d="M296 64h-80a7.91 7.91 0 0 0-8 8v24h96V72a7.91 7.91 0 0 0-8-8"/><path fill="none" d="M292 64h-72a4 4 0 0 0-4 4v28h80V68a4 4 0 0 0-4-4"/><path fill="#ED1C24" d="M447.55 96H336V48a16 16 0 0 0-16-16H192a16 16 0 0 0-16 16v48H64.45L64 136h33l20.09 314A32 32 0 0 0 149 480h214a32 32 0 0 0 31.93-29.95L415 136h33ZM176 416l-9-256h33l9 256Zm96 0h-32V160h32Zm24-320h-80V68a4 4 0 0 1 4-4h72a4 4 0 0 1 4 4Zm40 320h-33l9-256h33Z"/></svg>`;
            //alert(data[0].fileType);
            for (var i = 0; i < data.length; i++) {
                if (data[i].fileType == "SSS") {
                    checksss = data[i].fileType;
                    $("#sssuploadedfile").append(
                        '<div class="uploaded-file">'
                            + '<div>'
                                + '<a  href="'
                                    + data[i].filePath 
                                    + '">'
                                    + svgpdf
                                    + ' '
                                    + data[i].fileName 
                                    + '</a>'
                            + '</div>'
                            + '<div>'
                                + '<a data-id="'
                                + data[i].id 
                                + '" data-filename="'
                                + data[i].fileName 
                                + '" class="delete-file">'
                                + svgtrash
                                + '</a>'
                            + '</div>'
                        + '</div>');
                }
                if (data[i].fileType == "PagIbig") {
                    checkpagibig = data[i].fileType;
                    $("#pagibiguploadedfile").append(
                        '<div class="uploaded-file">'
                            + '<div>'
                                + '<a  href="'
                                    + data[i].filePath 
                                    + '">'
                                    + svgpdf
                                    + ' '
                                    + data[i].fileName 
                                    + '</a>'
                            + '</div>'
                            + '<div>'
                                + '<a data-id="'
                                + data[i].id 
                                + '" data-filename="'
                                + data[i].fileName 
                                + '" class="delete-file">'
                                + svgtrash
                                + '</a>'
                            + '</div>'
                        + '</div>');
                }
                if (data[i].fileType == "PhilHealth") {
                    checkphilhealth = data[i].fileType;
                    $("#philhealthuploadedfile").append(
                        '<div class="uploaded-file">'
                            + '<div>'
                                + '<a  href="'
                                    + data[i].filePath 
                                    + '">'
                                    + svgpdf
                                    + ' '
                                    + data[i].fileName 
                                    + '</a>'
                            + '</div>'
                            + '<div>'
                                + '<a data-id="'
                                + data[i].id 
                                + '" data-filename="'
                                + data[i].fileName 
                                + '" class="delete-file">'
                                + svgtrash
                                + '</a>'
                            + '</div>'
                        + '</div>');
                }
                if (data[i].fileType == "tax") {
                    checktax = data[i].fileType;
                    $("#taxuploadedfile").append(
                        '<div class="uploaded-file">'
                            + '<div>'
                                + '<a  href="'
                                    + data[i].filePath 
                                    + '">'
                                    + svgpdf
                                    + ' '
                                    + data[i].fileName 
                                    + '</a>'
                            + '</div>'
                            + '<div>'
                                + '<a data-id="'
                                + data[i].id 
                                + '" data-filename="'
                                + data[i].fileName 
                                + '" class="delete-file">'
                                + svgtrash
                                + '</a>'
                            + '</div>'
                        + '</div>');
                }

                if (data[i].fileType == "other") {
                    $("#otheruploadedfile").append(
                        '<div class="uploaded-file">'
                            + '<div>'
                                + '<a  href="'
                                    + data[i].filePath 
                                    + '">'
                                    + svgpdf
                                    + ' '
                                    + data[i].fileName 
                                    + '</a>'
                            + '</div>'
                            + '<div>'
                                + '<a data-id="'
                                + data[i].id 
                                + '" data-filename="'
                                + data[i].fileName 
                                + '" class="delete-file">'
                                + svgtrash
                                + '</a>'
                            + '</div>'
                        + '</div>');
                }
            }

            if (checksss != "") {
                document.getElementById('sss-upload-container').style.display = "none";
            }
            else {
                document.getElementById('sss-upload-container').style.display = "block";
            }
            if (checkpagibig != "") {
                
                document.getElementById('pagibig-upload-container').style.display = "none";
            }
            else {
                document.getElementById('pagibig-upload-container').style.display = "block";
            }
            if (checkphilhealth != "") {
                document.getElementById('philhealth-upload-container').style.display = "none";
            }
            else {
                document.getElementById('philhealth-upload-container').style.display = "block";
            }
            if (checktax != "") {
                document.getElementById('tax-upload-container').style.display = "none";
            }
            else {
                document.getElementById('tax-upload-container').style.display = "block";
            }
        }
    });
}
function deleteUploadedFile(id, filename) {
    //alert(id)
    var data = [];
    //data.id = id;
    data.push({
        Id: id,
        FileType: '',
        FileName: filename,
        FilePath: ''
    });
    $.ajax({
        url: '/Employee/RequiredDocuments',
        data: {
            data: data,
        },
        type: "POST",
        datatype: "json",
        success: function (result) {
            console.log("Success:", result);
            fetchRequiredDocuments();
        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
        }
    });

}

$(document).on('click', '.delete-file', function () {
    var id = $(this).data('id');
    var filename = $(this).data('filename');
    deleteUploadedFile(id, filename);

});


$("#import-employee").click(function () {
    document.getElementById('employee-filing-container').style.display = "block";
    actionemployee.style.display = "none";
    pencilemployee.style.display = "none";
});
$("#employee-close-filing-container").click(function () {
    document.getElementById('employee-filing-container').style.display = "none";
    pencilemployee.style.display = "flex";
});
document.getElementById("export-employee").addEventListener("click", function () {
    document.getElementById("exportEmployee").click();
    notifyMsg('Success!', 'Exported Successfully!', 'green', 'fas fa-check');
    actionemployee.style.display = "none";
    pencilemployee.style.display = "flex";
});