
function GetDailySellsChart(branchId, month, callback) {
    $.ajax({
        dataType: "json",
        url: '/Report/GetDailySellsChart?branchId='+ branchId +'&month=' + month,
        success: function (recordSet) {
            var dataSet = [];
            if (recordSet.length > 0) {
                for (var i = 0; i < recordSet.length; i++) {
                    var record = [];
                    record.push(recordSet[i].Day);
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

function getTotalSells(data) {
    var result = [];
    for (var i = 0; i < data.length; i++) {
        result.push(data[i][1]);
    }
    return result;
}

$(document).ready(function () {
   
});

app.factory('dailySalesChartService', [
    '$http', function ($http) {

        return {
            getBranchList: function () {
                return $http.get('/Branch/GetUserBranchList');
            }
        };
    }
]);

app.controller('DailySalesChartCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'dailySalesChartService', function ($rootScope, $scope, $http, $filter, $location, Enum, dailySalesChartService) {

    getBranchList();

    // Populate the years
    var currMonth = new Date().getMonth() + 1;
    currMonth = pad(currMonth, 2);

    $('#month').val(currMonth);
    
    // Populate sales chart
    pupulateSalesChart(currMonth);
    
    $('#branch').on('change', function () {
        pupulateSalesChart();
    });

    $('#month').on('change', function () {
        pupulateSalesChart();
    });

    function pad(str, max) {
        str = str.toString();
        return str.length < max ? pad("0" + str, max) : str;
    }

    function getBranchList() {
        dailySalesChartService.getBranchList()
        .success(function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#branch').append($("<option />").val(data[i].Id).text(data[i].Name));
            }
        })
        .error(function (xhr) { });
    }
    
    function pupulateSalesChart(month) {

        var branchId = $('#branch').val();
        var month = $('#month').val();

        $('.loader').show();
        GetDailySellsChart(branchId, month, function (data) {
            
            $('#daily-sales-chart').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Daily sells chart'
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
                        enabled: false,
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
                    name: 'Total Daily Sells',
                    color: '#283593',
                    data: getTotalSells(data)

                }                
                ]
            });

        });
    }

}]);
