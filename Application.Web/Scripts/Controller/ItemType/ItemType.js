// SERVICE
app.factory('itemTypeService', ['$http', function ($http) {

    return {

        createItemType: function (itemType) {
            return $http({
                url: '/ItemType/CreateItemType',
                method: 'POST',
                data: itemType
            })
        },

        updateItemType: function (itemType) {
            return $http({
                url: '/ItemType/UpdateItemType',
                method: 'POST',
                data: itemType
            })
        },

        deleteItemType: function (itemType) {
            return $http({
                url: '/ItemType/DeleteItemType',
                method: 'POST',
                data: itemType
            })
        },

        getItemTypeList: function () {
            return $http.get('/ItemType/GetItemTypeList');
        }
    };

}]);

// CONTROLLER
app.controller('ItemTypeCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'itemTypeService', function ($scope, $http, $window, $filter, $location, Enum, itemTypeService) {

    getItemTypeList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (itemType) {
        itemType.editMode = !itemType.editMode;
    };

    $scope.createItemType = function () {

        if (!$scope.itemType.Name) {
            bootbox.alert("<h3>Please select itemType name!</h3>", function () { });
            return;
        }

        var itemType = {};
        itemType["Id"] = $scope.itemType.Id;
        itemType["Name"] = $scope.itemType.Name;                

        itemTypeService.createItemType(itemType)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getItemTypeList();
                $scope.itemType.Name = '';                                
                $scope.toggleAddMode();
            }
        })
        .error(function (xhr) {
            ShowError('Error to saving itemType');
        });
    };

    $scope.updateItemType = function (itemType) {
        itemTypeService.updateItemType(itemType)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getItemTypeList();
            }
        })
        .error(function (xhr) {
            ShowError('Error to update records');
        });
    };

    $scope.deleteItemType = function (itemType) {
        var deleteConfirm = $window.confirm('Are you sure to delete itemType "' + itemType.Name + '"?');
        if (deleteConfirm) {
            itemTypeService.deleteItemType(itemType)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getItemTypeList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to delete itemTypes');
            });
        }
    };

    $scope.addAttr = function (itemType) {        
        window.open('/Admin/AttributeConfig?catId=' + itemType.Id + '&catName=' + itemType.Name, '_blank');
    };

    function getItemTypeList() {
        itemTypeService.getItemTypeList()
        .success(function (data) {
            $scope.itemTypeList = data;
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