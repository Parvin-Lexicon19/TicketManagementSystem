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
        url: 'GetCustomers',
        type: "POST",
        dataType: "JSON",
        data: { companyId: companyId },
        success: function (customers) {
            $("#customerDropDownList").html("");   //clear before appending new list
            $("#customerDropDownList").append(
                $('<option value="">Välj Användare</option>'));
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
                $('<option value="">--Välj Lösning--</option>'));
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
        url: 'Tickets/GetCompanyProjects',
        type: "POST",
        dataType: "JSON",
        data: { companyId: companyId },
        success: function (projects) {
            $("#projectDDL").html("");   //clear before appending new list
            $("#projectDDL").append(
                $('<option value="">Välj Lösning</option>'));
            $.each(projects, function (i, project) {
                $("#projectDDL").append(
                    $('<option value="' + project.value + '">' + project.text + '</option>'));
            });
        }
    });
}

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
//    $(".keepdata").value = "Hi";
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

