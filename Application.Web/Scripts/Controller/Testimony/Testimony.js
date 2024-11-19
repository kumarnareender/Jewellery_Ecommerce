$(document).ready(function () {
    getTestimonies();
    $(document).on("click", "#addTestimony", function () {
        addTestimony();
    });


    //$("#testimonial-slider").owlCarousel({
    //    items: 2,
    //    itemsDesktop: [1000, 2],
    //    itemsDesktopSmall: [979, 2],
    //    itemsTablet: [768, 1],
    //    pagination: true,
    //    autoPlay: false
    //});

});


function addTestimony() {
    let description = $("#testimonyDescription").val();

    $.ajax({
        dataType: "json",
        contentType: 'application/json',
        url: '/Testimony/CreateTestimony',
        data: JSON.stringify({ Description: description }),
        type: 'POST',
        success: function (data) {
            if (data.IsSuccess) {
                bootbox.alert("<h4>Successful!</h4>", function () { });
                //getTestimonies();
                location.reload();
            }
            else {
                hideLoader();
                bootbox.alert("<h4>Failed to save testimony!</h4>", function () { });
            }
            $('#updateStatus').html('');
        },
        error: function (xhr) {
            hideLoader();
            $('#updateStatus').html('');
            bootbox.alert("<h4>Error occured while saving testimony!</h4>", function () { });
        }
    });
}





function getTestimonies() {
    $.ajax({
        dataType: "json",
        contentType: 'application/json',
        url: '/Testimony/GetActiveTestimonies',
        type: 'GET',
        success: function (data) {
            let html = "";
            for (let i = 0; i < data.length; i++) {
                //html += "<div class='center'><p style='font-size:25px;'>" + data[i].Description + "</p></div>"

                html += '<div class="testimonial">'
                    + '<div class="testimonial-content">'
                    + '<div class="testimonial-icon">'
                    + '<i class="fa fa-quote-left"></i>'
                    + '</div>'
                    + '<p class="description">' + data[i].Description + '  </p>'
                    + '</div>'
                    + '<h3 class="title">' + data[i].CreatedByName + '</h3>'
                    + '</div>';
            }

            $("#testimonial-slider").html(html);
            $("#testimonial-slider").owlCarousel({
                items: 3,
                itemsDesktop: [1000, 3],
                itemsDesktopSmall: [980, 2],
                itemsTablet: [768, 2],
                itemsMobile: [650, 1],
                pagination: true,
                navigation: false,
                slideSpeed: 1000,
                autoPlay: true
            });
        },
        error: function (xhr) {
            hideLoader();
            $('#updateStatus').html('');
            bootbox.alert("<h4>Error occured while getting testimonies!</h4>", function () { });
        }
    });
}