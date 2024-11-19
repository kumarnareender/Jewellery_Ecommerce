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
app.factory('categoryPhotoService', ['$http', function ($http) {

    return {
        savePhoto: function (catId, formData) {
            return $http.post('/Category/SaveCategoryPhoto?catId=' + catId, formData, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        },

        getCategoryPhoto: function (catId) {
            return $http({
                url: '/Category/GetCategoryPhoto',
                method: 'GET',
                params: { catId: catId },
                cache: false
            });
        },

        deletePhoto: function (catId) {
            return $http({
                url: '/Category/DeleteCategoryPhoto',
                method: 'POST',
                data: { catId: catId }
            });
        }


    };

}
]);

// CONTROLLER
app.controller('categoryPhotoCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'categoryPhotoService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, categoryPhotoService) {

    var categoryId = getParam('catId');
    $scope.IsFileValid = false;

    loadPhotos();

    function loadPhotos() {
        categoryPhotoService.getCategoryPhoto(categoryId)
            .success(function (data) {
                $scope.ImageName = data.imageName;
            })
            .error(function (xhr) {
            });
    };
    
    $scope.selectPhoto = function (files) {
        $scope.memberPhoto = files[0];
    };

    $scope.deletePhoto = function () {
        bootbox.confirm("<h4>Are you sure to delete this image?</h4>",
            function (result) {
                if (result) {
                    categoryPhotoService.deletePhoto(categoryId)
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

        categoryPhotoService.savePhoto(categoryId, formData)
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
            bootbox.alert("Please choose category image!", function () { });
        }
    };
}]);