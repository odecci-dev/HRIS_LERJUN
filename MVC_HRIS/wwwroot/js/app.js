//let toggleButton = window.localStorage.getItem("toggleButton");
let toggle = document.querySelector("#toggleButton");


document.getElementById('toggle-sidebar').addEventListener('click', () => {
    let isActive = localStorage.getItem('sidebar-active') === 'true';
    localStorage.setItem('sidebar-active', !isActive);

    updateSidebar();
});
function refreshPage() {
    window.location.reload();
}
function updateSidebar() {
    const isActive = localStorage.getItem('sidebar-active') === 'true';
    const sidebar = document.getElementById('mysidebar');
    const subnav = document.getElementById('subnav');
    const dashboaradsidebar = document.getElementById('dashboard-main-container') == null ? document.getElementById('emp-main-container') : document.getElementById('dashboard-main-container');
    //const empdashboard = document.getElementById('emp-main-container');
    //console.log(dashboaradsidebar);
    if (isActive) {
        sidebar.classList.add('active');
        //subnav.classList.add('active');
        dashboaradsidebar.classList.add('active');
        //empdashboard.classList.add('active');
        //console.log(localStorage.getItem('sidebar-active'));
    } else {
        sidebar.classList.remove('active');
        //subnav.classList.remove('active');
        dashboaradsidebar.classList.remove('active');
        dashboaradsidebar.classList.remove('active');
        //empdashboard.classList.remove('active');
    }
}
function showloadingoverlay() {
    const loadingOverlay = document.getElementById('loadingOverlay');
    loadingOverlay.style.display = 'flex'; // Display the loading overlay
}
function showodcloading() {
    const odcloading = document.getElementById('odc-loader');
    odcloading.style.display = 'flex'; // Display the loading overlay
}
function showsubnav() {

    localStorage.setItem('subnav', 0);
    localStorage.setItem('topbarsubnav', 0);
    $('#maintenance').on('click', function () {
        var isSubNav = localStorage.getItem('subnav');
        var subnav = document.getElementById('subnav');
        const arrow = document.getElementById('maintenanceArrow');

        if (isSubNav == 0) {
            localStorage.setItem('subnav', 1);

            subnav.style.display = 'block';
            // Remove a class
            arrow.classList.remove('fa-chevron-down');
            // Add a class
            arrow.classList.add('fa-chevron-up');
        }
        else {
            localStorage.setItem('subnav', 0);

            subnav.style.display = 'none';

            // Remove a class
            arrow.classList.remove('fa-chevron-up');
            // Add a class
            arrow.classList.add('fa-chevron-down');
        }
    });
    $('#topbarmaintenance').on('click', function () {
        var topbarisSubNav = localStorage.getItem('topbarsubnav');
        var topbarsubnav = document.getElementById('topbarsubnav');
        const topbararrow = document.getElementById('topbarmaintenanceArrow');

        if (topbarisSubNav == 0) {
            localStorage.setItem('topbarsubnav', 1);

            topbarsubnav.style.display = 'block';
            // Remove a class
            topbararrow.classList.remove('fa-chevron-down');
            // Add a class
            topbararrow.classList.add('fa-chevron-up');
        }
        else {
            localStorage.setItem('topbarsubnav', 0);

            topbarsubnav.style.display = 'none';

            // Remove a class
            topbararrow.classList.remove('fa-chevron-up');
            // Add a class
            topbararrow.classList.add('fa-chevron-down');
        }
    });
}

function hideodcloading() {

    const odcloading = document.getElementById('odc-loader');
    odcloading.style.display = 'none';
}
function hideloadingoverlay() {

    loadingOverlay.style.display = 'none';
}

document.addEventListener('DOMContentLoaded', updateSidebar);

function notifyMsg(title, msg, color, icon) {
    iziToast.show({
        title: title,
        message: msg,
        theme: "light",
        color: color,
        icon: icon,
        transitionIn: "bounceInDown",
        transitionOut: "flipOutX",
        position: "topCenter",
    });
}

