$(document).ready(function () {

    function showHideLoading(show) {
        if (show) {
            $('.status-msg').show();
            $('#email').attr('disabled', 'disabled');
            $('#btnSendPassword').attr('disabled', 'disabled');
        }
        else {
            $('.status-msg').hide();
            $('#email').removeAttr('disabled');
            $('#btnSendPassword').removeAttr('disabled');
        }
    }

    $(".forgotPassword").click(function () {

        var model = {};
        model.Username = $('#mobile').val();

        showHideLoading(true);

        $.ajax({
            dataType: "json",
            url: '/Security/ForgotPassword',
            type: 'POST',
            data: model,
            success: function (data) {
                showHideLoading(false);
                if (data.isSuccess) {
                    bootbox.alert("<br /><h4>Your password has been send to your email account. Please check your email.</h4><br />", function () { });                    
                }
                else {
                    if (data.message) {
                        bootbox.alert("<h4>"+ data.message +"</h4>", function () { });
                    }
                    else {
                        bootbox.alert("<h4>Failed to send password! Please contact with admin</h4>", function () { });
                    }
                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error occured while sending your password! Please contact with admin</h4>", function () { });
                showHideLoading(false);
            }
        });
    });

});