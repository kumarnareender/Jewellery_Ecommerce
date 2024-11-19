$(document).ready(function () {
    $('#btnUpdateUserInfo').click(function () {
        var name = $('#name').val();
        var email = $('#email').val();

        $.ajax({
            dataType: "json",
            url: '/Account/UpdateUserInfo',
            data: { name: name, email: email },
            method: 'POST',
            success: function (data) {
                if (data.isSuccess) {
                    bootbox.alert("<h4>User information has been updated sucessfully!</h4>", function () { });
                }
                else {
                    bootbox.alert("<h4>Failed to update!</h4>", function () { });
                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error occured while checking current password!</h4>", function () { });
            }
        });
    });
});

