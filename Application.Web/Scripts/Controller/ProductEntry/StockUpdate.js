

// SERVICE
app.factory('updateStockService', ['$http', function ($http) {

    return {
        updateStock: function (productList) {

            return $http({
                url: '/ProductEntry/PostBulkProduct',
                method: 'POST',
                data: JSON.stringify(productList)
            })
            
        },
        
        getBranchList: function () {
            return $http.get('/Branch/GetUserBranchList');
        },

        getRootCategoryList: function () {
            return $http.get('/Category/GetParentCategoryList');
        }
    };

}]);

// CONTROLLER
app.controller('updateStockCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'updateStockService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, updateStockService) {

    getBranchList();
    //getRootCategoryList();
    
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

    function showHideLoading(show) {
        if (show) {
            $('.post-product-status').show();
            $('#btnPostProduct').attr('disabled', 'disabled');
        }
        else {
            $('.post-product-status').hide();
            $('#btnPostProduct').removeAttr('disabled');
        }
    }

    $scope.changeBranch = function () {
        populateProducts();
    }

    function populateProductList(callback) {

        var branchId = $scope.branchId;
        var categoryId = $scope.categoryId;

        $('.item-loading').show();
        $.ajax({
            dataType: "json",
            url: '/Admin/GetStockList',
            data: { branchId: branchId, categoryId: categoryId },
            success: function (recordSet) {
                $('.item-loading').hide();                
                var dataSet = [];
                if (recordSet.length > 0) {
                    for (var i = 0; i < recordSet.length; i++) {
                        var record = [];
                        record.push(recordSet[i].Id);
                        record.push(recordSet[i].PrimaryImageName);
                        record.push(recordSet[i].Barcode);
                        record.push(recordSet[i].Title);
                        record.push(recordSet[i].Quantity);
                        record.push(recordSet[i].LowStockAlert);
                        record.push(recordSet[i].ExpireDate);
                        
                        dataSet.push(record);
                    }
                }

                callback(dataSet);
            },
            error: function (xhr) {
                $('.item-loading').hide();
            }
        });
    }

    
    function populateProducts() {

        populateProductList(function (records) {
            $('#dt-update-stock').dataTable({
                "data": records,
                "destroy": true,
                "bLengthChange": false,
                "bFilter": true,
                "pageLength": 100,
                "columns": [
                    { "title": "Id" },
                    { "title": "Image", "width": "70px" },
                    { "title": "Barcode" },
                    { "title": "Title" },
                    { "title": "Qty", "class": "center", "width": "150px" },
                    { "title": "Stock Alert", "class": "center" },
                    { "title": "Exp Date", "class": "center" },
                    { "title": "Action", "class": "center", "width": "100px" }
                ],
                "aoColumnDefs": [
                    {
                        "aTargets": [0, 5, 6],
                        "visible": false
                    },
                    {
                        "aTargets": [1],
                        "sortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<a href=/Product/Details/?id=' + row[0] + '>' + '<img alt="" style="width:40px;" src="' + row[1] + '"/> </a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [3],
                        "mRender": function (data, type, row) {
                            var text = '<a href=/Product/Details/?id=' + row[0] + '>' + row[3] + '</a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [4],
                        "sortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<input id="qty-'+ row[0] +'" type="number" class="form-control" style="width:100px; text-align:center;" value="'+ row[4] +'">';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [5],
                        "sortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<input id="lowStock-' + row[0] + '" type="number" class="form-control" style="width:100px; text-align:center;" value="' + row[5] + '">';
                            return $("<div/>").append(text).html();
                        }
                    },
                    
                    {
                        "aTargets": [7],
                        "searchable": false,
                        "sortable": false,
                        "mRender": function (data, type, row) {
                            var buttons = '<div><a title="Update Stock" id=' + row[0] + ' class="btn btn-primary update-stock cursor-pointer"><b>Update</b></a></div>';
                            return $("<div/>").append(buttons).html();
                        }
                    }
                ]
            });
        });

    }

    $('#dt-update-stock').on('click', '.update-stock', function () {
        var productId = $(this).attr('id');
        var quantity = $('#qty-' + productId).val();

        //var currentTr = $(this).closest("tr");

        updateStock(productId, quantity, function () {
            
        });
    });

    function updateStock(productId, quantity, callback) { 
        $.ajax({
            dataType: "json",
            url: '/Admin/UpdateStock',
            data: { productId: productId, quantity: quantity },
            success: function (data) {
                if (!data.IsSuccess) {
                    bootbox.alert("<h4>Sorry, Failed to update stock!</h4>", function () { });                    
                }
                callback();
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error occured while updating stock!</h4>", function () { });
            }
        });
    }



    function getBranchList() {
        updateStockService.getBranchList()
        .success(function (data) {
            $scope.branchList = data;
            if ($scope.branchList && $scope.branchList.length == 1) {                
            }
        })
        .error(function (xhr) {});
    }

    function getRootCategoryList() {
        updateStockService.getRootCategoryList()
        .success(function (data) {
            $scope.categoryList = data;
            if ($scope.categoryList && $scope.categoryList.length == 1) {
                $scope.categoryId = $scope.categoryListList[0].Id;
            }
        })
        .error(function (xhr) { });
    }
        
}]);