async function fetchpositionselect() {


    $.ajax({
        url: '/Position/GetPositionSelect',
        data: {
        },
        type: "GET",
        datatype: "json",
        success: function (data) {
            //console.log(data)
            $("#position").empty();
            $("#position").append('<option value="" disabled selected>Select Position</option>');
            for (var i = 0; i < data.length; i++) {
                $("#position").append('<option value="' + data[i].id + '">' + data[i].name + "</option>");
            }

        }
    });
}
async function fetchpositionlevelselect() {


    $.ajax({
        url: '/Position/GetPositionLevelSelect',
        data: {
        },
        type: "GET",
        datatype: "json",
        success: function (data) {
            //console.log(data)
            $("#poslevel").empty();
            $("#poslevel").append('<option value="" disabled selected>Select Position Level</option>');
            for (var i = 0; i < data.length; i++) {
                $("#poslevel").append('<option value="' + data[i].id + '">' + data[i].level + "</option>");
            }

        }
    });
}
async function fetchmanagerselect() {


    $.ajax({
        url: '/Employee/GetManagerSelect',
        data: {
        },
        type: "GET",
        datatype: "json",
        success: function (data) {
            //console.log(data)
            $("#manager").empty();
            $("#manager").append('<option value="" disabled selected>Select Manager</option>');
            for (var i = 0; i < data.length; i++) {
                $("#manager").append('<option value="' + data[i].id + '">' + data[i].name + "</option>");
            }
            $("#departmenthead").empty();
            $("#departmenthead").append('<option value="" disabled selected>Select Department Head</option>');
            for (var i = 0; i < data.length; i++) {
                $("#departmenthead").append('<option value="' + data[i].id + '">' + data[i].name + "</option>");
            }

        }
    });
}
async function fetchpendingnotificationcount() {

    const noticount = document.getElementById('mnoticount');
    $.ajax({
        url: '/TimeLogs/GetNotificationPendingCount',
        data: {
        },
        type: "GET",
        datatype: "json",
        success: function (data) {
            //console.log(data);
            if (data != 0) {
                $("#mnoticount").empty();
                $("#mnoticount").append(data);
            }
            else {
                noticount.style.display = 'none';
            }
        }
    });
}
function fetchdepartmentselect() {

    const data = { Position: '', page: 1 };
    $.ajax({
        url: '/Department/GetDepartmentList',
        data: {
            data: data,
        },
        type: "POST",
        datatype: "json",
        success: function (data) {
            //console.log(data)
            $("#department").empty();
            $("#department").append('<option value="0" disabled selected>-Select Department-</option>');
            for (var i = 0; i < data[0].items.length; i++) {
                $("#department").append('<option value="' + data[0].items[i].id + '">' + data[0].items[i].departmentName + "</option>");
            }
            $("#selectDap").empty();
            $("#selectDap").append('<option value="" disabled selected>Select Department</option>');
            $("#selectDap").append('<option value="0" >Select All</option>');
            for (var i = 0; i < data[0].items.length; i++) {
                $("#selectDap").append('<option value="' + data[0].items[i].id + '">' + data[0].items[i].departmentName + "</option>");
            }

        }
    });
}
function fetchtimlogsuserselect() {

    var depart = $('#selectDap').val() ? $('#selectDap').val() : 0;
    const data = {
        Usertype: 0,
        UserId: 0,
        datefrom: null,
        dateto: null,
        Department: 0
    };
    //console.log(data);
    $.ajax({
        url: '/TimeLogs/GetTimelogsListSelect',
        data: {
            data: data,
        },
        type: "POST",
        datatype: "json",
        success: function (data) {
            console.log(data);
            $("#selectUser").empty();
            $("#selectUser").append('<option value="" disabled selected>Select User</option>');
            $("#selectUser").append('<option value="0" >Select All</option>');
            // Use a Set to store distinct userIds
            const distinctUserIds = [...new Set(data.map(item => item.userId))];

            // Iterate over the distinct userIds
            distinctUserIds.forEach(userId => {
                // Find the user details corresponding to the current userId
                const user = data.find(item => item.userId === userId);

                // Append the user to the select element
                if (user) {
                    $("#selectUser").append('<option value="' + user.userId + '"><div style="display: block"><span>' + user.fname + " " + user.lname + " </span></div></option>");
                }
            });
        }
    });
}

