// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('.custom-file-input').on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).next('.custom-file-label').html(fileName);
    });
});

function FillCustomers() {
    var companyId = $('#companyDropDownList').val();
    $.ajax({
        url: '/Tickets/GetCustomers',
        type: "POST",
        dataType: "JSON",
        data: { companyId: companyId },
        success: function (customers) {
            $("#customerDropDownList").html("");   //clear before appending new list
            $("#customerDropDownList").append(
                $('<option value="">-----Välj Användare-----</option>'));
            $.each(customers, function (i, customer) {
                $("#customerDropDownList").append(
                    $('<option value="' + customer.value + '">' + customer.text + '</option>'));
            });
        }
    });
}

function FillProjects() {
    var customerId = $('#customerDropDownList').val();
    $.ajax({
        url: 'GetProjects',
        type: "POST",
        dataType: "JSON",
        data: { customerId: customerId },
        success: function (projects) {
            $("#projectDropDownList").html("");   //clear before appending new list
            $("#projectDropDownList").append(
                $('<option value="">-----Välj Lösning-----</option>'));
            $.each(projects, function (i, project) {
                $("#projectDropDownList").append(
                    $('<option value="' + project.value + '">' + project.text + '</option>'));
            });
        }
    });
}

function FillCompanyProjects() {
    var companyId = $('#companyDDL').val();
    $.ajax({
        url: window.bitoreq.GetCompanyProjects,
        data: {companyId: companyId},
        type: "POST",
        dataType: "JSON",
        success: function (projects) {
            $("#projectDDL").html("");   //clear before appending new list
            $("#projectDDL").append(
                $('<option value="">-----Välj Lösning-----</option>'));
            $.each(projects, function (i, project) {
                $("#projectDDL").append(
                    $('<option value="' + project.value + '">' + project.text + '</option>'));
            });
        }
    });
}

function FillRoleCompanies() {
    //var roleId = $('#rolesDDL').val();
    var roleId = $('#rolesDDL option:selected').text();
    $.ajax({
        url: window.bitoreq.FillRoleCompaniesURL,
        data: {roleId: roleId},
        type: "POST",
        dataType: "JSON",
        success: function (companies) {
            $("#companiesDDL").html("");   //clear before appending new list
            $("#companiesDDL").append(
                $('<option value="">-----Välj Företag-----</option>'));
            $.each(companies, function (i, company) {
                $("#companiesDDL").append(
                    $('<option value="' + company.value + '">' + company.text + '</option>'));
            });
        }
    });
}

//function FillRoleCompanies() {
//    var roleId = $('#rolesDDL').val();
//    $.ajax({
//        url: '../Controllers/ApplicationUsers/GetRoleCompanies',
//        type: "POST",
//        dataType: "JSON",
//        data: { roleId: roleId },
//        success: function (companies) {
//            $("#companiesDDL").html("");   //clear before appending new list
//            $("#companiesDDL").append(
//                $('<option value="">-----Välj Företag-----</option>'));
//            $.each(companies, function (i, company) {
//                $("#companiesDDL").append(
//                    $('<option value="' + company.value + '">' + company.text + '</option>'));
//            });
//        }
//    });
//}

//this will add * next to required label
//$(':input').each(function () { //for all input types
$('input[type=text],input[type=radio],textarea,select,input[type=email],input[type=number],input[type=password],input[type=date]').each(function () {
    var req = $(this).attr('data-val-required');
    var exclude = $(this).attr('data-exclude');
    if (undefined != req && undefined == exclude) {
        var label = $('label[for="' + $(this).attr('id') + '"]');
        var text = label.text();
        if (text.length > 0) {
            label.append('<span style="color:red"> *</span>');
        }
    }
});

//function getOption() {
//    //var status = $('#statusSearchId').val();
//    //if (status != null) {
//    //    $("#statusSearchId").val(status);
//    //}
//    var status = localStorage.getItem("statusSearchId");
//    if (status != null) {
//        $("select[name=statusSearch]").val(status);
//    }
//    //$("statusSearchId").on("change", function () {
//    //    $("#statusSearchId").val($(this).val());
//    //});
//}

//function KeepSearch() {
//    $('#searchupdate').text(response.);
//};
//function KeepSearch() {
//    $(".keepdata").value = "test";
//};

//function getOption() {
//    selectElement =
//        document.querySelector('#stausSearch1');

//    output = selectElement.value;

//    document.querySelector('.output').textContent
//        = output;
//} 

//function deleteit(no) {
//    document.getElementById(this.deleteit)
//}

//function deleteit(no) {
//    var retVal = confirm("Är du säker på att du vill ta bort filen?");
//    if (retVal == true) {
//        document.getElementById(this.de)
//        return true;
//    } else {
//        document.write("User does not want to continue!");
//        return false;
//    }
//}

