function populateRecordSet( callback) {

    $.ajax({
        dataType: "json",
        url: '/UserManagement/GetUserList',
        success: function (recordSet) {
            var dataSet = [];
            if (recordSet.length > 0) {
                for (var i = 0; i < recordSet.length; i++) {
                    var record = [];
                    record.push(recordSet[i].Id);
                    record.push(recordSet[i].Name);                    
                    record.push(recordSet[i].Username);
                    record.push(recordSet[i].Password);
                    record.push(recordSet[i].Code);
                    record.push(recordSet[i].IsActive);
                    record.push(recordSet[i].RoleNames);
                    record.push(recordSet[i].BranchNames);

                    dataSet.push(record);
                }
            }

            callback(dataSet);
        },
        error: function (xhr) {
            var error = xhr;
        }
    });
}

function deleteUser(id, callback) {

    $.ajax({
        dataType: "json",
        url: '/UserManagement/DeleteUser',
        data: { userId: id },
        method: 'POST',
        async: false,
        success: function (data) {
            if (data.isSuccess) {
                bootbox.alert("<h4>User has been deleted sucessfully</h4>", function () { });
                callback();
            }
            else {
                bootbox.alert("<h4>Error occured. Failed to delete this user</h4>", function () { });
            }
        },
        error: function (xhr) {
            bootbox.alert("<h4>Error occured while deleting user!</h4>", function () { });
        }
    });
}

function bindGrid() {
    populateRecordSet(function (records) {
        $('#userMgtList-datatable').dataTable({
            "data": records,
            "pageLength": 50,
            "bDestroy": true,
            "columns": [
                { "title": "Id" },
                { "title": "Name" },
                { "title": "Username" },
                { "title": "Password" },
                { "title": "Code" },
                { "title": "Active", "class": "center" },
                { "title": "Roles" },
                { "title": "Branches" },
                { "title": "Action", "class": "center" }
            ],
            "aoColumnDefs": [
                {
                    "aTargets": [0],
                    "bVisible": false
                },
                {
                    "aTargets": [8],
                    "searchable": false,
                    "mRender": function (data, type, row) {
                        var buttons = '<span title="Edit User" id="' + row[0] + '" class="editBtn glyphicon glyphicon-edit"></span>&nbsp;&nbsp;&nbsp;';
                        buttons += '<span title="Delete User" id="' + row[0] + '" name="' + row[1] + '" class="deleteBtn glyphicon glyphicon-remove cursor-pointer"></span>';
                        return $("<div/>").append(buttons).html();
                    }
                }
            ]
        });
    });
}

app.factory('userManagementListService', [
    '$http', function ($http) {

        return {

        };
    }
]);

app.controller('UserManagementListCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'userManagementListService', function ($rootScope, $scope, $http, $filter, $location, Enum, userManagementListService) {
    
    bindGrid();

    $('#userMgtList-datatable').on('click', '.editBtn', function () {
        var id = $(this).attr('id');
        window.location.href = "/UserManagement/CreateUser?id=" + id;
    });

    $('#userMgtList-datatable').on('click', '.deleteBtn', function () {
        var id = $(this).attr('id');
        var name = $(this).attr('name');
        var currentTr = $(this).closest("tr");

        bootbox.confirm("<h4>Are you sure to delete this user?</h4>",
            function (result) {
                if (result) {
                    deleteUser(id, function () { $(currentTr).remove(); });
                }
            });
    });

}]);