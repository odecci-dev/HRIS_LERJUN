function departmentModalClose() {

    modal = document.getElementById('department-modal');
    modal.style.display = "none";
}
function departmentModalOpen() {
    document.getElementById('departmentid').value = 0;
    document.getElementById('departmentname').value = "";
    document.getElementById('departmentdescription').value = "";
    document.getElementById('departmenthead').value = "";
    document.getElementById('createdby').value = defaultCreatedBy;
    modal = document.getElementById('department-modal');
    modal.style.display = "flex";
}


function departmentDOM() {

    $('#dep-table').on('click', '.tbl-edit', function () {
        document.getElementById('departmentid').value = $(this).data('id');
        document.getElementById('departmentname').value = $(this).data('departmentname');
        document.getElementById('departmentdescription').value = $(this).data('description');
        document.getElementById('departmenthead').value = $(this).data('departmenthead');
        document.getElementById('createdby').value = $(this).data('createdby');


        modal = document.getElementById('department-modal');
        modal.style.display = "flex";
    });
    $('#dep-table').on('click', '.tbl-delete', function () {
        var deptid = $(this).data('id');
        var deptname = $(this).data('departmentname');
        var deptdescription = $(this).data('description');
        var depthead = $(this).data('departmenthead');
        var createdby = $(this).data('createdby');

        localStorage.setItem('deptid', deptid);
        localStorage.setItem('deptname', deptname);
        localStorage.setItem('deptdescription', deptdescription);
        localStorage.setItem('depthead', depthead);
        localStorage.setItem('createdby', createdby);

        deletedepartmentmodal();
        $("#alertmodal").modal('show');
    });

    $("#add-department-form").on("submit", function (event) {
        event.preventDefault();
        var deptid = document.getElementById('departmentid').value;
        var deptname = document.getElementById('departmentname').value;
        var deptdescription = document.getElementById('departmentdescription').value;
        var depthead = document.getElementById('departmenthead').value;
        var createdby = document.getElementById('createdby').value;

        var data = {};
        data.id = deptid;
        data.departmentName = deptname;
        data.description = deptdescription;
        data.deleteFlag = 0;
        data.departmentHead = depthead;
        data.createdBy = createdby;
        //console.log(data);
        $.ajax({
            url: '/Department/SaveDepartment',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            var status = data.status;
            console.log(status);
            if (status === 'Entity already exists') {
                notifyMsg('Warning!', 'Department already exists', 'yellow', 'fas fa-check');
            }
            else {
                notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
            }
            modal = document.getElementById('department-modal');
            modal.style.display = "none";
            initializeDataTable();
        });

    });
}

function delete_item_department() {
    //alert("Department Deleted");

    var deptid = localStorage.getItem('deptid');
    var deptname = localStorage.getItem('deptname');
    var deptdescription = localStorage.getItem('deptdescription');
    var depthead = localStorage.getItem('depthead');
    var createdby = localStorage.getItem('createdby');

    var data = {};
    data.id = deptid;
    data.deleteFlag = 1;
    console.log(data);
    $.ajax({
        url: '/Department/SaveDepartment',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        var status = data.status;
        console.log(status);
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        $("#alertmodal").modal('hide');
        initializeDataTable();
    });
}