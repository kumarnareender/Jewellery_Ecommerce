app.factory('promotionService', ['$http', function ($http) {

    return {

        createPromotion: function (promotion) {
            return $http({
                url: '/Promotion/CreatePromotion',
                method: 'POST',
                data: promotion
            })
        },

        updatePromotion: function (promotion) {
            return $http({
                url: '/Promotion/UpdatePromotion',
                method: 'POST',
                data: promotion
            })
        },

        deletePromotion: function (promotion) {
            return $http({
                url: '/Promotion/DeletePromotion',
                method: 'POST',
                data: promotion
            })
        },

        getPromotionList: function (name) {
            return $http.get('/Promotion/GetPromotions?coupon=' + name);
        }
    };

}]);

// CONTROLLER
app.controller('PromotionCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'promotionService', function ($scope, $http, $window, $filter, $location, Enum, promotionService) {

    $('.endDate').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });
    $('.startDate').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });


    getPromotionList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (promotion) {
        $('.endDateEdit').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });
        $('.startDateEdit').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });

        //$(".startDateEdit").val(promotion.StartDate);
        //$(".endDateEdit").val(promotion.EndDate);
        promotion.editMode = !promotion.editMode;
    };

    $scope.createPromotion = function () {

        if (!$scope.promotion.Coupon) {
            bootbox.alert("<h3>Please select coupon name!</h3>", function () { });
            return;
        }

        var promotion = {};
        promotion["Id"] = $scope.promotion.Id;
        promotion["StartDate"] = $(".startDate").val();//$scope.promotion.StartDate;
        promotion["EndDate"] = $(".endDate").val();//$scope.promotion.EndDate;
        promotion["Coupon"] = $scope.promotion.Coupon;
        promotion["Percentage"] = $scope.promotion.Percentage;
    
        promotionService.createPromotion(promotion)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getPromotionList();
                    $scope.promotion.StartDate = '';
                    $scope.promotion.EndDate = '';
                    $scope.promotion.Coupon = '';
                    $scope.promotion.Percentage = '';
                    $scope.toggleAddMode();
                }
            })
            .error(function (xhr) {
                ShowError('Error to saving promotion');
            });
    };

    $scope.updatePromotion = function (promotion) {

        //return false;
        promotionService.updatePromotion(promotion)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getPromotionList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to update records');
            });
    };

    $scope.deletePromotion = function (promotion) {
        var deleteConfirm = $window.confirm('Are you sure to delete promotion "' + promotion.Coupon + '"?');
        if (deleteConfirm) {
            promotionService.deletePromotion(promotion)
                .success(function (data) {
                    if (data && data.IsSuccess) {
                        getPromotionList();
                    }
                })
                .error(function (xhr) {
                    ShowError('Error to delete promotion');
                });
        }
    };

    $scope.addAttr = function (promotion) {
        window.open('/Admin/AttributeConfig?catId=' + promotion.Id + '&catStartDate=' + promotion.StartDate + '&catEndDate=' + promotion.EndDate + '&catCoupon=' + promotion.Coupon + '&catPercentage=' + promotion.Percentage , '_blank');
    };

    function getPromotionList() {
        promotionService.getPromotionList()
            .success(function (data) {

                for (let i = 0; i < data.length; i++) {
                    data[i].EndDate = moment(data[i].EndDate, "x").format('YYYY-MM-DD')
                    data[i].StartDate = moment(data[i].StartDate, "x").format('YYYY-MM-DD')
                }

                $scope.promotionList = data;
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