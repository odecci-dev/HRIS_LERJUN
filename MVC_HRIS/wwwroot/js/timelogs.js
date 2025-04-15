
var modalIdentifier = 0;
const selectedEmployeeFilter = []

async function timeLogs() {

    $("#add-time-logs-form").on("submit", function (event) {
        event.preventDefault();

        var mtlid = document.getElementById('mtlid').value;
        var mtldate = document.getElementById('mtldate').value;
        var mtltimein = document.getElementById('mtltimein').value;
        var mtltimeout = document.getElementById('mtltimeout').value;
        var manualtask = document.getElementById('manualtask').value;
        var mtlremarks = document.getElementById('mtlremarks').value;
        var mtlbreak = document.getElementById('mtlbreak').value; 

        var data = {};
        data.id = mtlid;
        data.userId = uid;
        data.date = mtldate;
        data.timeIn = mtltimein;
        data.timeOut = mtltimeout;
        data.renderedHours = ((new Date(mtltimeout) - new Date(mtltimein)) / 3600000) - mtlbreak;
        data.TaskId = manualtask;
        data.deleteFlag = 1;
        data.Remarks = mtlremarks;
        data.Identifier = "Manual";
        //console.log(data);
        $.ajax({
            url: '/TimeLogs/ManualLogs',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            //console.log(data);
            //notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
            //tlmodal.style.display = "none";
            if (mtlid == 0) {
                var notifmessage = "User " + uid + " wants to add a new manual time log.\nCheck the details below:\n" +
                    "User ID: " + uid +
                    "\nDate: " + mtldate +
                    "\nTime In: " + mtltimein +
                    "\nTime Out: " + mtltimeout +
                    "\nTask Description: " + mtlremarks;
            }
            else {
                var notifmessage = "User " + uid + " wants to update the time log.\nCheck the details below:\n" +
                    "User ID: " + uid +
                    "\nDate: " + mtldate +
                    "\nTime In: " + mtltimein +
                    "\nTime Out: " + mtltimeout +
                    "\nTask Description: " + mtlremarks;
            }

            var ndata = {};
            ndata.id = 0;
            ndata.userId = uid;
            ndata.notification = notifmessage;
            ndata.date = mtldate;
            ndata.statusId = 3;
            //console.log(ndata);
            $.ajax({
                url: '/TimeLogs/LogsNotification',
                data: ndata,
                type: "POST",
                dataType: "json"
            }).done(function (data) {
                //console.log(data);
                notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
                tlmodal.style.display = "none";
                initializeDataTable();
            });
            //initializeDataTable();
        });

    });
    $('#timeoutreason').on('change', function () {

        document.getElementById("add-timeout").disabled = false;
        var otherreason = document.getElementById('timeoutreasonholder');
        const textarea = document.getElementById('otherreason');
        if ($("#timeoutreason").val() == "2") {
            textarea.required
            otherreason.style.display = "block";
            textarea.setAttribute('required', 'required');
        }
        else {

            otherreason.style.display = "none";
            textarea.removeAttribute('required');
        }
    });
    $('#show-timeout-modal').on('click', function () {
        var otherreason = document.getElementById('timeoutModal');
        otherreason.style.display = "flex";
        document.getElementById("add-timeout").disabled = true;
    });

}
function delete_item_timelogs() {

    //console.log(localStorage.getItem('id'));
    //console.log(localStorage.getItem('status'));
    //console.log(localStorage.getItem('task'));
    //console.log(localStorage.getItem('dateString'));
    //console.log(localStorage.getItem('timein'));
    //console.log(localStorage.getItem('timeout'));
    //console.log(localStorage.getItem('remarks'));
    //console.log(localStorage.getItem('userid'));


    var mtlid = localStorage.getItem('id');
    var mtldate = localStorage.getItem('dateString');
    var mtltimein = localStorage.getItem('timein');
    var mtltimeout = localStorage.getItem('timeout');
    var manualtask = localStorage.getItem('task');
    var mtlremarks = localStorage.getItem('remarks');

    var data = {};
    data.id = mtlid;
    data.userId = uid;
    data.date = mtldate;
    data.timeIn = mtltimein;
    data.timeOut = mtltimeout;
    data.renderedHours = (new Date(mtltimeout) - new Date(mtltimein)) / 3600000;
    data.TaskId = manualtask;
    data.deleteFlag = 0;
    data.Remarks = mtlremarks;
    //console.log(data);
    $.ajax({
        url: '/TimeLogs/ManualLogs',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        //console.log(data);
        $("#alertmodal").modal('hide');
        notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
        initializeDataTable();
    });
}


