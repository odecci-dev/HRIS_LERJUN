
function myFunction() {
    pmodal.style.display = "none";
}
function myFunctionOpenModal() {
    document.getElementById('posid').value = "0";
    document.getElementById('posname').value = "";
    document.getElementById('desc').value = "";
    pmodal.style.display = "flex";
    actionPos.style.display = "none";
    pencil.style.display = "block";
}
function multiDeleteFunction() {
    alert("Deleted!");
    actionPos.style.display = "none";
    pencil.style.display = "block";
}
function PosActionFunction() {

    pmodal.style.display = "none";
    actionPos.style.display = "flex";
    pencil.style.display = "none";
}
async function position() {

    $("#submitpos").on("submit", function (event) {
        event.preventDefault();
        var posid = document.getElementById('posid').value;
        var posdate = document.getElementById('posdate').value;
        var posname = document.getElementById('posname').value;
        var posdesc = document.getElementById('desc').value;
        //alert("Hi Position");
        //console.log(posid);
        //console.log(posname);
        //console.log(posdesc);

        var datetoday = new Date().toISOString();
        console.log(datetoday);
        var data = {};
        data.id = posid;
        data.name = posname;
        data.description = posdesc;
        data.deleteFlag = 0;
        if (posid == 0) {

            data.dateCreated = datetoday;
        }
        else {
            data.dateCreated = posdate;
        }
        data.positionId = 'POS-0';
        console.log(data);
        $.ajax({
            url: '/Position/SavePosition',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            console.log(data);
            notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
            pmodal.style.display = "none";
            initializePositionDataTable();
        });

    });
    // Edit Time Logs
    $('#pos-table').on('click', '.edit-table', function () {
        var id = $(this).data('id');
        var date = $(this).data('date');
        var posname = $(this).data('name');
        var posdesc = $(this).data('description');


        // Extract the date and time part from the string
        //let dateParts = new Date().split(" ")[0].split("/"); // Get "05/01/2025"
        //let day = dateParts[0];
        //let month = dateParts[1];
        //let year = dateParts[2];
        //// Format the Date object to YYYY-MM-DD
        //let formattedDate = year + '-' + month + '-' + day;
        document.getElementById('posid').value = id;
        document.getElementById('posdate').value = date;
        document.getElementById('posname').value = posname;
        document.getElementById('desc').value = posdesc;
        pmodal.style.display = "flex";
    });

    $('#pos-table').on('click', '.tbl-delete', function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        var description = $(this).data('name');

        //var status = $(this).data('status');
        //var task = $(this).data('task');
        //var dateString = $(this).data('date');
        //var timein = $(this).data('timein');
        //var timeout = $(this).data('timeout');
        //var remarks = $(this).data('remarks');
        //var userid = $(this).data('userid');
        localStorage.setItem('posid', id);
        localStorage.setItem('posname', name);
        localStorage.setItem('posdesc', description);
        //localStorage.setItem('status', status);
        //localStorage.setItem('task', task);
        //localStorage.setItem('dateString', dateString);
        //localStorage.setItem('timein', timein);
        //localStorage.setItem('timeout', timeout);
        //localStorage.setItem('remarks', remarks);
        //localStorage.setItem('userid', userid);

        deletemodal();
        $("#alertmodal").modal('show');
    });
    //$('#add-pos').on('click', function (event) {
    //    event.preventDefault();
    //    loadModal('/Position/PositionModal', '#defaultmodal', '<i class="fa-solid fa-business-time"></i> Add Position', 'l', false)
    //    // document.getElementById('timodal').style.display = "flex";
    //});
}

function delete_item() {



    var posid = localStorage.getItem('posid');
    var posname = localStorage.getItem('posname');
    var posdesc = localStorage.getItem('posdesc');


    //var mtldate = localStorage.getItem('dateString');
    //var mtltimein = localStorage.getItem('timein');
    //var mtltimeout = localStorage.getItem('timein');
    //var manualtask = localStorage.getItem('task');
    //var mtlremarks = localStorage.getItem('remarks');

    var data = {};
    data.id = posid;
    console.log(data);
    $.ajax({
        url: '/Position/DeletePosition',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        //console.log(data);
        notifyMsg('Success!', 'Successfully Deleted', 'red', 'fas fa-check');
        $("#alertmodal").modal('hide');
        initializePositionDataTable();
    });

}

function initializePositionDataTable() {
    var tableId = '#pos-table';
    var lastSelectedRow = null;
    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    var dtProperties = {
        ajax: {
            url: '/Position/GetPositionList',
            type: "GET",
            data: {

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
        lengthChange: false,
        dom: 'frtip',
        responsive: true,
        columns: [
            { "title": "<input type='checkbox' id='checkAllpos' class='checkAllpos'>", "data": null, "orderable": false },
            {
                "title": "Position",
                "data": "name"
            },
            {
                "title": "Description",
                "data": "description"
            },
            {
                "title": "Date Created",
                "data": "dateCreated"
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
                                        <button class="edit-table btn btn-info act-btn" id="" title="Time Out"
                                            data-id="${data}"
                                            data-status="${row.status}"
                                            data-name="${row.name}"
                                            data-description="${row.description}"
                                            data-date="${row.dateCreated}"
                                            data-positionid="${row.positionId}"
                                        >
                                            <i class="fa-solid fa-pen-to-square"></i> edit
                                        </button>
                                        <button class="tbl-delete btn btn-danger act-btn" id="" title="Delete"
                                                data-id="${data}"
                                                data-status="${row.status}"
                                                data-name="${row.name}"
                                                data-description="${row.description}"
                                                data-date="${row.dateCreated}"
                                                data-positionid="${row.positionId}"
                                            >
                                            <i class="fa-solid fa-trash"></i> delete
                                        </button>
                                    </nav>
                                </label>`;

                   
                    return button;
                }
            }
        ],
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
                    return '<input type="checkbox" id="" class="pos-row-checkbox" value="' + row.id + '">';
                },
                orderable: false,
            },
            {
                width: '25%', targets: 1,
                orderable: false,
            }, 
            {
                targets: 0,
                orderable: false,
            },
            {
                targets: 2,
                orderable: false,
            },
            {
                width: '25%', targets: 3,
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
                width: '10%', targets: 4,
                orderable: false,
            }
        ],
        //scrollY: '100vh',
        //scrollCollapse: true,
    };

    var table = $(tableId).DataTable(dtProperties);

    $('#pos-table').on('page.dt', function () {
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