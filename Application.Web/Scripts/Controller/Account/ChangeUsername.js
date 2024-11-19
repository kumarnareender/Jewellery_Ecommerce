$(document).ready(function () {
    $('#btnChangeUsername').click(function () {

        var username = $('#username').val();

        $.ajax({
            dataType: "json",
            url: '/Account/ChangeUsername',
            data: { username: username },
            method: 'POST',
            async: true,
            success: function (data) {
                if (data.isSuccess) {
                    $('#divChangeUsername').hide();
                    $('#divVerifyMobile').show();

                    var scope = angular.element($("body")).scope();
                    scope.username = username;
                }
                else {
                    bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error occured!</h4>", function () { });
            }
        });
    });
});

// SERVICE CALL
app.factory('changeUsernameService', [
    '$http', function ($http) {

        return {
            verifyToken: function (token) {
                return $http({
                    url: '/Account/VerifyTokenFromChangeUsername',
                    method: 'POST',
                    params: { token: token }
                });
            },

            resendToken: function () {
                return $http({
                    url: '/Account/ResendToken',
                    method: 'POST',
                    params: {}
                });
            }
        };

    }
]);

// CONTROLLER
app.controller('ChangeUsernameCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'changeUsernameService', function ($rootScope, $scope, $http, $filter, $location, Enum, changeUsernameService) {
    
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

    function showVerifyAccountLoader() {
        $('.verify-account-loading').show();
    }

    function hideVerifyAccountLoader() {
        $('.verify-account-loading').hide();
    }

    $scope.resendToken = function () {
        $('.resend-token-loading').show();
        changeUsernameService.resendToken()
            .success(function (data) {
                if (data.isSuccess) {
                } else {
                    bootbox.alert("<h4>Resending token failed!</h4>", function () { });
                }
                $('.resend-token-loading').hide();
            })
            .error(function (xhr) {
                $('.resend-token-loading').hide();
                bootbox.alert("<h4>Error: Resending token failed!</h4>", function () { });
            });
    }

    $scope.VerifyToken = function () {

        if (!$scope.Token) {
            bootbox.alert("<h4>Token is empty!</h4>", function () { });
            return;
        }

        showVerifyAccountLoader()
        setButtonState('btnVerifyToken', true);

        // Calling changeUsernameService
        changeUsernameService.verifyToken($scope.Token)
            .success(function (data) {
                if (data.isSuccess) {
                    window.location.href = '/Admin/Index';
                } else {
                    bootbox.alert("<h4>Invalid Token!</h4>", function () { });
                }

                setButtonState('btnVerifyToken', false);
                hideVerifyAccountLoader();
            })
            .error(function (xhr) {
                bootbox.alert("<h4>Error occured while verifying your token. Please contact with admin!</h4>", function () { });
                setButtonState('btnVerifyToken', false);
                hideVerifyAccountLoader();
            });

    }
}]);

