//Timelogs Tab
function initializeTimlogsDataTable() {
    var tableId = '#pending-timelogs-table';
    var lastSelectedRow = null;
    var img = "/image/emptypic.png";
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    var user = $('#selectUserPending').val() ? $('#selectUserPending').val() : 0;
    var dateF = document.getElementById('ptl-datefrom').value;
    var datet = document.getElementById('ptl-dateto').value;
    const data = {
        UserId: user,
        datefrom: dateF,
        dateto: datet,
        status: tlStatusFilter
    };
     //console.log(data);
    var dtProperties = {
        ajax: {
            url: '/TimeLogs/GetPedingTimelogsList',
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
                // console.log('table1', _currentPage);
                table.page(_currentPage - 1).draw('page');

                // Compute total rendered hours after data is loaded
                //computeTotalRenderedHours();

                const data = xhr.responseJSON.data;
                // Populate Department filter
                const tldepartments = [...new Set(data.map(item => item.departmentName))];
                const $tldeptFilter = $('#tldepartmentFilter');
                if ($tldeptFilter.length && $tldeptFilter.children('option').length <= 1) {
                    tldepartments.forEach(dep => {
                        if (dep) {
                            $tldeptFilter.append(`<option value="${dep}">${dep}</option>`);
                        }
                    });
                }
                // Populate Position filter
                const tlpositions = [...new Set(data.map(item => item.position))];
                const $tlposFilter = $('#tlpositionFilter');
                if ($tlposFilter.length && $tlposFilter.children('option').length <= 1) {
                    tlpositions.forEach(pos => {
                        if (pos) {
                            $tlposFilter.append(`<option value="${pos}">${pos}</option>`);
                        }
                    });
                }

                // Populate Position LEvel filter
                const tlpositionLevels = [...new Set(data.map(item => item.positionLevel))];
                const $tlposlvlFilter = $('#tlpositionLevelFilter');
                if ($tlposlvlFilter.length && $tlposlvlFilter.children('option').length <= 1) {
                    tlpositionLevels.forEach(poslvl => {
                        if (poslvl) {
                            $tlposlvlFilter.append(`<option value="${poslvl}">${poslvl}</option>`);
                        }
                    });
                }
                // Populate Employee Type filter
                const tlemployeeTypes = [...new Set(data.map(item => item.employeeType))];
                const $tlemployeeTypeFilter = $('#tlemployeeType');
                if ($tlemployeeTypeFilter.length && $tlemployeeTypeFilter.children('option').length <= 1) {
                    tlemployeeTypes.forEach(employeeType => {
                        if (employeeType) {
                            $tlemployeeTypeFilter.append(`<option value="${employeeType}">${employeeType}</option>`);
                        }
                    });
                }
                // Populate Fullname filter
                const tlfullnames = [...new Set(data.map(item => item.fname+' '+item.lname))];
                const $tlfullnameFilter = $('#tlfullname');
                if ($tlfullnameFilter.length && $tlfullnameFilter.children('option').length <= 1) {
                    tlfullnames.forEach(fullname => {
                        if (fullname) {
                            $tlfullnameFilter.append(`<option value="${fullname}">${fullname}</option>`);
                        }
                    });
                }
            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        dom: 'Brtip',
        columns: [
            { "title": "<input type='checkbox' id='checkAllTL' class='checkAllTL'>", "data": null, "orderable": false },
            {
                "title": "Profile",
                "data": "id",
                //"render": function (data, type, row) {
                //    var images = row['filePath'] == null ? img : row['filePath'];
                //    //var images = img;
                //    var fullname = row.fname + " " + row.lname;
                //    var btn = `<div  style="display:flex; gap: 10px; align-items: center;">
                //                            <div class="data-img">
                //                                <img src='${images}' width="100%" />
                //                            </div>
                //                            <div style="align-items: center;">
                //                                <h6 style="text-align: left; margin: 0; font-size: 14px;">${fullname}</h6>
                //                                <p style="text-align: left; margin: 0; font-size: 12px;">${row.employeeID}</p>
                //                            </div>
                //                        </div>`;
                //    return btn;
                //}
                "render": function (data, type, row) {

                    // var images = "https://eportal.odeccisolutions.com/" + row['filePath'] == null ? img : "https://eportal.odeccisolutions.com/" + row['filePath'];
                    var images = "../../" + row['filePath'] == null ? img : "../../" + row['filePath'];
                    let profile = "";
                    var initial = row['fname'].charAt(0) + row['lname'].charAt(0);
                    var fullname = row.fname + " " + row.lname;
                    initial = initial.toUpperCase()
                    if (row['filePath'] == "" || row['filePath'] == null) {
                        profile = `<div  style="display:flex; gap: 10px; align-items: center;">
                                        <div class="data-img">
                                            <div class="initial"> ${initial} </div>
                                        </div>
                                        <div style="align-items: center;">
                                            <h6 style="text-align: left; margin: 0; font-size: 14px;">${fullname}</h6>
                                            <p style="text-align: left; margin: 0; font-size: 12px;">${row.employeeID}</p>
                                        </div>
                                    </div>
                                    `;
                    }
                    else {
                        profile = `<div  style="display:flex; gap: 10px; align-items: center;">
                                        <div class="data-img">
                                            <img src='${images}' width="100%" />
                                        </div>
                                        <div style="align-items: center;">
                                            <h6 style="text-align: left; margin: 0; font-size: 14px;">${fullname}</h6>
                                            <p style="text-align: left; margin: 0; font-size: 12px;">${row.employeeID}</p>
                                        </div>
                                    </div>
                                    `;
                    }
                    return profile;
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
                    var button = "";
                    if (row.statusId == '0') {
                        //button = `<div class="action" style="justify-content: start !important">
                        //                            <button class="tbl-decline btn btn-danger" id="aprroved-timein" title="Delete"
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
                        //                                <i class="fa-solid fa-circle-xmark"></i> Decline
                        //                            </button>
                        //                            <button class="tbl-approve btn btn-success" id="add-timeout" title="Time Out"
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
                        //                                <i class="fa-solid fa-circle-check"></i> Approve
                        //                            </button>
                        //                        </div>`;
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
                                                    data-status="${row.statusId}"
                                                    data-task="${row.taskId}"
                                                    data-date="${row.date}"
                                                    data-timein="${row.timeIn}"
                                                    data-timeout="${row.timeOut}"
                                                    data-remarks="${row.remarks}"
                                                    data-userid="${row.userId}"
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
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
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                >
                                                <i class="fa-solid fa-circle-check"></i> Approve
                                            </button>
                                        </nav>
                                    </label>`;
                    }
                    return button;
                }
            },
            {
                "title": "Department",
                "data": "departmentName",
                name: "department",
                visible: false,
                searchable: true
            },
            {
                "title": "Position",
                "data": "position",
                name: "position",
                visible: false,
                searchable: true
            },
            {
                "title": "PositionLevel",
                "data": "positionLevel",
                name: "positionLevel",
                visible: false,
                searchable: true
            },
            {
                "title": "EmployeeType",
                "data": "employeeType",
                name: "employeeType",
                visible: false,
                searchable: true
            },
            {
                "title": "Fullname",
                "data": "fname",
                name: "fullname",
                visible: false,
                searchable: true,
                "render": function (data,row) {
                    return data+" "+row.lname;
                }

            }
            //,
            //{
            //    "title": "Add Reason:",
            //    "data": "id",
            //    //"render": function (data, type, row) {
            //    //    var textfield = "";
            //    //    textfield = ``;

            //    //    return textfield;
            //    //},
            //}
        ],
        buttons: [
            {
                extend: 'excel',
                text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-file-arrow-down"></i> Export Excel File</span>',
                title: 'Leave Request List', // Set custom title in the file
                filename: 'Leave_Request_List', // Custom file name
                className: 'btn btn-info',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8] // Specify column indexes to export
                }
            },
            {
                text: 'Filters',
                action: function () { },
                init: function (api, node, config) {
                    let filterUI = "";
                    if (userType === 'Admin') {
                        filterUI = `
                            <div class="d-flex gap-2">
                                <select id="tldepartmentFilter" class="btn btn-info">
                                    <option value="">All Departments</option>
                                </select>
                                <select id="tlpositionFilter" class="btn btn-info">
                                    <option value="">All Positions</option>
                                </select>
                                <select id="tlpositionLevelFilter" class="btn btn-info">
                                    <option value="">All Positions Level</option>
                                </select>
                                <select id="tlemployeeType" class="btn btn-info">
                                    <option value="">All Employee Type</option>
                                </select>
                                <select id="tlfullname" class="btn btn-info">
                                    <option value="">All Users</option>
                                </select>
                                
                            </div>`;
                    }
                    else {

                        filterUI = `
                            <div class="d-flex gap-2">
                                <select id="lrfullname" class="btn btn-info">
                                    <option value="">All Users</option>
                                </select>
                            </div>`;
                    }
                    $(node).html(filterUI);
                }
            }
        ]
        , responsive: true
        // , columnDefs:  columnDefsConfig
        , columnDefs: [
            {
                targets: [0], // Checkbox column
                orderable: false,
                searchable: false,
                width: "5%", // Adjust width
                "className": "text-center",
                render: function (data, type, row) {
                    return '<input type="checkbox" class="tl-row-checkbox" value="' + row.id + '">';
                }
            },
            { targets: 2, className: 'left-align' },
            { responsivePriority: 10010, targets: 7 },
            { responsivePriority: 10010, targets: 8 },
            { responsivePriority: 10010, targets: 9 },
            { responsivePriority: 10008, targets: 1 },
            { targets: 3, className: 'none' },
            { targets: 4, className: 'none' },
            { targets: 5, className: 'none' },
            { targets: 6, className: 'none' },
            { "type": "date", "targets": 1 },
            { width: '25%', targets: 1 },
            {
                targets: [9],
                width: "5%", "className": "text-center", "targets": "7"
            },
            //{
            //    targets: 10,
            //    "visible": false
            //},
        ],
        order: [[1, 'desc']] // Sort the second column (index 1) by descending order
    };

    //var table = $(tableId).DataTable(dtProperties);
    tltable = $(tableId).DataTable(dtProperties);
    // Attach computeTotalRenderedHours to the search event
    $(tableId + '_filter input').on('keyup', function () {
        computeTotalRenderedHours();
    });

    $('#time-table').on('page.dt', function () {
        var info = tltable.page.info();
        var url = new URL(window.location.href);
        url.searchParams.set('page01', (info.page + 1));
        window.history.replaceState(null, null, url);
    });

    $(tableId + '_filter input').attr('placeholder', 'Search anything here...');

    $(tableId + ' tbody').on('click', 'tr', function () {
        var data = tltable.row(this).data();
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

    $(document).on('change', '#tldepartmentFilter', function () {
        const departmentval = $(this).val();
        if (departmentval == "") {
            table.column('department:name').search(departmentval).draw();
        }
        else {
            table.column('department:name').search('^' + departmentval + '$', true, false).draw();
        }
    });

    $(document).on('change', '#tlpositionFilter', function () {
        const positionval = $(this).val();
        if (positionval == "") {
            table.column('position:name').search(positionval).draw();
        }
        else {
            table.column('position:name').search('^' + positionval + '$', true, false).draw();
        }
    });
    $(document).on('change', '#tlpositionLevelFilter', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('positionLevel:name').search(val).draw();
        }
        else {
            table.column('positionLevel:name').search('^' + val + '$', true, false).draw();
        }
    });
    $(document).on('change', '#tlemployeeType', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('employeeType:name').search(val).draw();
        }
        else {
            table.column('employeeType:name').search('^' + val + '$', true, false).draw();
        }
    });
    $(document).on('change', '#tlfullname', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('fullname:name').search(val).draw();
        }
        else {
            table.column('fullname:name').search('^' + val + '$', true, false).draw();
        }
    });
}
function timelogsTableMOD() {
    $('#selectUserPending').on('change', function () {

        initializeTimlogsDataTable();
    });
    $('#pending-timelogs-table').on('click', '.tbl-decline', function () {
        var id = $(this).data('id');
        var action = 1;

        localStorage.setItem('id', id);
        localStorage.setItem('action', action);
        declinemodal();
        $("#alertmodal").modal('show');
    });
    $('#pending-timelogs-table').on('click', '.tbl-approve', function () {
        var id = $(this).data('id');
        var action = 0;

        localStorage.setItem('id', id);
        localStorage.setItem('action', action);
        approvemodal();
        $("#alertmodal").modal('show');
    });
}
function decline_item() {

    //console.log(localStorage.getItem('id'));
    //alert("Declined");
    var mtlid = localStorage.getItem('id');
    var action = localStorage.getItem('action');
    var data = {};
    data.id = mtlid;
    data.action = action;
    //console.log(data);
    $.ajax({
        url: '/TimeLogs/UpdateLogStatus',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        //console.log(data);
        //alert("Declined");
        $("#alertmodal").modal('hide');
        if (action == 1) {
            notifyMsg('Success!', 'Successfully Decline', 'green', 'fas fa-check');
        }
        else {
            notifyMsg('Success!', 'Successfully Approve', 'green', 'fas fa-check');
        }
        initializeTimlogsDataTable();
    });
}

function viewRejectedTL() {
    var statusLabel = document.getElementById('TLStatusLabel');
    if (tlStatusFilter == 0) {
        tlStatusFilter = 1;
        showodcloading();
        setTimeout(function () {
            initializeTimlogsDataTable();
            hideodcloading();
            statusLabel.innerHTML = "Pending"
        }, 1000); // Delay execution by 2 seconds (2000 milliseconds)


    }
    else {
        tlStatusFilter = 0;
        showodcloading();
        setTimeout(function () {
            initializeTimlogsDataTable();
            hideodcloading();
            statusLabel.innerHTML = "Rejected"
        }, 1000); // Delay execution by 2 seconds (2000 milliseconds)
    }
}
//OverTime Tab

var table; //  Declare early here
function initializeOTDataTable() {
    var tableId = '#pending-overtime-table';

    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
    var empNo = "0";
    //empNo = document.getElementById('selectUserOTPending').value;
    empNo = empNo === '' ? '0' : empNo;
    var sdate = document.getElementById('pot-datefrom').value;
    var edate = document.getElementById('pot-dateto').value;
    var managerId = 0;
    if (userType != 'Admin') {
        managerId = userId
    }
    let data = {
        EmployeeNo: "0",
        startDate: sdate,
        endDate: edate,
        status: otStatusFilter,
        ManagerId: managerId
    };
    //console.log(data);
    var dtProperties = {

        ajax: {
            url: '/OverTime/GetPendingOvertTimeList',
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

                //  Extract unique statusName values from the AJAX response
                const data = xhr.responseJSON.data;
                // Populate Department filter
                const departments = [...new Set(data.map(item => item.department))];
                const $deptFilter = $('#departmentFilter');
                if ($deptFilter.length && $deptFilter.children('option').length <= 1) {
                    departments.forEach(dep => {
                        if (dep) {
                            $deptFilter.append(`<option value="${dep}">${dep}</option>`);
                        }
                    });
                }
                // Populate Position filter
                const positions = [...new Set(data.map(item => item.position))];
                const $posFilter = $('#positionFilter');
                if ($posFilter.length && $posFilter.children('option').length <= 1) {
                    positions.forEach(pos => {
                        if (pos) {
                            $posFilter.append(`<option value="${pos}">${pos}</option>`);
                        }
                    });
                }

                // Populate Position LEvel filter
                const positionLevels = [...new Set(data.map(item => item.positionLevel))];
                const $poslvlFilter = $('#positionLevelFilter');
                if ($poslvlFilter.length && $poslvlFilter.children('option').length <= 1) {
                    positionLevels.forEach(poslvl => {
                        if (poslvl) {
                            $poslvlFilter.append(`<option value="${poslvl}">${poslvl}</option>`);
                        }
                    });
                }
                // Populate Employee Type filter
                const employeeTypes = [...new Set(data.map(item => item.employeeType))];
                const $employeeTypeFilter = $('#employeeType');
                if ($employeeTypeFilter.length && $employeeTypeFilter.children('option').length <= 1) {
                    employeeTypes.forEach(employeeType => {
                        if (employeeType) {
                            $employeeTypeFilter.append(`<option value="${employeeType}">${employeeType}</option>`);
                        }
                    });
                }
                // Populate Fullname filter
                const fullnames = [...new Set(data.map(item => item.fullname))];
                const $fullnameFilter = $('#fullname');
                if ($fullnameFilter.length && $fullnameFilter.children('option').length <= 1) {
                    fullnames.forEach(fullname => {
                        if (fullname) {
                            $fullnameFilter.append(`<option value="${fullname}">${fullname}</option>`);
                        }
                    });
                }


            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        responsive: true,
        dom: 'Brtip',
        "columns": [
            { "title": "<input type='checkbox' id='checkAllOT' class='checkAllOT'>", "data": null, "orderable": false },
            {
                "title": "OT-Number",
                "data": "otNo", "orderable": false,
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
                "title": "Reason:",
                "data": "remarks", "orderable": false
            }
            ,
            {
                "title": "Convert To Leave",
                "data": "convertToLeave", "orderable": false,
                "render": function (data, type, row) {
                    result = "";
                    if (data == 0) {
                        result = "No";

                    } else if (data == 1) {
                        result = "Yes";
                    }
                    return result;
                }
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

                    var button = "";

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
                                                    data-status="1005"
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                >
                                                <i class="fa-solid fa-circle-xmark"></i> Decline
                                            </button>
                                            <button class="tbl-approve btn btn-success" id="add-timeout" title="Time Out"
                                                    data-id="${data}"
                                                    data-status="5"          
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                >
                                                <i class="fa-solid fa-circle-check"></i> Approve
                                            </button>
                                    </nav>
                                </label>`;
                    return button;
                }
            },
            {
                title: "Department",
                data: "department",
                name: "department",
                visible: false,
                searchable: true
            },
            {
                title: "Position",
                data: "position",
                name: "position",
                visible: false,
                searchable: true
            },
            {
                title: "Position Level",
                data: "positionLevel",
                name: "positionLevel",
                visible: false,
                searchable: true
            },
            {
                title: "Eemployee Type",
                data: "employeeType",
                name: "employeeType",
                visible: false,
                searchable: true
            },
            {
                title: "Fullname",
                data: "fullname",
                name: "fullname",
                visible: false,
                searchable: true
            }
        ],
        columnDefs: [
            {
                targets: [0], // Checkbox column
                orderable: false,
                searchable: false,
                width: "5%", // Adjust width
                "className": "text-center",
                render: function (data, type, row) {
                    return '<input type="checkbox" id="" class="ot-row-checkbox" value="' + row.id + '">';
                }
            },
            {
                targets: [1], // OT-Number column
                width: "20%"
            },
            {
                targets: [2], // Date column (only sortable column)
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
                targets: [3, 4, 8], // Start Time, End Time
                orderable: false,
                className: 'none'
            },
            {
                targets: [5, 6], // Start Date, End Date
                orderable: false,
                className: 'none',
                render: function (data, type, row) {
                    if (data && (type === 'display' || type === 'filter')) {
                        let date = new Date(data);
                        return date.toLocaleDateString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric' });
                    }
                    return data;
                }
            },
            {
                targets: [7], // Hours Filed, Hours Approved
                orderable: false,
                width: "10%"
            },
            {
                targets: [8], // Remarks column
                orderable: false,
                width: "10%"
            },
            {
                targets: [9], // Convert To Leave Column
                orderable: false,
                width: "10%"

            },
            {
                targets: [10], // Status Column
                orderable: false,
                width: "10%",
                createdCell: function (td, cellData, rowData, row, col) {
                    if (cellData === "APPROVED") {
                        $(td).css('color', 'green').css('font-weight', 'bold');
                    } else if (cellData === "PENDING") {
                        $(td).css('color', 'orange').css('font-weight', 'bold');
                    } else if (cellData === "DECLINED") {
                        $(td).css('color', 'red').css('font-weight', 'bold');
                    }
                }
            },
            {
                targets: [11], // Convert To Leave Column
                orderable: false,
                width: "100px", "className": "text-center", "targets": "7"

            },
        ],
        //searching: false, // Disables the search input
        buttons: [
            {
                extend: 'pdf',
                text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-file-arrow-down"></i> Export PDF File</span>',
                title: 'Overtime List', // Set custom title in the file
                filename: 'Overtime_List', // Custom file name
                className: 'btn btn-info',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10] // Specify column indexes to export
                }
            },
            {
                extend: 'excel',
                text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-file-arrow-down"></i> Export Excel File</span>',
                title: 'Overtime List', // Set custom title in the file
                filename: 'Overtime_List', // Custom file name
                className: 'btn btn-info',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10] // Specify column indexes to export
                }
            },
            //{
            //    text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-circle-minus"></i> Decline</span>',
            //    className: 'btn btn-danger',
            //    action: function () {
            //        DeclineOvertime(); // Call your custom function
            //    }
            //},
            {
                text: 'Filters',
                action: function () { },
                init: function (api, node, config) {
                    let filterUI = "";
                    if (userType === 'Admin') {
                        filterUI = `
                            <div class="d-flex gap-2">
                                <select id="departmentFilter" class="btn btn-info">
                                    <option value="">All Departments</option>
                                </select>
                                <select id="positionFilter" class="btn btn-info">
                                    <option value="">All Positions</option>
                                </select>
                                <select id="positionLevelFilter" class="btn btn-info">
                                    <option value="">All Positions Level</option>
                                </select>
                                <select id="employeeType" class="btn btn-info">
                                    <option value="">All Employee Type</option>
                                </select>
                                <select id="fullname" class="btn btn-info">
                                    <option value="">All Users</option>
                                </select>
                            </div>`;
                    }
                    else {

                        filterUI = `
                            <div class="d-flex gap-2">
                                <select id="fullname" class="btn btn-info">
                                    <option value="">All Users</option>
                                </select>
                            </div>`;
                    }
                    $(node).html(filterUI);
                }
            }
        ],
    };

    $('#pending-overtime-table').on('page.dt', function () {

        var info = table.page.info();
        var url = new URL(window.location.href);
        url.searchParams.set('page01', (info.page + 1));
        window.history.replaceState(null, null, url);
    });

    table = $(tableId).DataTable(dtProperties);
    $(tableId + '_filter input').attr('placeholder', 'Searching...');
    $(tableId + ' tbody').on('click', 'tr', function () {
        var data = table.row(this).data();

    });


    $(document).on('change', '#departmentFilter', function () {
        const departmentval = $(this).val();
        if (departmentval == "") {
            table.column('department:name').search(departmentval).draw();
        }
        else {
            table.column('department:name').search('^' + departmentval + '$', true, false).draw();
        }

    });

    $(document).on('change', '#positionFilter', function () {
        const positionval = $(this).val();
        if (positionval == "") {
            table.column('position:name').search(positionval).draw();
        }
        else {
            table.column('position:name').search('^' + positionval + '$', true, false).draw();
        }
    });
    $(document).on('change', '#positionLevelFilter', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('positionLevel:name').search(val).draw();
        }
        else {
            table.column('positionLevel:name').search('^' + val + '$', true, false).draw();
        }
    });
    $(document).on('change', '#employeeType', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('employeeType:name').search(val).draw();
        }
        else {
            table.column('employeeType:name').search('^' + val + '$', true, false).draw();
        }
    });
    $(document).on('change', '#fullname', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('fullname:name').search(val).draw();
        }
        else {
            table.column('fullname:name').search('^' + val + '$', true, false).draw();
        }
    });
}
function OTTableMOD() {
    $('#selectUserOTPending').on('change', function () {

        initializeOTDataTable();
    });
    $('#pending-overtime-table').on('click', '.tbl-decline', function () {
        var id = $(this).data('id');
        var status = $(this).data('status');

        localStorage.setItem('id', id);
        localStorage.setItem('status', status);
        //declinemodal();
        //$("#alertmodal").modal('show');
        OTdeclinemodal();
        $("#alertmodal").modal('show');
    });
    $('#pending-overtime-table').on('click', '.tbl-approve', function () {
        var id = $(this).data('id');
        var status = $(this).data('status');

        localStorage.setItem('id', id);
        localStorage.setItem('status', status);
        //approvemodal();
        OTapprovemodal();
        $("#alertmodal").modal('show');
    });
    $('#pot-datefrom').on('change', function () {

        initializeOTDataTable();
    });
    $('#pot-dateto').on('change', function () {

        initializeOTDataTable();
    });

}
function OTapprovemodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="changeStatus_item()"/>';
    var cancelButton = '<input type="submit" value="NO" id="btn-cancel" class="btn-NO" data-dismiss="modal"/>';
    $('.input-container-button').empty();
    $('.img-header').empty();

    content.forEach(content => {
        content.style.setProperty("border-radius", "15px 15px 15px 15px", "important");
        content.style.setProperty("border-bottom", "7px #17a2b8 solid", "important");

    });
    modal_span.forEach(modal_span => {
        modal_span.style.setProperty("text-align", "center", "important");
        modal_span.style.setProperty("width", "100%", "important");
    });
    element.forEach(element => {
        element.style.setProperty("color", "white", "important");
        element.style.setProperty("background-color", "#17a2b8", "important");
        element.style.setProperty("border-radius", "15px 15px 0 0", "important");
        element.style.setProperty("text-align", "center", "important");
    });
    document.getElementById('message').textContent = 'Are you sure you want to aprroved this item?';
    document.getElementById('validation').textContent = 'Confirmation';
    $('.input-container-button').append(cancelButton);
    $('.input-container-button').append(delete_);
    $('.img-header').append('<img id="modalImage" src="/img/OPTION.webp" alt="Modal Image" />');
}
function OTdeclinemodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-decline-ot" class="btn-pay"  onclick="changeStatus_item()"/>';
    var cancelButton = '<input type="submit" value="NO" id="btn-cancel" class="btn-NO" data-dismiss="modal"/>';
    var declineReason = `<div class="input-holder" id="timeoutreasonholder">
                            <span class="label" > Specify Reason:</span >
                                <div class="input-container">
                                    <textarea id="declineReason" style="height: 66px; width: 100%"></textarea>
                                </div>
                        </div > `;

    $('.input-container-button').empty();
    $('.img-header').empty();
    content.forEach(content => {
        content.style.setProperty("border-radius", "15px 15px 15px 15px", "important");
        content.style.setProperty("border-bottom", "7px #d03a4b solid", "important");

    });
    modal_span.forEach(modal_span => {
        modal_span.style.setProperty("text-align", "center", "important");
        modal_span.style.setProperty("width", "100%", "important");
    });
    element.forEach(element => {
        element.style.setProperty("color", "white", "important");
        element.style.setProperty("background-color", "#d03a4b", "important");
        element.style.setProperty("border-radius", "15px 15px 0 0", "important");
        element.style.setProperty("text-align", "center", "important");
    });
    document.getElementById('message').textContent = 'Are you sure you want to decline this item?';
    document.getElementById('validation').textContent = 'Confirmation';
    $('.input-container-button').append(cancelButton);
    $('.input-container-button').append(delete_);
    $('.modal-body .input-holder').remove();
    $('.modal-body').append(declineReason);
    $('.img-header').append('<img id="modalImage" src="/img/OPTION.webp" alt="Modal Image" />');
}
function changeStatus_item() {
    var id = localStorage.getItem('id');
    var status = localStorage.getItem('status');
    var data = {};
    data.id = id;
    data.status = status;


    $.ajax({
        url: '/OverTime/updateStatus',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        var status = data.status;
        console.log(status);
        if (status === 'Entity already exists') {
            notifyMsg('Warning!', 'OT already exists', 'yellow', 'fas fa-check');
        }
        else {
            notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
        }

        $("#alertmodal").modal('hide');
        initializeOTDataTable();
    });
}

