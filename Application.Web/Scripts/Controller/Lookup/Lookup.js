// SERVICE
app.factory('lookupService', ['$http', function ($http) {

    return {

        createLookup: function (lookup) {
            return $http({
                url: '/Lookup/CreateLookup',
                method: 'POST',
                data: lookup
            })
        },

        updateLookup: function (lookup) {
            return $http({
                url: '/Lookup/UpdateLookup',
                method: 'POST',
                data: lookup
            })
        },

        deleteLookup: function (lookup) {
            return $http({
                url: '/Lookup/DeleteLookup',
                method: 'POST',
                data: lookup
            })
        },

        getLookupList: function (name) {
            return $http.get('/Lookup/GetLookups?name=' + name);
        }
    };

}]);

// CONTROLLER
app.controller('LookupCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'lookupService', function ($scope, $http, $window, $filter, $location, Enum, lookupService) {

    window.name = getParam('name');
    $('#lookupName').html(window.name);

    getLookupList(window.name);

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (lookup) {
        lookup.editMode = !lookup.editMode;
    };

    $scope.createLookup = function () {

        if (!$scope.lookup.Value) {
            bootbox.alert("<h3>Please select lookup name!</h3>", function () { });
            return;
        }

        var lookup = {};        
        lookup["Name"] = window.name;
        lookup["Value"] = $scope.lookup.Value;

        lookupService.createLookup(lookup)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getLookupList();
                $scope.lookup.Value = '';                                
                $scope.toggleAddMode();
            }
        })
        .error(function (xhr) {
            ShowError('Error to saving lookup');
        });
    };

    $scope.updateLookup = function (lookup) {        
        lookupService.updateLookup(lookup)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getLookupList();
            }
        })
        .error(function (xhr) {
            ShowError('Error to update records');
        });
    };

    $scope.deleteLookup = function (lookup) {
        var deleteConfirm = $window.confirm('Are you sure to delete lookup "' + lookup.Name + '"?');
        if (deleteConfirm) {
            lookupService.deleteLookup(lookup)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getLookupList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to delete lookups');
            });
        }
    };

    function getLookupList() {

        lookupService.getLookupList(window.name)
        .success(function (data) {
            $scope.lookupList = data;
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