function salaryModalClose() {

    //modal = document.getElementById('salary-modal');
    //modal.style.display = "none";

    salarymodal.style.display = "none";
    actionsalary.style.display = "none";
    pencilsalary.style.display = "flex";
}
function salaryModalOpen() {
    document.getElementById('salaryid').value = 0;
    document.getElementById('salarytype').value = "";
    document.getElementById('salarydescription').value = "";
    document.getElementById('salaryrate').value = "";
    document.getElementById('salarycreatedby').value = defaultCreatedBy;
//    modal = document.getElementById('salary-modal');
//    modal.style.display = "flex";
    salarymodal.style.display = "flex";
    actionsalary.style.display = "none";
    pencilsalary.style.display = "none";
}
function salaryActionFunction() {

    //payrollmodal.style.display = "none";
    actionsalary.style.display = "flex";
    pencilsalary.style.display = "none";
}
function salaryDOM() {

    $('#salary-table').on('click', '.tbl-edit', function () {
        document.getElementById('salaryid').value = $(this).data('id');
        document.getElementById('salarytype').value = $(this).data('salarytype');
        document.getElementById('salarydescription').value = $(this).data('description');
        document.getElementById('salaryrate').value = $(this).data('rate');;
        document.getElementById('salarycreatedby').value = $(this).data('createdby');


        //modal = document.getElementById('salary-modal');
        //modal.style.display = "flex";

        salarymodal.style.display = "flex";
        actionsalary.style.display = "none";
        pencilsalary.style.display = "none";
    });
    $('#salary-table').on('click', '.tbl-delete', function () {
        var salaryid = $(this).data('id');

        localStorage.setItem('salaryid', salaryid);

        deletesalarymodal();
        $("#alertmodal").modal('show');
    });
    $("#add-salary-form").on("submit", function (event) {
        event.preventDefault();
        var salaryid = document.getElementById('salaryid').value;
        var salarytype = document.getElementById('salarytype').value;
        var salarydescription = document.getElementById('salarydescription').value;
        var salaryrate = document.getElementById('salaryrate').value;
        var salarycreatedby = document.getElementById('salarycreatedby').value;


        var data = {};
        data.id = salaryid;
        data.salaryType = salarytype;
        data.description = salarydescription;
        data.rate = salaryrate;
        data.deleteFlag = 0;
        data.createdBy = salarycreatedby;
        $.ajax({
            url: '/Salary/SaveSalary',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            var status = data.status;
            console.log(status);
            if (status === 'Entity already exists') {
                notifyMsg('Warning!', 'Salary already exists', 'yellow', 'fas fa-check');
            }
            else {
                notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
            }
            salaryModalClose();
            initializeSalaryDataTable();
        });

    });
    $('#salary-table').on('click', '.checkAllsalary', function () {

        var checkAll = document.getElementById("checkAllsalary");

        if (checkAll.checked == true) {
            var checkboxes = document.querySelectorAll('.salary-row-checkbox');
            for (var checkbox of checkboxes) {
                checkbox.checked = this.checked;
            }
        }
        else {
            var checkboxes = document.querySelectorAll('.salary-row-checkbox');
            for (var checkbox of checkboxes) {
                checkbox.checked = false;
            }
        }
    });
}
function delete_item_salary() {
    //alert("Payroll Deleted");

    var salaryid = localStorage.getItem('salaryid');

    var data = {};
    data.id = salaryid;
    data.deleteFlag = 1;
    //console.log(data);
    $.ajax({
        url: '/Salary/SaveSalary',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        var status = data.status;
        //console.log(status);
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        $("#alertmodal").modal('hide');
        initializeSalaryDataTable();
    });
}
function MultiDeleteSalary() {
    var checkboxes = document.querySelectorAll('.salary-row-checkbox');
    var checkedCheckboxes = Array.from(checkboxes).filter(checkbox => checkbox.checked);


    actionsalary.style.display = "none";
    pencilsalary.style.display = "flex";
    if (checkedCheckboxes.length == 0) {
        notifyMsg('Warning!', 'Select Salary First', 'yellow', 'fas fa-check');
    }
    else {

        for (var checkbox of checkedCheckboxes) {
            var value = checkbox.value;

            var data = {};
            data.id = value;
            data.deleteFlag = 1;
            
            //console.log(data);
            $.ajax({
                url: '/Salary/SaveSalary',
                data: data,
                type: "POST",
                dataType: "json"
            }).done(function (data) {
                var status = data.status;
                //console.log(status);
                //notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
                //$("#alertmodal").modal('hide');
                //initializeSalaryDataTable();
            });

        }

        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        window.location.reload();
    }
}
function initializeSalaryDataTable() {
    var tableId = '#salary-table';
    var lastSelectedRow = null;
    // Check if DataTable is already initialized
    if ($.fn.DataTable.isDataTable(tableId)) {
        // Destroy the existing DataTable instance
        $(tableId).DataTable().clear().destroy();
    }
    var dtProperties = {
        ajax: {
            url: '/Salary/GetSalaryTypeList',
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
        }, lengthChange: false,
        dom: 'frtip',
        responsive: true,
        pagingType: "simple_numbers",
        language: {
            searchPlaceholder: "Type to search...",
            search: ""
        },
        columns: [
            { "title": "<input type='checkbox' id='checkAllsalary' class='checkAllsalary'>", "data": null, "orderable": false },
            {
                "title": "Salary Type",
                "data": "salaryType"
            },
            {
                "title": "Description",
                "data": "description"
            },
            {
                "title": "Rate",
                "data": "rate"
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
                                        <button class="tbl-delete btn btn-danger act-btn" id="" title="Delete"
                                                data-id="${data}"
                                                data-salaryType="${row.salaryType}"
                                                data-description="${row.description}"
                                                data-rate="${row.rate}"
                                                data-createdby="${row.createdBy}"
                                                data-deleteflag="${row.deleteFlag}"
                                            >
                                            <i class="fa-solid fa-trash"></i> delete
                                        </button>
                                            <button class="tbl-edit btn btn-info act-btn" id="" title="Time Out"
                                                data-id="${data}"
                                                data-salaryType="${row.salaryType}"
                                                data-description="${row.description}"
                                                data-rate="${row.rate}"
                                                data-createdby="${row.createdBy}"
                                                data-deleteflag="${row.deleteFlag}"
                                            >
                                            <i class="fa-solid fa-pen-to-square"></i> edit
                                        </button>
                                    </nav>
                                </label>`;
                    return button;
                }
            }
        ],
        order: [[0, 'desc']], // Sort the second column (index 1) by descending order
        columnDefs: [
            {
                targets: [0], // Checkbox column
                searchable: false,
                width: "5%", // Adjust width
                "className": "text-center",
                render: function (data, type, row) {
                    return '<input type="checkbox" id="" class="salary-row-checkbox" value="' + row.id + '">';
                },
                orderable: false,
            },
            {
                targets: 0,
                orderable: false,
            },
            {
                width: '25%', targets: 1,
                orderable: false,
            },
            {
                targets: 2,
                orderable: false,
            },
            {
                targets: 3,
                orderable: false,
            },
            {
                width: '25%', targets: 4,
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
                targets: 5,
                orderable: false,
            }, 
        ]
    };

    var table = $(tableId).DataTable(dtProperties);

    $('#salary-table').on('page.dt', function () {
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