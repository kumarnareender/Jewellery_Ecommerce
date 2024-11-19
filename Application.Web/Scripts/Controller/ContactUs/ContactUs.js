$(document).ready(function () {


    $("#saveFeedback").on("click", function () {
        saveFeedback();
    });


    $.ajax({
        dataType: "json",
        url: '/Account/GetLoggedInUserAddress',
        success: function (data) {
            if (data) {
                $('#name').val(data.FirstName);
                $('#email').val(data.Email);
            }
        },
        error: function (xhr) {
        }
    });

});

function saveFeedback() {
    //var contactForm = function () {
    if ($('#contactForm').length > 0) {
        $("#contactForm").validate({
            rules: {
                name: "required",
                subject: "required",
                email: {
                    required: true,
                    email: true
                },
                message: {
                    required: true,
                    minlength: 5
                }
            },
            messages: {
                name: "Please enter your name",
                subject: "Please enter your subject",
                email: "Please enter a valid email address",
                message: "Please enter a message"
            },
            /* submit via ajax */

            submitHandler: function (form) {
                var $submit = $('.submitting'),
                    waitText = 'Submitting...';
                let name = $("#name").val();
                let email = $("#email").val();
                let feedback = $("#feedbackMessage").val();

                $.ajax({
                    dataType: "json",
                    contentType: 'application/json',
                    url: '/ContactUs/AddFeedback',
                    data: JSON.stringify({
                        Name: name, Email: email, Description: feedback
                    }),
                    type: 'POST',
                    success: function (data) {
                        if (data.IsSuccess) {
                            bootbox.alert("<h4>Your feedback is send Successfully!</h4>", function () {
                                //location.reload();
                                //$("#name").val('');
                                //$("#email").val('');
                                $("#feedbackMessage").val('');
                            });
                            //getTestimonies();
                        }
                        else {
                            hideLoader();
                            bootbox.alert("<h4>Failed to save feedback!</h4>", function () { });
                        }
                        $('#updateStatus').html('');
                    },
                    error: function (xhr) {
                        hideLoader();
                        $('#updateStatus').html('');
                        bootbox.alert("<h4>Error occured while saving feedback!</h4>", function () { });
                    }
                });
            } // end submitHandler

        });
    }
    //};



















    //$.ajax({
    //    dataType: "json",
    //    contentType: 'application/json',
    //    url: '/ContactUs/AddFeedback',
    //    data: JSON.stringify({
    //        Name: name, Email: email, Description: feedback
    //    }),
    //    type: 'POST',
    //    success: function (data) {
    //        if (data.IsSuccess) {
    //            bootbox.alert("<h4>Your feedback is send Successfully!</h4>", function () {
    //            location.reload();

    //            });
    //            //getTestimonies();
    //        }
    //        else {
    //            hideLoader();
    //            bootbox.alert("<h4>Failed to save feedback!</h4>", function () { });
    //        }
    //        $('#updateStatus').html('');
    //    },
    //    error: function (xhr) {
    //        hideLoader();
    //        $('#updateStatus').html('');
    //        bootbox.alert("<h4>Error occured while saving feedback!</h4>", function () { });
    //    }
    //});

}