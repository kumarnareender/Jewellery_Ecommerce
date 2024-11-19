

// SERVICE
app.factory('customerListService', ['$http', function ($http) {

    return {
        getCustomerList: function () {
            return $http.get('/Admin/GetCustomerList');
        }
    };

}]);

// CONTROLLER
app.controller('customerListCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'customerListService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, customerListService) {

    populateRecords();

    function populateCustomerList(callback) {

        $('.item-loading').show();
        $.ajax({
            dataType: "json",
            url: '/Admin/GetCustomerList',
            data: { },
            success: function (recordSet) {
                $('.item-loading').hide();                
                var dataSet = [];
                if (recordSet.length > 0) {
                    for (var i = 0; i < recordSet.length; i++) {
                        var record = [];

                        var name = 

                        record.push(recordSet[i].Id);
                        record.push(recordSet[i].Username);
                        record.push(recordSet[i].Password);
                        record.push(recordSet[i].CustomerCode);
                        record.push(recordSet[i].FirstName);
                        record.push(recordSet[i].LastName);                                                
                        record.push(recordSet[i].ShipState);
                        record.push(recordSet[i].ShipZipCode);
                        record.push(recordSet[i].ShipCity);
                        record.push(recordSet[i].ShipAddress);
                        
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
        
    function populateRecords() {

        populateCustomerList(function (records) {
            $('#table-customer-list').dataTable({
                "data": records,
                "destroy": true,
                "bLengthChange": false,
                "bFilter": true,
                "pageLength": 100,
                "columns": [
                    { "title": "Id" },
                    { "title": "Username" },
                    { "title": "Password" },
                    { "title": "Code" },
                    { "title": "First Name" },
                    { "title": "Last Name" },                                        
                    { "title": "State" },
                    { "title": "Postal Code" },
                    { "title": "City" },
                    { "title": "Address" }
                ],
                "aoColumnDefs": [
                    {
                        "aTargets": [0],
                        "visible": false
                    },                    
                    {
                        "aTargets": [10],
                        "searchable": false,
                        "sortable": false,
                        "mRender": function (data, type, row) {
                            var buttons = '<div><a style="width:100px;" title="Edit" id=' + row[0] + ' class="btn btn-primary edit-user cursor-pointer"><b>Edit</b></a></div>';
                            return $("<div/>").append(buttons).html();
                        }
                    }
                ]
            });
        });

    }

    $('#table-customer-list').on('click', '.edit-user', function () {
        var userId = $(this).attr('id');
        window.location.href = '/Admin/CustomerAdd?id=' + userId;
    });
        
}]);