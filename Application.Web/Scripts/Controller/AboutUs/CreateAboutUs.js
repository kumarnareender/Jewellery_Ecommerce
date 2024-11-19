$(document).ready(function () {

    $(document).on("click", "#btnSaveAboutUs", function () {
        addAboutUs();
    })



    $("#aboutUsInputFile").change(function () {
        console.log("input files");
        readURL(this);
    });

    $("#btnPhotoCancel").click(function () {
        $('#imgTemp').attr('src', '/Images/no-image.png');
        $('#divSavePhoto').hide();
    });

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#imgTemp').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);

            $('#divSavePhoto').show();
        }
    }

});


function addAboutUs() {
    let description = $("#aboutUsDescription").val();
    let description1 = $("#aboutUsDescription1").val();
    let description2 = $("#aboutUsDescription2").val();
    let id = $("#aboutUsId").val();
    let images = $("#images").val();

    $.ajax({
        dataType: "json",
        contentType: 'application/json',
        url: '/AboutUs/CreateAboutUs',
        data: JSON.stringify({ Description: description, Description1: description1, Description2: description2, Id: id, Images: images }),
        type: 'POST',
        success: function (data) {
            if (data.IsSuccess) {
                bootbox.alert("<h4>Successful!</h4>", function () { });
                //getTestimonies();
                //location.reload();
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

// SERVICE
app.factory('aboutUsService', ['$http', function ($http) {

    return {
        savePhoto: function (formData) {
            return $http.post('/AboutUs/SavePhoto', formData, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        },

        getAboutUs: function () {
            return $http({
                url: '/AboutUs/GetAboutUs',
                method: 'GET',
                cache: false
            });
        },

        deletePhoto: function (photoId) {
            return $http({
                url: '/AboutUs/DeletePhoto',
                method: 'POST',
                data: { imageId: photoId }
            });
        }


    };

}
]);

// CONTROLLER
app.controller('aboutUsPhotoCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'aboutUsService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, aboutUsService) {

    //var productId = getParam('id');
    $scope.IsFileValid = false;
    
    let imagesVal = "2be5b345-2e4e-4bc1-bd57-f8a790a04d47.jpg,dccc850b-4e10-4001-8af9-90019c674e93.jpg,be653844-6e2f-4554-b049-da212c053d94.jpg";//$("#images").val();
    
    loadAboutUs();

    function loadAboutUs() {
        aboutUsService.getAboutUs()
            .success(function (data) {
                data = data[0];
                let imagesData = [];
                let images = data.Images.split(',');
                $("#aboutUsDescription").val(data.Description);
                $("#aboutUsDescription1").val(data.Description1);
                $("#aboutUsDescription2").val(data.Description2);
                $("#aboutUsId").val(data.Id);

                for (let i = 0; i < images.length; i++) {
                    if (images[i] != "") {
                        imagesData.push({
                            ImageName: images[i],
                            Id: images[i],
                        });
                    }

                }

                $scope.photoList = imagesData;
            })
            .error(function (xhr) {
            });
    };

    $scope.selectPhoto = function (files) {
        $scope.memberPhoto = files[0];
    };
    

    $scope.deleteAboutUsPhoto = function (photoId) {
        bootbox.confirm("<h4>Are you sure to delete this image?</h4>",
            function (result) {
                if (result) {
                    aboutUsService.deletePhoto(photoId)
                        .success(function (data) {
                            if (data.isSuccess) {
                                loadAboutUs();
                            }
                            else {
                                bootbox.alert("<h4>Failed to delete this image!</h4>", function () { });
                            }
                        })
                        .error(function (exception) {
                            bootbox.alert("<h4>Error occured while deleting this image!</h4>", function () { });
                        });
                }
            });
    }

    $scope.uploadAboutUsPhoto = function () {
        $scope.submitted = true;

        if ($scope.myForm.$invalid) {
            return false;
        }

        savePhoto();
    };

    var savePhoto = function () {
        var file = $scope.memberPhoto;

        $scope.CheckFile(file);
        if (!$scope.IsFileValid) {
            return false;
        }

        var formData = new FormData();
        formData.append("file", file);

        aboutUsService.savePhoto(formData)
            .success(function (data) {
                if (data.isSuccess) {
                    loadAboutUs();

                    $('#imgTemp').attr('src', '/Images/no-image.png');
                    $('#divSavePhoto').hide();

                }
                else {
                    bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                }
            })
            .error(function (exception) {
                bootbox.alert("<h4>Error while saving your photo!</h4>", function () { });
            });

    };

    //File type(images) validation
    $scope.CheckFile = function (file) {
        $scope.IsFileValid = false;
        if (file != null) {
            if ((file.type == 'image/gif' || file.type == 'image/png' || file.type == 'image/jpeg') && file.size <= (4096 * 1024)) { // limit photo size to 4 mb
                $scope.FileInvalidMessage = "";
                $scope.IsFileValid = true;
            }
            else {
                $scope.IsFileValid = false;
                bootbox.alert("<h4>Invalid file is selected. (File format must be gif or png or jpeg. Maximum file size 4 mb)</h4>", function () { });
            }
        }
        else {
            bootbox.alert("Please choose product image!", function () { });
        }
    };
}]);
