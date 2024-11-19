$(document).ready(function () {

    function showHideLoading(show) {
        if (show) {
            $('.status-msg').show();
            $('#name').attr('disabled', 'disabled');
            $('#contactNumber').attr('disabled', 'disabled');
            $('#message').attr('disabled', 'disabled');
            $('#btnSend').attr('disabled', 'disabled');
        }
        else {
            $('.status-msg').hide();
            $('#name').removeAttr('disabled');
            $('#contactNumber').removeAttr('disabled');
            $('#message').removeAttr('disabled');
            $('#btnSend').removeAttr('disabled');
        }
    }

    function clearControl() {
        $('#name').val('');
        $('#contactNumber').val('');
        $('#message').val('');
    }

    $("#btnSend").click(function () {

        var name = $('#name').val();
        var phone = $('#contactNumber').val();
        var message = $('#message').val();

        showHideLoading(true);

        $.ajax({
            dataType: "json",
            url: '/Static/ContactUs',
            type: 'POST',
            data: { name: name, phone: phone, message: message },
            success: function (data) {
                showHideLoading(false);
                if (data.isSuccess) {
                    var hotlineNumber = data.hotlineNumber;
                    bootbox.alert("<h4>Your message has been posted successfully!. We will contact with you shortly. <br/>If it is urgent then you can call us to <b>"+ hotlineNumber +"</b> </h4>", function () { });
                    clearControl();
                }
                else {
                    bootbox.alert("<h4>Failed to post your message!</h4>", function () { });
                }
            },
            error: function (xhr) {
                showHideLoading(false);
            }
        });
    });

});