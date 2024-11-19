// SERVICE
app.factory('supplierService', ['$http', function ($http) {

    return {

        createSupplier: function (supplier) {
            return $http({
                url: '/Supplier/CreateSupplier',
                method: 'POST',
                data: supplier
            })
        },

        updateSupplier: function (supplier) {
            return $http({
                url: '/Supplier/UpdateSupplier',
                method: 'POST',
                data: supplier
            })
        },

        deleteSupplier: function (supplier) {
            return $http({
                url: '/Supplier/DeleteSupplier',
                method: 'POST',
                data: supplier
            })
        },

        getSupplierList: function () {
            return $http.get('/Supplier/GetSupplierList');
        }
    };

}]);

// CONTROLLER
app.controller('SupplierCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'supplierService', function ($scope, $http, $window, $filter, $location, Enum, supplierService) {

    getSupplierList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (supplier) {
        supplier.editMode = !supplier.editMode;
        let html = localStorage.getItem('citiesHtml');
        $("#city_"+supplier.Id).html(html);
        $("#city_" + supplier.Id).val(supplier.City);
    };

    $scope.createSupplier = function () {

        if (!$scope.supplier.Name) {
            bootbox.alert("<h3>Please select supplier name!</h3>", function () { });
            return;
        }

        var supplier = {};
        supplier["Id"] = $scope.supplier.Id;
        supplier["Name"] = $scope.supplier.Name;
        supplier["Contactperson"] = $scope.supplier.Contactperson;
        supplier["Mobile"] = $scope.supplier.Mobile;
        supplier["City"] = $scope.supplier.City;
        supplier["Address"] = $scope.supplier.Address;
        supplier["Email"] = $scope.supplier.Email;


        supplierService.createSupplier(supplier)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getSupplierList();
                    $scope.supplier.Name = '';
                    $scope.toggleAddMode();
                }
            })
            .error(function (xhr) {
                ShowError('Error to saving supplier');
            });
    };

    $scope.updateSupplier = function (supplier) {
        supplierService.updateSupplier(supplier)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getSupplierList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to update records');
            });
    };

    $scope.deleteSupplier = function (supplier) {
        var deleteConfirm = $window.confirm('Are you sure to delete supplier "' + supplier.Name + '"?');
        if (deleteConfirm) {
            supplierService.deleteSupplier(supplier)
                .success(function (data) {
                    if (data && data.IsSuccess) {
                        getSupplierList();
                    }
                })
                .error(function (xhr) {
                    ShowError('Error to delete suppliers');
                });
        }
    };

    $scope.addAttr = function (supplier) {
        window.open('/Admin/AttributeConfig?catId=' + supplier.Id + '&catName=' + supplier.Name, '_blank');
    };

    function getSupplierList() {
        supplierService.getSupplierList()
            .success(function (data) {
                console.log("Supplier: ", data);
                $scope.supplierList = data;
             
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