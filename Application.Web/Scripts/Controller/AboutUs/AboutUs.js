$(document).ready(function () {
    getAboutUs();

});

function getAboutUs() {
    $.ajax({
        dataType: "json",
        contentType: 'application/json',
        url: '/AboutUs/GetAboutUs',
        type: 'GET',
        success: function (data) {
            data = data[0];
            if (data == null || data == undefined) {
                //bootbox.alert("<h4>No data available!</h4>", function () { });
                return;
            }
            let html = "<i>" + data.Description + "</i>";

            let imgaesHtml = '';
            let images = data.Images.split(',');
            for (let i = 0; i < images.length; i++) {
                if (images[i] != "")
                    imgaesHtml += '<div class="column">' +
                        ' <div class="card">' +
                        ' <img src="/ProductImages/AboutUs/' + images[i] + '" alt="Jane" style="width:100%">' +
                        '</div>' +
                        '</div>';
            }
            $("#images").html(imgaesHtml);
            $("#description").html(html);
            $("#description1").html("<i>" + data.Description1 + "</i>");
            $("#description2").html("<i>" + data.Description2 + "</i>");

        },
        error: function (xhr) {
            hideLoader();
            $('#updateStatus').html('');
            bootbox.alert("<h4>Error occured while getting data!</h4>", function () { });
        }
    });
}