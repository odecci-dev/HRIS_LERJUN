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
    console.log(data);
    var dtProperties = {
        responsive: true, // Enable responsive behavior
        scrollX: true,    // Enable horizontal scrolling if needed
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
            serverSide: true,
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
        "columns": [
            { "title": "<input type='checkbox' id='checkAll'>", "data": null, "orderable": false },
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
                "data": "hoursApproved", "orderable": false
            },
            {
                "title": "Remarks",
                "data": "remarks", "orderable": false
            }
            ,
            {
                "title": "Convert To Leave",
                "data": "convertToLeave", "orderable": false
            }
            ,
            {
                "title": "Convert To Offset",
                "data": "convertToOffset", "orderable": false
            }
            ,
            {
                "title": "Status",
                "data": "statusName", "orderable": false
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
                    return '<input type="checkbox" class="row-checkbox" value="' + row.id + '">';
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
            {
                targets: [12], // Status Column
                orderable: false,
                width: "10%",
                createdCell: function (td, cellData, rowData, row, col) {
                    if (cellData === "APPROVED") {
                        $(td).css('color', 'green').css('font-weight', 'bold');
                    } else if (cellData === "PENDING") {
                        $(td).css('color', 'orange').css('font-weight', 'bold');
                    } else if (cellData === "Rejected") {
                        $(td).css('color', 'red').css('font-weight', 'bold');
                    }
                }
            }
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

function OverTimeDOM() {
    $("#otsumbit").on("submit", function (event) {
        event.preventDefault();
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

document.addEventListener("DOMContentLoaded", function () {
    const potmonthSelect = document.getElementById("pot-monthSelect");
    const potcurrentYear = new Date().getFullYear();
    for (let potmonth = 0; potmonth < 12; potmonth++) {
        const potmonthName = new Date(potcurrentYear, potmonth).toLocaleString('default', { month: 'long' });
        const potoption = document.createElement("option");
        potoption.value = `${potcurrentYear}-${String(potmonth + 1).padStart(2, '0')}`;
        potoption.text = `${potmonthName} ${potcurrentYear}`;
        potmonthSelect.appendChild(potoption);
    }
    // Set default to current month
    potmonthSelect.value = `${potcurrentYear}-${String(new Date().getMonth() + 1).padStart(2, '0')}`;
    setCutOffDatesPOT();

    //const otmonthSelect = document.getElementById("ot-monthSelect");
    //const otcurrentYear = new Date().getFullYear();
    //for (let otmonth = 0; otmonth < 12; otmonth++) {
    //    const otmonthName = new Date(otcurrentYear, otmonth).toLocaleString('default', { month: 'long' });
    //    const otoption = document.createElement("option");
    //    otoption.value = `${otcurrentYear}-${String(otmonth + 1).padStart(2, '0')}`;
    //    otoption.text = `${otmonthName} ${otcurrentYear}`;
    //    otmonthSelect.appendChild(otoption);
    //}
    //otmonthSelect.value = `${otcurrentYear}-${String(new Date().getMonth() + 1).padStart(2, '0')}`;
    //setCutOffDatesPOT();
});
function setCutOffDatesPOT() {
    const potselectedMonth = document.getElementById("pot-monthSelect").value;
    const potCuttOff = document.getElementById("potCuttOff").value;
    const [year, month] = potselectedMonth.split('-').map(Number);
    let fromDate, toDate;
    //console.log(month);
    if (potCuttOff == 0) {
        fromDate = new Date(year, month - 2, 26);
        toDate = new Date(year, month - 1, 10);
    } else if (potCuttOff == 1) {
        fromDate = new Date(year, month - 1, 11);
        toDate = new Date(year, month - 1, 25);
    }

    // Format as YYYY-MM-DD
    const formatFromDate = (fromDate) => {
        let year = fromDate.getFullYear();
        let month = fromDate.getMonth() + 1; // Month is zero-indexed, so add 1
        let day = fromDate.getDate();
        // Ensure month and day are always two digits
        if (month < 10) month = '0' + month;
        if (day < 10) day = '0' + day;
        return `${year}-${month}-${day}`;
    };
    const formatToDate = (toDate) => {
        let year = toDate.getFullYear();
        let month = toDate.getMonth() + 1; // Month is zero-indexed, so add 1
        let day = toDate.getDate();
        // Ensure month and day are always two digits
        if (month < 10) month = '0' + month;
        if (day < 10) day = '0' + day;
        return `${year}-${month}-${day}`;
    };
    document.getElementById('pot-datefrom').value = formatFromDate(fromDate);
    document.getElementById('pot-dateto').value = formatToDate(toDate);

    //const otselectedMonth = document.getElementById("ot-monthSelect").value;
    //const otCuttOff = document.getElementById("otCuttOff").value;
    //const [otyear, otmonth] = otselectedMonth.split('-').map(Number);
    //// Adjust to weekday if falls on weekend
    //fromDate = stladjustToWeekday(fromDate);
    //toDate = stladjustToWeekday(toDate);
    //if (otCuttOff == 0) {
    //    otfromDate = new Date(otyear, otmonth - 2, 26);
    //    ottoDate = new Date(otyear, otmonth - 1, 10);
    //} else if (otCuttOff == 1) {
    //    otfromDate = new Date(otyear, otmonth - 1, 11);
    //    ottoDate = new Date(otyear, otmonth - 1, 25);
    //}
    //// Adjust to weekday if falls on weekend
    //otfromDate = stladjustToWeekday(otfromDate);
    //ottoDate = stladjustToWeekday(ottoDate);
    //const formatOTFromDate = (otfromDate) => {
    //    let year = otfromDate.getFullYear();
    //    let month = otfromDate.getMonth() + 1; // Month is zero-indexed, so add 1
    //    let day = otfromDate.getDate();
    //    // Ensure month and day are always two digits
    //    if (month < 10) month = '0' + month;
    //    if (day < 10) day = '0' + day;
    //    return `${year}-${month}-${day}`;
    //};
    //const formatOTToDate = (ottoDate) => {
    //    let year = ottoDate.getFullYear();
    //    let month = ottoDate.getMonth() + 1; // Month is zero-indexed, so add 1
    //    let day = ottoDate.getDate();
    //    // Ensure month and day are always two digits
    //    if (month < 10) month = '0' + month;
    //    if (day < 10) day = '0' + day;
    //    return `${year}-${month}-${day}`;
    //};
    //document.getElementById('ot-datefrom').value = formatOTFromDate(otfromDate);
    //document.getElementById('ot-dateto').value = formatOTToDate(ottoDate);
}
$('#potCuttOff').on('change', function () {
    setCutOffDatesPOT();
    initializeOTDataTable();
});
$('#pot-monthSelect').on('change', function () {
    setCutOffDatesPOT();
    initializeOTDataTable();
});

//$('#otCuttOff').on('change', function () {
//    setCutOffDatesPOT();
//    initializeLeaveDataTable();
//});
//$('#ot-monthSelect').on('change', function () {
//    setCutOffDatesPOT();
//    initializeLeaveDataTable();
//});
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
    alert("Hello World!");

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
