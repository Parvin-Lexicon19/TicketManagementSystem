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
                $('<option value="">Välj Lösning</option>'));
            $.each(projects, function (i, project) {
                $("#projectDropDownList").append(
                    $('<option value="' + project.value + '">' + project.text + '</option>'));
            });
        }
    });
}

function deleteit(no) {
    document.getElementById(this.deleteit)
}

