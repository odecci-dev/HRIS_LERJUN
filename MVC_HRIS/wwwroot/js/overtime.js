function FetchOvertimeList() {

    var tableId = '#overtime-table';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
    var sdate = document.getElementById('ot-datefrom').value;
    var edate = document.getElementById('ot-dateto').value;
    let data = {
        StartDate: sdate,
        EndDate: edate,
        EmployeeNo: EmployeeID
    };
    //console.log(data);
    var dtProperties = {
        //responsive: true, // Enable responsive behavior
        //scrollX: true,    // Enable horizontal scrolling if needed
        //processing: true,
        //serverSide: true,
        ajax: {
            url: '/OverTime/GetOverTimeList',
            type: "POST",
            data: {
                data: data
            },
            dataType: "json",
            processing: true,
            //serverSide: true,
            complete: function (xhr) {
                var url = new URL(window.location.href);
                var _currentPage = url.searchParams.get("page01") == null ? 1 : url.searchParams.get("page01");
                //console.log('table1', _currentPage);
                table.page(_currentPage - 1).draw('page');
            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        dom: 'frtip',
        responsive: true,
        pagingType: "simple_numbers",
        language: {
            searchPlaceholder: "Type to search...",
            search: ""
        },
        "columns": [
            { "title": "<input type='checkbox' id='checkAllOTList' class='checkAllOTList'>", "data": null, "orderable": false },
            {
                "title": "OT-Number",
                "data": "otNo", "orderable": false
            },

            {
                "title": "Date",
                "data": "date", "orderable": true
            },
            {
                "title": "Start Time",
                "data": "startTime", "orderable": false
            },
            {
                "title": "End Time",
                "data": "endTime", "orderable": false
            },
            {
                "title": "Start Date",
                "data": "startDate", "orderable": false
            },
            {
                "title": "End Date",
                "data": "endDate", "orderable": false
            },
            {
                "title": "Hours Filed",
                "data": "hoursFiled", "orderable": false
            },
            {
                "title": "Hours Approved",
                "data": "hoursApproved",
                "orderable": false,
                "render": function (data, type, row) {
                    if (data == "" || data == null) {
                        return '0';
                    }
                    else {
                        return data;
                    }

                }
            },
            {
                "title": "Remarks",
                "data": "remarks",
                "orderable": false,
                "visible": false
            }
            ,
            {
                "title": "Convert To Leave",
                "data": "convertToLeave",
                "orderable": false,
                "visible": false
            }
            ,
            {
                "title": "Convert To Offset",
                "data": "convertToOffset",
                "orderable": false,
                "visible": false
            }
            ,
            {
                "title": "Status",
                "data": "statusName",
                "render": function (data, type, row) {
                    var badge = "";
                    if(data == 'APPROVED') {
                        badge = "<span class='bg-success p-1 px-3 text-light' style='border-radius: 15px; font-size: .8rem'>APPROVED</span>";
                        }
                    else if(data == 'PENDING') {
                        badge = "<span class='bg-warning p-1 px-3 text-light' style='border-radius: 15px; font-size: .8rem'>PENDING</span>";
                    }
                    else if (data == 'DECLINED') {
                        badge = "<span class='bg-danger p-1 px-3 text-light' style='border-radius: 15px; font-size: .8rem'>DECLINED</span>";
                    }
                    
                return badge;
                }
            },
            {
                "title": "Action",
                "data": "id", "orderable": false,
                "render": function (data, type, row) {
                    var button = '';
                    //if (row.statusName == 'PENDING') {
                    //    button = `<a class="editot" style="cursor: pointer"
                    //                data-id="${data}"
                    //                data-sdate="${row.startDate}"
                    //                data-edate="${row.endDate}"
                    //                data-dfiled="${row.daysFiled}"
                    //                data-leavetype="${row.leaveTypeId}"
                    //                data-reason="${row.reason}"
                    //            id="editot"><svg  xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--dark)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 14.66V20a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h5.34"></path><polygon points="18 2 22 6 12 16 8 16 8 12 18 2"></polygon></svg></a>`;
                    //}
                    //return button;
                    if (row.statusName == 'PENDING') {
                        button = `<label class="popup">
                                        <input type="checkbox">
                                        <div class="burger" tabindex="0">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="5" viewBox="0 0 20 5" fill="none">
                                                <path fill-rule="evenodd" clip-rule="evenodd" d="M17.5 5C16.837 5 16.2011 4.73661 15.7322 4.26777C15.2634 3.79893 15 3.16304 15 2.5C15 1.83696 15.2634 1.20107 15.7322 0.732234C16.2011 0.263393 16.837 0 17.5 0C18.163 0 18.7989 0.263393 19.2678 0.732234C19.7366 1.20107 20 1.83696 20 2.5C20 3.16304 19.7366 3.79893 19.2678 4.26777C18.7989 4.73661 18.163 5 17.5 5ZM2.5 5C1.83696 5 1.20107 4.73661 0.732233 4.26777C0.263392 3.79893 0 3.16304 0 2.5C0 1.83696 0.263392 1.20107 0.732233 0.732234C1.20107 0.263393 1.83696 0 2.5 0C3.16304 0 3.79893 0.263393 4.26777 0.732234C4.73661 1.20107 5 1.83696 5 2.5C5 3.16304 4.73661 3.79893 4.26777 4.26777C3.79893 4.73661 3.16304 5 2.5 5ZM10 5C9.33696 5 8.70107 4.73661 8.23223 4.26777C7.76339 3.79893 7.5 3.16304 7.5 2.5C7.5 1.83696 7.76339 1.20107 8.23223 0.732234C8.70107 0.263393 9.33696 0 10 0C10.663 0 11.2989 0.263393 11.7678 0.732234C12.2366 1.20107 12.5 1.83696 12.5 2.5C12.5 3.16304 12.2366 3.79893 11.7678 4.26777C11.2989 4.73661 10.663 5 10 5Z" fill="#205375"/>
                                            </svg>
                                        </div>
                                        <nav class="popup-window">
                                            <button class="tbl-decline btn btn-danger" id="aprroved-timein" title="Delete"
                                                    data-id="${data}"
                                                    data-shdate="${row.date}"
                                                    data-sdate="${row.startDate}"
                                                    data-edate="${row.endDate}"
                                                    data-etime="${row.startTime}"
                                                    data-etime="${row.endTime}"
                                                    data-hfiled="${row.hoursFiled}"
                                                    data-toleave="${row.convertToLeave}"
                                                    data-tooffset="${row.convertToOffset}"
                                                    data-remarks="${row.remarks}"
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                >
                                                <i class="fa-solid fa-circle-xmark"></i> Delete
                                            </button>
                                            <button class="tbl-approve btn btn-info editot" id="editot" title="Time Out"
                                                    data-id="${data}"
                                                    data-shdate="${row.date}"
                                                    data-sdate="${row.startDate}"
                                                    data-edate="${row.endDate}"
                                                    data-stime="${row.startTime}"
                                                    data-etime="${row.endTime}"
                                                    data-hfiled="${row.hoursFiled}"
                                                    data-toleave="${row.convertToLeave}"
                                                    data-tooffset="${row.convertToOffset}"
                                                    data-remarks="${row.remarks}"
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                >
                                                <i class="fa-solid fa-circle-check"></i> Edit
                                            </button>
                                        </nav>
                                    </label>`;
                    }
                    else {
                        button = `<label class="popup">
                                        <input type="checkbox">
                                        <div class="burger" tabindex="0">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="5" viewBox="0 0 20 5" fill="none">
                                                <path fill-rule="evenodd" clip-rule="evenodd" d="M17.5 5C16.837 5 16.2011 4.73661 15.7322 4.26777C15.2634 3.79893 15 3.16304 15 2.5C15 1.83696 15.2634 1.20107 15.7322 0.732234C16.2011 0.263393 16.837 0 17.5 0C18.163 0 18.7989 0.263393 19.2678 0.732234C19.7366 1.20107 20 1.83696 20 2.5C20 3.16304 19.7366 3.79893 19.2678 4.26777C18.7989 4.73661 18.163 5 17.5 5ZM2.5 5C1.83696 5 1.20107 4.73661 0.732233 4.26777C0.263392 3.79893 0 3.16304 0 2.5C0 1.83696 0.263392 1.20107 0.732233 0.732234C1.20107 0.263393 1.83696 0 2.5 0C3.16304 0 3.79893 0.263393 4.26777 0.732234C4.73661 1.20107 5 1.83696 5 2.5C5 3.16304 4.73661 3.79893 4.26777 4.26777C3.79893 4.73661 3.16304 5 2.5 5ZM10 5C9.33696 5 8.70107 4.73661 8.23223 4.26777C7.76339 3.79893 7.5 3.16304 7.5 2.5C7.5 1.83696 7.76339 1.20107 8.23223 0.732234C8.70107 0.263393 9.33696 0 10 0C10.663 0 11.2989 0.263393 11.7678 0.732234C12.2366 1.20107 12.5 1.83696 12.5 2.5C12.5 3.16304 12.2366 3.79893 11.7678 4.26777C11.2989 4.73661 10.663 5 10 5Z" fill="#205375"/>
                                            </svg>
                                        </div>
                                        <nav class="popup-window">
                                            <button class="tbl-decline btn btn-danger" id="aprroved-timein" title=""
                                                    
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                disabled>
                                                <i class="fa-solid fa-circle-xmark"></i> Delete
                                            </button>
                                            <button class="tbl-approve btn btn-info" id="add-timeout" title=""
                                                   
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                disabled>
                                                <i class="fa-solid fa-circle-check"></i> Edit
                                            </button>
                                        </nav>
                                    </label>`;
                    }

                    return button;
                }
            }
        ],
        //dom: 't',
        columnDefs: [

            {
                targets: [0], // Checkbox column
                orderable: false,
                searchable: false,
                width: "5%", // Adjust width
                "className": "text-center",
                render: function (data, type, row) {
                    if (row.statusName == 'PENDING') {
                        return '<input type="checkbox" class="ot-list-row-checkbox" value="' + row.id + '">';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                targets: [1], // OT-Number column
                width: "10%"
            },
            {
                targets: [2], // Date column (only sortable column)
                type: 'date',
                width: "10%"
            },
            {
                targets: [3, 4], // Start Time, End Time
                orderable: false,
                width: "10%"
            },
            {
                targets: [2, 5, 6], // Start Date, End Date
                orderable: false,
                width: "10%",
                render: function (data, type, row) {
                    if (data && (type === 'display' || type === 'filter')) {
                        let date = new Date(data);
                        return date.toLocaleDateString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric' });
                    }
                    return data;
                }
            },
            {
                targets: [7, 8], // Hours Filed, Hours Approved
                orderable: false,
                width: "8%"
            },
            {
                targets: [9], // Remarks column
                orderable: false,
                width: "15%"
            },
            {
                targets: [10, 11], // Convert To Leave Column
                orderable: false,
                width: "10%",
                createdCell: function (td, cellData, rowData, row, col) {
                    if (cellData === true) {
                        $(td).css('color', 'green').css('font-weight', 'bold');
                    } else {
                        $(td).css('color', 'orange').css('font-weight', 'bold');
                    }
                },
                render: function (data, type, row) {
                    var value = "";
                    if (data === true) {
                        value = "YES";
                    } else {
                        value = "NO";
                    }
                    return value;
                }

            },
            //{
            //    targets: [12], // Status Column
            //    orderable: false,
            //    width: "10%",
            //    createdCell: function (td, cellData, rowData, row, col) {
            //        if (cellData === "APPROVED") {
            //            $(td).css('color', 'green').css('font-weight', 'bold');
            //        } else if (cellData === "PENDING") {
            //            $(td).css('color', 'orange').css('font-weight', 'bold');
            //        } else if (cellData === "Rejected") {
            //            $(td).css('color', 'red').css('font-weight', 'bold');
            //        }
                    
            //    }
            //}
        ]
    };

    $('#overtime-table').on('page.dt', function () {

        var info = table.page.info();
        var url = new URL(window.location.href);
        url.searchParams.set('page01', (info.page + 1));
        window.history.replaceState(null, null, url);
    });

    var table = $(tableId).DataTable(dtProperties);
    $(tableId + '_filter input').attr('placeholder', 'Searching...');
    $(tableId + ' tbody').on('click', 'tr', function () {
        var data = table.row(this).data();

    });
}
function otfilingActionFunction() {
    actionotfiling.style.display = "flex";
    pencilotfiling.style.display = "none";
}
function OverTimeDOM() {
    $("#otsumbit").on("submit", function (event) {
        event.preventDefault();
        //alert('Hello World')
        //$.ajax({
        //    url: '/Overtime/SaveSalary',
        //    data: data,
        //    type: "POST",
        //    dataType: "json"
        //}).done(function (data) {
        //    var status = data.status;
        //    console.log(status);
        //    if (status === 'Entity already exists') {
        //        notifyMsg('Warning!', 'Salary already exists', 'yellow', 'fas fa-check');
        //    }
        //    else {
        //        notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
        //    }
        //    modal = document.getElementById('salary-modal');
        //    modal.style.display = "none";
        //    initializeDataTable();
        //});

    });



}

function viewRejectedOT() {
    var statusLabel = document.getElementById('StatusLabel');
    if (otStatusFilter == 0) {
        otStatusFilter = 1;
        showodcloading();
        setTimeout(function () {
            initializeOTDataTable();
            hideodcloading();
            statusLabel.innerHTML = "Pending"
        }, 1000); // Delay execution by 2 seconds (2000 milliseconds)


    }
    else {
        otStatusFilter = 0;
        showodcloading();
        setTimeout(function () {
            initializeOTDataTable();
            hideodcloading();
            statusLabel.innerHTML = "Rejected"
        }, 1000); // Delay execution by 2 seconds (2000 milliseconds)
    }
}
function downloadTemplate() {
    location.replace('../OverTime/DownloadHeader');
}
function POTExportFunction() {

    // Create the EmployeeIdFilter object with the necessary properties
    var empNo = "0";
    empNo = document.getElementById('selectUserOTPending').value;
    empNo = empNo === '' ? '0' : empNo;
    var sdate = document.getElementById('pot-datefrom').value;
    var edate = document.getElementById('pot-dateto').value;
    let data = {
        EmployeeNo: empNo,
        startDate: sdate,
        endDate: edate,
        status: otStatusFilter
    };
    $.ajax({
        url: '/Overtime/ExportPendingOvertimeList',
        data: {
            data: data,
        },
        type: "POST",
        datatype: "json",
        success: function (data) {
            //console.log(data);
            $("#selectUserPending").empty();
            $("#selectUserPending").append('<option value="" disabled selected>Select User</option>');
            $("#selectUserPending").append('<option value="0" >Select All</option>');
            // Use a Set to store distinct userIds
            const distinctUserIds = [...new Set(data.map(item => item.userId))];

            // Iterate over the distinct userIds
            distinctUserIds.forEach(userId => {
                // Find the user details corresponding to the current userId
                const user = data.find(item => item.userId === userId);

                // Append the user to the select element
                if (user) {
                    $("#selectUserPending").append('<option value="' + user.userId + '"><div style="display: block"><span>' + user.fname + " " + user.lname + " </span></div></option>");
                }
            });
        }
    });
}

function sendOTEmail() {
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

$("#ot-select-date").click(function () {
   
    document.getElementById('select-date-container').style.display = "block";

    pencilotfiling.style.display = "none";
});
$("#close-ot-select-date").click(function () {

    document.getElementById('select-date-container').style.display = "none";
    pencilotfiling.style.display = "block";
});
$("#action-navbar-otfiling").click(function () {

    actionotfiling.style.display = "none";
    pencilotfiling.style.display = "block";

});
document.addEventListener('keydown', function (event) {
    if (event.keyCode === 27) {
        actionotfiling.style.display = "none";
        pencilotfiling.style.display = "block";
        document.getElementById('ot-filing-container').style.display = "none";
        document.getElementById('select-date-container').style.display = "none";
    }
});
$("#ot-apply-date").click(function () {

    document.getElementById('select-date-container').style.display = "none";
    pencilotfiling.style.display = "block";
    FetchOvertimeList();

});
$('#ot-quick-select-date').on('change', function () {
    var value = document.getElementById('ot-quick-select-date').value;
    //alert(value)
    ottoDate = new Date();
    const formatOTToDate = (ottoDate) => {
        let year = ottoDate.getFullYear();
        let month = ottoDate.getMonth() + 1; // Month is zero-indexed, so add 1
        let day = ottoDate.getDate();
        // Ensure month and day are always two digits
        if (month < 10) month = '0' + month;
        if (day < 10) day = '0' + day;
        return `${year}-${month}-${day}`;
    };
    document.getElementById('ot-dateto').value = formatOTToDate(ottoDate);
    if (value == 1) {
        document.getElementById('ot-datefrom').value = formatOTToDate(ottoDate);
    }
    else if (value == 7) {
        var formatOTFromDate = (ottoDate) => {
            let year = ottoDate.getFullYear();
            let month = ottoDate.getMonth() + 1; // Month is zero-indexed, so add 1
            let day = ottoDate.getDate() - 7;
            // Ensure month and day are always two digits
            if (month < 10) month = '0' + month;
            if (day < 10) day = '0' + day;
            return `${year}-${month}-${day}`;
        };
        document.getElementById('ot-datefrom').value = formatOTFromDate(ottoDate);
    }
    else if (value == 30) {
        var formatOTFromDate = (ottoDate) => {
            let year = ottoDate.getFullYear();
            let month = ottoDate.getMonth(); // Month is zero-indexed, so add 1
            let day = ottoDate.getDate();
            // Ensure month and day are always two digits
            if (month < 10) month = '0' + month;
            if (day < 10) day = '0' + day;
            return `${year}-${month}-${day}`;
        };
        document.getElementById('ot-datefrom').value = formatOTFromDate(ottoDate);
    }
    else if (value == 12) {
        var formatOTFromDate = (ottoDate) => {
            let year = ottoDate.getFullYear() - 1;
            let month = ottoDate.getMonth() + 1; // Month is zero-indexed, so add 1
            let day = ottoDate.getDate();
            // Ensure month and day are always two digits
            if (month < 10) month = '0' + month;
            if (day < 10) day = '0' + day;
            return `${year}-${month}-${day}`;
        };
        document.getElementById('ot-datefrom').value = formatOTFromDate(ottoDate);
    }
});

$('#pot-quick-select-date').on('change', function () {
    var value = document.getElementById('pot-quick-select-date').value;
    //alert(value)
    ottoDate = new Date();
    const formatOTToDate = (ottoDate) => {
        let year = ottoDate.getFullYear();
        let month = ottoDate.getMonth() + 1; // Month is zero-indexed, so add 1
        let day = ottoDate.getDate();
        // Ensure month and day are always two digits
        if (month < 10) month = '0' + month;
        if (day < 10) day = '0' + day;
        return `${year}-${month}-${day}`;
    };
    document.getElementById('pot-dateto').value = formatOTToDate(ottoDate);
    if (value == 1) {
        document.getElementById('pot-datefrom').value = formatOTToDate(ottoDate);
    }
    else if (value == 7) {
        var formatOTFromDate = (ottoDate) => {
            let year = ottoDate.getFullYear();
            let month = ottoDate.getMonth() + 1; // Month is zero-indexed, so add 1
            let day = ottoDate.getDate() - 7;
            // Ensure month and day are always two digits
            if (month < 10) month = '0' + month;
            if (day < 10) day = '0' + day;
            return `${year}-${month}-${day}`;
        };
        document.getElementById('pot-datefrom').value = formatOTFromDate(ottoDate);
    }
    else if (value == 30) {
        var formatOTFromDate = (ottoDate) => {
            let year = ottoDate.getFullYear();
            let month = ottoDate.getMonth(); // Month is zero-indexed, so add 1
            let day = ottoDate.getDate();
            // Ensure month and day are always two digits
            if (month < 10) month = '0' + month;
            if (day < 10) day = '0' + day;
            return `${year}-${month}-${day}`;
        };
        document.getElementById('pot-datefrom').value = formatOTFromDate(ottoDate);
    }
    else if (value == 12) {
        var formatOTFromDate = (ottoDate) => {
            let year = ottoDate.getFullYear() - 1;
            let month = ottoDate.getMonth() + 1; // Month is zero-indexed, so add 1
            let day = ottoDate.getDate();
            // Ensure month and day are always two digits
            if (month < 10) month = '0' + month;
            if (day < 10) day = '0' + day;
            return `${year}-${month}-${day}`;
        };
        document.getElementById('pot-datefrom').value = formatOTFromDate(ottoDate);
    }
});

$("#pot-select-date").click(function () {

    document.getElementById('pot-select-date-container').style.display = "block";

    //pencilotfiling.style.display = "none";
});
$("#close-pot-select-date").click(function () {

    document.getElementById('pot-select-date-container').style.display = "none";
});

var otFilterIdentifier = 0;
$("#pot-show-filter").click(function () {
    if (otFilterIdentifier == 0) {
        document.getElementById('pot-filter').style.display = "block";
        otFilterIdentifier = 1;
    }
    else {
        document.getElementById('pot-filter').style.display = "none";
        otFilterIdentifier = 0;

    }
    
    //pencilotfiling.style.display = "block";
});
            