app.factory('cityService', ['$http', function ($http) {

    return {

        createCity: function (city) {
            return $http({
                url: '/City/CreateCity',
                method: 'POST',
                data: city
            })
        },

        updateCity: function (city) {
            return $http({
                url: '/City/UpdateCity',
                method: 'POST',
                data: city
            })
        },

        deleteCity: function (city) {
            return $http({
                url: '/City/DeleteCity',
                method: 'POST',
                data: city
            })
        },

        getCityList: function (name) {
            return $http.get('/City/GetCities?name=' + name);
        }
    };

}]);

// CONTROLLER
app.controller('CityCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'cityService', function ($scope, $http, $window, $filter, $location, Enum, cityService) {

    getCityList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (city) {
        city.editMode = !city.editMode;
    };

    $scope.createCity = function () {

        if (!$scope.city.Name) {
            bootbox.alert("<h3>Please select city name!</h3>", function () { });
            return;
        }

        var city = {};
        city["Id"] = $scope.city.Id;
        city["Name"] = $scope.city.Name;
        city["Postcode"] = $scope.city.Postcode;
        city["ShippingCharge"] = $scope.city.ShippingCharge;
        city["IsAllowOnline"] = $scope.city.IsAllowOnline;

        cityService.createCity(city)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getCityList();
                    $scope.city.Name = '';
                    $scope.city.Postcode = '';
                    $scope.city.ShippingCharge = '';
                    $scope.city.IsAllowOnline = false;
                    $scope.toggleAddMode();
                }
            })
            .error(function (xhr) {
                ShowError('Error to saving city');
            });
    };

    $scope.updateCity = function (city) {
        cityService.updateCity(city)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getCityList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to update records');
            });
    };

    $scope.deleteCity = function (city) {
        var deleteConfirm = $window.confirm('Are you sure to delete city "' + city.Name + '"?');
        if (deleteConfirm) {
            cityService.deleteCity(city)
                .success(function (data) {
                    if (data && data.IsSuccess) {
                        getCityList();
                    }
                })
                .error(function (xhr) {
                    ShowError('Error to delete city');
                });
        }
    };

    $scope.addAttr = function (city) {
        window.open('/Admin/AttributeConfig?catId=' + city.Id + '&catName=' + city.Name + '&catPostcode=' + city.Postcode + '&catShippingCharge=' + city.ShippingCharge, '_blank');
    };

    function getCityList() {
        cityService.getCityList()
            .success(function (data) {
                $scope.cityList = data;
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