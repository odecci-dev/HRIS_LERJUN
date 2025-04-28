function myFunction() {
    pmodal.style.display = "none";
    plmodal.style.display = "none";
    pencil.style.display = "block";
    pencilPosLvl.style.display = "block";
}
function myFunctionOpenModal() {
    document.getElementById('posid').value = "0";
    document.getElementById('posname').value = "";
    document.getElementById('desc').value = "";
    pmodal.style.display = "flex";
    actionPos.style.display = "none";
    pencil.style.display = "none";
}
function myFunctionOpenPositionLevelModal() {

    document.getElementById('poslevelid').value = "";
    document.getElementById('poslevel').value = "";
    document.getElementById('posleveldesc').value = "";
    plmodal.style.display = "flex";
    actionPosLvl.style.display = "none";
    pencilPosLvl.style.display = "none";
}
function PosActionFunction() {

    pmodal.style.display = "none";
    actionPos.style.display = "flex";
    pencil.style.display = "none";
}
async function position() {
    $("#submitposlevel").on("submit", function (event) {
        event.preventDefault();
        var poslevelid = document.getElementById('poslevelid').value;
        var poslevel = document.getElementById('poslevel').value;
        var posleveldesc = document.getElementById('posleveldesc').value;

        var data = {};
        data.id = poslevelid;
        data.level = poslevel;
        data.description = posleveldesc;
        console.log(data);
        $.ajax({
            url: '/Position/SavePositionLevel',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            //console.log(data);
            notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
            plmodal.style.display = "none";
            pencilPosLvl.style.display = "block";
            initializePositionLevelDataTable();
        });

    });
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
        //console.log(data);
        $.ajax({
            url: '/Position/SavePosition',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            console.log(data);
            notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
            pmodal.style.display = "none";
            pencil.style.display = "block";
            initializePositionDataTable();
        });

    });
    // Edit Time Logs
    $('#pos-table').on('click', '.edit-table', function () {
        var id = $(this).data('id');
        var date = $(this).data('date');
        var posname = $(this).data('name');
        var posdesc = $(this).data('description');

        document.getElementById('posid').value = id;
        document.getElementById('posdate').value = date;
        document.getElementById('posname').value = posname;
        document.getElementById('desc').value = posdesc;
        pmodal.style.display = "flex";
    });
    $('#poslevel-table').on('click', '.edit-table', function () {
        var id = $(this).data('id');
        var poslevel = $(this).data('level');
        var posleveldesc = $(this).data('description');

        document.getElementById('poslevelid').value = id;
        document.getElementById('poslevel').value = poslevel;
        document.getElementById('posleveldesc').value = posleveldesc;
        plmodal.style.display = "flex";
    });
    $('#pos-table').on('click', '.tbl-delete', function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        var description = $(this).data('name');
        localStorage.setItem('posid', id);
        localStorage.setItem('posname', name);
        localStorage.setItem('posdesc', description);

        deletemodal();
        $("#alertmodal").modal('show');
    });
    $('#pos-table').on('click', '.checkAllpos', function () {

        var checkAll = document.getElementById("checkAllpos");

        if (checkAll.checked == true) {
            var checkboxes = document.querySelectorAll('.pos-row-checkbox');
            for (var checkbox of checkboxes) {
                checkbox.checked = this.checked;
            }
        }
        else {
            var checkboxes = document.querySelectorAll('.pos-row-checkbox');
            for (var checkbox of checkboxes) {
                checkbox.checked = false;
            }
        }
    });
    $('#poslevel-table').on('click', '.tbl-delete', function () {
        var id = $(this).data('id');
        var poslevel = $(this).data('level');
        var posleveldesc = $(this).data('description');

        localStorage.setItem('poslevelid', id);
        localStorage.setItem('poslevelname', poslevel);
        localStorage.setItem('posleveldesc', posleveldesc);

        deleteposlevelmodal();
        $("#alertmodal").modal('show');
    });
    $('#poslevel-table').on('click', '.checkAllposlevel', function () {

        var checkAll = document.getElementById("checkAllposlevel");

        if (checkAll.checked == true) {
            var checkboxes = document.querySelectorAll('.poslevel-row-checkbox');
            for (var checkbox of checkboxes) {
                checkbox.checked = this.checked;
            }
        }
        else {
            var checkboxes = document.querySelectorAll('.poslevel-row-checkbox');
            for (var checkbox of checkboxes) {
                checkbox.checked = false;
            }
        }
    });
}
function MultiDeletePosLevel() {
    var checkboxes = document.querySelectorAll('.poslevel-row-checkbox');
    var checkedCheckboxes = Array.from(checkboxes).filter(checkbox => checkbox.checked);

    actionPosLvl.style.display = "none";
    pencilPosLvl.style.display = "block";
    if (checkedCheckboxes.length == 0) {
        notifyMsg('Warning!', 'Select Position First', 'yellow', 'fas fa-check');
    }
    else {

        for (var checkbox of checkedCheckboxes) {
            var value = checkbox.value;

            var data = {};
            data.id = value;
            data.level = "";
            data.description = "";
            //console.log(data);
            $.ajax({
                url: '/Position/SavePositionLevel',
                data: data,
                type: "POST",
                dataType: "json"
            }).done(function (data) {
                //console.log(data);
                notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
                $("#alertmodal").modal('hide');
                initializePositionLevelDataTable();
            });
        }
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');

    }
}
function MultiDeletePos() {
    var checkboxes = document.querySelectorAll('.pos-row-checkbox');
    var checkedCheckboxes = Array.from(checkboxes).filter(checkbox => checkbox.checked);

    actionPos.style.display = "none";
    pencil.style.display = "block";
    if (checkedCheckboxes.length == 0) {
        notifyMsg('Warning!', 'Select Position First', 'yellow', 'fas fa-check');
    }
    else {

        for (var checkbox of checkedCheckboxes) {
            var value = checkbox.value;
            //selectedRequest.push(value);
            //console.log(value);
            var data = {};
            data.id = value;
            //console.log(data);
            $.ajax({
                url: '/Position/DeletePosition',
                data: data,
                type: "POST",
                dataType: "json"
            }).done(function (data) {
                //console.log(data);

                $("#alertmodal").modal('hide');
                initializePositionDataTable();
            });
        }
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');

    }

}
function delete_item() {



    var posid = localStorage.getItem('posid');
    var posname = localStorage.getItem('posname');
    var posdesc = localStorage.getItem('posdesc');


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
function deleteposlevel_item() {

    var posid = localStorage.getItem('poslevelid');
    var data = {};
    data.id = posid;
    data.level = "";
    data.description = "";
    console.log(data);
    $.ajax({
        url: '/Position/SavePositionLevel',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        //console.log(data);
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        $("#alertmodal").modal('hide');
        initializePositionLevelDataTable();
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
function initializePositionLevelDataTable() {
    var tableId = '#poslevel-table';
    var lastSelectedRow = null;
    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    var dtProperties = {
        ajax: {
            url: '/Position/GetPositionLevelList',
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
            { "title": "<input type='checkbox' id='checkAllposlevel' class='checkAllposlevel'>", "data": null, "orderable": false },
            {
                "title": "Level",
                "data": "level"
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
                                            data-level="${row.level}"
                                            data-description="${row.description}"
                                        >
                                            <i class="fa-solid fa-pen-to-square"></i> edit
                                        </button>
                                        <button class="tbl-delete btn btn-danger act-btn" id="" title="Delete"
                                            data-id="${data}"
                                            data-level="${row.level}"
                                            data-description="${row.description}"
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
                    return '<input type="checkbox" id="" class="poslevel-row-checkbox" value="' + row.id + '">';
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

    $('#poslevel-table').on('page.dt', function () {
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
function PoslvlActionFunction() {

    //pmodal.style.display = "none";
    actionPosLvl.style.display = "flex";
    pencilPosLvl.style.display = "none";
}
function deleteposlevelmodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="deleteposlevel_item()"/>';
    var cancelButton = '<input type="submit" value="NO" id="btn-cancel" class="btn-NO" data-dismiss="modal"/>';
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
    document.getElementById('message').textContent = 'Are you sure you want to delete this item?';
    document.getElementById('validation').textContent = 'Confirmation';
    $('.input-container-button').append(cancelButton);
    $('.input-container-button').append(delete_);
    $('.img-header').append('<img id="modalImage" src="/img/OPTION.webp" alt="Modal Image" />');
}