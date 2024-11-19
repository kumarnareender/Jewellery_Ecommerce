$(document).ready(function () {
            
});

// SERVICE
app.factory('editPostService', ['$http', function ($http) {

    return {        
        updateProduct: function (product) {
            return $http({
                url: '/ProductEntry/UpdateProduct',
                method: 'POST',
                async: true,
                data: { product: product }
            });
        },

        getProduct: function (id) {
            return $http({
                url: '/Product/GetProduct',
                method: 'GET',
                async: true,
                params: { id: id }
            });
        },

        getBranchList: function () {
            return $http.get('/Branch/GetUserBranchList');
        },

        getSupplierList: function () {
            return $http.get('/Supplier/GetSupplierList');
        },
        getItemtypeList: function () {
            return $http.get('/ItemType/GetItemTypeList');
        },
        getColorList: function () {
            return $http.get('/Lookup/GetLookups?name=color');
        },
        getConditionList: function () {
            return $http.get('/Lookup/GetLookups?name=condition');
        },
        getCapacityList: function () {
            return $http.get('/Lookup/GetLookups?name=capacity');
        },
        getManufacturerList: function () {
            return $http.get('/Lookup/GetLookups?name=manufacturer');
        }
    };

}
]);

// CONTROLLER
app.controller('editPostCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'editPostService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, editPostService) {
 
    var formData = new FormData();
    var productId = getParam('id');

    // Initialize date picker
    $('#expireDate').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });

    // Load branch & supplier list
    getBranchList();
    getSupplierList();
    getItemTypeList();
    getColorList();
    getCapacityList();
    getManufacturerList();
    getConditionList();

    getProduct(productId);

    function getProduct(id) {
        editPostService.getProduct(id)
        .success(function (product) {

            $scope.barcode = product.Barcode
            $scope.branchId = product.BranchId;
            $scope.title = product.Title;
            $scope.shortCode = product.ShortCode;
            $scope.description = product.Description;
            $scope.costPrice = product.CostPrice;

            $scope.retailPrice = parseFloat(product.RetailPrice, 10);
            $scope.wholesalePrice = parseFloat(product.WholesalePrice, 10);
            $scope.onlinePrice = parseFloat(product.OnlinePrice, 10) + parseFloat(product.OnlineDiscount, 10);

            $scope.retailDiscount = product.RetailDiscount;
            $scope.wholesaleDiscount = product.WholesaleDiscount;
            $scope.onlineDiscount = product.OnlineDiscount;
            
            $scope.weight = product.Weight;
            $scope.unit = product.Unit;
            $scope.quantity = product.Quantity;
            $scope.lowStockAlert = product.LowStockAlert;
            $scope.isFrozen = product.IsFrozen;
            $scope.isFeatured = product.IsFeatured;
            $scope.isInternal = product.IsInternal;
            $scope.isFastMoving = product.IsFastMoving;
            $scope.isMainItem = product.IsMainItem;
            $scope.itemTypeId = product.ItemTypeId;
            $scope.supplierId = product.SupplierId;

            $scope.color = product.Color;
            $scope.capacity = product.Capacity;
            $scope.condition = product.Condition;
            $scope.manufacturer = product.Manufacturer;
            $scope.imei = product.IMEI;
            $scope.modelNumber = product.ModelNumber;
            $scope.warrantyPeriod = product.WarrantyPeriod;
            
            $scope.expireDate = $filter("date")(json2JavascriptDate(product.ExpireDate), 'yyyy-MM-dd');


            // If electronics item fields are filled up then show the electronics section
            if ($scope.color || $scope.capacity || $scope.condition || $scope.manufacturer || $scope.imei || $scope.modelNumber || $scope.warrantyPeriod) {
                $("#isElectronicsItem").prop("checked", true);
                $('.electronics-item-body').show();
            }
            
        })
        .error(function (xhr) {
            
        });
    }    

    $scope.numberOnly = function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    }       

    $scope.sellTypeChange = function () {
        if (parseInt($scope.sellType, 10) === 1) {
            $scope.isAuction = false;
        }
        else {
            $scope.isAuction = true;
        }
    }

    $scope.updateProduct = function () {

        $scope.submitted = true;
        if ($scope.myForm.$invalid) {
            return false;
        }

        $scope.shortCode = $('#shortCode').val();

        // Validation
        if (!$scope.branchId) {
            bootbox.alert("<h4>Please select a branch!</h4>", function () { });
            return;
        }
        else if (!$scope.title) {
            bootbox.alert("<h4>Please select product title!</h4>", function () { });
            return;
        }
        else if (!$scope.shortCode) {
            bootbox.alert("<h4>Please select product short code!</h4>", function () { });
            return;
        }
        else if (!$scope.description) {
            bootbox.alert("<h4>Please select product description!</h4>", function () { });
            return;
        }
        else if (!$scope.supplierId) {
            bootbox.alert("<h4>Please select product supplier!</h4>", function () { });
            return;
        }
        //else if (!$scope.itemTypeId) {
        //    bootbox.alert("<h4>Please select product type!</h4>", function () { });
        //    return;
        //}
        else if (!$scope.costPrice) {
            bootbox.alert("<h4>Please enter product cost price!</h4>", function () { });
            return;
        }
        else if (!$scope.retailPrice) {
            bootbox.alert("<h4>Please enter product retail price!</h4>", function () { });
            return;
        }

        var retailPrice = parseFloat($scope.retailPrice, 10);
        var wholesalePrice = !$scope.wholesalePrice ? retailPrice : parseFloat($scope.wholesalePrice, 10);
        var onlinePrice = !$scope.onlinePrice ? retailPrice : parseFloat($scope.onlinePrice, 10);

        var product = {};
        product.Id = productId;        
        product.Barcode = $scope.barcode;
        product.BranchId = $scope.branchId;
        product.Title = $scope.title;
        product.ShortCode = $scope.shortCode;
        product.Description = $scope.description;
        product.CategoryId = parseInt(window.selectedCategoryId, 10);
        product.CostPrice = parseFloat($scope.costPrice, 10);

        product.RetailPrice = retailPrice;
        product.WholesalePrice = wholesalePrice;
        product.OnlinePrice = onlinePrice;

        product.RetailDiscount = !$scope.retailDiscount ? 0 : parseFloat($scope.retailDiscount, 10);
        product.WholesaleDiscount = !$scope.wholesaleDiscount ? 0 : parseFloat($scope.wholesaleDiscount, 10);
        product.OnlineDiscount = !$scope.onlineDiscount ? 0 : parseFloat($scope.onlineDiscount, 10);
        
        product.Weight = !$scope.weight ? 0 : parseFloat($scope.weight, 10);
        product.Unit = $scope.unit;
        product.Quantity = !$scope.quantity ? 0 : parseInt($scope.quantity, 10);
        product.LowStockAlert = !$scope.lowStockAlert ? 0 : parseInt($scope.lowStockAlert, 10);
        product.IsFrozen = !$scope.isFrozen ? 0 : 1;
        product.IsFeatured = !$scope.isFeatured ? 0 : 1;
        product.IsInternal = !$scope.isInternal ? 0 : 1;
        product.IsFastMoving = !$scope.isFastMoving ? 0 : 1;
        product.IsMainItem = !$scope.isMainItem ? 0 : 1;
        product.ItemTypeId = $scope.itemTypeId;
        product.SupplierId = $scope.supplierId;        
        product.ExpireDate = $('#expireDate').val();

        product.Color = $scope.color;
        product.Condition = $scope.condition;
        product.Capacity = $scope.capacity;
        product.Manufacturer = $scope.manufacturer;
        product.IMEI = $scope.imei;
        product.ModelNumber = $scope.modelNumber;
        product.WarrantyPeriod = $scope.warrantyPeriod;

        editPostService.updateProduct(product)
            .success(function (data) {
                if (data.isSuccess) {
                    window.location.href = "/ProductEntry/PostProductMessage";
                }
                else {
                    if (data.message) {
                        bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                    }
                    else {
                        bootbox.alert("<h4>Something wrong! Failed to post the product</h4>", function () { });
                    }                    
                }
            })
            .error(function (exception) {
                bootbox.alert("<h4>Error Occured!!!</h4>", function () { });
            });
    }

    function getStringToDate(ddmmyyyy) {
        var from = ddmmyyyy.split("-");
        var dt = new Date(from[2], from[1] - 1, from[0]);
        return dt;
    }

    function getBranchList() {
        editPostService.getBranchList()
        .success(function (data) {
            $scope.branchList = data;
        })
        .error(function (xhr) {
            ShowError('Error to get branches');
        });
    }

    function getSupplierList() {
        editPostService.getSupplierList()
        .success(function (data) {
            $scope.supplierList = data;
        })
        .error(function (xhr) {
            ShowError('Error to get suppliers');
        });
    }

    function getItemTypeList() {
        editPostService.getItemtypeList()
        .success(function (data) {
            $scope.itemTypeList = data;
        })
        .error(function (xhr) {
            ShowError('Error to get suppliers');
        });
    }

    function getColorList() {
        editPostService.getColorList()
        .success(function (data) {
            $scope.colorList = data;
        })
        .error(function (xhr) {
        });
    }

    function getCapacityList() {
        editPostService.getCapacityList()
        .success(function (data) {
            $scope.capacityList = data;
        })
        .error(function (xhr) {
        });
    }

    function getConditionList() {
        editPostService.getConditionList()
        .success(function (data) {
            $scope.conditionList = data;
        })
        .error(function (xhr) {
        });
    }

    function getManufacturerList() {
        editPostService.getManufacturerList()
        .success(function (data) {
            $scope.manufacturerList = data;
        })
        .error(function (xhr) {
        });
    }
    
    $scope.CheckFile = function (file) {        
        if (file != null) {
            if ((file.type == 'image/png' || file.type == 'image/jpeg') && file.size <= (4096 * 1024)) { // limit photo size to 4 mb
                $scope.FileInvalidMessage = "";
                $scope.IsFileValid = true;
            }
            else {
                $scope.IsFileValid = false;
                bootbox.alert("<h4>Invalid file is selected. (File format must be png or jpeg. Maximum file size 4 mb)</h4>", function () { });
            }
        }        
    };

    $("#isElectronicsItem").change(function () {
        if (this.checked) {
            $('.electronics-item-body').show();
        }
        else {
            $('.electronics-item-body').hide();
        }
    });


}]);