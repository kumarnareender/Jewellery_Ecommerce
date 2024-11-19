
// SERVICE CALL
app.factory('registerService', [
    '$http', function ($http) {

        return {
            registerAccount: function (user) {
                return $http({
                    url: '/Account/RegisterAccount',
                    method: 'POST',
                    data: user
                });
            }
        };
    }
]);
$.ajax({
    dataType: "json",
    url: '/City/GetCities',
    success: function (data) {
        if (data) {
            let html = "";
            html += "<option value='0'>Select City</option>";
            for (let i = 0; i < data.length; i++) {
                html += "<option value='" + data[i].Name + "' postcode='" + data[i].Postcode + "' name='" + data[i].Postcode + "'>" + data[i].Name + " </option>";
            }

            $("#ShipCity").html(html);
            $(".city").html(html);
            //$(".editCity").html(html);
            localStorage.setItem("citiesHtml", html);
        }
    },
    error: function (xhr) {
    }
});
// CONTROLLER
app.controller('RegisterCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'registerService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, registerService) {

    $scope.ShipCountry = 'Australia';
    allowGuestCheckout();

    $scope.numberOnly = function (e) {
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            return;
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    }

    function isGuest() {
        var param = getParam('guest');
        if (param === 'true') {
            return true;
        }
        else {
            return false;
        }
    }

    function allowGuestCheckout() {
        if (isGuest()) {
            $('.row-login-credential').hide();
            $('#btnRegister').html('Continue');
        }
    }

    function showCreateAccountLoader() {
        $('.create-account-loading').show();
    }

    function hideCreateAccountLoader() {
        $('.create-account-loading').hide();
    }

    $scope.login = function () {
        window.location.href = "/Security/Login";
    }
    $("#ShipCity").on("change", function () {
        let postcode = $('option:selected', this).attr('postcode');
        $("#ShipZipCode").val(postcode);
        $scope.ShipZipCode = postcode;

    })
    $scope.registerAccount = function (form) {

        $scope.submitted = true;
        if ($scope.myForm.$invalid) {
            return false;
        }

        // Validation
        if (!isGuest()) {
            if (!$scope.Username) {
                bootbox.alert("<h4>Please enter your username!</h4>", function () { });
                return;
            }
            else if (!$scope.Password) {
                bootbox.alert("<h4>Please enter your password!</h4>", function () { });
                return;
            }
        }
        //if (!$scope.FirstName) {
        //    bootbox.alert("<h4>Please enter your first name!</h4>", function () { });
        //    return;
        //}
        if (!$scope.FirstName) {
            bootbox.alert("<h4>Please enter your first name!</h4>", function () { });
            return;
        }
        else if (!$scope.ShipCountry) {
            bootbox.alert("<h4>Please enter your country!</h4>", function () { });
            return;
        }
        else if (!$scope.ShipState) {
            bootbox.alert("<h4>Please enter your prefecture!</h4>", function () { });
            return;
        }
        else if (!$scope.ShipCity) {
            bootbox.alert("<h4>Please enter your city!</h4>", function () { });
            return;
        }
        else if (!$scope.Mobile) {
            bootbox.alert("<h4>Please enter your mobile number!</h4>", function () { });
            return;
        }
        else if (!$scope.Email) {
            bootbox.alert("<h4>Please enter your email address!</h4>", function () { });
            return;
        }
        else if (!$scope.ShipAddress) {
            bootbox.alert("<h4>Please enter your shipping address!</h4>", function () { });
            return;
        }
        else if (!$scope.ShipZipCode) {
            bootbox.alert("<h4>Please enter your ShipCode!</h4>", function () { });
            return;
        }


        $scope.submitted = true;
        setButtonState('btnRegister', true);
        showCreateAccountLoader();


        var userName = '';
        var password = '';

        if (isGuest()) {
            userName = 'guest_' + Date.now();
            password = 'guest';
        }
        else {
            userName = $scope.Username;
            password = $scope.Password;
        }

        var user = {};
        user["Username"] = userName;
        user["Password"] = password;
        user["FirstName"] = $scope.FirstName;
        user["LastName"] = $scope.LastName;
        user["ShipAddress"] = $scope.ShipAddress;
        user["ShipZipCode"] = $scope.ShipZipCode;
        user["ShipCity"] = $scope.ShipCity;
        user["ShipState"] = $scope.ShipState;
        user["ShipCountry"] = $scope.ShipCountry;
        user["Mobile"] = $scope.Mobile;
        user["Email"] = $scope.Email;
        registerService.registerAccount(user)
            .success(function (data) {
                if (data.isSuccess) {
                    if (isGuest()) {
                        window.location.href = '/Cart?step=confirm-order';
                    }
                    else {
                        window.location.href = '/Customer/Index';
                    }
                } else {
                    if (isGuest()) {
                        bootbox.alert("<h4>Failed to continue!</h4>", function () { });
                    }
                    else {
                        if (data.message) {
                            bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                        }
                        else {
                            bootbox.alert("<h4>Failed to register your account. Please contact with site admin!</h4>", function () { });
                        }
                    }
                }

                setButtonState('btnRegister', false);
                hideCreateAccountLoader();
            })
            .error(function (xhr) {
                bootbox.alert("<h4>Error occured while registering your account. Please contact with site admin!</h4>", function () { });
                setButtonState('btnRegister', false);
                hideCreateAccountLoader();
            });
    };
}]);