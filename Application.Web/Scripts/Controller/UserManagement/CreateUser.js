
app.factory('roleService', [
    '$http', function ($http) {

        return {
            createUser: function (user) {
                return $http({
                    url: '/UserManagement/CreateUser',
                    method: 'POST',
                    data: user
                });
            },

            getUser: function (id) {
                return $http({
                    url: '/UserManagement/GetUser?userId=' + id,
                    method: 'GET'
                });
            },

            getRoles: function () {
                return $http({
                    url: '/UserManagement/GetManagementRoles',
                    method: 'GET',
                    async: false
                });
            },

            getBranchList: function () {
                return $http({
                    url: '/Branch/GetBranchList',
                    method: 'GET',
                    async: false
                });
            }
        };

    }
]);

// CONTROLLER
function userCtrl($rootScope, $scope, $http, $filter, $location, Enum, roleService) {
    
    populateRoleList();
    populateBranchList();

    $scope.IsActive = true;

    // Populate student info for edit mode
    if (isEditModel()) {
        var userId = getParam("id");
        roleService.getUser(userId)
            .success(function (data) {
                populateUser(data);
            })
            .error(function (xhr) {
                bootbox.alert("<h4>User not found!</h4>", function () { });
            });
    }

    function getPermissionNames() {

        var valueChecked = '';

        if ($('#ckbProduct').is(':checked')) {
            valueChecked += 'product-m;';
        }
        if ($('#ckbAddProduct').is(':checked')) {
            valueChecked += 'addProduct-sm;';
        }
        if ($('#ckbProductList').is(':checked')) {
            valueChecked += 'productList-sm;';
        }
        if ($('#ckbPurchase').is(':checked')) {
            valueChecked += 'purchase-m;';
        }
        if ($('#ckbCustomer').is(':checked')) {
            valueChecked += 'customer-m;';
        }
        if ($('#ckbManageStock').is(':checked')) {
            valueChecked += 'manageStock-m;';
        }
        if ($('#ckbOrder').is(':checked')) {
            valueChecked += 'order-m;';
        }
        if ($('#ckbConfiguration').is(':checked')) {
            valueChecked += 'configuration-m;';
        }
        if ($('#ckbReport').is(':checked')) {
            valueChecked += 'report-m;';
        }
        if ($('#ckbSetting').is(':checked')) {
            valueChecked += 'setting-sm;';
        }

        return valueChecked;
    }

    $scope.createUser = function () {

        $scope.submitted = true;

        // Return if form is invalid
        if ($scope.myForm.$invalid) {
            return false;
        }

        // Check role is selected or not
        if (!$scope.SelectedRoles || $scope.SelectedRoles.length === 0) {
            bootbox.alert("<h4>Please select a role!</h4>", function () { });
            return;
        }

        // Ge the permissions
        var permissions = getPermissionNames();

        // User Info
        var user = {};
        user["Id"] = getParam("id");
        user["FirstName"] = $scope.FirstName;
        user["LastName"] = $scope.LastName;
        user["Username"] = $scope.Username;
        user["Password"] = $scope.Password;
        user["Code"] = $scope.Code;
        user["Mobile"] = $scope.Mobile;
        user["Email"] = $scope.Email;
        user["Permissions"] = permissions;
        user["IsActive"] = $scope.IsActive;

        // Roles
        var roles = [];
        for (var i = 0; i < $scope.SelectedRoles.length; i++) {
            roles.push({ Id: $scope.SelectedRoles[i] });
        }
        user["Roles"] = roles;

        // Branches
        var branches = [];
        for (var i = 0; i < $scope.SelectedBranches.length; i++) {
            branches.push({ Id: $scope.SelectedBranches[i] });
        }
        user["Branches"] = branches;

        // Disable the submit button
        setButtonState('btnSave', true);

        // Calling service
        roleService.createUser(user)
            .success(function (data) {
                if (data.isSuccess) {
                    if (isEditModel()) {
                        showAlertMessage(true, "User has been updated successfully");
                    }
                    else {
                        showAlertMessage(true, "User has been created successfully");
                    }

                    window.location.href = '/UserManagement/UserList';

                } else {
                    if (data.message) {
                        bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                    }
                    else {
                        bootbox.alert("<h4>Error Occured!</h4>", function () { });
                    }
                }

                // Enable the submit button
                setButtonState('btnSave', false);
            })
            .error(function (xhr) {
                alert('error in saving data');

                // Enable the submit button
                setButtonState('btnSave', false);
            });
    };

    function clearControls() {
        $scope.FirstName = '';
        $scope.LastName = '';
        $scope.Username = '';
        $scope.Password = '';
        $scope.Mobile = '';
        $scope.Email = '';
        $scope.Code = '';
        $scope.SelectedRoles = [];
    }

    function isEditModel() {
        if (window.location.search !== '') {
            return true;
        } else {
            return false;
        }
    }


    function populateRoleList() {
        roleService.getRoles()
        .success(function (data) {
            $scope.roles = data;
        })
        .error(function (xhr) {
            bootbox.alert("<h4>Error! Failed to load role list!</h4>", function () { });
        });
    }

    function populateBranchList() {
        roleService.getBranchList()
        .success(function (data) {
            $scope.branchList = data;
        })
        .error(function (xhr) {
            bootbox.alert("<h4>Error! Failed to load branch list!</h4>", function () { });
        });
    }

    function populateUser(user) {
        $scope.FirstName = user.FirstName;
        $scope.LastName = user.LastName;
        $scope.Username = user.Username;
        $scope.Password = user.Password;
        $scope.Code = user.Code;
        $scope.Mobile = user.Mobile;
        $scope.Email = user.Email;
        $scope.IsActive = user.IsActive;
        $scope.SelectedRoles = [];
        $scope.SelectedBranches = [];

        populatePermissions(user.Permissions);

       for (var i = 0; i < user.RoleList.length; i++) {
            $scope.SelectedRoles.push(user.RoleList[i].Id);
       }

       for (var i = 0; i < user.BranchList.length; i++) {
           $scope.SelectedBranches.push(user.BranchList[i].Id);
       }

    }

    function populatePermissions(permissions) {

        $('#ckbProduct').prop('checked', false);
        $('#ckbAddProduct').prop('checked', false);
        $('#ckbProductList').prop('checked', false);
        $('#ckbPurchase').prop('checked', false);
        $('#ckbCustomer').prop('checked', false);
        $('#ckbManageStock').prop('checked', false);
        $('#ckbOrder').prop('checked', false);
        $('#ckbConfiguration').prop('checked', false);
        $('#ckbReport').prop('checked', false);
        $('#ckbSetting').prop('checked', false);

        var items = permissions.split(';');
        for (var i = 0; i < items.length; i++) {
            var item = items[i];

            if (item === 'product-m') {
                $('#ckbProduct').prop('checked', true);
            }
            else if (item === 'addProduct-sm') {
                $('#ckbAddProduct').prop('checked', true);
            }
            else if (item === 'productList-sm') {
                $('#ckbProductList').prop('checked', true);
            }
            else if (item === 'purchase-m') {
                $('#ckbPurchase').prop('checked', true);
            }
            else if (item === 'customer-m') {
                $('#ckbCustomer').prop('checked', true);
            }
            else if (item === 'manageStock-m') {
                $('#ckbManageStock').prop('checked', true);
            }
            else if (item === 'order-m') {
                $('#ckbOrder').prop('checked', true);
            }
            else if (item === 'configuration-m') {
                $('#ckbConfiguration').prop('checked', true);
            }
            else if (item === 'report-m') {
                $('#ckbReport').prop('checked', true);
            }
            else if (item === 'setting-sm') {
                $('#ckbSetting').prop('checked', true);
            }
        }
    }
}