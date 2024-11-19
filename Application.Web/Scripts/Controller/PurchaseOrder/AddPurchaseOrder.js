function getBranchList() {
    $.ajax({
        dataType: "json",
        url: '/Branch/GetBranchList',
        success: function (recordSet) {
            $.each(recordSet, function () {
                $('#branchList').append($("<option />").val(this.Id).text(this.Name));
            });
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

function getItemTypeList() {
    $.ajax({
        dataType: "json",
        url: '/ItemType/GetItemTypeList',
        success: function (recordSet) {
            $.each(recordSet, function () {
                $('#itemTypeList').append($("<option />").val(this.Id).text(this.Name));
            });
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

function getSupplierList() {
    $.ajax({
        dataType: "json",
        url: '/Supplier/GetSupplierList',
        success: function (recordSet) {
            $.each(recordSet, function () {
                $('#supplierList').append($("<option />").val(this.Id).text(this.Name));
            });
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

function getCategoryList() {
    $.ajax({
        dataType: "json",
        url: '/Category/GetParentCategoryList',
        success: function (recordSet) {
            $.each(recordSet, function () {
                $('#categoryList').append($("<option />").val(this.Id).text(this.Name));
            });
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

function getLowStockItemCount() {
    $.ajax({
        dataType: "json",
        url: '/Admin/GetLowStockCount',
        success: function (data) {
            $('#lowStockItemCount').html(data);
        },
        error: function (xhr) {

        }
    });
}

function getPurchaseProductList(pageNo, isRenderPaging, callback) {

    var branchId = $('#branchList').val();
    var categoryId = $('#categoryList').val();
    var itemTypeId = $('#itemTypeList').val();
    var attribute = $('#attributeList').val();
    var lowStock = $('#lowStock').val();
    var supplierId = $('#supplierList').val();
    var searchText = $('#txt-search').val();

    $('.item-loading').show();
    blockFilterArea(true);

    $.ajax({
        dataType: "json",
        url: '/Admin/GetAdminProductList',
        data: { pageNo: pageNo, searchText: searchText, branchId: branchId, categoryId: categoryId, itemTypeId: itemTypeId, supplierId: supplierId, attribute: attribute, lowStock: lowStock },
        success: function (data) {
            $('.item-loading').hide();
            blockFilterArea(false);
            var dataSet = [];

            $('#totalRecordFound').html("Total <b>" + data.totalRecords + "</b> records found");
            if (data.recordList.length > 0) {
                for (var i = 0; i < data.recordList.length; i++) {

                    var recordSet = data.recordList[i];

                    var record = [];
                    record.push(recordSet.Id);
                    record.push(recordSet.PrimaryImageName);
                    record.push(recordSet.Barcode);
                    record.push(recordSet.Title);
                    record.push(recordSet.LastReceivedQuantity);
                    record.push(recordSet.Quantity);
                    record.push(recordSet.CostPriceText);
                    record.push(recordSet.SupplierName);
                    record.push(recordSet.WeightText);
                    record.push(recordSet.CostPrice);
                    dataSet.push(record);
                }
            }

            // Render pagination
            if (isRenderPaging) {
                $('#pagination-ul').remove();
                $('.pagination-container').html('<ul id="pagination-ul" class="pagination-md"></ul>').show();

                $('#pagination-ul').twbsPagination({
                    totalPages: data.totalPages,
                    visiblePages: 9,
                    initiateStartPageClick: false,
                    onPageClick: function (event, pageNo) {
                        getProductsForPurchaseOrder(pageNo, false);
                    }
                });
            }

            callback(dataSet);
        },
        error: function (xhr) {
            $('.item-loading').hide();
            blockFilterArea(false);
        }
    });
}

function deleteProduct(productId, callback) {
    $.ajax({
        dataType: "json",
        url: '/Admin/DeleteProduct',
        data: { productId: productId },
        success: function (data) {
            if (data.IsSuccess) {
                bootbox.alert("<h4>product has been deleted sucessfully</h4>", function () { });
                callback();
            }
            else {
                bootbox.alert("<h4>Operation failed!</h4>", function () { });
            }
        },
        error: function (xhr) {
            bootbox.alert("<h4>Error occured while deleting product!</h4>", function () { });
        }
    });
}

function blockFilterArea(isBlock) {

    if (!isBlock) {
        $("#tbl-wrap").css({ top: 0, left: 0, width: 0, height: 0 });
        return;
    }

    var filterLeft = $('.tbl-filter').position().left;
    var filterTop = $('.tbl-filter').position().top;
    var filterWidth = $('.tbl-filter').width();
    var filterHeight = $('.tbl-filter').height();

    $("#tbl-wrap").css({ top: filterTop, left: filterLeft, width: filterWidth, height: filterHeight });
}

function getProductsForPurchaseOrder(pageNo, isRenderPaging) {

    $('#dt-purchaseOrder-Products tbody').empty();
    //$('#dt-myProducts_wrapper').hide();

    var that = this;

    var isLoad = 'all';
    if (isLoad) {
        getPurchaseProductList(pageNo, isRenderPaging, function (records) {

            //$('#dt-myProducts_wrapper').show();

            $('#dt-purchaseOrder-Products').dataTable({
                "data": records,
                "destroy": true,
                "bLengthChange": false,
                "bFilter": true,
                "bSort": false,
                "paging": false,
                "bFilter": false,
                "bInfo": false,
                "pageLength": 10,
                //"fixedColumns": {
                //    leftColumns: 2,
                //    rightColumns: 1
                //},
                "fixedHeader": true,

                dom: "<'row'<'col-sm-6'f><'col-sm-6'p>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-5'i><'col-sm-7'p>>",

                "columns": [
                    { "title": "Id" },
                    { "title": "Image" },
                    { "title": "Barcode" },
                    { "title": "Title" },
                    { "title": "Last Recv Q.", "class": "center" },
                    { "title": "T.Qty", "class": "center" },
                    { "title": "Cost<br/>Price", "class": "right" },
                    { "title": "Supplier" },
                    { "title": "Weight" },
                    { "title": "Qty" },
                    { "title": "Action", "class": "center", "width": "100px" }
                ],
                "aoColumnDefs": [
                    {
                        "aTargets": [0],
                        "visible": false
                    },
                    {
                        "aTargets": [1],
                        "sortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<a href=/ProductEntry/EditPost/?id=' + row[0] + '>' + '<img alt="" style="width:70px;" src="' + row[1] + '"/> </a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [2],
                        "sortable": false
                    },
                    {
                        "aTargets": [3],
                        "mRender": function (data, type, row) {
                            var text = '<a href=/ProductEntry/EditPost/?id=' + row[0] + '>' + row[3] + '</a>';
                            return $("<div/>").append(text).html();
                        }
                    },

                    {
                        "aTargets": [9],
                        "mRender": function (data, type, row) {
                            var text = '<input id="btnQty_' + row[0] + '" value="1" style="width:75px;" type="number" class="form-control" />';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [10],
                        "searchable": false,
                        "sortable": false,
                        "mRender": function (data, type, row) {
                            var buttons = '';
                            buttons += '<div class="ad-action"><a title="Add to cart" cost-price=' + row[9] + ' imageUrl=' + row[1] + ' name="' + row[3] + '" product-id=' + row[0] + ' id=' + row[0] + ' class="purchase-order-add-to-cart cursor-pointer">Add to cart</a></div>';
                            return $("<div/>").append(buttons).html();
                        }
                    }
                ]
            });
        });

    }
}

app.factory('adminAddpurchaseOrderService', [
    '$http', function ($http) {

        return {
        };
    }
]);

app.controller('AdminAddPurchaseOrderCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'adminAddpurchaseOrderService', function ($rootScope, $scope, $http, $filter, $location, Enum, adminAddpurchaseOrderService) {

    //$("#txt-search").focus();
    getLowStockItemCount();
    getBranchList();
    getCategoryList();
    getSupplierList();
    getItemTypeList();
    getProductsForPurchaseOrder(1, true);

    $('#dt-myProducts').on('click', '.delete-product', function () {
        var productId = $(this).attr('id');
        var currentTr = $(this).closest("tr");

        bootbox.confirm("<h4>Are you sure to delete this product?</h4>",
            function (result) {
                if (result) {
                    deleteProduct(productId, function () { $(currentTr).remove(); });
                }
            });
    });

    $('#dt-myProducts').on('click', '.edit-product', function () {
        var productId = $(this).attr('id');
        window.location.href = '/ProductEntry/EditPost?id=' + productId;
    });

    $('#dt-myProducts').on('click', '.edit-category', function () {
        var productId = $(this).attr('id');
        window.location.href = '/ProductEntry/EditCategory?id=' + productId;
    });

    $('#dt-myProducts').on('click', '.manage-photo', function () {
        var productId = $(this).attr('id');
        window.location.href = '/Photo/ManagePhoto?id=' + productId;
    });

    $('#dt-purchaseOrder-Products').on("click", '.purchase-order-add-to-cart', function () {

        let orderType = localStorage.getItem("OrderType");
        if (orderType == null || orderType != "Purchase") {
            localStorage.removeItem("cart");
            localStorage.setItem("OrderType", "Purchase")
        }
        else {
            localStorage.setItem("OrderType", "Purchase")
        }
        //localStorage.setItem("OrderType")

        let prodId = $(this).attr('product-id');
        let name = $(this).attr('name');
        let qty = $("#btnQty_" + prodId).val();
        if (qty <= 0) {
            bootbox.alert("<h4>Quantity must be greater than 0!</h4>", function () { });
            return 0;
        }
        let costPrice = +$(this).attr('cost-price');
        let imageUrl = $(this).attr('imageUrl');

        addToCart(prodId, name, qty, costPrice, imageUrl, 0, 0);

        bootbox.alert("<h4>" + name + " added to the cart.</h4>");

        //animateAddToCart(this);
    });


    $('#btn-search-item').on('click', function () {
        var searchText = $('#txt-search').val();
        getProductsForPurchaseOrder(1, true);
    });

    $('#btnFilterProduct').on('click', function () {
        getProductsForPurchaseOrder(1, true);
    });

    $("#branchList").change(function () {
        getProductsForPurchaseOrder(1, true);
    });

    $("#categoryList").change(function () {
        getProductsForPurchaseOrder(1, true);
    });

    $("#supplierList").change(function () {
        getProductsForPurchaseOrder(1, true);
    });

    $("#itemTypeList").change(function () {
        getProductsForPurchaseOrder(1, true);
    });

    $("#attributeList").change(function () {
        getProductsForPurchaseOrder(1, true);
    });

    $("#lowStock").change(function () {
        getProductsForPurchaseOrder(1, true);
    });

    $scope.showHideBarcodeScanner = function () {

        var isVisible = $('#barcode-reader-container').is(":visible");
        if (isVisible) {
            $('#barcode-reader-container').hide();
        }
        else {
            $('#barcode-reader-container').show();

            var html5QrcodeScanner = new Html5QrcodeScanner("barcode-reader", { fps: 10, qrbox: 250 });
            var lastResult, countResults = 0;

            html5QrcodeScanner.render(onScanSuccess, onScanError);

            $('a', '#barcode-reader').hide();

            function onScanSuccess(decodedText, decodedResult) {

                if (decodedText !== lastResult) {
                    lastResult = decodedText;

                    $('#txt-search').val(decodedText);
                    document.getElementById("barcode-reader-container").style.display = "none";

                }
            }

            function onScanError(qrCodeError) {
            }
        }
    }

}]);