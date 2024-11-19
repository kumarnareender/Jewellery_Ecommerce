
function populatePaymentReport(branchId, fromDate, toDate, orderMode, orderStatus, callback) {

    $('.item-loading').show();
    $.ajax({
        dataType: "json",
        url: '/Report/GetPaymentReport',
        data: { branchId: branchId, fromDate: fromDate, toDate: toDate, orderMode: orderMode, payment: orderStatus },
        success: function (recordSet) {
            $('.item-loading').hide();
            var dataSet = [];
            if (recordSet.length > 0) {
                console.log("PaymentReport: ", recordSet);
                $('#fotter-sum').show();

                for (var i = 0; i < recordSet.length; i++) {
                    var record = [];
                    record.push(recordSet[i].Id);
                    record.push(recordSet[i].OrderCode);
                    record.push(recordSet[i].OrderMode);
                    record.push(recordSet[i].OrderStatus);
                    record.push(recordSet[i].ActionDateString);
                    record.push(recordSet[i].Payment);
                    record.push(recordSet[i].PaymentType);
                    record.push(recordSet[i].TotalAmount);
                    record.push(recordSet[i].AmountDeposited);
                    record.push(recordSet[i].DueAmount);
                    record.push(recordSet[i].TotalDeposited);

                    //record.push(recordSet[i].TotalCostPrice);
                    //record.push(recordSet[i].ItemTotal);
                    //record.push(recordSet[i].Discount);
                    //record.push(recordSet[i].Vat);
                    //record.push(recordSet[i].ShippingAmount);
                    //record.push(recordSet[i].PayAmount);
                    //record.push(recordSet[i].TotalProfit);

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

app.factory('dailySalesService', [
    '$http', function ($http) {

        return {
            getBranchList: function () {
                return $http.get('/Branch/GetUserBranchList');
            }
        };
    }
]);

app.controller('ReportPaymentCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'dailySalesService', function ($rootScope, $scope, $http, $filter, $location, Enum, dailySalesService) {

    getBranchList();

    $('#dateFrom').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });
    $('#dateTo').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });

    // Get user info
    var userInfo = getUserStatus();
    if (userInfo.isAdmin) {
        window.isAdmin = true;
    }

    function getBranchList() {
        dailySalesService.getBranchList()
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

    let year = new Date().getFullYear();
    let month = new Date().getMonth();

    let startDate = moment(new Date(year, month, 1)).format("YYYY-MM-DD")
    let endDate = moment(new Date(year, month, new Date(year, month + 1, 0).getDate())).format("YYYY-MM-DD")
    $('#dateFrom').val(startDate);
    $('#dateTo').val(endDate);


    getPaymentReport();
    $scope.showReport = function () {
        getPaymentReport();
    }


    function getPaymentReport() {

        var fromDate = $('#dateFrom').val();
        var toDate = $('#dateTo').val();
        var orderMode = $('#orderMode').val();
        var orderStatus = $('#orderPayment').val();
        var branchId = $('#branch').val();

        var isAdmin = window.isAdmin && window.isAdmin === true ? true : false;

        populatePaymentReport(branchId, fromDate, toDate, orderMode, orderStatus, function (records) {
            $('#data-table-payment-report').dataTable({
                "data": records,
                "bLengthChange": false,
                "bFilter": true,
                //"pageLength": 100,
                "paging": false,
                "bDestroy": true,
                "order": [[1, "asc"]],
                "columns": [
                    { "title": "Order Id", "class": "center" },
                    { "title": "Order Code", "class": "center" },
                    { "title": "Order Mode" },
                    { "title": "Order Status" },
                    { "title": "Date" },
                    { "title": "Payment", "class": "right" },
                    { "title": "Payment Type", "class": "right" },
                    { "title": "Total Amount", "class": "right" },
                    { "title": "Amount Deposited", "class": "right" },
                    { "title": "Due Amount", "class": "right" },
                    { "title": "TotalDeposited", "class": "right" },
                    //{ "title": "Sells Profit", "class": "right" }
                ],
                "aoColumnDefs": [
                    {
                        "aTargets": [7, 8, 9, 10],
                        "sorting": false
                    },
                    {
                        "aTargets": [0],
                        "visible": false
                    },
                    {
                        "aTargets": [1],
                        "mRender": function (data, type, row) {
                            var text = '<a href=/Order/OrderDetails?orderId=' + row[0] + '>' + row[1] + '</a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [3],
                        "mRender": function (data, type, row) {

                            var status = row[3];
                            var className = 'order-status-processing';

                            if (status === 'Completed') {
                                className = 'order-status-completed';
                            }
                            else if (status === 'Processing') {
                                className = 'order-status-processing';
                            }

                            var text = '<span class="' + className + '">' + status + '</span>';
                            return $("<div/>").append(text).html();
                        }
                    },
                    {
                        "aTargets": [4],
                        "bSortable": false
                    },
                    {
                        "aTargets": [5, 10],
                        "visible": isAdmin
                    }
                ],
                "fnFooterCallback": function (row, data, start, end, display) {
                    var api = this.api(), data;

                    // Remove the formatting to get integer data for summation
                    var intVal = function (i) {
                        return typeof i === 'string' ?
                            i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                                i : 0;
                    };

                    // Total column 5 ----------------------------------------------
                    //var columnNo5 = 5;

                    //// Total over all pages
                    //var total = api.column(columnNo5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Total over this page
                    //var pageTotal = api.column(columnNo5, { page: 'current' }).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Update footer
                    //$(api.column(columnNo5).footer()).html(pageTotal);

                    //// Total column 6 ----------------------------------------------
                    //var columnNo6 = 6;

                    //// Total over all pages
                    //total = api.column(columnNo6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Total over this page
                    //pageTotal = api.column(columnNo6, { page: 'current' }).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Update footer
                    //$(api.column(columnNo6).footer()).html(pageTotal);

                    //// Total column 7 ----------------------------------------------
                    //var columnNo7 = 7;

                    //// Total over all pages
                    //var total = api.column(columnNo7).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Total over this page
                    //var pageTotal = api.column(columnNo7, { page: 'current' }).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Update footer
                    //$(api.column(columnNo7).footer()).html(pageTotal);


                    // Total column 8 ----------------------------------------------
                    var columnNo8 = 8;

                    // Total over all pages
                    var total = api.column(columnNo8).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    // Total over this page
                    var pageTotal = api.column(columnNo8, { page: 'current' }).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    // Update footer
                    $(api.column(columnNo8).footer()).html(pageTotal);

                    // Total column 9 ----------------------------------------------
                    //var columnNo9 = 9;

                    //// Total over all pages
                    //total = api.column(columnNo9).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Total over this page
                    //pageTotal = api.column(columnNo9, { page: 'current' }).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Update footer
                    //$(api.column(columnNo9).footer()).html(pageTotal);

                    //// Total column 10 ----------------------------------------------
                    //var columnNo10 = 10;

                    //// Total over all pages
                    //total = api.column(columnNo10).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Total over this page
                    //pageTotal = api.column(columnNo10, { page: 'current' }).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Update footer
                    //$(api.column(columnNo10).footer()).html(pageTotal);

                    //// Total column 11 ----------------------------------------------
                    //var columnNo11 = 8;

                    //// Total over all pages
                    //total = api.column(columnNo11).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Total over this page
                    //pageTotal = api.column(columnNo11, { page: 'current' }).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

                    //// Update footer
                    //$(api.column(columnNo11).footer()).html(pageTotal);

                } // end footercallback


            });
        });
    }

}]);
