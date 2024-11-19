
$(document).ready(function () {
    $('.changePassword').click(function () {
        $("#change-password-modal").modal('show');
    });
    $('#btnChangePasswordCancel').click(function () {
        $("#change-password-modal").modal('hide');
    });

    $('#btnChangePassword').click(function () {
        var currentPassword = $('#currentPassword').val();
        var newPassword = $('#newPassword').val();
        var confirmPassword = $('#confirmPassword').val();

        // Validation
        if (newPassword !== confirmPassword) {
            bootbox.alert("Confirm password don't match!", function () { });
            return;
        }

        // Check user validity
        $.ajax({
            dataType: "json",
            url: '/Security/IsValidUser',
            data: { password: currentPassword },
            method: 'POST',
            //async: false,
            success: function (data) {
                if (data.isSuccess) {
                    $.ajax({
                        dataType: "json",
                        url: '/Security/ChangePassword',
                        data: { newPassword: newPassword },
                        method: 'POST',
                        //async: false,
                        success: function (data) {
                            if (data.isSuccess) {
                                clearForm();
                                bootbox.alert("<h4>Password has been changed sucessfully</h4>", function () { });
                            }
                            else {
                                clearForm();
                                bootbox.alert("<h4>Failed to change the password</h4>", function () { });
                            }
                        },
                        error: function (xhr) {
                            bootbox.alert("<h4>Error occured while changing password!</h4>", function () { });
                        }
                    });
                }
                else {
                    clearForm();
                    bootbox.alert("<h4>Current password is invalid!</h4>", function () { });
                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error occured while checking current password!</h4>", function () { });
            }
        });
    });
});

function clearForm() {
    $('#currentPassword').val('');
    $('#newPassword').val('');
    $('#confirmPassword').val('');
}

