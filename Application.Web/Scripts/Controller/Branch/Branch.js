// SERVICE
app.factory('branchService', ['$http', function ($http) {

    return {

        createBranch: function (branch) {
            return $http({
                url: '/Branch/CreateBranch',
                method: 'POST',
                data: branch
            })
        },

        updateBranch: function (branch) {
            return $http({
                url: '/Branch/UpdateBranch',
                method: 'POST',
                data: branch
            })
        },

        deleteBranch: function (branch) {
            return $http({
                url: '/Branch/DeleteBranch',
                method: 'POST',
                data: branch
            })
        },

        getBranchList: function () {
            return $http.get('/Branch/GetBranchList');
        }
    };

}]);

// CONTROLLER
app.controller('BranchCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'branchService', function ($scope, $http, $window, $filter, $location, Enum, branchService) {

    getBranchList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (branch) {
        branch.editMode = !branch.editMode;
    };

    $scope.createBranch = function () {

        if (!$scope.branch.Name) {
            bootbox.alert("<h3>Please select branch name!</h3>", function () { });
            return;
        }

        var branch = {};
        branch["Id"] = $scope.branch.Id;
        branch["Name"] = $scope.branch.Name;        
        branch["IsAllowOnline"] = $scope.branch.IsAllowOnline;

        branchService.createBranch(branch)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getBranchList();
                $scope.branch.Name = '';                
                $scope.branch.IsAllowOnline = false;
                $scope.toggleAddMode();
            }
        })
        .error(function (xhr) {
            ShowError('Error to saving branch');
        });
    };

    $scope.updateBranch = function (branch) {
        branchService.updateBranch(branch)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getBranchList();
            }
        })
        .error(function (xhr) {
            ShowError('Error to update records');
        });
    };

    $scope.deleteBranch = function (branch) {
        var deleteConfirm = $window.confirm('Are you sure to delete branch "' + branch.Name + '"?');
        if (deleteConfirm) {
            branchService.deleteBranch(branch)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getBranchList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to delete branchs');
            });
        }
    };

    $scope.addAttr = function (branch) {        
        window.open('/Admin/AttributeConfig?catId=' + branch.Id + '&catName=' + branch.Name, '_blank');
    };

    function getBranchList() {
        branchService.getBranchList()
        .success(function (data) {
            $scope.branchList = data;
        })
        .error(function (xhr) {
            ShowError('Error to retrieve Student Class');
        });
    }

    function ShowError(errorText) {
        $('.error-message').show();
        $('.error-list').append('<li>' + errorText + '</li>');
    }
}]);