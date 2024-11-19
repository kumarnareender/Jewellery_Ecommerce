$(document).ready(function () {

    var fromRegistration = getParam("fromReg");
    if (fromRegistration === 'yes') {
        $('#statusMsg').show();
    }

    $("#inputFile").change(function () {
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

// SERVICE
app.factory('photoService', ['$http', function ($http) {

    return {
        savePhoto: function (productId, formData) {
            return $http.post('/Photo/SavePhoto?productId=' + productId, formData, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        },

        getPhotoList: function (productId) {
            return $http({
                url: '/Photo/GetPhotoList',
                method: 'GET',
                params: { productId: productId },
                cache: false
            });
        },

        setPrimaryPhoto: function (productId, photoId) {
            return $http({
                url: '/Photo/SetPrimaryPhoto',
                method: 'POST',
                data: { productId: productId, photoId: photoId }
            });
        },

        deletePhoto: function (photoId) {
            return $http({
                url: '/Photo/DeletePhoto',
                method: 'POST',
                data: { imageId: photoId }
            });
        }


    };

}
]);

// CONTROLLER
app.controller('photoCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'photoService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, photoService) {

    var productId = getParam('id');
    $scope.IsFileValid = false;

    loadPhotos();

    function loadPhotos() {
        photoService.getPhotoList(productId)
            .success(function (data) {
                $scope.photoList = data;
            })
            .error(function (xhr) {
            });
    };
    
    $scope.selectPhoto = function (files) {
        $scope.memberPhoto = files[0];
    };

    $scope.setPrimaryPhoto = function (productId, photoId) {
        photoService.setPrimaryPhoto(productId, photoId)
            .success(function (data) {
                loadPhotos();
            })
            .error(function (exception) {
                
            });
    }

    $scope.deletePhoto = function (photoId) {
        bootbox.confirm("<h4>Are you sure to delete this image?</h4>",
            function (result) {
                if (result) {
                    photoService.deletePhoto(photoId)
                        .success(function (data) {
                            if (data.isSuccess) {
                                loadPhotos();
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

    $scope.uploadPhoto = function (whosPhoto) {
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

        photoService.savePhoto(productId, formData)
        .success(function (data) {
            if (data.isSuccess) {
                loadPhotos();

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