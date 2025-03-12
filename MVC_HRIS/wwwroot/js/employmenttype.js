
function fetchemploymentoption() {

    $.ajax({
        url: '/Dashboard/GetEmploymentTypeList',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        console.log(data)
        $("#emp_type").empty();
        $("#emp_type").append('<option value="" disabled selected>Select Employment Type</option>');
        for (var i = 0; i < data.length; i++) {
            $("#emp_type").append('<option value="' + data[i].id + '">' + data[i].description + "</option>");
        }
    });
}

function FetcTotalRenderedHoursList(datefrom, dateto, usertype) {

    //$.ajax({
    //    url: '/Dashboard/TotalRenderedHoursList',
    //    data: {
    //        data: data,
    //    },
    //    type: "POST",
    //    datatype: "json",
    //    success: function (data) {
    //        console.log(data)

    //    }
    //});

    var tableId = '#userhour-table';
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
    let data = {
        Usertype: usertype,
        datefrom: datefrom,
        dateto: dateto,
    };
    console.log(data);
    var dtProperties = {

        ajax: {
            url: '/Dashboard/TotalRenderedHoursList',
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
                console.log('table1', _currentPage);
                table.page(_currentPage - 1).draw('page');
            },
            error: function (err) {
                alert(err.responseText);
            }
        },
        "columns": [

            {
                "title": "UserID",
                "data": "userId",
            },

            {
                "title": "Fullname",
                "data": "fullname"
            },
            {
                "title": "Username",
                "data": "username"
            },
            {
                "title": "Approved",
                "data": "approvedHours"
            },
            {
                "title": "Breaks",
                "data": "breakHours"
            },
            {
                "title": "Pending",
                "data": "pendingHours"
            },
            {
                "title": "Total Hours",
                "data": "totalRenderedHours"
            }
        ],
        pageLength: 5,
        order: [[0, 'desc']], // Sort the second column (index 1) by descending order
        columnDefs: [
            {
                targets: 0,
                type: 'date' // Ensure DataTables recognizes this column as date type
            }
        ]
    };

    $('#userhour-table').on('page.dt', function () {

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

function FetchUserTypeCountModel() {

    $.ajax({
        url: '/Dashboard/EmployeeTypeCount',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        console.log(data)
        $("#proj_based").text(data[0].userCount);
        $("#part_time").text(data[1].userCount);
        $("#full_time").text(data[2].userCount);
        $("#hour_based").text(data[3].userCount);
        $("#total_employee").text(data[4].userCount);
    });
}