function payrollModalClose() {

    modal = document.getElementById('payroll-modal');
    modal.style.display = "none";
}
function payrollModalOpen() {
    document.getElementById('payrollid').value = 0;
    document.getElementById('payrolltype').value = "";
    document.getElementById('payrolldescription').value = "";
    document.getElementById('payrollcreatedby').value = defaultCreatedBy;
    modal = document.getElementById('payroll-modal');
    modal.style.display = "flex";
}


function payrollDOM() {

    $('#payroll-table').on('click', '.tbl-edit', function () {
        document.getElementById('payrollid').value = $(this).data('id');
        document.getElementById('payrolltype').value = $(this).data('payrolltype');
        document.getElementById('payrolldescription').value = $(this).data('description');
        document.getElementById('payrollcreatedby').value = $(this).data('createdby');


        modal = document.getElementById('payroll-modal');
        modal.style.display = "flex";
    });
    $('#payroll-table').on('click', '.tbl-delete', function () {
        var payrollid = $(this).data('id');
        var payrolltype = $(this).data('payrolltype');
        var payrolldescription = $(this).data('description');
        var payrollcreatedby = $(this).data('createdby');

        localStorage.setItem('payrollid', payrollid);
        localStorage.setItem('payrolltype', payrolltype);
        localStorage.setItem('payrolldescription', payrolldescription);
        localStorage.setItem('payrollcreatedby', payrollcreatedby);

        deletepayrollmodal();
        $("#alertmodal").modal('show');
    });
    $("#add-payroll-form").on("submit", function (event) {
        event.preventDefault();
        alert("Add");
        var payrollid = document.getElementById('payrollid').value;
        var payrolltype = document.getElementById('payrolltype').value;
        var payrolldescription = document.getElementById('payrolldescription').value;
        var payrollcreatedby = document.getElementById('payrollcreatedby').value;


        var data = {};
        data.id = payrollid;
        data.payrollType = payrolltype;
        data.description = payrolldescription;
        data.deleteFlag = 0;
        data.createdBy = payrollcreatedby;
        console.log(data);
        $.ajax({
            url: '/Payroll/SavePayroll',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            var status = data.status;
            console.log(status);
            if (status === 'Entity already exists') {
                notifyMsg('Warning!', 'Payroll already exists', 'yellow', 'fas fa-check');
            }
            else {
                notifyMsg('Success!', 'Successfully Saved', 'green', 'fas fa-check');
            }
            modal = document.getElementById('payroll-modal');
            modal.style.display = "none";
            initializeDataTable();
        });

    });
}

function delete_item_payroll() {
    //alert("Payroll Deleted");

    var payrollid = localStorage.getItem('payrollid');

    var data = {};
    data.id = payrollid;
    data.deleteFlag = 1;
    //console.log(data);
    $.ajax({
        url: '/Payroll/SavePayroll',
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