function fetchtimlogsuserpendingselect() {


    const data = {
        UserId: 0
    };
    //console.log(data);

    $.ajax({
        url: '/TimeLogs/GetPendingTimelogsListSelect',
        data: {
            data: data,
        },
        type: "POST",
        datatype: "json",
        success: function (data) {
            //console.log(data);
            $("#selectUserPending").empty();
            $("#selectUserPending").append('<option value="" disabled selected>Select User</option>');
            $("#selectUserPending").append('<option value="0" >Select All</option>');
            // Use a Set to store distinct userIds
            const distinctUserIds = [...new Set(data.map(item => item.userId))];

            // Iterate over the distinct userIds
            distinctUserIds.forEach(userId => {
                // Find the user details corresponding to the current userId
                const user = data.find(item => item.userId === userId);

                // Append the user to the select element
                if (user) {
                    $("#selectUserPending").append('<option value="' + user.userId + '"><div style="display: block"><span>' + user.fname + " " + user.lname + " </span></div></option>");
                }
            });
        }
    });
}
//function fetchOTuserpendingselect() {


//    var empNo = "0";
//    empNo = document.getElementById('selectUserOTPending').value;
//    empNo = empNo === '' ? '0' : empNo;
//    var sdate = document.getElementById('pot-datefrom').value;
//    var edate = document.getElementById('pot-dateto').value;
//    let data = {
//        EmployeeNo: empNo,
//        startDate: sdate,
//        endDate: edate,
//        status: otStatusFilter
//    };
//    //console.log(data);
//    $.ajax({
//        url: '/OverTime/GetPendingOvertTimeListSelect',
//        data: {
//            data: data,
//        },
//        type: "POST",
//        datatype: "json",
//        success: function (data) {
//            //console.log(data);
//            $("#selectUserOTPending").empty();
//            $("#selectUserOTPending").append('<option value="0" disabled selected>Select User</option>');
//            $("#selectUserOTPending").append('<option value="0" >Select All</option>');
//            //Use a Set to store distinct userIds
//            const distinctUserIds = [...new Set(data.map(item => item.employeeNo))];

//            // Iterate over the distinct userIds
//            distinctUserIds.forEach(employeeNo => {
//                // Find the user details corresponding to the current userId
//                const user = data.find(item => item.employeeNo === employeeNo);

//                // Append the user to the select element
//                if (user) {
//                    $("#selectUserOTPending").append('<option value="' + user.employeeNo + '"><div style="display: block"><span>' + user.fullname + " </span></div></option>");
//                }
//            });
//        }
//    });
//}
function fetchusertypeselect() {

    $.ajax({
        url: '/UserType/GetUserTypeList',
        data: {
        },
        type: "GET",
        datatype: "json",
        success: function (data) {
            //console.log(data)
            $("#emptype").empty();
            $("#emp_type2").empty();
            $("#emptype").append('<option value="" disabled selected>Select Employee Type</option>');
            $("#emp_type2").append('<option value="" disabled selected>Select Employee Type</option>');
            for (var i = 0; i < data.length; i++) {
                $("#emptype").append('<option value="' + data[i].id + '">' + data[i].userType + "</option>");
                $("#emp_type2").append('<option value="' + data[i].id + '">' + data[i].userType + "</option>");
            }

        }
    });
}
function fetchpayrolltypeselect() {

    $.ajax({
        url: '/Payroll/GetPayrollType',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        //console.log(data)
        $("#payrolltype").empty();
        $("#payrolltype").append('<option value="" disabled selected>Select Payroll Type</option>');
        for (var i = 0; i < data.length; i++) {
            $("#payrolltype").append('<option value="' + data[i].id + '">' + data[i].payrollType + "</option>");
        }
    });
}

