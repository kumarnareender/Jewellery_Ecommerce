// SERVICE
app.factory('categoryService', ['$http', function ($http) {

    return {

        createCategory: function (category) {
            return $http({
                url: '/Category/CreateCategory',
                method: 'POST',
                data: category
            })
        },

        updateCategory: function (category) {
            return $http({
                url: '/Category/UpdateCategory',
                method: 'POST',
                data: category
            })
        },

        deleteCategory: function (category) {
            return $http({
                url: '/Category/DeleteCategory',
                method: 'POST',
                data: category
            })
        },

        getCategoryList: function () {
            return $http.get('/Category/GetCategoryList');
        }
    };

}]);

// CONTROLLER
app.controller('CategoryCtrl', ['$scope', '$http', '$window', '$filter', '$location', 'Enum', 'categoryService', function ($scope, $http, $window, $filter, $location, Enum, categoryService) {

    getCategoryList();

    $scope.addMode = false;

    $scope.toggleAddMode = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.toggleEditMode = function (category) {
        category.editMode = !category.editMode;
    };

    $scope.createCategory = function () {

        if (!$scope.category.Name) {
            bootbox.alert("<h3>Please select category name!</h3>", function () { });
            return;
        }

        var category = {};
        category["Id"] = $scope.category.Id;
        category["Name"] = $scope.category.Name;
        category["ParentId"] = $scope.category.ParentId;
        category["IsPublished"] = $scope.category.IsPublished;
        category["ShowInHomepage"] = $scope.category.ShowInHomepage;
        category["DisplayOrder"] = $scope.category.DisplayOrder;

        categoryService.createCategory(category)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getCategoryList();
                $scope.category.Name = '';
                $scope.category.DisplayOrder = '';
                $scope.category.IsPublished = false;
                $scope.category.HowInHomepage = false;
                $scope.toggleAddMode();
            }
        })
        .error(function (xhr) {
            ShowError('Error to saving category');
        });
    };

    $scope.updateCategory = function (category) {
        categoryService.updateCategory(category)
        .success(function (data) {
            if (data && data.IsSuccess) {
                getCategoryList();
            }
        })
        .error(function (xhr) {
            ShowError('Error to update records');
        });
    };

    $scope.deleteCategory = function (category) {
        var deleteConfirm = $window.confirm('Are you sure to delete category "' + category.Name + '"?');
        if (deleteConfirm) {
            categoryService.deleteCategory(category)
            .success(function (data) {
                if (data && data.IsSuccess) {
                    getCategoryList();
                }
            })
            .error(function (xhr) {
                ShowError('Error to delete categorys');
            });
        }
    };

    $scope.addPhoto = function (category) {        
        window.open('/Category/CategoryPhoto?catId=' + category.Id + '&catName=' + category.Name, '_blank');
    };

    function getCategoryList() {
        categoryService.getCategoryList()
        .success(function (data) {
            $scope.categoryList = data;
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