$(document).ready(function () {

    allowGuestCheckout();

    function showHideLoading(show) {
        if (show) {
            $('.logging').show();
            $('#username').attr('disabled', 'disabled');
            $('#password').attr('disabled', 'disabled');
            $('#loginBtn').attr('disabled', 'disabled');
            $('.message').removeClass('error-message');
        }
        else {
            $('.logging').hide();
            $('#username').removeAttr('disabled');
            $('#password').removeAttr('disabled');
            $('#loginBtn').removeAttr('disabled');
        }
    }

    function allowGuestCheckout() {
        var param = getParam("returnUrl");
        if (param === '/cart') {
            $('#checkoutGuest').show();
        }
    }

    $("#register").click(function (event) {
        window.location.href = '/Account/Register';
    });

    $("#password").keyup(function (event) {
        if (event.keyCode == 13) {
            $(".login").click();
        }
    });

    $(".login").click(function () {

        var model = {};
        model.Username = $('#username').val();
        model.Password = $('#password').val();
        model.RememberMe = $('#rememberMe').is(":checked") ? true : false;

        showHideLoading(true);

        var returnUrl = getParam('returnUrl');

        $.ajax({
            dataType: "json",
            url: '/Security/Login',
            type: 'POST',
            data: model,
            success: function (data) {
                showHideLoading(false);
                if (data.isSuccess) {

                    if (returnUrl) {
                        window.location.href = returnUrl;
                    }
                    else {
                        window.location.href = data.redirectUrl;
                    }
                }
                else {
                    $('.message').addClass('login-error').html(data.message);
                }
            },
            error: function (xhr) {
                $('.message').addClass('login-error').html('Operation Failed!');
                showHideLoading(false);
            }
        });
    });

});