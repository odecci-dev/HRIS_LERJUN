function FetchLeaveRequestList() {

    var tableId = '#leave-table';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
    var sdate = document.getElementById('lr-datefrom').value;
    var edate = document.getElementById('lr-dateto').value;
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
            url: '/Leave/GetLeaveRequestList',
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
            { "title": "<input type='checkbox' id='checkAllLRList' class='checkAllLRList'>", "data": null, "orderable": false },
            {
                "title": "LR-Number",
                "data": "leaveRequestNo", "orderable": false
            },
            {
                "title": "Employee No",
                "data": "employeeNo", "orderable": false
            },

            {
                "title": "Date",
                "data": "date", "orderable": true
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
                "title": "Days Filed",
                "data": "daysFiled", "orderable": false
            },
            {
                "title": "Reason",
                "data": "reason", "orderable": false
            }
            ,
            {
                "title": "Status",
                "data": "status", "orderable": false,
                "render": function (data, type, row) {
                    var badge = "";
                    if (data == 5) {
                        badge = "<span class='bg-success p-1 px-3 text-light' style='border-radius: 15px;'>Approved</span>";
                    }
                    else if (data == 1004) {
                        badge = "<span class='bg-warning p-1 px-3 text-light' style='border-radius: 15px;'>Pending</span>";
                    }
                    else if (data == 1005) {
                        badge = "<span class='bg-danger p-1 px-3 text-light' style='border-radius: 15px;'>Declined</span>";
                    }

                    return badge;
                }
            },
            {
                "title": "Action",
                "data": "id", "orderable": false,
                "render": function (data, type, row) {
                    var button = "";
                    if (row.status == 1004) {
                        button = `<a class="editlr" style="cursor: pointer" 
                                    data-id="${data}"
                                    data-sdate="${row.startDate}" 
                                    data-edate="${row.endDate}" 
                                    data-dfiled="${row.daysFiled}" 
                                    data-leavetype="${row.leaveTypeId}" 
                                    data-reason="${row.reason}" 
                                id="editlr"><svg  xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--dark)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 14.66V20a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h5.34"></path><polygon points="18 2 22 6 12 16 8 16 8 12 18 2"></polygon></svg></a>`;
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

                    if (row.status == 1004) {
                        return '<input type="checkbox" class="lr-list-row-checkbox" value="' + row.id + '">';
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
                targets: [2], // OT-Number column
                width: "10%"
            },
            {
                targets: [3, 4, 5], // Start Date, End Date
                orderable: false,
                type: 'date',
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
                targets: [6], // OT-Number column
                width: "10%"
            },
            {
                targets: [7], // OT-Number column
                width: "10%"
            },
            {
                targets: [8], // Hours Filed, Hours Approved
                orderable: false,
                width: "8%"
            },
        ]
    };

    $('#leave-table').on('page.dt', function () {

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

function LeaveDOM() {
    $('#leave-table').on('click', '.editlr', function () {
        //loadModal('/Leave/LeaveFiling', '#defaultmodal', '<i class="fa-solid fa-business-time"></i> New Leave ', 'l', false);
        //alert('Hello');
    });
    //$(".editlr").on("click", function () {
    //    //loadModal('/Leave/LeaveFiling', '#defaultmodal', '<i class="fa-solid fa-business-time"></i> New Leave ', 'l', false);
    //    alert('Hello');
    //});
}

function viewRejectedLR() {
    var statusLabel = document.getElementById('plrStatusLabel');
    if (plrStatusFilter == 0) {
        plrStatusFilter = 1;
        showodcloading();
        setTimeout(function () {
            initializeLeaveDataTable();
            hideodcloading();
            statusLabel.innerHTML = "Pending"
        }, 1000); // Delay execution by 2 seconds (2000 milliseconds)
    }
    else {
        plrStatusFilter = 0;
        showodcloading();
        setTimeout(function () {
            initializeLeaveDataTable();
            hideodcloading();
            statusLabel.innerHTML = "Rejected"
        }, 1000); // Delay execution by 2 seconds (2000 milliseconds)
    }
}
function downloadLeaveTemplate() {
    location.replace('../Leave/DownloadHeader');
}

$('#plr-datefrom').on('change', function () {
    initializeLeaveDataTable();
});
$('#plr-dateto').on('change', function () {
    initializeLeaveDataTable();
});