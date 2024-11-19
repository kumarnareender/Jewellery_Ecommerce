
function populateActivityLogs(fromDate, toDate, callback) {

    $('.item-loading').show();
    $.ajax({
        dataType: "json",
        url: '/Report/GetActionLogHistory',
        data: { fromDate: fromDate, toDate: toDate },
        success: function (recordSet) {
            $('.item-loading').hide();
            var dataSet = [];
            if (recordSet.length > 0) {

                for (var i = 0; i < recordSet.length; i++) {
                    var record = [];
                    record.push(recordSet[i].Id);
                    record.push(recordSet[i].Module);
                    record.push(recordSet[i].ActionType);
                    record.push(recordSet[i].Description);
                    record.push(recordSet[i].Value);
                    record.push(recordSet[i].ActionDate);
                    record.push(recordSet[i].ActionBy);

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

app.factory('activityLogService', [
    '$http', function ($http) {

        return {
            getBranchList: function () {
                return $http.get('/Branch/GetUserBranchList');
            }
        };
    }
]);

app.controller('ActivityLogCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'activityLogService', function ($rootScope, $scope, $http, $filter, $location, Enum, activityLogService) {

    $('#dateFrom').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });
    $('#dateTo').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });
    
    $scope.showReport = function () {

        var fromDate = $('#dateFrom').val();
        var toDate = $('#dateTo').val();        

        populateActivityLogs(fromDate, toDate, function (records) {
            $('#data-table-activity-logs').dataTable({
                "data": records,
                "pageLength": 100,
                "bDestroy": true,
                "order": [[5, "desc"]],
                "aoColumnDefs": [
                    {
                        "aTargets": [0,1,2],
                        "visible": false
                    },
                    {
                        "aTargets": [5],
                        "render": function (data) {
                            return moment(data, "x").format('DD MMM YYYY h:mm:ss a');
                        }
                    }
                ],

                "columns": [
                    { "title": "Id" },
                    { "title": "Module" },
                    { "title": "Action Type" },
                    { "title": "Description" },
                    { "title": "Value" },
                    { "title": "Action Date", "class": "center" },
                    { "title": "Action By", "class": "center" }
                ]

            });
        });
    }
       
}]);
