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
            },
            {
                "title": "Action",
                "data": "id", "orderable": false,
                "render": function (data, type, row) {
                    var button = '';
                    if (row.statusName == 'PENDING') {
                        button = `<a class="editot" style="cursor: pointer" 
                                    data-id="${data}"
                                    data-sdate="${row.startDate}" 
                                    data-edate="${row.endDate}" 
                                    data-dfiled="${row.daysFiled}" 
                                    data-leavetype="${row.leaveTypeId}" 
                                    data-reason="${row.reason}" 
                                id="editot"><svg  xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--dark)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 14.66V20a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h5.34"></path><polygon points="18 2 22 6 12 16 8 16 8 12 18 2"></polygon></svg></a>`;
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
