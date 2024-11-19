$(document).ready(function () {

    window.filterLocIds = '';
    window.locFilters = [];
    window.locFilters.push({ level: 0, id: 0, name: 'All Locations' });

    window.filterCatIds = '';
    window.catFilters = [];
    window.catFilters.push({ level: 0, id: 0, name: 'All Categories' });

    var isLoad = getParam('cat');
    if (isLoad) {
        loadCategoryTree();                
        buildCatSelectionText();
    }
    
    
    var categoryList = [];
    var locationList = [];

    function loadCategoryTree() {

        var loader = $('.category-loader');
        loader.show();

        $.ajax({
            dataType: "json",
            url: '/Category/GetCategoryTree',
            success: function (recordSet) {
                categoryList = recordSet;
                load_category();

                // If search by header search box
                var catId = getParam('cat');
                if (catId && catId !== "0") {
                    var catName = '';
                    for (var i = 0; i < categoryList.length; i++) {
                        var item = categoryList[i];
                        if (item.Id === parseInt(catId, 10)) {
                            catName = (item.name) ? item.name : item.Name;
                            break;
                        }
                    }

                    RenderCategoryById(catId, catName, true);
                }

                loader.hide();
            },
            error: function (xhr) {
                loader.hide();
            }
        });
    }    

    function load_category(catList) {
        if (!catList) {
            catList = getRootCategory();
        }

        $.each(catList, function (index, item) {
            var catId = (item.id) ? item.id : item.Id;
            var catName = (item.name) ? item.name : item.Name;

            $('#category-filter').append('<li><a class="cat-area" href="#" catId="' + catId + '">' + catName + '</a></li>');
        });
    }

    function setFilterIds_category_location(id, isCategory) {

        if (isCategory) {
            window.filterCatIds = '';
            var obj = getCategory(id);
            getChildIds(obj, true);

            if (window.filterCatIds) {
                window.filterCatIds = window.filterCatIds + id;
            }
            else {
                window.filterCatIds = id;
            }
        }
        else {
            window.filterLocIds = '';
            var obj = getLocation(id);
            getChildIds(obj, false);

            if (window.filterLocIds) {
                window.filterLocIds = window.filterLocIds + id;
            }
            else {
                window.filterLocIds = id;
            }
        }
    }

    function RenderCategoryById(catId, catName, isOnLoad) {

        // Set category filter
        setFilterIds_category_location(catId, true);

        // Refine search
        if (!isOnLoad) {
            refineSearch();
        }

        clearList(true);

        getChildList(parseInt(catId, 10), true);
        if (window.subCatList && window.subCatList.length > 0) {
            load_category(window.subCatList);
        }

        window.catFilters.push({ level: window.catFilters.length, id: catId, name: catName });

        buildCatSelectionText();
    }

    function refineSearch() {
        angular.element($('#searchCtl')).scope().refineSearch();
    }        

    $(".cat-area").live("click", function () {
        var catId = $(this).attr('catId');
        var catName = $(this).html();
        
        RenderCategoryById(catId, catName, false);        
    });

    function buildCatSelectionText(catLevel) {

        $('#category-filter-selection').html('');

        var count = 0;
        $.each(window.catFilters, function (index, item) {

            var paddingLeft = count * 10;

            if (catLevel) {
                if (item.level <= parseInt(catLevel, 10)) {
                    $('#category-filter-selection').append('<li class="icon-left-open-big" style="padding-left:' + paddingLeft + 'px; position:relative; left:-5px;" ><a class="cat-area-selection" href="#" catId="' + item.id + '" catLevel="' + item.level + '" >' + item.name + '</a></li>');
                }
            } else {
                $('#category-filter-selection').append('<li class="icon-left-open-big" style="padding-left:' + paddingLeft + 'px; position:relative; left:-5px;" ><a class="cat-area-selection" href="#" catId="' + item.id + '" catLevel="' + item.level + '" >' + item.name + '</a></li>');
            }

            count++;
        });

        $('#category-filter').css('padding-left', (window.catFilters.length * 10) + 12);

        $('#category-filter-selection').removeClass('filter-cat-selected');
        $("li", $('#category-filter-selection')).last().addClass('filter-cat-selected');
    }

    $(".cat-area-selection").live("click", function () {
        var catId = $(this).attr('catId');
        var catLevel = $(this).attr('catLevel');
        var catName = $(this).html();
        
        // Set category filter
        setFilterIds_category_location(catId, true);

        // Refine search
        refineSearch();

        clearList(true);

        if (parseInt(catLevel, 10) === 0) {
            load_category();
        }
        else {
            getChildList(parseInt(catId, 10), true);
            if (window.subCatList && window.subCatList.length > 0) {
                load_category(window.subCatList);
            }
        }

        // Remove useless text
        var newList = [];

        $.each(window.catFilters, function (index, item) {
            if (item.level <= parseInt(catLevel, 10)) {
                newList.push(item);
            }
        });

        if (newList.length === 0) {
            newList.push({ level: 0, id: 0, name: 'All Categories' });
        }

        window.catFilters = newList;

        buildCatSelectionText(catLevel);
    });
    
    $('#itemSortOrder').on('change', function () {
        refineSearch();
    });

    function clearList(isCategory) {

        if (isCategory) {
            $('#category-filter').html('');
            
        } else {
            $('#location-filter').html('');
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
                if (!categoryList[i].ParentId) {
                    rootCatList.push({ id: categoryList[i].Id, name: categoryList[i].Name });
                }
            }
        }
        return rootCatList;
    }

    function getChildIds(obj, isCategory) {

        if (obj) {
            if (isCategory) {
                if (obj.ChildCategories && obj.ChildCategories.length > 0) {
                    for (var i = 0; i < obj.ChildCategories.length; i++) {
                        window.filterCatIds += obj.ChildCategories[i].Id + ',';
                        getChildIds(obj.ChildCategories[i], isCategory);
                    }
                }
            }            
        }
    }

    function getCategory(id) {
        for (var i = 0; i < categoryList.length; i++) {
            if (categoryList[i].Id === parseInt(id, 10)) {
                return categoryList[i];
            }
        }
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
    }
    
});


