var addressCondition = 0;

function getRegionsData() {
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refregion.csv",
        //dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].replaceAll('"', '').split(",");
            //console.log(array.length);
            //console.log(headers);
            $("#region").empty();
            $("#region").append('<option value="0" disabled selected>Select Region</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].replaceAll('"', '').split(",");

                $("#region").append('<option value="' + headers[2] + '" data-id="' + headers[3] + '">' + headers[2] + "</option>");
            }
        }
    });
}
function getProvinceData() {
    var region = $("#region option:selected").data('id').toString().trim();
    region = parseInt(region, 10);
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refprovince.csv",
        dataType: "text",
        success: function (data) {
            const array = data.toString().split("\n");
            $("#province").empty();
            $("#province").append('<option value="0" disabled selected>Select Province</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].replaceAll('"', '').split(",");
                var lookupProvince = parseInt(headers[3], 10);
                //console.log(headers[3]);
                if (lookupProvince == region) {
                    //console.log(headers[2].replaceAll('"', ''));
                    //console.log(headers[2]);
                    //console.log(lookupProvince);
                    //console.log(region);
                    $("#province").append('<option value="' + headers[2] + '"  data-id="' + headers[4] + '">' + headers[2] + "</option>");
                }

            }


        }
    });
}
function getCityData() {
    var province = $("#province option:selected").data('id').toString().trim();
    province = parseInt(province, 10);
    //console.log("province");
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refcitymun.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",");
            //console.log(array.length);
            $("#municipality").empty();
            $("#municipality").append('<option value="0" disabled selected>Select City</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].replaceAll('"', '').split(",");
                var lookUpCity = parseInt(headers[4], 10);
                //console.log(headers[2]);
                if (lookUpCity == province) {
                    $("#municipality").append('<option value="' + headers[2] + '"  data-id="' + headers[5] + '">' + headers[2] + "</option>");
                }

            }

        }
    });
}
function getBarangayData() {
    var city = $("#municipality option:selected").data('id').toString().trim();

    city = parseInt(city, 10);
    //console.log("city");
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refbrgy.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].replaceAll('"', '').split(",");
            //console.log(array.length);
            $("#barangay").empty();
            $("#barangay").append('<option value="0" disabled selected>Select Barangay</option>');
            for (var i = 0; i < array.length; i++) {
                let headers = array[i].replaceAll('"', '').split(",");
                //console.log(headers[2]);
                if (headers[5] == city) {
                    $("#barangay").append('<option value="' + headers[2] + '">' + headers[2].toUpperCase() + "</option>");
                }

            }

        }
    });
}
function addressDOM() {
    var region = document.getElementById('region');
    var province = document.getElementById('province');
    var municipality = document.getElementById('municipality');
    var barangay = document.getElementById('barangay');

    province.disabled = 'true';
    municipality.disabled = 'true';
    barangay.disabled = 'true';
    $("#region").change(function () {
        var regionValue = $("#region option:selected").data('id');
        //console.log(regionValue);

        localStorage.setItem('region', regionValue);

        getProvinceData();
        $("#province").removeAttr('disabled');
        //console.log($("#region option:selected").text());
    });
    $("#province").change(function () {
        getCityData();
        $("#municipality").removeAttr('disabled');
        //console.log($("#province option:selected").text());
    });
    $("#municipality").change(function () {
        getBarangayData();
        $("#barangay").removeAttr('disabled');
        console.log($("#municipality option:selected").text());
    });
    $("#barangay").change(function () {
        console.log($("#barangay option:selected").text());
    });
}

function GetAllDataAddress() {
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refregion.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#region").empty();
            $("#region").append('<option value="0" disabled selected>Select Region</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",");
                //console.log(headers[2]);
                $("#region").append('<option value="' + headers[2].replaceAll('"', '') + '" data-id="' + headers[3].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '') + "</option>");
            }


        }
    });
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refprovince.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#province").empty();
            $("#province").append('<option value="0" disabled selected>Select Province</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",")
                //console.log(region);
                $("#province").append('<option value="' + headers[2].replaceAll('"', '') + '"  data-id="' + headers[4].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '') + "</option>");

            }

        }
    });
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refcitymun.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#municipality").empty();
            $("#municipality").append('<option value="0" disabled selected>Select City</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",")
                //console.log(headers[2]);
                $("#municipality").append('<option value="' + headers[2].replaceAll('"', '') + '"  data-id="' + headers[5].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '') + "</option>");


            }

        }
    });
    $.ajax({
        type: "GET",
        url: "../../../excel/barangay_files/refbrgy.csv",
        dataType: "text",
        success: function (data) {
            //console.log(data);
            const array = data.toString().split("\n");
            let headers = array[0].split(",")
            //console.log(array.length);
            $("#barangay").empty();
            $("#barangay").append('<option value="0" disabled selected>Select Barangay</option>');
            for (var i = 0; i < array.length - 1; i++) {
                let headers = array[i].split(",")
                //console.log(headers[2]);
                $("#barangay").append('<option value="' + headers[2].replaceAll('"', '') + '">' + headers[2].replaceAll('"', '').toUpperCase() + "</option>");


            }

        }
    });
}