function fetchsalarytypeselect() {

    $.ajax({
        url: '/Salary/GetSalaryType',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        //console.log(data)
        //$('#rate').val(data[0].rate);
        $("#salarytype").empty();
        $("#salarytype").append('<option value="" disabled selected>Select Salary Type</option>');
        for (var i = 0; i < data.length; i++) {
            $("#salarytype").append('<option value="' + data[i].id + '">' + data[i].salaryType + "</option>");
        }
    });
}
function fetchstatusselect() {

    $.ajax({
        url: '/Employee/GetStatusType',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        //console.log(data)
        $("#status").empty();
        $("#status").append('<option value="" disabled selected>Select Status</option>');
        for (var i = 0; i < data.length; i++) {
            $("#status").append('<option value="' + data[i].id + '">' + data[i].status + "</option>");
        }
    });
}
function fetchtaskselect() {

    $.ajax({
        url: '/Task/GetTaskList',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        //console.log(data)
        $("#task").empty();
        $("#task").append('<option value="" disabled selected>Select Task</option>');
        $("#manualtask").empty();
        $("#manualtask").append('<option value="" disabled selected>Select Task</option>');
        for (var i = 0; i < data.length; i++) {
            $("#task").append('<option value="' + data[i].id + '">' + data[i].title + "</option>");
            $("#manualtask").append('<option value="' + data[i].id + '">' + data[i].title + "</option>");
        }
    });
}
function fetchBreakselect() {

    $.ajax({
        url: '/Task/GetBreakList',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        //console.log(data)
        $("#timeoutreason").empty();
        $("#timeoutreason").append('<option value="" disabled selected>Select Reason</option>');
        for (var i = 0; i < data.length; i++) {
            $("#timeoutreason").append('<option value="' + data[i].id + '">' + data[i].title + "</option>");
        }
    });
}
function GetScheduleListOption() {

    $.ajax({
        url: '/Schedule/GetScheduleListOption',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        //console.log(data)
        $("#schedule").empty();
        $("#schedule").append('<option value="" disabled selected>Select Schedule</option>');
        for (var i = 0; i < data.length; i++) {
            $("#schedule").append('<option value="' + data[i].id + '">' + data[i].title + "</option>");
        }
    });
}
function GetETypeListOption() {

    $.ajax({
        url: '/EmployeeType/GetETypeListOption',
        data: {},
        type: "GET",
        datatype: "json"
    }).done(function (data) { // @* //  *@
        //console.log(data)
        $("#uType").empty();
        $("#uType").append('<option value="" disabled selected>Select Employee Type</option>');
        for (var i = 0; i < data.length; i++) {
            $("#uType").append('<option value="' + data[i].id + '">' + data[i].title + "</option>");
        }
    });
}
function printPage() {
    var printWindow = window.open('/Home/OR_Print', '_blank');
    // Wait for the new window to load and then trigger the print dialog
    $('#modal-success').modal('show');
    $('#defaultmodal').modal('hide');
    printWindow.onload = function () {
        printWindow.print();

    };

}