// SERVICE CALL
app.factory('productSearchService', [
    '$http', function ($http) {

        return {
            searchResult: function (searchModel) {
                return $http({
                    url: '/Product/SearchResult',
                    method: 'POST',
                    data: searchModel
                });
            }
        };

    }
]);

// CONTROLLER
app.controller('ProductSearchCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'productSearchService', function ($rootScope, $scope, $http, $filter, $location, Enum, productSearchService) {

    var catId = getParam('cat');    
    var freeText = getParam('search');
    var onlyDiscount = getParam('discount');

    var searchModel = {};
    if (freeText) {
        searchModel.FreeText = freeText;
    }
    if (onlyDiscount) {
        searchModel.OnlyDiscount = true;
    }
    if (catId !== "0") {
        searchModel.CategoryId = catId;
    }
    
    searchModel.PageNo = 1;

    $scope.searchModel = searchModel;    

    // Search profiles
    searchProduct(searchModel, true);

    function Load_SearchItems(productList) {

        $('.grid-item', '#search-item-container').remove();
        for (var i = 0; i < productList.length; i++) {
           
            var addToCartAttr = 'productId="' + productList[i].Id + '" name="' + productList[i].Title + '" price="' + productList[i].OnlinePrice + '" imageUrl="' + productList[i].PrimaryImageName + '"';

            var plus_attr = 'id="btnPlus_' + productList[i].Id + '" productId="' + productList[i].Id + '"';
            var minus_attr = 'id="btnMinus_' + productList[i].Id + '" productId="' + productList[i].Id + '"';
            var qty_attr = 'id="txtQty_' + productList[i].Id + '" productId="' + productList[i].Id + '"';

            $('#search-item-container').append(
                '<div class="grid-item"> ' +
                '<div class="div-item-container">' +
                '<a class="item-link-container" href="/Product/Details?id=' + productList[i].Id + '"> ' +
                '<div class="grid-item-image"> ' +
                '<img src="' + productList[i].PrimaryImageName + '" /> ' +
                '</div> ' +
                '<div class="grid-item-info"> ' +
                '<span class="h-p-title center">' + productList[i].Title + '</span> ' +
                '<div class="center"> ' +
                '<span class="old-price">' + productList[i].PriceTextOld + '</span>' + '<span class="h-p-price">' + productList[i].PriceText + '</span>' +
                '</div> ' +
                '<div class="center"> ' +

                '<div class="btn-qty-container">' +
                '<button ' + minus_attr + ' class="btn-minus btn btn-default">-</button>' +
                '<input ' + qty_attr + ' type="text" value="1" class="txtQty form-control" style="    background-color: #ebebeb !important;"> ' +
                '<button ' + plus_attr + ' class="btn-plus btn btn-default">+</button>' +
                '</div>' +

                '<div class="item-basket">' +
                '<img ' + addToCartAttr + ' class="h-cart home-add-to-cart" title="Add to cart" src="/images/basket.png" style="float:right;" />' +
                '</div>' +

                '</div> ' +
                '</div> ' +
                '</a> ' +
                '</div>' +
                '</div> ');
        }            
    }

    $('#search-item-container').on('click', '.btn-plus', function (event) {
        var productId = $(this).attr('productId');
        var qty = $('#txtQty_' + productId).val();
        var newQty = parseInt(qty, 10) + 1;

        $('#txtQty_' + productId).val(newQty);
        event.preventDefault();
    });

    $('#search-item-container').on('click', '.btn-minus', function (event) {
        var productId = $(this).attr('productId');
        var qty = $('#txtQty_' + productId).val();

        var newQty = parseInt(qty, 10) - 1;

        if (newQty >= 1) {
            $('#txtQty_' + productId).val(newQty);
        }

        event.preventDefault();
    });

    $('#search-item-container').on('click', '.home-add-to-cart', function (event) {

        let orderType = localStorage.getItem("OrderType");
        if (orderType != "Online") {
            localStorage.removeItem("cart");
        } else {
            localStorage.setItem("OrderType", "Online")
        }

        var productId = $(this).attr('productId');
        var name = $(this).attr('name');
        var price = $(this).attr('price');
        var imageUrl = $(this).attr('imageUrl');
        var qty = $('#txtQty_' + productId).val();

        addToCart(productId, name, qty, price, imageUrl);
        event.preventDefault();

        animateAddToCart(this);
    });

    $scope.register = function () {
        window.location.href = '/Account/Register';
    }

    $scope.expandCollapse = function (type) {
        if (type === 'price') {
            var container = $('.price-filter');

            if ($(container).is(":visible")) {
                $('.price-icon-showhide').removeClass('icon-minus-3').addClass('icon-plus-3');
            }
            else {
                $('.price-icon-showhide').removeClass('icon-plus-3').addClass('icon-minus-3');
            }

            container.slideToggle();
        }        
        else if (type === 'category') {
            var container = $('.category-filter');
            if ($(container).is(":visible")) {
                $('.category-icon-showhide').removeClass('icon-minus-3').addClass('icon-plus-3');
            }
            else {
                $('.category-icon-showhide').removeClass('icon-plus-3').addClass('icon-minus-3');
            }
            container.slideToggle();
        }        
    }

    function showLoader() {
        $('.search-loading').show();
        $('.search-result').hide();
        $('.no-record-found').hide();
        $('.product-search-result').remove();
    }
    function hideLoader() {
        $('.search-loading').hide();
        $('.search-result').show();
    }
    
    $scope.closeWrapper = function () {
        $('.searchlist-action-wrapper').hide();
    }

    $scope.viewFullProfile = function (userId) {

        // Check user account status
        var userStatus = getUserStatus();
        if (!userStatus.isLoggedIn) {
            $("#loginModal").modal();
            return;
        }
        else if (!userStatus.isVerified) {
            bootbox.alert("<h4>Your account is not verified yet. Its under verification process!</h4>", function () { });
            return;
        }

        // Go to profile details page
        window.location.href = '/Product/Details/?id=' + userId;
    }   

    $scope.priceFilter = function () {
        $scope.refineSearch();
    }

    $scope.refineSearch = function () {
        var searchModel = {};
        searchModel.FreeText = '';
        
        if (window.filterCatIds && window.filterCatIds !== "0") { searchModel.CategoryId = window.filterCatIds; }

        // Sort order
        var sortOrder = $('#itemSortOrder').val();
        searchModel.SortOrder = sortOrder;
               
        // Price
        searchModel.MinPrice = $scope.MinPrice;
        searchModel.MaxPrice = $scope.MaxPrice
        
        searchModel.PageNo = 1;

        $scope.searchModel = searchModel;

        // Search profiles
        searchProduct(searchModel, true);
    }

    function renderSearchResultHeading(totalRecords) {

        var resultText = "";
        var searchText = $('#header-search-box').val();

        if (searchText) {
            resultText = "Total <b>" + totalRecords + "</b> records found for <b>" + searchText + "</b>";
        }
        else {
            resultText = "Total <b>" + totalRecords + "</b> records found";
        }

        $('.search-criteria').html(resultText);
    }

    function searchProduct(searchModel, isRenderPaging) {
        
        $("html, body").animate({ scrollTop: 0 }, "fast");

        showLoader();

        searchModel.IsGetTotalRecord = isRenderPaging;

        productSearchService.searchResult(searchModel)
        .success(function (data) {

            Load_SearchItems(data.recordList);

            $scope.searchResult = data.recordList;
            hideLoader();

            renderSearchResultHeading(data.totalRecords);

            if (data.recordList.length > 0) {                
                if (isRenderPaging) {
                    $('#pagination-ul').remove();
                    $('.pagination-container').html('<ul id="pagination-ul" class="pagination-md"></ul>').show();

                    $('#pagination-ul').twbsPagination({
                        totalPages: data.totalPages,
                        visiblePages: 9,
                        initiateStartPageClick: false,
                        onPageClick: function (event, pageNo) {
                            $scope.searchModel.PageNo = pageNo;
                            searchProduct($scope.searchModel, false);
                        }
                    });
                }
            }
            else {
                $('.pagination-container').hide();
                $('.no-record-found').show();
            }
        })
        .error(function (xhr) {
            hideLoader();
            bootbox.alert("<h4>Error to get search records!</h4>", function () { });
        });
    }
}]);