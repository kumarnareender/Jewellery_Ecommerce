function populateOrderList(branchId, fromDate, toDate, orderMode, orderStatus, callback) {

    $('.item-loading').show();
    $.ajax({
        dataType: "json",
        url: '/Order/GetOrderList',
        data: { branchId: branchId, fromDate: fromDate, toDate: toDate, orderStatus: orderStatus, orderMode: orderMode },
        success: function (recordSet) {
            $('.item-loading').hide();
            $('#delete-btn-container').show();

            var dataSet = [];
            if (recordSet.length > 0) {
                for (var i = 0; i < recordSet.length; i++) {
                    var record = [];
                    record.push(recordSet[i].Id);
                    record.push(recordSet[i].OrderCode);
                    //record.push(recordSet[i].TableNumber);
                    record.push(siteCurrency() + recordSet[i].PayAmount.toFixed(2));
                    record.push(recordSet[i].OrderMode);
                    //record.push(recordSet[i].OrderStatus);
                    //record.push(recordSet[i].PaymentStatus);
                    //record.push(recordSet[i].PaymentType);
                    record.push(recordSet[i].ActionDateString);

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

$(document).ready(function () {

});

app.factory('orderListService', [
    '$http', function ($http) {

        return {
            getBranchList: function () {
                return $http.get('/Branch/GetUserBranchList');
            }
        };
    }
]);

app.controller('AdminOrderListCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'orderListService', function ($rootScope, $scope, $http, $filter, $location, Enum, orderListService) {

    //$.ajax({
    //    dataType: "json",
    //    url: '/Order/GetStatuses',
    //    success: function (result) {
    //        if (result.IsSuccess) {
    //            let html = "";
    //            html += "<option value='0'>Select Status</option>";
    //            for (let i = 0; i < result.data.length; i++) {
    //                html += "<option value='" + result.data[i].Name + "' data-id='" + result.data[i].Id + "'>" + result.data[i].Name + " </option>";
    //            }

    //            $("#orderStatus").html(html);
    //            $('#orderStatus').val("Processing");
    //        }
    //    }
    //});


    $('#orderType').val("Sales")


    getBranchList();
    showOrderRecords();
    $('#dateFrom').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });
    $('#dateTo').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });

    var orderMode = getParam('orderMode');
    if (orderMode) {
        $('.order-mode').removeClass('order-mode-active');
        $('#' + orderMode).addClass('order-mode-active');
    }
    else {
        $('.order-mode').removeClass('order-mode-active');
        $('#All').addClass('order-mode-active');
    }

    function populateOrders(branchId, fromDate, toDate, orderMode, orderStatus) {
        populateOrderList(branchId, fromDate, toDate, orderMode, orderStatus, function (records) {
            $('#data-table-admin-orderlist').dataTable({
                "data": records,
                "bLengthChange": false,
                "bFilter": true,
                "pageLength": 50,
                "bDestroy": true,
                "order": [[1, "desc"]],
                "columns": [
                    { "title": "Delete", "class": "center" },
                    { "title": "Order Code", "class": "center" },
                    //{ "title": "Table Number", "class": "right" },
                    { "title": "Grand Total", "class": "right" },
                    { "title": "Order Mode", "class": "center" },
                    //{ "title": "Order Status", "class": "center" },
                    //{ "title": "Payment Status", "class": "center" },
                    //{ "title": "Payment Type", "class": "center" },
                    { "title": "Order Date" },
                    { "title": "Edit", "class": "center" },
                    { "title": "Payment", "class": "center" },
                    { "title": "Action", "class": "center" },
                    //{ "title": "Payment", "class": "center" }
                ],
                "aoColumnDefs": [
                    {
                        "aTargets": [0],
                        "visible": true,
                        "bSortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<input class="check-box" type="checkbox" id="' + row[0] + '" value="' + row[0] + '">';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [1],
                        "mRender": function (data, type, row) {
                            var text = '<a href=/Cart/Index?OrderId=' + row[1] + '>' + row[1] + '</a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [5],
                        "bSortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<a id=' + row[0] + ' href="/Order/SalesReturn?orderId=' + row[0] + '" class="btn btn-warning btn-sales-return">Edit</a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [6],
                        "bSortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<a id=' + row[0] + ' href="/OrderPaymentStatus/paymentsOrderDetails?orderId=' + row[0] + '&ordertype=order" class="btn btn-info btn-sales-return">Payment</a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [7],
                        "bSortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<a id=' + row[0] + ' class="btn btn-success btn-order-complete">Complete</a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    //{
                    //    "aTargets": [9],
                    //    "bSortable": false,
                    //    "mRender": function (data, type, row) {
                    //        var text = '<a id=' + row[0] + ' href="/Order/SalesReturn?orderId=' + row[0] + '" class="btn btn-warning btn-sales-return">Edit</a>';
                    //        return $("<div/>").append(text).html();
                    //    }
                    //},
                    //{
                    //    "aTargets": [10],
                    //    "bSortable": false,
                    //    "mRender": function (data, type, row) {
                    //        var text = '<a id=' + row[0] + ' href="/OrderPaymentStatus/paymentsOrderDetails?orderId=' + row[0] + '&ordertype=order" class="btn btn-info btn-sales-return">Payment</a>';
                    //        return $("<div/>").append(text).html();
                    //    }
                    //}
                ]
            });

            // Hide the action button column in completed list
            if (orderStatus === 'Completed') {
                var table = $('#data-table-admin-orderlist').DataTable();
                var column = table.column(8);
                column.visible(false);
            }


        });
    }

    function getBranchList() {
        orderListService.getBranchList()
            .success(function (data) {
                for (var i = 0; i < data.length; i++) {
                    $('#branch').append($("<option />").val(data[i].Id).text(data[i].Name));
                }
            })
            .error(function (xhr) { });
    }

    $("#branch").change(function () {
        var val = this.value;
        if (val != "") { //All Branches
            var orderMode = $('#orderMode').val();
            if (orderMode === "Online") {
                $('#orderMode').val('');
            }
        }
    });

    $("#orderMode").change(function () {
        var val = this.value;
        if (val === "Online") {
            $('#branch').val('');
        }
    });

    $('#showOrders').click(function () {
        showOrderRecords();
    });

    function showOrderRecords() {
        var fromDate = $('#dateFrom').val();
        var toDate = $('#dateTo').val();
        var orderMode = $('#orderMode').val();
        var orderType = $('#orderType').val();
        var branchId = $('#branch').val();

        if (!orderType) {
            bootbox.alert("<h4>Please select an order status!</h4>", function () { });
            return;
        }

        populateOrders(branchId, fromDate, toDate, orderMode, orderType);
    }


    $('#btn-delete').click(function () {

        var checkIds = '';
        var count = 0;

        $('.check-box:checkbox:checked').each(function () {
            var id = (this.checked ? $(this).val() : "");

            if (id) {
                checkIds += id + ',';
                count++;
            }
        });

        if (count === 0) {
            bootbox.alert("<h4>Please select an order to delete!</h4>", function () { });
            return;
        }
        else if (count > 100) {
            bootbox.alert("<h4>Please delete maximum 100 orders at a time!</h4>", function () { });
            return;
        }

        bootbox.confirm("<h4>Are you sure to delete selected orders?</h4>",
            function (result) {
                if (result) {
                    DeleteOrder(checkIds, function () {
                    });
                }
            });

    });

    $('#data-table-admin-orderlist').on('click', '.btn-order-complete', function () {

        var orderId = $(this).attr('id');
        var currentTr = $(this).closest("tr");

        bootbox.confirm("<h4>Are you sure to completed this order?</h4>",
            function (result) {
                if (result) {
                    CompleteOrder(orderId, function () {
                        $(currentTr).hide();
                    });
                }
            });
    });

    function CompleteOrder(orderId, callback) {
        $.ajax({
            dataType: "json",
            url: '/Order/CompleteOrder',
            data: { orderId: orderId },
            success: function (data) {
                if (data.isSuccess) {
                    bootbox.alert("<h4>Order marked complete successfully.</h4>", function () { });
                    callback();
                }
                else {
                    bootbox.alert("<h4>Failed to complete this order!</h4>", function () { });
                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error to complete this order!</h4>", function () { });
            }
        });
    }

    function DeleteOrder(ids, callback) {
        $.ajax({
            dataType: "json",
            url: '/Order/DeleteOrder',
            data: { orderIds: ids },
            success: function (data) {

                if (data.message) {
                    bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                }
                else {
                    if (data.deletedOrderCodes) {
                        showOrderRecords();
                    }
                    else {
                        bootbox.alert("<h4>Orders are not deleted!</h4>", function () { });
                    }
                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error to deleting orders!</h4>", function () { });
            }
        });
    }

}]);
