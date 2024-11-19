$(document).ready(function () {

    loadCategoryTree();

    var categoryList = [];

    function loadCategoryTree() {
        $.ajax({
            dataType: "json",
            contentType: 'application/json',
            url: '/Category/GetCategoryTree',
            method: 'GET',
            async: true,
            data: {},
            success: function (recordSet) {
                categoryList = recordSet;
                load_category();
            },
            error: function (xhr) {
            }
        })
    };
        
    $('#oneSelectEditCategory').on('change', function () {
        clearList(1, true);
        getChildList(parseInt(this.value, 10), true);
        if (window.subCatList && window.subCatList.length > 0) {
            populateListControls('twoSelectEditCategory', window.subCatList);
        }
        else {
            setSelectedText(1, true);            
            window.selectedEditCategoryId = this.value;
        }
    });

    $('#twoSelectEditCategory').on('change', function () {
        clearList(2, true);
        getChildList(parseInt(this.value, 10), true);
        if (window.subCatList && window.subCatList.length > 0) {
            populateListControls('threeSelectEditCategory', window.subCatList);
        }
        else {
            setSelectedText(2, true);            
            window.selectedEditCategoryId = this.value;
        }
    });

    $('#threeSelectEditCategory').on('change', function () {
        clearList(3, true);
        getChildList(parseInt(this.value, 10), true);
        if (window.subCatList && window.subCatList.length > 0) {
            populateListControls('fourSelectEditCategory', window.subCatList);
        }
        else {
            setSelectedText(3, true);            
            window.selectedEditCategoryId = this.value;
        }
    });

    $('#fourSelectEditCategory').on('change', function () {
        setSelectedText(4, true);        
        window.selectedEditCategoryId = this.value;
    });    

    $('#btnCategoryContinue').on('click', function () {
        $('.selectedCategoryText').html(window.selectedCategoryText);
        
        $('.cat-edit-section').show();
        $('.cat-continue-section').hide();
        $('.category-picker').hide();
    });

    $('#btnEditCategory').on('click', function () {

        $('.cat-edit-section').hide();
        $('.cat-continue-section').show();
        $('.category-picker').show();        
    });    

    function clearList(number, isCategory) {

        if (isCategory) {
            disableButton('btnCategoryContinue');
            if (number === 1) {
                $('#twoSelectEditCategory').empty();
                $('#threeSelectEditCategory').empty();
                $('#fourSelectEditCategory').empty();
            }
            else if (number === 2) {
                $('#threeSelectEditCategory').empty();
                $('#fourSelectEditCategory').empty();
            }
            else if (number === 3) {
                $('#fourSelectEditCategory').empty();
            }
        }
    }

    function populateListControls(id, items) {
        var list = $("#" + id);
        list.empty();
        list.show();        
        $.each(items, function (index, item) {
            list.append(new Option(item.Name, item.Id));
        });
    }    

    function getRootCategory() {
        var rootCatList = [];
        if (categoryList && categoryList.length > 0) {
            for (var i = 0; i < categoryList.length; i++) {
                rootCatList.push({ id: categoryList[i].Id, name: categoryList[i].Name });
            }
        }
        return rootCatList;
    }

    function getChildList(id, isCategory) {

        if (isCategory) {
            var childCatList = [];
            for (var i = 0; i < categoryList.length; i++) {
                if (categoryList[i].Id === id) {
                    window.subCatList = categoryList[i].ChildCategories;
                    return categoryList[i].ChildCategories;
                } else {
                    childCatList = getChilds(categoryList[i], id, isCategory);
                }
            }
            return childCatList;
        }
        else {
            var childLocList = [];
            for (var i = 0; i < locationList.length; i++) {
                if (locationList[i].Id === id) {
                    window.subLocList = locationList[i].ChildLocations;
                    return locationList[i].ChildLocations;
                } else {
                    childLocList = getChilds(locationList[i], id, isCategory);
                }
            }
            return childLocList;
        }
    }

    function getChilds(obj, id, isCategory) {
        
        if (isCategory) {
            if (obj.Id === id) {
                window.subCatList = obj.ChildCategories;
                return obj.ChildCategories;
            }
            else {
                if (obj.ChildCategories) {
                    for (var i = 0; i < obj.ChildCategories.length; i++) {
                        getChilds(obj.ChildCategories[i], id, isCategory);
                    }
                }
            }
        }
        else {
            if (obj.Id === id) {
                window.subLocList = obj.ChildLocations;
                return obj.ChildLocations;
            }
            else {
                if (obj.ChildLocations) {
                    for (var i = 0; i < obj.ChildLocations.length; i++) {
                        getChilds(obj.ChildLocations[i], id, isCategory);
                    }
                }
            }
        }
    }

    function setSelectedText(controlId, isCategory) {

        if (isCategory) {
            window.selectedCategoryText = '';
            if (controlId === 1) {
                window.selectedCategoryText = $("#oneSelectEditCategory option:selected").text();
            }
            else if (controlId === 2) {
                window.selectedCategoryText = $("#oneSelectEditCategory option:selected").text() + ' / ' + $("#twoSelectEditCategory option:selected").text();
            }
            else if (controlId === 3) {
                window.selectedCategoryText = $("#oneSelectEditCategory option:selected").text() + ' / ' + $("#twoSelectEditCategory option:selected").text() + ' / ' + $("#threeSelectEditCategory option:selected").text();
            }
            else if (controlId === 4) {
                window.selectedCategoryText = $("#oneSelectEditCategory option:selected").text() + ' / ' + $("#twoSelectEditCategory option:selected").text() + ' / ' + $("#threeSelectEditCategory option:selected").text() + ' / ' + $("#fourSelectEditCategory option:selected").text();
            }
            activateButton('btnCategoryContinue');
        }        
    }
    function activateButton(id) {
        $('#' + id).prop("disabled", false);
        $('#' + id).addClass('btn-active');
        $('#' + id).removeClass('btn-disable');
    }
    function disableButton(id) {
        $('#' + id).prop("disabled", true);
        $('#' + id).removeClass('btn-active');
        $('#' + id).addClass('btn-disable');
    }

    function load_category() {
        // Load root categories
        var catItems = getRootCategory();
        var catBox = $("#oneSelectEditCategory");
        $.each(catItems, function (index, item) {
            catBox.append(new Option(item.name, item.id));
        });        
    }

    window.subCatList = [];
    window.selectedCategoryText = '';
    window.selectedEditCategoryId = ''; 
    
   
});