function print_or() {
    fetch(`/Home/OR_Print`) // Adjust URL to your endpoint
        .then(response => response.text())
        .then(html => {
            document.querySelector('#modal-xl .modal-body-2').innerHTML = html;
            $('#modal-xl').modal('show'); // Show the modal using Bootstrap
        })
        .catch(error => console.error('Error loading content:', error));

    //$('#modal-success').modal('show');
}
function printDiv(divId) {

    $('#defaultmodal').modal('hide');
    var iframe = document.createElement('iframe');
    iframe.style.position = 'absolute';
    iframe.style.width = '0';
    iframe.style.height = '0';
    iframe.style.border = 'none';

    document.body.appendChild(iframe);

    var doc = iframe.contentWindow.document;
    doc.open();
    doc.write('<html><head><title>Print</title>');
    doc.write('<style>body{font-family: Arial, sans-serif;}</style>');
    doc.write('</head><body>');
    doc.write(document.getElementById(divId).innerHTML);
    doc.write('</body></html>');
    doc.close();

    iframe.contentWindow.focus();
    iframe.contentWindow.print();

    document.body.removeChild(iframe);
    $('#modal-success').modal('show'); c
}
function loadModal(url, modal, title, size, isToggled) {


    $(modal + ' .overlay').removeClass('d-none');
    $(modal + ' .modal-dialog').removeClass('modal-sm');
    $(modal + ' .modal-dialog').removeClass('modal-md');
    $(modal + ' .modal-dialog').removeClass('modal-lg');
    $(modal + ' .modal-dialog').removeClass('modal-xl');
    $(modal + ' .modal-dialog').removeClass('modal-fullscreen');
    $(modal + ' .modal-dialog').addClass('modal-' + size);
    $(modal + ' .modal-body').html('<div class="mt-5 mb-5"></div>');
    //console.log(url);
    //if not toggle in button
    if (!isToggled)
        $(modal).modal('show');

    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {

            $(modal + ' .modal-body').html(res);
            $(modal + ' .modal-title').html(title);
            $(modal + ' .overlay').addClass('d-none');

            //load tooltip
            $('.modal [data-toggle="tooltip"]')
                .tooltip({ trigger: 'hover' });
        },
        error: function (result) {
            $(modal + ' .modal-body').html(`<p>${result.responseText}</p>`);
            if (result.status == 403 || result.status == 401)
                $(modal + ' .modal-body').html(`<p><b>Unauthorized!</b> Access to the requested resource is forbidden.</p>`);

            $(modal + ' .modal-title').html('Error Message');
            $(modal + ' .overlay').addClass('d-none');

        }
    });
};

function deletemodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_item()"/>';
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
function deleteusermodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_user_item()"/>';
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
function deletedepartmentmodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_item_department()"/>';
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
function deletepayrollmodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_item_payroll()"/>';
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
function deletesalarymodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_item_salary()"/>';
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
function deletemodalTimelogs() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_item_timelogs()"/>';
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
function deletemodalEType() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_item_EType()"/>';
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
function declinemodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="decline_item()"/>';
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
    document.getElementById('message').textContent = 'Are you sure you want to decline this item?';
    document.getElementById('validation').textContent = 'Confirmation';
    $('.input-container-button').append(cancelButton);
    $('.input-container-button').append(delete_);
    $('.img-header').append('<img id="modalImage" src="/img/OPTION.webp" alt="Modal Image" />');
}
function approvemodal() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="decline_item()"/>';
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
function successmodal(Id) {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="OK" id="btn-delete_item" class="btn-pay"  data-dismiss="modal"/>';
    var cancelButton = '<input type="submit" value="NO" id="btn-cancel" class="btn-NO" data-dismiss="modal"/>';
    $('.input-container-button').empty();
    $('.img-header').empty();

    content.forEach(content => {
        content.style.setProperty("border-radius", "15px 15px 15px 15px", "important");
        content.style.setProperty("border-bottom", "7px var(--dark) solid", "important");

    });
    modal_span.forEach(modal_span => {
        modal_span.style.setProperty("text-align", "center", "important");
        modal_span.style.setProperty("width", "100%", "important");
    });
    element.forEach(element => {
        element.style.setProperty("color", "white", "important");
        element.style.setProperty("background-color", "var(--dark)", "important");
        element.style.setProperty("border-radius", "15px 15px 0 0", "important");
        element.style.setProperty("text-align", "center", "important");
    });
    var contenttext = Id == null ? "New data Successfully Saved" : "Data Successfully Updated";
    document.getElementById('message').textContent = contenttext;
    document.getElementById('validation').textContent = 'SUCCESS';
    $('.input-container-button').append(delete_);
    $('.img-header').append('<img id="modalImage" src="/img/SUCCESS.webp" alt="Modal Image" />');
}
document.addEventListener('DOMContentLoaded', function () {
});


function topBarDOM() {
    $('#open-nav').click(function () {
        document.getElementById('open-nav').style.display = "none";
        document.getElementById('close-nav').style.display = "block";
        document.getElementById('top-bar-menu').style.display = "block";

    });
    $('#close-nav').click(function () {
        document.getElementById('close-nav').style.display = "none";
        document.getElementById('open-nav').style.display = "block";
        document.getElementById('top-bar-menu').style.display = "none";
    });
}

