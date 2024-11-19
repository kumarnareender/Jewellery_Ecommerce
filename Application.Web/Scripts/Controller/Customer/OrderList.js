function populateMemberOrderList(callback) {

    $('.item-loading').show();    
    $.ajax({
        dataType: "json",
        url: '/Customer/GetOrderList',
        data: {},
        success: function (recordSet) {
            $('.item-loading').hide();
            var dataSet = [];
            if (recordSet.length > 0) {
                for (var i = 0; i < recordSet.length; i++) {
                    var record = [];
                    record.push(recordSet[i].Id);
                    record.push(recordSet[i].OrderCode);
                    record.push(siteCurrency() + recordSet[i].PayAmount);
                    record.push(recordSet[i].OrderMode);
                    record.push(recordSet[i].OrderStatus);
                    record.push(recordSet[i].PaymentStatus);
                    record.push(recordSet[i].PaymentType);
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

app.factory('customerOrderListService', [
    '$http', function ($http) {

        return {
            
        };
    }
]);

app.controller('CustomerOrderListCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'customerOrderListService', function ($rootScope, $scope, $http, $filter, $location, Enum, customerOrderListService) {

    populateMemberOrderList(function (records) {
        $('#customerOrderList-datatable').dataTable({
            "data": records,
            "bLengthChange": false,
            "bFilter": true,
            "pageLength": 50,
            "sorting":false,
            "bDestroy": true,            
            "columns": [
                { "title": "Order Id" },
                { "title": "Order Code" },
                { "title": "Grand Total" },
                { "title": "Order Mode", "class": "center" },
                { "title": "Order Status" },
                { "title": "Payment Status", "class": "center" },
                { "title": "Payment Type", "class": "center" },
                { "title": "Order Date" }
            ],
            "aoColumnDefs": [
                {
                    "aTargets": [0],
                    "visible": false
                },
                {
                    "aTargets": [1],
                    "mRender": function (data, type, row) {
                        var text = '<a href=/Customer/OrderDetails?orderId=' + row[0] + '>' + row[1] + '</a>';
                        return $("<div/>").append(text).html();
                    }
                }
            ]
        });
    });

}]);
