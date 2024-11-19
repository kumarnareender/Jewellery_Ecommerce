// SERVICE
app.factory('stockLocationService', ['$http', function ($http) {

    return {

        createStockLocation: function (stockLocation) {
            return $http({
                url: '/StockLocation/CreateStockLocation',
                method: 'POST',
                data: stockLocation
            })
        },

        updateStockLocation: function (stockLocation) {
            return $http({
                url: '/StockLocation/UpdateStockLocation',
                method: 'POST',
                data: stockLocation
            })
        },

        deleteStockLocation: function (stockLocation) {
            return $http({
                url: '/StockLocation/DeleteStockLocation',
                method: 'POST',
                data: stockLocation
            })
        },

        getStockLocationList: function () {
            return $http.get('/StockLocation/GetStockLocationList');
        }
    };

}]);

// CONTROLLER
app.controller('StockLocationCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'stockLocationService', function ($scope, $http, $window, $filter, $location, Enum, stockLocationService) {

    getStockLocationList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (stockLocation) {
        stockLocation.editMode = !stockLocation.editMode;
    };

    $scope.createStockLocation = function () {

        if (!$scope.stockLocation.Name) {
            bootbox.alert("<h3>Please select stockLocation name!</h3>", function () { });
            return;
        }

        var stockLocation = {};
        stockLocation["Id"] = $scope.stockLocation.Id;
        stockLocation["Name"] = $scope.stockLocation.Name;                

        stockLocationService.createStockLocation(stockLocation)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getStockLocationList();
                $scope.stockLocation.Name = '';                                
                $scope.toggleAddMode();
            }
        })
        .error(function (xhr) {
            ShowError('Error to saving stockLocation');
        });
    };

    $scope.updateStockLocation = function (stockLocation) {
        stockLocationService.updateStockLocation(stockLocation)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getStockLocationList();
            }
        })
        .error(function (xhr) {
            ShowError('Error to update records');
        });
    };

    $scope.deleteStockLocation = function (stockLocation) {
        var deleteConfirm = $window.confirm('Are you sure to delete stockLocation "' + stockLocation.Name + '"?');
        if (deleteConfirm) {
            stockLocationService.deleteStockLocation(stockLocation)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getStockLocationList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to delete stockLocations');
            });
        }
    };

    $scope.addAttr = function (stockLocation) {        
        window.open('/Admin/AttributeConfig?catId=' + stockLocation.Id + '&catName=' + stockLocation.Name, '_blank');
    };

    function getStockLocationList() {
        stockLocationService.getStockLocationList()
        .success(function (data) {
            $scope.stockLocationList = data;
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