//Leave Tab
function initializeLeaveDataTable() {

    var tableId = '#pending-leave-table';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
    var empNo = "0";
    //empNo = document.getElementById('selectUserOTPending').value;
    empNo = empNo === '' ? '0' : empNo;
    var sdate = document.getElementById('plr-datefrom').value;
    var edate = document.getElementById('plr-dateto').value;
    var managerId = 0;
    if (userType != 'Admin') {
        managerId = userId
    }
    let data = {
        EmployeeNo: "0",
        startDate: sdate,
        endDate: edate,
        status: plrStatusFilter,
        ManagerId: managerId
    };
    //console.log(data);
    var dtProperties = {

        ajax: {
            url: '/Leave/GetPendingLeaveRequestList',
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

                const data = xhr.responseJSON.data;
                // Populate Department filter
                const lrdepartments = [...new Set(data.map(item => item.department))];
                const $lrdeptFilter = $('#lrdepartmentFilter');
                if ($lrdeptFilter.length && $lrdeptFilter.children('option').length <= 1) {
                    lrdepartments.forEach(dep => {
                        if (dep) {
                            $lrdeptFilter.append(`<option value="${dep}">${dep}</option>`);
                        }
                    });
                }
                // Populate Position filter
                const lrpositions = [...new Set(data.map(item => item.position))];
                const $lrposFilter = $('#lrpositionFilter');
                if ($lrposFilter.length && $lrposFilter.children('option').length <= 1) {
                    lrpositions.forEach(pos => {
                        if (pos) {
                            $lrposFilter.append(`<option value="${pos}">${pos}</option>`);
                        }
                    });
                }

                // Populate Position LEvel filter
                const lrpositionLevels = [...new Set(data.map(item => item.positionLevel))];
                const $lrposlvlFilter = $('#lrpositionLevelFilter');
                if ($lrposlvlFilter.length && $lrposlvlFilter.children('option').length <= 1) {
                    lrpositionLevels.forEach(poslvl => {
                        if (poslvl) {
                            $lrposlvlFilter.append(`<option value="${poslvl}">${poslvl}</option>`);
                        }
                    });
                }
                // Populate Employee Type filter
                const lremployeeTypes = [...new Set(data.map(item => item.employeeType))];
                const $lremployeeTypeFilter = $('#lremployeeType');
                if ($lremployeeTypeFilter.length && $lremployeeTypeFilter.children('option').length <= 1) {
                    lremployeeTypes.forEach(employeeType => {
                        if (employeeType) {
                            $lremployeeTypeFilter.append(`<option value="${employeeType}">${employeeType}</option>`);
                        }
                    });
                }
                // Populate Fullname filter
                const lrfullnames = [...new Set(data.map(item => item.fullname))];
                const $lrfullnameFilter = $('#lrfullname');
                if ($lrfullnameFilter.length && $lrfullnameFilter.children('option').length <= 1) {
                    lrfullnames.forEach(fullname => {
                        if (fullname) {
                            $lrfullnameFilter.append(`<option value="${fullname}">${fullname}</option>`);
                        }
                    });
                }

                //$('#lrcustomFilterButtons').html(`
                //    <button class="btn btn-warning" id="refresh-leave" title="Refresh" onclick="initializeLeaveDataTable()">
                //        <i class="fa-solid fa-arrows-rotate"></i> Refresh
                //    </button>
                //    <button class="btn btn-danger" id="decline-leave" title="Delete" onclick="rejectLeave()">
                //        <i class="fa-solid fa-circle-minus"></i> Decline
                //    </button>
                //    <button class="btn btn-success" id="approve-leave" title="Approve" onclick="approveLeave()">
                //        <i class="fa-solid fa-file-arrow-down"></i> Approve
                //    </button>

                //`);
            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        responsive: true,
        "columns": [

            { "title": "<input type='checkbox' id='checkAllLeave' class='checkAllLeave'>", "data": null, "orderable": false },
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
                "data": "statusName", "orderable": false

            },
            {
                "title": "Action",
                "data": "id",
                "render": function (data, type, row) {
                    var button = "";
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
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                >
                                                <i class="fa-solid fa-circle-xmark"></i> Decline
                                            </button>
                                            <button class="tbl-approve btn btn-success" id="add-timeout" title="Time Out"
                                                    data-id="${data}"
                                                    style="width: 100px; font-size:13px; padding: 5px 5px"
                                                >
                                                <i class="fa-solid fa-circle-check"></i> Approve
                                            </button>
                                        </nav>
                                    </label>`;

                    return button;
                }
            },
            {
                title: "Department",
                data: "department",
                name: "department",
                visible: false,
                searchable: true
            },
            {
                title: "Position",
                data: "position",
                name: "position",
                visible: false,
                searchable: true
            },
            {
                title: "Position Level",
                data: "positionLevel",
                name: "positionLevel",
                visible: false,
                searchable: true
            },
            {
                title: "Eemployee Type",
                data: "employeeType",
                name: "employeeType",
                visible: false,
                searchable: true
            },
            {
                "title": "Fullname",
                "data": "fullname",
                name: "fullname",
                visible: false,
                searchable: true

            }
        ],
        dom: 'Brtip',
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
        ],
        buttons: [
            {
                extend: 'excel',
                text: '<span style="color: white; font-weight: 400;"><i class="fa-solid fa-file-arrow-down"></i> Export Excel File</span>',
                title: 'Leave Request List', // Set custom title in the file
                filename: 'Leave_Request_List', // Custom file name
                className: 'btn btn-info',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8] // Specify column indexes to export
                }
            },
            {
                text: 'Filters',
                action: function () { },
                init: function (api, node, config) {
                    let filterUI = "";
                    if (userType === 'Admin') {
                        filterUI = `
                            <div class="d-flex gap-2">
                                <select id="lrdepartmentFilter" class="btn btn-info">
                                    <option value="">All Departments</option>
                                </select>
                                <select id="lrpositionFilter" class="btn btn-info">
                                    <option value="">All Positions</option>
                                </select>
                                <select id="lrpositionLevelFilter" class="btn btn-info">
                                    <option value="">All Positions Level</option>
                                </select>
                                <select id="lremployeeType" class="btn btn-info">
                                    <option value="">All Employee Type</option>
                                </select>
                                <select id="lrfullname" class="btn btn-info">
                                    <option value="">All Users</option>
                                </select>
                            </div>`;
                    }
                    else {

                        filterUI = `
                            <div class="d-flex gap-2">
                                <select id="lrfullname" class="btn btn-info">
                                    <option value="">All Users</option>
                                </select>
                            </div>`;
                    }
                    $(node).html(filterUI);
                }
            }
        ]
    };

    $('#pending-leave-table').on('page.dt', function () {

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


    $(document).on('change', '#lrdepartmentFilter', function () {
        const departmentval = $(this).val();
        if (departmentval == "") {
            table.column('department:name').search(departmentval).draw();
        }
        else {
            table.column('department:name').search('^' + departmentval + '$', true, false).draw();
        }
    });

    $(document).on('change', '#lrpositionFilter', function () {
        const positionval = $(this).val();
        if (positionval == "") {
            table.column('position:name').search(positionval).draw();
        }
        else {
            table.column('position:name').search('^' + positionval + '$', true, false).draw();
        }
    });
    $(document).on('change', '#lrpositionLevelFilter', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('positionLevel:name').search(val).draw();
        }
        else {
            table.column('positionLevel:name').search('^' + val + '$', true, false).draw();
        }
    });
    $(document).on('change', '#lremployeeType', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('employeeType:name').search(val).draw();
        }
        else {
            table.column('employeeType:name').search('^' + val + '$', true, false).draw();
        }
    });
    $(document).on('change', '#lrfullname', function () {
        const val = $(this).val();
        if (val == "") {
            table.column('fullname:name').search(val).draw();
        }
        else {
            table.column('fullname:name').search('^' + val + '$', true, false).draw();
        }
    });
}
function viewRejectedLR() {
    var statusLabel = document.getElementById('StatusLabel');
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
