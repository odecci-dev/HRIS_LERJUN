function FetchOvertimeList() {

    var tableId = '#overtime-table';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
    let data = {
        EmployeeNo: EmployeeID
    };
    //console.log(data);
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
                "title": "Status",
                "data": "statusName", "orderable": false
            }
        ],
        dom: 't',
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
                targets: [10], // Convert To Leave Column
                orderable: false,
                width: "10%",

            },
            {
                targets: [11], // Status Column
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