
function myFunction() {
    pmodal.style.display = "none";
}
function myFunctionOpenModal() {
    document.getElementById('posid').value = "0";
    document.getElementById('posname').value = "";
    document.getElementById('desc').value = "";
    pmodal.style.display = "flex";
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
            initializeDataTable();
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
        initializeDataTable();
    });

}