async function modalDom() {

    // Get today's date
    let today = new Date();

    // Format it to YYYY-MM-DD (required format for the input type="date")
    let formattedDate = today.toISOString().split('T')[0];

    // Set the min attribute of the input element
    document.getElementById("mtldate").setAttribute("max", formattedDate);

    // Add event listeners to update the min value of timeout whenever timein changes
    document.getElementById('mtltimein').addEventListener('input', updateTimeoutMin);

    // Add event listener to timeout input to validate whenever timeout changes
    document.getElementById('mtltimeout').addEventListener('input', validateTimeout);

    // Initial call to ensure that min is updated when the page loads
    updateTimeoutMin();

}

// Function to update the 'min' value of the timeout input
function updateTimeoutMin() {
    const timein = document.getElementById('mtltimein').value;
    const timeoutInput = document.getElementById('mtltimeout');

    if (timein) {
        // Set the min value of timeout to the current timein value
        timeoutInput.setAttribute('min', timein);
    } else {
        // If timein is not set, reset the min value of timeout
        timeoutInput.removeAttribute('min');
    }
}

// Function to handle the validation and enable/disable the submit button
function validateTimeout() {
    const timein = document.getElementById('mtltimein').value;
    const timeout = document.getElementById('mtltimeout').value;
    const errorMessage = document.getElementById('error-message');
    const submitBtn = document.getElementById('add-time-logs');

    if (timein && timeout) {
        const timeinDate = new Date(timein);
        const timeoutDate = new Date(timeout);

        if (timeoutDate < timeinDate) {
            // Show error message and disable the submit button
            errorMessage.style.display = 'block';
            submitBtn.disabled = true;
            submitBtn.style.background = 'gray';
        } else {
            // Hide error message and enable the submit button
            errorMessage.style.display = 'none';
            submitBtn.disabled = false;
            submitBtn.style.background = 'var(--dark)';
        }
    } else {
        // Hide error message and disable the submit button if the fields are incomplete
        errorMessage.style.display = 'none';
        submitBtn.disabled = true;
        submitBtn.style.background = 'gray';
    }
}
function initializeNotiDataTable() {
    var tableId = '#noti-table';
    var lastSelectedRow = null;
    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    const data = {
        //Usertype: '',
        //UserId: user,
        //datefrom: $('#datefrom').val(),
        //dateto: $('#dateto').val(),
        //Department: depart
        StatusID: null
    };
    var dtProperties = {
        ajax: {
            url: '/TimeLogs/GetNotificationList',
            type: "Post",
            data: data,
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
        columns: [
            {
                "title": "Notification",
                "data": "notification",
                "render": function (data, type, row) {
                    var result = ""
                    result = "<a style='white-space: pre-line;'>" + data + "</a>";
                    return result;

                }
            },
            {
                "title": "Date",
                "data": "date"
            },
            {
                "title": "Status",
                "data": "statusName",
                "render": function (data, type, row) {
                    var result = ""
                    if (data == 4) {
                        result = "Read";
                    }
                    else {
                        result = "Unread";
                    }
                    return result;

                }

            }
            //,
            //{
            //    "title": "Action",
            //    "data": "id",
            //    "render": function (data, type, row) {

            //        var button = `<div class="action">
            //                                        <button class="tbl-delete btn btn-danger" id="" title="Delete" 
            //                                            data-id="${data}"
            //                                            data-status="${row.status}"
            //                                            data-name="${row.name}"
            //                                            data-description="${row.description}"
            //                                            data-date="${row.dateCreated}"                                
            //                                            data-positionid="${row.positionId}"
            //                                        >
            //                                        <i class="fa-solid fa-trash"></i> delete
            //                                    </button>
            //                                        <button class="edit-table btn btn-info" id="" title="Time Out"
            //                                            data-id="${data}"
            //                                            data-status="${row.status}"
            //                                            data-name="${row.name}"
            //                                            data-description="${row.description}"
            //                                            data-date="${row.dateCreated}"                                
            //                                            data-positionid="${row.positionId}"
            //                                        >
            //                                            <i class="fa-solid fa-pen-to-square"></i> edit
            //                                        </button>
            //                            </div>`;
            //        return button;
            //    }
            //}
        ],
        order: [[1, 'desc']], // Sort the second column (index 1) by descending order
        columnDefs: [
            {
                targets: 1,
                type: 'date' // Ensure DataTables recognizes this column as date type
            },
            { className: 'dt-left', targets: [0,] },
            {

                width: '50%', targets: 0
            }
        ]
    };

    var table = $(tableId).DataTable(dtProperties);

    $('#time-table').on('page.dt', function () {
        var info = table.page.info();
        var url = new URL(window.location.href);
        url.searchParams.set('page01', (info.page + 1));
        window.history.replaceState(null, null, url);
    });

    $(tableId + '_filter input').attr('placeholder', 'Search Here');

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
function renderedHours() {
    var depart = $('#selectDap').val() ? $('#selectDap').val() : 0;
    var user = $('#selectUser').val() ? $('#selectUser').val() : 0;
    const data = {
        Usertype: '',
        UserId: user,
        datefrom: $('#datefrom').val(),
        dateto: $('#dateto').val(),
        Department: depart
    };

    //console.log(data);
    $.ajax({
        url: '/Timelogs/GetTimelogsTotalHours',
        data: {
            data: data
        },
        type: "Post",
        datatype: "json",
        success: function (data) {
            var total = 0;

            var hours = 0;
            for (var i = 0; i < data.length; i++) {
                //console.log(data[i].statusId);
                if (data[i].statusId == 1) {

                    hours = data[i].renderedHours;
                }
                else {
                    hours = 0;
                }
                total += parseFloat(hours);
            }
            //console.log(total.toFixed(2));
            //$('#totalamount').html("Total Rendered Hours: " + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + total.toFixed(2));
        }
    });
}

function floatButtonDOM() {
    $('#open-float-btn').click(function () {
        document.getElementById('open-float-btn').style.display = "none";
        document.getElementById('close-float-btn').style.display = "block";
        document.getElementById('time-btn-holder').style.display = "flex";

    });
    $('#close-float-btn').click(function () {
        document.getElementById('open-float-btn').style.display = "block";
        document.getElementById('close-float-btn').style.display = "none";
        document.getElementById('time-btn-holder').style.display = "none";
    });
    $(window).resize(function () {
        //initializeDataTable();
        if (screen.width > 790) {
            var res = document.querySelectorAll('.taskDesc');
            for (var i = 0; i < res.length; i++) {
                res[i].style.display = "none";
            }
            document.getElementById('time-btn-holder').style.display = "flex";
        }
        else {
            var res = document.querySelectorAll('.taskDesc');
            for (var i = 0; i < res.length; i++) {
                res[i].style.display = "flex";
            }
            document.getElementById('time-btn-holder').style.display = "none";

        }
    });
}
function initializeDataTable() {
    var tableId = '#time-table';
    var lastSelectedRow = null;
    var img = "/img/OPTION.webp";
    var columnDefsConfig = [];
    var tableuserid = localStorage.getItem('tableuserid');
    if (screen.width > 790) {
        columnDefsConfig = [{ width: '800px', targets: 0 }];
    } else {
        columnDefsConfig = [{ width: '500px', targets: 0 }];
    }
    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    const data = {
        Usertype: '',
        UserId: tableuserid,
        datefrom: $('#datefrom').val(),
        dateto: $('#dateto').val(),
        Department: ''
    };

    var dtProperties = {
        ajax: {
            url: '/TimeLogs/GetTimelogsList',
            type: "POST",
            data: {
                data: data
            },
            dataType: "json",
            processing: true,
            serverSide: true,
            complete: function (xhr) {
                // Compute total rendered hours after data is loaded
                computeTotalRenderedHoursEmp();
            },
            error: function (err) {
                setTimeout(function () {
                    alert(err.responseText);
                }, 2000); // Delay execution by 2 seconds (2000 milliseconds)
            }
        },
        columns: [
            // {
            //     "title": "Profile",
            //     "data": "id",
            //     "render": function (data, type, row) {
            //         var images = row['filePath'] == null ? img : row['filePath'];
            //         return `<div class="data-img"><img src='/img/${images}' width="100%" /></div>`;
            //     }
            // },
            // {
            //     "title": "Employee ID #",
            //     "data": "employeeID"
            // },
            {
                "title": "Date",
                "data": "date",
                "render": function (data) {
                    const parts = data.split(' ');
                    const part = parts[0].split('/');
                    //console.log(part);
                    if (part.length === 3) {
                        // Convert to `YYYY-MM-DD`
                        const formattedDate = `${part[0]}-${part[1]}-${part[2]}`;
                        return formattedDate;
                    }
                    return data;
                },
                type: "date" // Ensures proper sorting by date
            },
            // {
            //     "title": "Task",
            //     "data": "task",
            //     "render": function (data, type, row) {
            //         if (row.remarks != "") {
            //             var details = `<div> ${data} </div>
            //                            <div class="taskDesc" id='taskDesc${row.id}'>
            //                                 <h4> Task Description: </h4>
            //                                 ${row.remarks} 
            //                             </div>`;

            //         }
            //         else {
            //             var details = `<div> ${data} </div>`;
            //         }
            //         return details;
            //     }
            // },
            {
                "title": "Task",
                "data": "task", "orderable": false,
            },
            {
                "title": "Task Description",
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
                "data": "renderedHours", "orderable": false,
                "render": function (data, type, row) {
                    if (data != '' || data != null) {
                        return data;
                    }
                    else {
                        
                        return 0;
                    }

                }
            },
            {
                "title": "Status",
                "data": "statusName", "orderable": false,
                "render": function (data, type, row) {
                    var badge = "";
                    if (data == 'Approved') {
                        badge = "<span class='bg-success p-1 px-3 text-light' style='border-radius: 15px;'>Approved</span>";
                    }
                    else if (data == 'Pending') {
                        badge = "<span class='bg-warning p-1 px-3 text-light' style='border-radius: 15px;'>Pending</span>";
                    }
                    else if (data == 'Declined') {
                        badge = "<span class='bg-danger p-1 px-3 text-light' style='border-radius: 15px;'>Declined</span>";
                    }

                    return badge;
                }
            }
            ,
            {
                "title": "Action",
                "data": "id", "orderable": false,
                "render": function (data, type, row) {
                    var images = row['filePath'] == null ? img : row['filePath'];
                    var status = row.statusId;
                    var task = row.taskId;
                    if (status == 2 || status == 5) {
                        //var button = `<div class="action" style="justify-content: start !important">
                        //                                <button class="default-btn btn btn-danger" id="" title="Delete" 
                        //                                    data-id="${data}"
                        //                                    data-status="${row.statusId}"
                        //                                    data-task="${row.taskId}"
                        //                                    data-date="${row.date}"
                        //                                    data-timein="${row.timeIn}"
                        //                                    data-timeout="${row.timeOut}"
                        //                                    data-remarks="${row.remarks}"
                        //                                    data-userid="${row.userId}"
                        //                                    style="width: 100px; font-size:13px !important; padding: 5px 5px !important"
                        //                                disabled>
                        //                            <i class="fa-solid fa-trash"></i> Delete
                        //                        </button>
                        //                                <button class="default-btn btn btn-info" id="add-timeout" title="Time Out"
                        //                                    data-id="${data}"
                        //                                    data-status="${row.statusId}"
                        //                                    data-task="${row.taskId}"
                        //                                    data-date="${row.date}"
                        //                                    data-timein="${row.timeIn}"
                        //                                    data-timeout="${row.timeOut}"
                        //                                    data-remarks="${row.remarks}"
                        //                                    data-userid="${row.userId}"
                        //                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                        //                                        disabled>
                        //                                    <i class="fa-solid fa-pen-to-square"></i> edit
                        //                                </button>
                        //                    </div>`;
                        var button = `<label class="popup">
                                      <input type="checkbox">
                                      <div class="burger" tabindex="0">
                                        <span></span>
                                        <span></span>
                                      </div>
                                      <nav class="popup-window">
                                            <button class="default-btn btn btn-danger" id="" title="Delete"
                                                data-id="${data}"
                                                data-status="${row.statusId}"
                                                data-task="${row.taskId}"
                                                data-date="${row.date}"
                                                data-timein="${row.timeIn}"
                                                data-timeout="${row.timeOut}"
                                                data-remarks="${row.remarks}"
                                                data-userid="${row.userId}"
                                                style="width: 100px; font-size:13px !important; padding: 5px 5px !important"
                                            disabled>
                                                <i class="fa-solid fa-trash"></i> Delete
                                            </button></br>
                                            <button class="default-btn btn btn-info" id="add-timeout" title="Time Out"
                                                data-id="${data}"
                                                data-status="${row.statusId}"
                                                data-task="${row.taskId}"
                                                data-date="${row.date}"
                                                data-timein="${row.timeIn}"
                                                data-timeout="${row.timeOut}"
                                                data-remarks="${row.remarks}"
                                                data-userid="${row.userId}"
                                                style="width: 100px; font-size:13px; padding: 5px 5px"
                                                    disabled>
                                                <i class="fa-solid fa-pen-to-square"></i> edit
                                            </button>
                                      </nav>
                                    </label>`;
                    }
                    else {
                        //var button = `<div class="action" style="justify-content: start !important">
                        //                                <button class="tbl-delete btn btn-danger" id="add-timein" title="Delete" 
                        //                                    data-id="${data}"
                        //                                    data-status="${row.statusId}"
                        //                                    data-task="${row.taskId}"
                        //                                    data-date="${row.date}"
                        //                                    data-timein="${row.timeIn}"
                        //                                    data-timeout="${row.timeOut}"
                        //                                    data-remarks="${row.remarks}"
                        //                                    data-userid="${row.userId}"
                        //                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                        //                                >
                        //                                    <i class="fa-solid fa-trash"></i> Delete
                        //                                </button></br>
                        //                                <button class="tbl-edit btn btn-info" id="add-timeout" title="Time Out"
                        //                                    data-id="${data}"
                        //                                    data-status="${row.statusId}"
                        //                                    data-task="${row.taskId}"
                        //                                    data-date="${row.date}"
                        //                                    data-timein="${row.timeIn}"
                        //                                    data-timeout="${row.timeOut}"
                        //                                    data-remarks="${row.remarks}"
                        //                                    data-userid="${row.userId}"
                        //                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                        //                                        >
                        //                                    <i class="fa-solid fa-pen-to-square"></i> Edit
                        //                                </button>
                        //                    </div>`;
                        var button = `<label class="popup">
                                        <input type="checkbox">
                                        <div class="burger" tabindex="0">
                                        <span></span>
                                        <span></span>
                                        </div>
                                        <nav class="popup-window">
                                            <button class="tbl-delete btn btn-danger" id="add-timein" title="Delete"
                                                data-id="${data}"
                                                data-status="${row.statusId}"
                                                data-task="${row.taskId}"
                                                data-date="${row.date}"
                                                data-timein="${row.timeIn}"
                                                data-timeout="${row.timeOut}"
                                                data-remarks="${row.remarks}"
                                                data-userid="${row.userId}"
                                                style="width: 100px; font-size:13px; padding: 5px 5px"
                                            >
                                                <i class="fa-solid fa-trash"></i> Delete
                                            </button></br>
                                            <button class="tbl-edit btn btn-info" id="add-timeout" title="Time Out"
                                                data-id="${data}"
                                                data-status="${row.statusId}"
                                                data-task="${row.taskId}"
                                                data-date="${row.date}"
                                                data-timein="${row.timeIn}"
                                                data-timeout="${row.timeOut}"
                                                data-remarks="${row.remarks}"
                                                data-userid="${row.userId}"
                                                style="width: 100px; font-size:13px; padding: 5px 5px"
                                                    >
                                                <i class="fa-solid fa-pen-to-square"></i> Edit
                                            </button>
                                        </nav>
                                    </label>`;

                    }
                    return button;
                }
            }
        ], "dom": 'rtip'
        , responsive: true
        // , columnDefs:  columnDefsConfig
        , columnDefs: [
            { targets: 1, className: 'left-align' },
            { responsivePriority: 10010, targets: 6 },
            { responsivePriority: 10009, targets: 5 },
            { responsivePriority: 10008, targets: 0 },
            { responsivePriority: 10007, targets: 4 },
            { responsivePriority: 10007, targets: 3 },
            { targets: 2, className: 'none' },
            { targets: 3, className: 'none' },
            { targets: 4, className: 'none' },
            { "type": "date", "targets": 0 },
            {
                targets: [0],
                width: "10%",
                className: 'left-align'
            },
            {
                targets: [1],
                width: "15%",
                className: 'dt-align'
            },
            {
                targets: [5],
                width: "10%",
                className: 'dt-body-right'
            },
            {
                targets: [6],
                width: "5%",
            },
            {
                targets: [7],
                width: "5%",
            }
        ],
        order: [[0, 'desc']] // Sort the second column (index 1) by descending order

        // columnDefs: [
        //     {
        //         targets: 0,
        //         type: 'date' // Ensure DataTables recognizes this column as date type
        //     }
        // ]
    };

    var table = $(tableId).DataTable(dtProperties);
    // France Function
    $(tableId).on('mouseenter', 'tbody tr', function () {
        var data = table.row(this).data();

        if (data) {
            // Get the column index where the hover occurred

            var descId = "taskDesc" + data.id;
            var descTask = document.getElementById(descId);
            // console.log(data);
            var columnIndexs = $(this).index(); // Get the column index of the cell
            if (descTask) {
                descTask.style.display = "flex"; // Hide the popup
            }

            $(this).on('mouseenter', 'td', function (e) {
                var columnIndex = $(this).index(); // Get the column index of the cell
                if (columnIndex === 6) { // Column 6 has zero-based index 5
                    if (descTask) {
                        descTask.style.display = "none"; // Hide the popup
                    }
                }
                else {
                    if (descTask) {
                        descTask.style.display = "flex"; // Hide the popup
                    }
                }
            });
        }

    });
    $(tableId).on('mouseleave', 'tbody tr', function (event) {
        var data = table.row(this).data();
        if (data) {

            var descId = "taskDesc" + data.id;
            var descTask = document.getElementById(descId);
            // console.log(data);

            if (descTask) {
                descTask.style.display = "none";
            }
        }
    });
    $(tableId).on('mouseenter', 'tbody tr td .action', function () {
        var data = table.row(this).data();
        if (data) {
            var descId = "taskDesc" + data.id;
            var descTask = document.getElementById(descId);
            // console.log(data);

            if (descTask) {
                descTask.style.display = "none";
            }
        }
    });
    // Attach computeTotalRenderedHours to the search event
    $(tableId + '_filter input').on('keyup', function () {
        computeTotalRenderedHoursEmp();
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
    });

    // Function to compute total rendered hours
    function computeTotalRenderedHoursEmp() {
        var totalHours = 0;

        // Get all visible rows after searching
        var rows = table.rows({ search: 'applied' }).nodes(); // Use 'applied' to get visible rows

        // Iterate over each visible row and sum the rendered hours
        $(rows).each(function () {
            var renderedHours = parseFloat($(this).find('td:nth-child(6)').text()) || 0; // 7th column (0-based index)
            // totalHours += renderedHours;
            var status = $(this).find('td:nth-child(7)').text(); // 7th column (0-based index)
            if (status == 'Approved') {
                totalHours += renderedHours;
            }
            // console.log(renderedHours);
        });

        // Display the total hours with spaces
        $('#totalamountEmp').html("Total Rendered Hours: " + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + totalHours.toFixed(2));
    }


}
//Summary Functionsstl-datefrom
function defaultdate() {

    const firstDayOfMonth = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    const lastDayOfMonth = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);

    // Format the dates as YYYY-MM-DD
    const formatDate = (date) => {
        let year = date.getFullYear();
        let month = date.getMonth() + 1; // Month is zero-indexed, so add 1
        let day = date.getDate();

        // Ensure month and day are always two digits
        if (month < 10) month = '0' + month;
        if (day < 10) day = '0' + day;

        return `${year}-${month}-${day}`;
    };

    // Set the values for the date inputs
    document.getElementById('stl-datefrom').value = formatDate(firstDayOfMonth);
    document.getElementById('stl-dateto').value = formatDate(lastDayOfMonth);
}
function fetchSummaryTimlogsUsers() {
    var datefrom = document.getElementById('stl-datefrom').value;
    var dateto = document.getElementById('stl-dateto').value;
    var fullname = document.getElementById('stl-search-user').value;
    const data = {
        fullname: fullname
    };

    //console.log(data);
    $.ajax({
        url: '/TimeLogs/GetSummaryTimelogsListSelect',
        data: {
            data: data,
        },
        type: "POST",
        datatype: "json", async: false,
        success: function (data) {
            //console.log(data);
            $("#stlSelectUser").empty();
            $("#stlSelectUser").append('<option value="0" disabled selected>Select User</option>');
            $("#stlSelectUser").append('<option value="0" >Select All</option>');
            for (var i = 0; i < data.length; i++) {
                $("#stlSelectUser").append('<option value="' + data[i].userID + '"><input type="checkbox" id="selectedUsers' + data[i].userID +'" value="' + data[i].userID + '">' + data[i].fullname + "</option>");
               
            }
            var form = document.getElementById('selectusersOptions');
            form.innerHTML = "";

            for (var i = 0; i < data.length; i++) {
                var div = document.createElement("div");
                div.className = "formControl";
                div.innerHTML = `
                    <input class="stlSelectUserOption" type="checkbox" class="stlSelectUserOption" data-value="`+ data[i].userID + `" id="stlSelectUserOption` + data[i].userID+`" name="`+ data[i].fullname + `" value="` + data[i].userID + `" />
                    <a>`+ data[i].fullname + `</a>`;
                form.appendChild(div);

                
            }
        }
    });
}
function initializeSTLDataTable() {

    var tableId = '#summary-timelogs-table';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
    var datefrom = document.getElementById('stl-datefrom').value;
    var dateto = document.getElementById('stl-dateto').value;
    //var userid = document.getElementById('stlSelectUser').value;
    var checkedUser = document.querySelectorAll('input[class="stlSelectUserOption"]:checked');
    selectedEmail = Array.from(checkedUser).map(x => x.value);
    //console.log(selectedEmail);
    const data = {
        UserId: selectedEmployeeFilter,
        datefrom: datefrom,
        dateto: dateto,
        Department: "0",
    };
    //console.log(data);
    var dtProperties = {

        ajax: {
            url: '/TimeLogs/GetSummaryTimelogsListManager',
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
        responsive: true,
        "columns": [

            {
                "title": "Full Name",
                "data": "fullname",
                "orderable": true
            },
            {
                "title": "Unfiled Overtime",
                "data": "unfiledOvertime", "orderable": false
            },
            {
                "title": "Overtime",
                "data": "approvedOvertimeHours", "orderable": false
            },
            {
                "title": "Undertime",
                "data": "undertimeHours", "orderable": false
            },
            {
                "title": "Offset Time",
                "data": "approvedOffsetTimeHours", "orderable": false
            },
            {
                "title": "Total Hours By Schedule",
                "data": "totalHoursBySchedule", "orderable": false
            },
            {
                "title": "Total Hours",
                "data": "totalHours", "orderable": false
            },
            {
                "title": "Required Hours",
                "data": "requiredHours", "orderable": false
            },
            {
                "title": "Days Late",
                "data": "daysLate", "orderable": false
            },
            {
                "title": "Working Days",
                "data": "workingDays", "orderable": false
            }
        ], "dom": 'rtip',
        columnDefs: [


            {
                targets: [0], // Fullname
                width: "30%",
                className: 'left-align'
            },
            {
                targets: [1], // overtime
                width: "15%",
                className: 'dt-body-right'
            },
            {
                targets: [2], // Undertime
                width: "15%",
                className: 'dt-body-right'
            },
            {
                targets: [3], // Offset Time
                width: "15%",
                className: 'dt-body-right'
            },
            {
                targets: [4], // Total Hours
                width: "15%",
                className: 'dt-body-right'
            },
            {
                targets: [5], // Required Hours
                width: "10%",
                className: 'dt-body-right'
            },
            {
                targets: [6], // Days Late
                width: "10%",
                className: 'dt-body-right'
            },
            {
                targets: [7], // Working Days
                width: "10%",
                className: 'dt-body-right'
            }

        ]
    };

    $('#summary-timelogs-table').on('page.dt', function () {

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
function stlDOM() {
    document.getElementById("stlSelectUser").onchange = function () {
        initializeSTLDataTable();
    }
    document.addEventListener("DOMContentLoaded", function () {
        const stlmonthSelect = document.getElementById("stl-monthSelect");
        const stlcurrentYear = new Date().getFullYear();

        for (let stlmonth = 0; stlmonth < 12; stlmonth++) {
            const stlmonthName = new Date(stlcurrentYear, stlmonth).toLocaleString('default', { month: 'long' });
            const stloption = document.createElement("option");
            stloption.value = `${stlcurrentYear}-${String(stlmonth + 1).padStart(2, '0')}`;
            stloption.text = `${stlmonthName} ${stlcurrentYear}`;
            stlmonthSelect.appendChild(stloption);
        }

        // Set default to current month
        stlmonthSelect.value = `${stlcurrentYear}-${String(new Date().getMonth() + 1).padStart(2, '0')}`;
        setCutOffDatesStl();
    });
    document.getElementById("stl-monthSelect").onchange = function () {
        setCutOffDatesStl();
        initializeSTLDataTable();
    }
    document.getElementById("stlCuttOff").onchange = function () {
        setCutOffDatesStl();
        initializeSTLDataTable();
    }
   

    document.getElementById("selectusers-stl").onclick = function () {
        var selectUsersModal = document.getElementById('selectusersOptionsModal');
        var arrowUp = document.getElementById('select-user-arrow-up');
        var arrowDown = document.getElementById('select-user-arrow-down');
        if (modalIdentifier == 0) {
            modalIdentifier = 1;
            selectUsersModal.style.display = "block";
            arrowDown.style.display = "none";
            arrowUp.style.display = "inline-block";
            document.getElementById("stl-search-user").focus();
        }
        else {
            modalIdentifier = 0;

            selectUsersModal.style.display = "none";
            arrowDown.style.display = "inline-block";
            arrowUp.style.display = "none";
            
        }
    }
    document.getElementById("clear-user-filter").onclick = function () {
        document.querySelectorAll('.stlSelectUserOption').forEach(checkbox => {
            checkbox.checked = false;
            selectedEmployeeFilter.length = 0;
        });
        initializeSTLDataTable();
    }
    $('#selectusersOptions').on('click', '.stlSelectUserOption', function () {
        var value = $(this).data('value');
        document.getElementById("stl-search-user").focus();
        if ($(this).is(":checked")) {

            //alert(value);
            selectedEmployeeFilter.push(value);
            initializeSTLDataTable();
        }
        else {
            let index = selectedEmployeeFilter.indexOf(value);

            if (index !== -1) {
                selectedEmployeeFilter.splice(index, 1);

                initializeSTLDataTable();
            }
        }
    });

    document.getElementById("stl-search-user").addEventListener("input", function () {


        fetchSummaryTimlogsUsers();
        for (var i = 0; i < selectedEmployeeFilter.length; i++) {
            var checkboxId = 'stlSelectUserOption' + selectedEmployeeFilter[i];
            var checkbox = document.getElementById(checkboxId);
            if (checkbox) {  // Ensure the checkbox exists before modifying it
                checkbox.checked = true;
            } else {
                console.warn("Checkbox not found:", checkboxId); // Debugging info
            }
        }
    });
}
function stladjustToWeekday(date) {
    const day = date.getDay();
    if (day === 0) { // Sunday -> Move to Saturday
        date.setDate(date.getDate() - 1);
    } else if (day === 6) { // Saturday -> Move to Friday
        date.setDate(date.getDate() - 1);
    }
    return date;
}
function setCutOffDatesStl() {
    const stlselectedMonth = document.getElementById("stl-monthSelect").value;
    const stlCuttOff = document.getElementById("stlCuttOff").value;
    const [year, month] = stlselectedMonth.split('-').map(Number);

    //let fromDate, toDate;
    //console.log(month);
    if (stlCuttOff == 0) {
        fromDate = new Date(year, month - 2, 26);
        toDate = new Date(year, month - 1, 10);
    } else if (stlCuttOff == 1) {
        fromDate = new Date(year, month - 1, 11);
        toDate = new Date(year, month - 1, 25);
    }
    // Adjust to weekday if falls on weekend
    fromDate = stladjustToWeekday(fromDate);
    toDate = stladjustToWeekday(toDate);
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
    document.getElementById('stl-datefrom').value = formatFromDate(fromDate);
    document.getElementById('stl-dateto').value = formatToDate(toDate);
}
async function STLExportFunction() {
    var btnexport = document.getElementById('export-timelogs');

    var datefrom = document.getElementById('stl-datefrom').value;
    var dateto = document.getElementById('stl-dateto').value;
    var userid = document.getElementById('stlSelectUser').value;
    var checkedUser = document.querySelectorAll('input[class="stlSelectUserOption"]');
    selectedUser = Array.from(checkedUser).map(x => x.value);
    var depart = 0;
    window.location = "/TimeLogs/ExportSummaryTimelogsList?Usertype=" + "&UserId=" + selectedUser + "&datefrom=" + $('#stl-datefrom').val() + "&dateto=" + $('#stl-dateto').val() + "&Department=" + depart;

    /*window.location = "/TimeLogs/ExportSummaryTimelogsList?Usertype=0" + "&UserId=" + userid + "&datefrom=" + datefrom + "&dateto=" + dateto + "&Department=0";*/

}