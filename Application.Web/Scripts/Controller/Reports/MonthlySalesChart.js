
function GetMonthlySellsChart(branchId, year, callback) {
    $.ajax({
        dataType: "json",
        url: '/Report/GetMonthlySellsChart?branchId='+ branchId +'&year=' + year,
        success: function (recordSet) {
            var dataSet = [];
            if (recordSet.length > 0) {
                for (var i = 0; i < recordSet.length; i++) {
                    var record = [];
                    record.push(recordSet[i].Month);
                    record.push(recordSet[i].TotalStoreSell);
                    record.push(recordSet[i].TotalPhoneOrderSell);
                    record.push(recordSet[i].TotalOnlineSell);
                    record.push(recordSet[i].TotalSell);
                    dataSet.push(record);
                }
            }

            $('.loader').hide();
            callback(dataSet);
        },
        error: function (xhr) {
            $('.loader').hide();
        }
    });
}

function getCategoryData(data) {
    var result = [];
    for (var i = 0; i < data.length; i++) {
        result.push(data[i][0]);
    }

    return result;
}

function getTotalCount(data) {
    var result = [];
    for (var i = 0; i < data.length; i++) {
        result.push(data[i][4]);
    }
    return result;
}

function getStoreSellsAmount(data) {
    var result = [];
    for (var i = 0; i < data.length; i++) {
        result.push(data[i][1]);
    }
    return result;
}

function getPhoneOrderSellsAmount(data) {
    var result = [];
    for (var i = 0; i < data.length; i++) {
        result.push(data[i][2]);
    }
    return result;
}

function getOnlineSellsAmount(data) {
    var result = [];
    for (var i = 0; i < data.length; i++) {
        result.push(data[i][3]);
    }
    return result;
}

$(document).ready(function () {
   
});

app.factory('monthlySalesChartService', [
    '$http', function ($http) {

        return {
            getBranchList: function () {
                return $http.get('/Branch/GetUserBranchList');
            }
        };
    }
]);

app.controller('MonthlySalesChartCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'monthlySalesChartService', function ($rootScope, $scope, $http, $filter, $location, Enum, monthlySalesChartService) {

    getBranchList();

    // Populate the years
    var nowY = new Date().getFullYear();
    for (var y = nowY; y >= 2010; y--) {
        $('#year').append($("<option />").val(y).text(y));
    }

    // Populate sales chart
    pupulateSalesChart(nowY);

    $('#branch').on('change', function () {
        pupulateSalesChart();
    });

    $('#year').on('change', function () {
        pupulateSalesChart();
    });


    function getBranchList() {
        monthlySalesChartService.getBranchList()
        .success(function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#branch').append($("<option />").val(data[i].Id).text(data[i].Name));
            }
        })
        .error(function (xhr) { });
    }
        
    function pupulateSalesChart(year) {

        var branchId = $('#branch').val();
        var year = $('#year').val();

        if (!year) {
            year = new Date().getFullYear();
        }

        $('.loader').show();
        GetMonthlySellsChart(branchId, year, function (data) {
            
            $('#monthly-sales-chart').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Monthly sells chart'
                },
                xAxis: {
                    categories: getCategoryData(data)
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Sales Amount'
                    },
                    stackLabels: {
                        enabled: true,
                        style: {
                            fontWeight: 'bold',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                        }
                    }
                },
                tooltip: {
                    headerFormat: '<b>{point.x}</b><br/>',
                    pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
                },
                plotOptions: {
                    column: {
                        stacking: 'normal',
                        dataLabels: {
                            enabled: true,
                            color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white'
                        }
                    }
                },
                series: [
                {
                    name: 'Online Sells',
                    color: '#e91e63',
                    data: getOnlineSellsAmount(data)

                },
                {
                    name: 'Phone Orders',
                    color: '#2e7d32',
                    data: getPhoneOrderSellsAmount(data)

                },
                {
                    name: 'Store Sells',
                    color: '#3f51b5',
                    data: getStoreSellsAmount(data)

                }
                ]
            });

        });
    }

}]);
