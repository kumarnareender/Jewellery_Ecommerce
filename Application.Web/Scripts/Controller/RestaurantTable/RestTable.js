// SERVICE
app.factory('tableService', ['$http', function ($http) {

    return {

        createTable: function (table) {
            return $http({
                url: '/RestaurantTables/CreateTable',
                method: 'POST',
                data: table
            })
        },

        //savephoto: function (formData) {
        //    //return $http.post('/RestaurantTables/SavePhoto?tableId=' + tableId, formdata, {
        //    return $http.post('/RestaurantTables/SavePhoto', formData, {
        //        withcredentials: true,
        //        headers: { 'content-type': undefined },
        //        transformrequest: angular.identity
        //    });
        //},

        savephoto: function (tableId, formData) {
            return $http.post('/RestaurantTables/SavePhoto?tableId=' + tableId, formData, {
                //return $http.post('/RestaurantTables/SavePhoto', formData, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        },

        updateTable: function (table) {
            return $http({
                url: '/RestaurantTables/UpdateTable',
                method: 'POST',
                data: table
            })
        },

        deleteTable: function (table) {
            return $http({
                url: '/RestaurantTables/DeleteTable',
                method: 'POST',
                data: table
            })
        },

        getTableList: function () {
            return $http.get('/RestaurantTables/GetRestaurantTables');
        }
    };

}]);

// CONTROLLER
app.controller('RestTableCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'tableService', function ($scope, $http, $window, $filter, $location, Enum, tableService) {

    getTableList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (table) {
        table.editMode = !table.editMode;
    };

    $scope.selectPhoto = function (files) {
        $scope.memberPhoto = files[0];
    };

    function saveTableImage() {
        var file = $scope.memberPhoto;

        //$scope.CheckFile(file);
        //if (!$scope.IsFileValid) {
        //    return false;
        //}

        var formData = new FormData();
        formData.append("file", file);
    }

    $scope.createTable = function () {

        if (!$scope.table.TableNumber) {
            bootbox.alert("<h3>Please enter a table number!</h3>", function () { });
            return;
        }

        var table = {};
        table["Id"] = $scope.table.Id;
        table["TableNumber"] = $scope.table.TableNumber;
        //table["IsAllowOnline"] = $scope.table.IsAllowOnline;

        var file = $scope.memberPhoto;

        $scope.CheckFile(file);
        if (!$scope.IsFileValid) {
            return false;
        }

        var formData = new FormData();
        formData.append("file", file);

        tableService.createTable(table)
            .success(function (data) {
                if (data && data.IsSuccess) {

                    tableService.savephoto(data.Data, formData)
                        .success(function (fdata) {
                            getTableList();
                            $scope.table.TableNumber = '';
                            //$scope.table.IsAllowOnline = false;
                            $scope.toggleAddMode();
                        })
                }
            })
            .error(function (xhr) {
                ShowError('Error to saving table');
            });
    };

    $scope.updateTable = function (table) {

        var file = $scope.memberPhoto;

        $scope.CheckFile(file);
        if (!$scope.IsFileValid) {
            return false;
        }

        var formData = new FormData();
        formData.append("file", file);

        tableService.updateTable(table)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    //getTableList();
                    tableService.savephoto(table.Id, formData)
                        .success(function (fdata) {
                            getTableList();
                            $scope.table.TableNumber = '';
                            //$scope.table.IsAllowOnline = false;
                            $scope.toggleAddMode();
                        })
                }
            })
            .error(function (xhr) {
                ShowError('Error to update records');
            });
    };

    $scope.deleteTable = function (table) {
        var deleteConfirm = $window.confirm('Are you sure to delete table "' + table.TableNumber + '"?');
        if (deleteConfirm) {
            tableService.deleteTable(table)
                .success(function (data) {
                    if (data && data.IsSuccess) {
                        getTableList();
                    }
                })
                .error(function (xhr) {
                    ShowError('Error to delete Table');
                });
        }
    };

    $scope.addAttr = function (table) {
        window.open('/Admin/AttributeConfig?catId=' + table.Id + '&catName=' + table.TableNumber, '_blank');
    };

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

    function getTableList() {
        tableService.getTableList()
            .success(function (data) {
                $scope.tableList = data;
            })
            .error(function (xhr) {
                ShowError('Error to retrieve Student Class');
            });
    }

    function ShowError(errorText) {
        $('.error-message').show();
        $('.error-list').append('<li>' + errorText + '</li>');
    }
}]);