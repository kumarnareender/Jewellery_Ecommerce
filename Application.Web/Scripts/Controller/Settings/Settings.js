// SERVICE CALL
app.factory('settingService', ['$http', function ($http) {

    return {

        createSetting: function (setting) {
            return $http({
                url: '/Setting/CreateSetting',
                method: 'POST',
                data: setting
            })
        },

        updateSetting: function (setting) {
            return $http({
                url: '/Setting/UpdateSetting',
                method: 'POST',
                data: setting
            })
        },

        deleteSetting: function (setting) {
            return $http({
                url: '/Setting/DeleteSetting',
                method: 'POST',
                data: setting
            })
        },

        getSettingList: function () {
            return $http.get('/Setting/GetSettingList');
        },

        getSettingNameList: function () {
            return $http.get('/Setting/GetSettingNameList');
        }
    };

}]);

// CONTROLLER
function SettingCtrl($scope, $window, $http, $filter, $location, Enum, settingService) {

    getSettingNameList();
    getSettingList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (setting) {
        setting.editMode = !setting.editMode;
    };

    $scope.createSetting = function () {
        var setting = {};
        setting["Name"] = $scope.SettingName;
        setting["Value"] = $scope.SettingValue;

        settingService.createSetting(setting)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getSettingList();
                $scope.SettingValue = '';
                $scope.toggleAddMode();
            }
            else {
                bootbox.alert("<h4>This setting is already created!</h4>", function () { });
            }
        })
        .error(function (xhr) {
            bootbox.alert("<h4>Error Occured!</h4>", function () { });
        });
    };

    $scope.updateSetting = function (setting) {
        settingService.updateSetting(setting)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getSettingList();
            }
        })
        .error(function (xhr) {
            bootbox.alert("<h4>Error Occured!</h4>", function () { });
        });
    };

    $scope.deleteSetting = function (setting) {
        var deleteConfirm = $window.confirm('Are you sure to delete setting "' + setting.Name + '"?');
        if (deleteConfirm) {
            settingService.deleteSetting(setting)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getSettingList();
                }
            })
            .error(function (xhr) {
                bootbox.alert("<h4>Error Occured!</h4>", function () { });
            });
        }
    };

    function getSettingList() {
        settingService.getSettingList()
        .success(function (data) {
            $scope.settingList = data;
        })
        .error(function (xhr) {
            ShowError('Error to retrieve setting');
        });
    }

    function getSettingNameList() {
        settingService.getSettingNameList()
        .success(function (data) {
            $scope.settingNameList = data;
        })
        .error(function (xhr) {
            alert('Error to get setting names');
        });
    }
}