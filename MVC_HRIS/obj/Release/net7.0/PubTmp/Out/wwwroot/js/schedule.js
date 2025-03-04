function scheduleDOM() {
    $("#monday").on("change", function () {
        var monday = document.getElementById('monday');
        var mondays = document.getElementById('mondays');
        var mondaye = document.getElementById('mondaye');
        if (monday.checked) {
            mondays.removeAttribute('required');    // Remove the 'required' attribute
            mondaye.removeAttribute('required');    // Remove the 'required' attribute
            mondays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            mondaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            mondays = document.getElementById('mondays').value = "";
            mondaye = document.getElementById('mondaye').value = "";
        }
        else {
            mondays.setAttribute('required', ''); // Adds the 'required' attribute
            mondaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            mondays.removeAttribute('disabled');    // Remove the 'required' attribute
            mondaye.removeAttribute('disabled');    // Remove the 'required' attribute
        }
    });
    $("#tuesday").on("change", function () {
        var tuesday = document.getElementById('tuesday');
        var tuesdays = document.getElementById('tuesdays');
        var tuesdaye = document.getElementById('tuesdaye');
        if (tuesday.checked) {
            tuesdays.removeAttribute('required');    // Remove the 'required' attribute
            tuesdaye.removeAttribute('required');    // Remove the 'required' attribute
            tuesdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            tuesdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            tuesdays = document.getElementById('tuesdays').value = "";
            tuesdaye = document.getElementById('tuesdaye').value = "";
        }
        else {
            tuesdays.setAttribute('required', ''); // Adds the 'required' attribute
            tuesdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            tuesdays.removeAttribute('disabled');    // Remove the 'required' attribute
            tuesdaye.removeAttribute('disabled');    // Remove the 'required' attribute
        }
    });
    $("#wednesday").on("change", function () {
        var wednesday = document.getElementById('wednesday');
        var wednesdays = document.getElementById('wednesdays');
        var wednesdaye = document.getElementById('wednesdaye');
        if (wednesday.checked) {
            wednesdays.removeAttribute('required');    // Remove the 'required' attribute
            wednesdaye.removeAttribute('required');    // Remove the 'required' attribute
            wednesdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            wednesdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            wednesdays = document.getElementById('wednesdays').value = "";
            wednesdaye = document.getElementById('wednesdaye').value = "";
        }
        else {
            wednesdays.setAttribute('required', ''); // Adds the 'required' attribute
            wednesdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            wednesdays.removeAttribute('disabled');    // Remove the 'required' attribute
            wednesdaye.removeAttribute('disabled');    // Remove the 'required' attribute
        }
    });
    $("#thursday").on("change", function () {
        var thursday = document.getElementById('thursday');
        var thursdays = document.getElementById('thursdays');
        var thursdaye = document.getElementById('thursdaye');
        if (thursday.checked) {
            thursdays.removeAttribute('required');    // Remove the 'required' attribute
            thursdaye.removeAttribute('required');    // Remove the 'required' attribute
            thursdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            thursdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            thursdays = document.getElementById('thursdays').value = "";
            thursdaye = document.getElementById('thursdaye').value = "";
        }
        else {
            thursdays.setAttribute('required', ''); // Adds the 'required' attribute
            thursdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            thursdays.removeAttribute('disabled');    // Remove the 'required' attribute
            thursdaye.removeAttribute('disabled');    // Remove the 'required' attribute
        }
    });
    $("#friday").on("change", function () {
        var friday = document.getElementById('friday');
        var fridays = document.getElementById('fridays');
        var fridaye = document.getElementById('fridaye');
        if (friday.checked) {
            fridays.removeAttribute('required');    // Remove the 'required' attribute
            fridaye.removeAttribute('required');    // Remove the 'required' attribute
            fridays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            fridaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            fridays = document.getElementById('fridays').value = "";
            fridaye = document.getElementById('fridaye').value = "";
        }
        else {
            fridays.setAttribute('required', ''); // Adds the 'required' attribute
            fridaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            fridays.removeAttribute('disabled');    // Remove the 'required' attribute
            fridaye.removeAttribute('disabled');    // Remove the 'required' attribute
        }
    });
    $("#saturday").on("change", function () {
        var saturday = document.getElementById('saturday');
        var saturdays = document.getElementById('saturdays');
        var saturdaye = document.getElementById('saturdaye');
        if (saturday.checked) {
            saturdays.removeAttribute('required');    // Remove the 'required' attribute
            saturdaye.removeAttribute('required');    // Remove the 'required' attribute
            saturdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            saturdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            saturdays = document.getElementById('saturdays').value = "";
            saturdaye = document.getElementById('saturdaye').value = "";
        }
        else {
            saturdays.setAttribute('required', ''); // Adds the 'required' attribute
            saturdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            saturdays.removeAttribute('disabled');    // Remove the 'required' attribute
            saturdaye.removeAttribute('disabled');    // Remove the 'required' attribute
        }
    });
    $("#sunday").on("change", function () {
        var sunday = document.getElementById('sunday');
        var sundays = document.getElementById('sundays');
        var sundaye = document.getElementById('sundaye');
        if (sunday.checked) {
            sundays.removeAttribute('required');    // Remove the 'required' attribute
            sundaye.removeAttribute('required');    // Remove the 'required' attribute
            sundays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            sundaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            sundays = document.getElementById('sundays').value = "";
            sundaye = document.getElementById('sundaye').value = "";
        }
        else {
            sundays.setAttribute('required', ''); // Adds the 'required' attribute
            sundaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            sundays.removeAttribute('disabled');    // Remove the 'required' attribute
            sundaye.removeAttribute('disabled');    // Remove the 'required' attribute
        }
    });

    $("#add-schedule-form").on("submit", function (event) {
        event.preventDefault();
        var title = document.getElementById('schedtitle').value;
        var description = document.getElementById('scheddescription').value;
        var schedId = document.getElementById('schedId').value;
        var mondays = document.getElementById('mondays').value;
        var mondaye = document.getElementById('mondaye').value;
        var tuesdays = document.getElementById('tuesdays').value;
        var tuesdaye = document.getElementById('tuesdaye').value;
        var wednesdays = document.getElementById('wednesdays').value;
        var wednesdaye = document.getElementById('wednesdaye').value;
        var thursdays = document.getElementById('thursdays').value;
        var thursdaye = document.getElementById('thursdaye').value;
        var fridays = document.getElementById('fridays').value;
        var fridaye = document.getElementById('fridaye').value;
        var saturdays = document.getElementById('saturdays').value;
        var saturdaye = document.getElementById('saturdaye').value;
        var sundays = document.getElementById('sundays').value;
        var sundaye = document.getElementById('sundaye').value;

        document.getElementById('mondays').value = fridays;
        var data = {};
        data.id = schedId;
        data.title = title;
        data.description = description;
        data.mondays = mondays;
        data.mondaye = mondaye;
        data.tuesdays = tuesdays;
        data.tuesdaye = tuesdaye;
        data.wednesdays = wednesdays;
        data.wednesdaye = wednesdaye;
        data.thursdays = thursdays;
        data.thursdaye = thursdaye;
        data.fridays = fridays;
        data.fridaye = fridaye;
        data.saturdays = saturdays;
        data.saturdaye = saturdaye;
        data.sundays = sundays;
        data.sundaye = sundaye;
        data.statusID = 1;
        console.log(data);

        var smodal = document.getElementById('schedmodal');
        $.ajax({
            url: '/Schedule/AddSchedule',
            data: data,
            type: "POST",
            dataType: "json"
        }).done(function (data) {
            //console.log(data);
            notifyMsg('Success!', data.status, 'green', 'fas fa-check');
            smodal.style.display = "none";
            initializeDataTable();
        });

    });

    $('#schedule-table').on('click', '.tbl-edit', function () {


        document.getElementById('schedId').value = $(this).data('id');
        document.getElementById('schedtitle').value = $(this).data('title');
        document.getElementById('scheddescription').value = $(this).data('description');
        if ($(this).data('mondays') == null) {
            mondays.removeAttribute('required');    // Remove the 'required' attribute
            mondaye.removeAttribute('required');    // Remove the 'required' attribute
            mondays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            mondaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        }
        else {
            monday.checked = false;
            mondays.setAttribute('required', ''); // Adds the 'required' attribute
            mondaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            mondays.removeAttribute('disabled');    // Remove the 'required' attribute
            mondaye.removeAttribute('disabled');    // Remove the 'required' attribute
            document.getElementById('mondays').value = $(this).data('mondays');
            document.getElementById('mondaye').value = $(this).data('mondaye');
        }
        if ($(this).data('tuesdays') == null) {
            tuesday.checked = true;
            tuesdays.removeAttribute('required');    // Remove the 'required' attribute
            tuesdaye.removeAttribute('required');    // Remove the 'required' attribute
            tuesdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            tuesdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        }
        else {
            tuesday.checked = false;
            tuesdays.setAttribute('required', ''); // Adds the 'required' attribute
            tuesdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            tuesdays.removeAttribute('disabled');    // Remove the 'required' attribute
            tuesdaye.removeAttribute('disabled');    // Remove the 'required' attribute
            document.getElementById('tuesdays').value = $(this).data('tuesdays');
            document.getElementById('tuesdaye').value = $(this).data('tuesdaye');
        }

        if ($(this).data('wednesdays') == null) {
            wednesday.checked = true;
            wednesdays.removeAttribute('required');    // Remove the 'required' attribute
            wednesdaye.removeAttribute('required');    // Remove the 'required' attribute
            wednesdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            wednesdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        }
        else {
            wednesday.checked = false;
            wednesdays.setAttribute('required', ''); // Adds the 'required' attribute
            wednesdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            wednesdays.removeAttribute('disabled');    // Remove the 'required' attribute
            wednesdaye.removeAttribute('disabled');    // Remove the 'required' attribute
            document.getElementById('wednesdays').value = $(this).data('wednesdays');
            document.getElementById('wednesdaye').value = $(this).data('wednesdaye');
        }
        if ($(this).data('thursdays') == null) {
            thursday.checked = true;
            thursdays.removeAttribute('required');    // Remove the 'required' attribute
            thursdaye.removeAttribute('required');    // Remove the 'required' attribute
            thursdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            thursdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        }
        else {
            thursday.checked = false;
            thursdays.setAttribute('required', ''); // Adds the 'required' attribute
            thursdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            thursdays.removeAttribute('disabled');    // Remove the 'required' attribute
            thursdaye.removeAttribute('disabled');    // Remove the 'required' attribute
            document.getElementById('thursdays').value = $(this).data('thursdays');
            document.getElementById('thursdaye').value = $(this).data('thursdaye');
        }
        if ($(this).data('fridays') == null) {
            friday.checked = true;
            fridays.removeAttribute('required');    // Remove the 'required' attribute
            fridaye.removeAttribute('required');    // Remove the 'required' attribute
            fridays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            fridaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        }
        else {
            friday.checked = false;
            fridays.setAttribute('required', ''); // Adds the 'required' attribute
            fridaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            fridays.removeAttribute('disabled');    // Remove the 'required' attribute
            fridaye.removeAttribute('disabled');    // Remove the 'required' attribute
            document.getElementById('fridays').value = $(this).data('fridays');
            document.getElementById('fridaye').value = $(this).data('fridaye');
        }
        if ($(this).data('saturdays') == null) {
            saturday.checked = true;
            saturdays.removeAttribute('required');    // Remove the 'required' attribute
            saturdaye.removeAttribute('required');    // Remove the 'required' attribute
            saturdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            saturdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        }
        else {
            saturday.checked = false;
            saturdays.setAttribute('required', ''); // Adds the 'required' attribute
            saturdaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            saturdays.removeAttribute('disabled');    // Remove the 'required' attribute
            saturdaye.removeAttribute('disabled');    // Remove the 'required' attribute
            document.getElementById('saturdays').value = $(this).data('saturdays');
            document.getElementById('saturdaye').value = $(this).data('saturdaye');
        }
        if ($(this).data('sundays') == null) {
            sunday.checked = true;
            sundays.removeAttribute('required');    // Remove the 'required' attribute
            sundaye.removeAttribute('required');    // Remove the 'required' attribute
            sundays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
            sundaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        }
        else {
            sunday.checked = false;
            sundays.setAttribute('required', ''); // Adds the 'required' attribute
            sundaye.setAttribute('required', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
            sundays.removeAttribute('disabled');    // Remove the 'required' attribute
            sundaye.removeAttribute('disabled');    // Remove the 'required' attribute
            document.getElementById('sundays').value = $(this).data('sundays');
            document.getElementById('sundaye').value = $(this).data('sundaye');
        }

        smodal = document.getElementById('schedmodal');
        smodal.style.display = "flex";
    });
    $('#schedule-table').on('click', '.tbl-view', function () {

        document.getElementById('add-schedule').style.display = "none";
        document.getElementById('schedId').value = $(this).data('id');
        document.getElementById('schedtitle').value = $(this).data('title');
        document.getElementById('scheddescription').value = $(this).data('description');
        document.getElementById('schedId').setAttribute('disabled', '');
        document.getElementById('schedtitle').setAttribute('disabled', '');
        document.getElementById('scheddescription').setAttribute('disabled', '');
        monday.checked = true;
        mondays.setAttribute('disabled', ''); // Adds the 'required' attribute
        mondaye.setAttribute('disabled', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
        document.getElementById('mondays').value = $(this).data('mondays');
        document.getElementById('mondaye').value = $(this).data('mondaye');

        tuesday.checked = true;
        tuesdays.setAttribute('disabled', ''); // Adds the 'required' attribute
        tuesdaye.setAttribute('disabled', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
        document.getElementById('tuesdays').value = $(this).data('tuesdays');
        document.getElementById('tuesdaye').value = $(this).data('tuesdaye');

        wednesday.checked = true;
        wednesdays.setAttribute('disabled', ''); // Adds the 'required' attribute
        wednesdaye.setAttribute('disabled', ''); // Adds the 'required' attributemondays.removeAttribute('required');   
        document.getElementById('wednesdays').value = $(this).data('wednesdays');
        document.getElementById('wednesdaye').value = $(this).data('wednesdaye');

        thursday.checked = true;
        thursdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        thursdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        document.getElementById('thursdays').value = $(this).data('thursdays');
        document.getElementById('thursdaye').value = $(this).data('thursdaye');

        friday.checked = true;
        fridays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        fridaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        //fridays.getElementById('fridays').value  = $(this).data('fridays');
        //fridaye.getElementById('fridaye').value  = $(this).data('fridaye');
        document.getElementById('fridays').value = $(this).data('fridays');
        document.getElementById('fridaye').value = $(this).data('fridaye');

        saturday.checked = true;
        saturdays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        saturdaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        document.getElementById('saturdays').value = $(this).data('saturdays');
        document.getElementById('saturdaye').value = $(this).data('saturdaye');

        sunday.checked = true;
        sundays.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        sundaye.setAttribute('disabled', ''); // Adds the 'disabled' attribute
        document.getElementById('sundays').value = $(this).data('sundays');
        document.getElementById('sundaye').value = $(this).data('sundaye');

        smodal = document.getElementById('schedmodal');
        smodal.style.display = "flex";
    });
}

function closeScheduleModal() {
    document.getElementById('add-schedule-form').reset();
    document.getElementById('schedId').value = "";
    document.getElementById('schedtitle').value = "";
    document.getElementById('scheddescription').value = "";

    monday.checked = false;
    mondays.setAttribute('required', ''); // Adds the 'required' attribute
    mondaye.setAttribute('required', ''); // Adds the 'required' mondays.removeAttribute('disabled');    // Remove the 'required' attribute
    mondaye.removeAttribute('disabled');    // Remove the 'required'    
    mondays.removeAttribute('disabled');    // Remove the 'required' attribute
    mondaye.removeAttribute('disabled');    // Remove the 'required' attribute 
    document.getElementById('mondays').value = "";
    document.getElementById('mondaye').value = "";

    tuesday.checked = false;
    tuesdays.setAttribute('required', ''); // Adds the 'required' attribute
    tuesdaye.setAttribute('required', ''); // Adds the 'required' tuesdays.removeAttribute('disabled');    // Remove the 'required' attribute
    tuesdaye.removeAttribute('disabled');    // Remove the 'required'    
    tuesdays.removeAttribute('disabled');    // Remove the 'required' attribute
    tuesdaye.removeAttribute('disabled');    // Remove the 'required' attribute 
    document.getElementById('tuesdays').value = "";
    document.getElementById('tuesdaye').value = "";

    wednesday.checked = false;
    wednesdays.setAttribute('required', ''); // Adds the 'required' attribute
    wednesdaye.setAttribute('required', ''); // Adds the 'required' wednesdays.removeAttribute('disabled');    // Remove the 'required' attribute
    wednesdaye.removeAttribute('disabled');    // Remove the 'required'    
    wednesdays.removeAttribute('disabled');    // Remove the 'required' attribute
    wednesdaye.removeAttribute('disabled');    // Remove the 'required' attribute 
    document.getElementById('wednesdays').value = "";
    document.getElementById('wednesdaye').value = "";

    thursday.checked = false;
    thursdays.setAttribute('required', ''); // Adds the 'required' attribute
    thursdaye.setAttribute('required', ''); // Adds the 'required' thursdays.removeAttribute('disabled');    // Remove the 'required' attribute
    thursdaye.removeAttribute('disabled');    // Remove the 'required'    
    thursdays.removeAttribute('disabled');    // Remove the 'required' attribute
    thursdaye.removeAttribute('disabled');    // Remove the 'required' attribute 
    document.getElementById('thursdays').value = "";
    document.getElementById('thursdaye').value = "";

    friday.checked = false;
    fridays.setAttribute('required', ''); // Adds the 'required' attribute
    fridaye.setAttribute('required', ''); // Adds the 'required' fridays.removeAttribute('disabled');    // Remove the 'required' attribute
    fridaye.removeAttribute('disabled');    // Remove the 'required'    
    fridays.removeAttribute('disabled');    // Remove the 'required' attribute
    fridaye.removeAttribute('disabled');    // Remove the 'required' attribute 
    document.getElementById('fridays').value = "";
    document.getElementById('fridaye').value = "";

    saturday.checked = false;
    saturdays.setAttribute('required', ''); // Adds the 'required' attribute
    saturdaye.setAttribute('required', ''); // Adds the 'required' saturdays.removeAttribute('disabled');    // Remove the 'required' attribute
    saturdaye.removeAttribute('disabled');    // Remove the 'required'    
    saturdays.removeAttribute('disabled');    // Remove the 'required' attribute
    saturdaye.removeAttribute('disabled');    // Remove the 'required' attribute 
    document.getElementById('saturdays').value = "";
    document.getElementById('saturdaye').value = "";

    sunday.checked = false;
    sundays.setAttribute('required', ''); // Adds the 'required' attribute
    sundaye.setAttribute('required', ''); // Adds the 'required' sundays.removeAttribute('disabled');    // Remove the 'required' attribute
    sundaye.removeAttribute('disabled');    // Remove the 'required'    
    sundays.removeAttribute('disabled');    // Remove the 'required' attribute
    sundaye.removeAttribute('disabled');    // Remove the 'required' attribute 
    document.getElementById('sundays').value = "";
    document.getElementById('sundaye').value = "";
    document.getElementById('add-schedule').style.display = "block";

    smodal = document.getElementById('schedmodal');
    smodal.style.display = "none";
}
function openScheduleModal() {
    smodal = document.getElementById('schedmodal');
    smodal.style.display = "flex";
}

function deletemodalSchedule() {
    var element = document.querySelectorAll(".modal-header");
    var content = document.querySelectorAll(".modal-content");
    var modal_span = document.querySelectorAll(".modal-header span");
    var delete_ = '<input type="submit" value="YES" id="btn-delete_item" class="btn-pay"  onclick="delete_item_schedule()"/>';
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
function delete_item_schedule() {
    var data = {};
    var schedId = localStorage.getItem('schedid');
    data.id = schedId;
    data.title = "";
    $.ajax({
        url: '/Schedule/AddSchedule',
        data: data,
        type: "POST",
        dataType: "json"
    }).done(function (data) {
        //console.log(data);
        notifyMsg('Success!', 'Successfully Deleted', 'green', 'fas fa-check');
        $("#alertmodal").modal('hide');
        initializeDataTable();
    });
}