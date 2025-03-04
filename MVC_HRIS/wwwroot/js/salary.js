function salaryModalClose() {

    modal = document.getElementById('salary-modal');
    modal.style.display = "none";
}
function salaryModalOpen() {
    document.getElementById('salaryid').value = 0;
    document.getElementById('salarytype').value = "";
    document.getElementById('salarydescription').value = "";
    document.getElementById('salaryrate').value = "";
    document.getElementById('salarycreatedby').value = defaultCreatedBy;
    modal = document.getElementById('salary-modal');
    modal.style.display = "flex";
}


function salaryDOM() {

    $('#salary-table').on('click', '.tbl-edit', function () {
        document.getElementById('salaryid').value = $(this).data('id');
        document.getElementById('salarytype').value = $(this).data('salarytype');
        document.getElementById('salarydescription').value = $(this).data('description');
        document.getElementById('salaryrate').value = $(this).data('rate');;
        document.getElementById('salarycreatedby').value = $(this).data('createdby');


        modal = document.getElementById('salary-modal');
        modal.style.display = "flex";
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
            modal = document.getElementById('salary-modal');
            modal.style.display = "none";
            initializeDataTable();
        });

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
        initializeDataTable();
    });
}