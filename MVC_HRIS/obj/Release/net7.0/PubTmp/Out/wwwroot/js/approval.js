function initializeTimlogsDataTable() {
    var tableId = '#pending-timelogs-table';
    var lastSelectedRow = null;
    var img = "/image/emptypic.png";
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    var user = $('#selectUserPending').val() ? $('#selectUserPending').val() : 0;
    const data = {
        UserId: user
    };
    // console.log(data);
    var dtProperties = {
        ajax: {
            url: '/TimeLogs/GetPedingTimelogsList',
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

                // Compute total rendered hours after data is loaded
                //computeTotalRenderedHours();
            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        columns: [
            {
                "title": "Profile",
                "data": "id",
                "render": function (data, type, row) {
                    var images = row['filePath'] == null ? img : row['filePath'];
                    //var images = img;
                    var fullname = row.fname + " " + row.lname;
                    var btn = `<div  style="display:flex; gap: 10px; align-items: center;">
                                            <div class="data-img">
                                                <img src='${images}' width="100%" />
                                            </div>
                                            <div style="align-items: center;">
                                                <h6 style="text-align: left; margin: 0; font-size: 16px;">${fullname}</h6>
                                                <p style="text-align: left; margin: 0; font-size: 12px;">${row.employeeID}</p>
                                            </div>
                                        </div>`;
                    return btn;
                }
            },
            // {
            //     "title": "Employee ID #",
            //     "data": "employeeID"
            // },
            //{
            //    "title": "Username",
            //    "data": "username"
            //},
            {
                "title": "Task",
                "data": "task"
            },
            {
                "title": "Task Decription",
                "data": "remarks"
            },
            {
                "title": "Time In",
                "data": "timeIn",
                "render": function (data, type, row) {
                    // var timeout = new Date(data).toLocaleTimeString('en-US');
                    var timein = new Date(data).toLocaleString('en-US');
                    timein = timein.replace(',', '').replaceAll('/', '-');
                    return timein;
                }
            },
            {
                "title": "Time Out",
                "data": "timeOut",
                "render": function (data, type, row) {
                    if (data == '') {
                        var noValue = "";
                        return noValue;
                    }
                    else {
                        // var timeout = new Date(data).toLocaleTimeString('en-US');
                        var timeout = new Date(data).toLocaleString('en-US');
                        timeout = timeout.replace(',', '').replaceAll('/', '-');
                        return timeout;
                    }

                }
            },
            {
                "title": "Total Rendered Hours",
                "data": "renderedHours"
            },
            {
                "title": "Date",
                "data": "date",
                "render": function (data) {
                    const parts = data.split(' ');
                    const part = parts[0].split('/');
                    //console.log(part);
                    if (part.length === 3) {
                        // Convert to `YYYY-MM-DD`
                        const formattedDate = `${part[1]}-${part[0]}-${part[2]}`;
                        return formattedDate;
                    }
                    return data;
                },
                type: "date" // Ensures proper sorting by date
            },
            {
                "title": "Status",
                "data": "statusName"
            },
            {
                "title": "Action",
                "data": "id",
                "render": function (data, type, row) {
                    var images = row['filePath'] == null ? img : row['filePath'];
                    var status = row.statusId;
                    var task = row.taskId;
                    var button = "";
                    if (row.statusId == '0') {
                        button = `<div class="action">
                                                    <button class="tbl-decline btn btn-danger" id="aprroved-timein" title="Delete"
                                                            data-id="${data}"
                                                            data-status="${row.statusId}"
                                                            data-task="${row.taskId}"
                                                            data-date="${row.date}"
                                                            data-timein="${row.timeIn}"
                                                            data-timeout="${row.timeOut}"
                                                            data-remarks="${row.remarks}"
                                                            data-userid="${row.userId}"
                                                        >
                                                        <i class="fa-solid fa-circle-xmark"></i> Decline
                                                    </button>
                                                    <button class="tbl-approve btn btn-success" id="add-timeout" title="Time Out"
                                                            data-id="${data}"
                                                            data-status="${row.statusId}"
                                                            data-task="${row.taskId}"
                                                            data-date="${row.date}"
                                                            data-timein="${row.timeIn}"
                                                            data-timeout="${row.timeOut}"
                                                            data-remarks="${row.remarks}"
                                                            data-userid="${row.userId}"
                                                        >
                                                        <i class="fa-solid fa-circle-check"></i> Approve
                                                    </button>
                                                </div>`;
                    }
                    return button;
                }
            }
        ]
        , responsive: true
        // , columnDefs:  columnDefsConfig
        , columnDefs: [
            { targets: 1, className: 'left-align' },
            { responsivePriority: 10010, targets: 6 },
            { responsivePriority: 10010, targets: 7 },
            { responsivePriority: 10010, targets: 8 },
            { responsivePriority: 10008, targets: 0 },
            { targets: 2, className: 'none' },
            { targets: 3, className: 'none' },
            { targets: 4, className: 'none' },
            { targets: 5, className: 'none' },
            { "type": "date", "targets": 0 },
            { width: '25%', targets: 0 },
            { width: '5%', targets: 8 }
        ],
        order: [[0, 'desc']] // Sort the second column (index 1) by descending order
    };

    var table = $(tableId).DataTable(dtProperties);

    // Attach computeTotalRenderedHours to the search event
    $(tableId + '_filter input').on('keyup', function () {
        computeTotalRenderedHours();
    });

    $('#time-table').on('page.dt', function () {
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
        // console.log(data);

    });
  
  
}

function timelogsTableMOD() {
    $('#selectUserPending').on('change', function () {

        initializeTimlogsDataTable();
    });
}