// SERVICE
app.factory('editCategoryService', ['$http', function ($http) {

    return {
        editCategory: function (productId, categoryId) {

            return $http.post('/ProductEntry/UpdateCategory?productId=' + productId + '&categoryId=' + categoryId, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        }
    };

}
]);

// CONTROLLER
app.controller('editCategoryCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'editCategoryService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, editCategoryService) {

    var productId = getParam('id');

    getCategoryName(productId);

    function getCategoryName(productId) {
        $.ajax({
            dataType: "json",
            contentType: 'application/json',
            url: '/Category/GetCategoryName?productId=' + productId,
            method: 'GET',
            async: true,
            data: {},
            success: function (data) {
                $('#catName').html(data);
            },
            error: function (xhr) {
            }
        })
    };

    $scope.updateCategory = function () {

        if (!window.selectedEditCategoryId) {
            bootbox.alert("<h4>Please select a product category!</h4>", function () { });
            return;
        }
        
        var productId = getParam('id');
        var categoryId = parseInt(window.selectedEditCategoryId, 10);
       
        editCategoryService.editCategory(productId, categoryId)
            .success(function (data) {
                
                if (data.isSuccess) {                    
                    window.location.href = "/ProductEntry/PostProductMessage";
                }
                else {
                    if (data.message) {
                        bootbox.alert("<h4>"+ data.message +"</h4>", function () { });
                    }
                    else {
                        bootbox.alert("<h4>Something wrong! Failed to update category</h4>", function () { });
                    }
                }                
            })
            .error(function (exception) {
                bootbox.alert("<h4>Error Occured!!!</h4>", function () { });
            });
    
    }
    
}]);