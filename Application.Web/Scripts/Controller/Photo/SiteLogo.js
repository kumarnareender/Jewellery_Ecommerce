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
app.factory('siteLogoService', ['$http', function ($http) {

    return {
        saveSiteLogo: function (formData) {
            return $http.post('/Photo/SaveLogo', formData, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        },
        
        deletePhoto: function () {
            return $http({
                url: '/Photo/DeleteSiteLogo',
                method: 'POST',
                data: {}
            });
        }


    };

}
]);

// CONTROLLER
app.controller('siteLogoCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'siteLogoService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, siteLogoService) {

    $scope.IsFileValid = false;

    loadPhotos();

    function loadPhotos() {
    
    };
    
    $scope.selectPhoto = function (files) {
        $scope.siteLogo = files[0];
    };
    
    $scope.deletePhoto = function () {
        bootbox.confirm("<h4>Are you sure to delete this logo?</h4>",
            function (result) {
                if (result) {
                    siteLogoService.deletePhoto()
                        .success(function (data) {
                            if (data.isSuccess) {
                                loadPhotos();
                            }
                            else {
                                bootbox.alert("<h4>Failed to delete this logo!</h4>", function () { });
                            }
                        })
                        .error(function (exception) {
                            bootbox.alert("<h4>Error occured while deleting this logo!</h4>", function () { });
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
        var file = $scope.siteLogo;

        $scope.CheckFile(file);
        if (!$scope.IsFileValid) {
            return false;
        }

        var formData = new FormData();
        formData.append("file", file);

        siteLogoService.saveSiteLogo(formData)
        .success(function (data) {
            if (data.isSuccess) {
                loadPhotos();

                var seconds = new Date().getTime() / 1000;

                $('#imgTemp').attr('src', '/Images/Logo/Logo.png?v=' + seconds);
                $('#divSavePhoto').hide();

            }
            else {
                bootbox.alert("<h4>" + data.message + "</h4>", function () { });
            }
        })
        .error(function (exception) {
            bootbox.alert("<h4>Error while saving site logo!</h4>", function () { });
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