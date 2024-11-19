$(document).ready(function () {
    var fromRegistration = getParam("fromReg");
    if (fromRegistration === 'yes') {
        $('#statusMsg').show();
    }

    $("#inputExcelFile").change(function () {
        readURL(this);
    });
    $("#btnCsvCancel").click(function () {
        $('#divSaveCsv').hide();
    });



    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            $('#csvName').html(input.files[0].name);

            //reader.onload = function (e) {
            //    $('#csvName').html(e.target.result);
            //}

            reader.readAsDataURL(input.files[0]);

            $('#divSaveCsv').show();
        }
    }
});

// SERVICE
app.factory('productUploadService', ['$http', function ($http) {

    return {
        uploadProducts: function (formData) {
            return $http.post('/ProductUpload/UploadProducts', formData, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        },

        downloadCSV: function () {
            return $http.get('/ProductUpload/DownloadCSV');
        },

    };
}]);

// CONTROLLER
app.controller('productUploadCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'productUploadService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, productUploadService) {



    $scope.selectPhoto = function (files) {
        $scope.memberPhoto = files[0];
    };


    $scope.uploadCSVFile = function (whosPhoto) {
        $scope.submitted = true;

        if ($scope.myForm.$invalid) {
            return false;
        }

        uploadProducts();
    };

    var uploadProducts = function () {
        $('.item-loading').show();
        var file = $scope.memberPhoto;

        $scope.CheckFile(file);
        if (!$scope.IsFileValid) {
            return false;
        }

        var formData = new FormData();
        formData.append("file", file);

        productUploadService.uploadProducts(formData)
            .success(function (data) {
                $('.item-loading').hide();
                if (data.isSuccess) {
                    bootbox.alert("<h4>Products uploaded successfully.</h4>", function () { });

                    $('#divSaveCsv').hide();
                    $("#csvName").text('')
                }
                else {
                    bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                }
            })
            .error(function (exception) {
                $('.item-loading').hide();
                bootbox.alert("<h4>Error while saving your data!</h4>", function () { });
            });

    };
    var downloadCSV = function (reportdata) {
        if (reportdata == "")
            return false;
        //var blob = new Blob([csvString]);
        var blob = new Blob([reportdata]);
        if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveBlob(blob, "Samplecsv.csv");
        }
        else {
            var a = window.document.createElement("a");

            a.href = window.URL.createObjectURL(blob, {
                type: "text/plain"
            });
            a.download = "Samplecsv.csv";
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        }
    }
    //File type(images) validation
    $scope.CheckFile = function (file) {
        $scope.IsFileValid = false;
        if (file != null) {
            if (file.type == 'text/csv') { // limit photo size to 4 mb
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

    $("#downloadCsv").click(function () {

        productUploadService.downloadCSV()
            .success(function (data) {
                if (data) {
                    console.log(data);

                    downloadCSV(data);

                    bootbox.alert("<h4>File downloaded</h4>", function () { });
                }
                else {
                    console.log("else: ", data);

                    bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                }
            })
            .error(function (exception) {
                bootbox.alert("<h4>Error while saving your photo!</h4>", function () { });
            });

        console.log('download csv');
    })
}]);