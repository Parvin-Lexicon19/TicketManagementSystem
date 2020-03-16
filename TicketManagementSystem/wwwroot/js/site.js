// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('.custom-file-input').on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).next('.custom-file-label').html(fileName);
    });
});
//$(document).ready(function () {
//    $.ajax({
//        url: "ReturnJSONDataToAJax",
//        type: "GET",
//        contentType: "application/json; charset=utf-8",
//        datatype: JSON,
//        success: function (result) {
//            $(result).each(function () {
//                $("#FromJson").append($("<option></option>").val(this.ID).html(this.Name));
//            });
//        },
//        error: function (data) { }
//    });
//});  

//function CustomerChanged() {
//    $.ajax({
//        url: "ReturnJSONDataToAJax",
//        type: "GET",
//        contentType: "application/json; charset=utf-8",
//        datatype: JSON,
//        success: function (result) {
//            $(result).each(function () {
//                $("#FromJson").append($("<option></option>").val(this.ID).html(this.Name));
//            });
//        },
//        error: function (data) { }
//    });
//};

//$(function () {

//    //AjaxCall('GetCustomers', null).done(function (response) {
//    //    if (response.length > 0) {
//    //        $('#customerDropDownList').html('');
//    //        var options = '';
//    //        options += '<option value="Select">Select</option>';
//    //        for (var i = 0; i < response.length; i++) {
//    //            options += '<option value="' + response[i] + '">' + response[i] + '</option>';
//    //        }
//    //        $('#customerDropDownList').append(options);

//    //    }
//    //}).fail(function (error) {
//    //    alert(error.StatusText);
//    //});

//    $('#customerDropDownList').change(function () {
//        var customer = $('#customerDropDownList').val();
//        var obj = { customerId: customerId };
//        AjaxCall('GetProjects', JSON.stringify(obj), 'POST').done(function (response) {
//            if (response.length > 0) {
//                $('#projectDropDownList').html('');
//                var options = '';
//                options += '<option value="Select">Select</option>';
//                for (var i = 0; i < response.length; i++) {
//                    options += '<option value="' + response[i] + '">' + response[i] + '</option>';
//                }
//                $('#projectDropDownList').append(options);

//            }
//        }).fail(function (error) {
//            alert(error.StatusText);
//        });
//    });

//});

//function AjaxCall(url, data, type) {
//    return $.ajax({
//        url: url,
//        type: type ? type : 'GET',
//        data: data,
//        contentType: 'application/json'
//    });
//}

