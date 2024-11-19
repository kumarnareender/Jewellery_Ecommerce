$(document).ready(function () {



    $(".top-cat-menu-header").click(function () {
        if ($(".top-cat-menu-container").is(":visible") == true) {
            $('.top-cat-menu-container', this).fadeOut("fast");
        }
        else {
            $('.top-cat-menu-container', this).fadeIn("fast");
        }
    });

    $(".top-cat-menu-container").mouseleave(function () {
        if ($(".top-cat-menu-container").is(":visible") == true) {
            $('.top-cat-menu-container').fadeOut("fast");
        }
    });

});

// SERVICE CALL
app.factory('homePageService', [
    '$http', function ($http) {

        return {

            getHomepageCategoryItems: function () {
                return $http({
                    url: '/Home/GetHomepageCategoryItems',
                    method: 'GET',
                    async: true
                });
            },

            getCategoryWithImage: function () {
                return $http({
                    url: '/Home/GetCategoryWithImage',
                    method: 'GET',
                    async: true
                });
            },

            getHomePage_Products: function (catId) {
                return $http({
                    url: '/Home/GetHomePage_Products',
                    method: 'GET',
                    async: true,
                    data: { categoryId: catId },
                });
            },
            getHomePage_FeaturedItems: function () {
                return $http({
                    url: '/Home/GetHomePage_FeaturedItems',
                    method: 'GET',
                    async: true
                });
            },
            getHomePage_PopularItems: function () {
                return $http({
                    url: '/Home/GetHomePage_PopularItems',
                    method: 'GET',
                    async: true
                });
            },

            getHomePage_NewArrivals: function () {
                return $http({
                    url: '/Home/GetHomePage_NewArrivals',
                    method: 'GET',
                    async: true
                });
            },

            getCategoryList: function () {
                return $http({
                    url: '/Category/GetParentCategoryList',
                    method: 'GET',
                    async: true
                });
            },

            getSliderImageList: function () {
                return $http({
                    url: '/HomeSlider/GetSliderImageList',
                    method: 'GET',
                    async: true
                });
            },

            getCustomerList: function () {
                return $http({
                    url: '/Home/GetCustomerList',
                    method: 'GET',
                    async: true,
                });
            },
            getBranchesList: function () {
                return $http({
                    url: '/Branch/GetUserBranchList',
                    method: 'GET',
                    async: true,
                    //data: { userId: userId }
                });
            }
        };
    }
]);

// CONTROLLER
app.controller('HomeCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'homePageService', function ($rootScope, $scope, $http, $filter, $location, Enum, homePageService) {

    var tableNumber = getParam('tableNumber');
    if (tableNumber != null && tableNumber != "" && tableNumber != undefined) {
        localStorage.setItem("tableNumber", tableNumber);
        localStorage.setItem("isItemGotForCart", "");
    }
    var productList = null;
    renderHomepageCategoryItems();
    renderCustomer();
    populateProducts(0);



    function renderHomepageCategoryItems() {

        homePageService.getHomepageCategoryItems()
            .success(function (itemList) {
                //for (var i = 0; i < itemList.length; i++) {
                //    var item = itemList[i];

                let html = "";
                html += "<option value='0'>Select Category</option>";
                for (let i = 0; i < itemList.length; i++) {
                    html += "<option value='" + itemList[i].CategoryId + "' data-id='" + itemList[i].Title + "'>" + itemList[i].Title + " </option>";
                }

                $("#category").html(html);

                //LoadCategoryItemList(i, item.Title, item.ImageName, item.ProductList);
                //}
            })
            .error(function (xhr) {

            });
    }

    function renderCustomer() {
        homePageService.getCustomerList()
            .success(function (itemList) {
                //for (var i = 0; i < itemList.length; i++) {
                //    var item = itemList[i];

                let html = "";
                html += "<option value='0'>Select Customer</option>";
                for (let i = 0; i < itemList.length; i++) {
                    html += "<option value='" + itemList[i].CategoryId + "' data-id='" + itemList[i].CustName + "'>" + itemList[i].CustName + " </option>";
                }

                $("#customers").html(html);

                //LoadCategoryItemList(i, item.Title, item.ImageName, item.ProductList);
                //}
            })
            .error(function (xhr) {

            });
    }

    //function renderBranches() {
    //    homePageService.getBranchesList()
    //        .success(function (itemList) {
    //            //for (var i = 0; i < itemList.length; i++) {
    //            //    var item = itemList[i];

    //            let html = "";
    //            html += "<option value='0'>Select Branch</option>";
    //            for (let i = 0; i < itemList.length; i++) {
    //                html += "<option value='" + itemList[i].Id + "' data-id='" + itemList[i].Name + "'>" + itemList[i].Name + " </option>";
    //            }

    //            $("#branches").html(html);
    //            let branchId = $("#branch-id").val();
    //            if (branchId == undefined || branchId == null || branchId == '' || branchId == "0")
    //                $("#branchModal").modal("show");
    //            //LoadCategoryItemList(i, item.Title, item.ImageName, item.ProductList);
    //            //}
    //        })
    //        .error(function (xhr) {

    //        });
    //}
    function renderProducts(catId) {


        $.ajax({
            dataType: "json",
            url: '/home/getHomePage_Products',
            data: { categoryId: catId },
            success: function (data) {
                if (data.isSuccess) {
                    productList = data.Products;

                    let html = "";
                    html += "<option value='0'>Select Product</option>";
                    for (let i = 0; i < data.Products.length; i++) {
                        html += "<option value='" + data.Products[i].Id + "' data-id='" + data.Products[i].Title + "'>" + data.Products[i].Title + " </option>";
                    }

                    $("#products").html(html);

                }
                else {
                    bootbox.alert("<h4>Failed to load products!</h4>", function () { });
                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error to load products!</h4>", function () { });
            }
        });
    }


    $(document).on('change', '#category', function (event) {
        //console.log( );
        let catId = $(this).val();
        //renderProducts(catId);
        populateProducts(catId);
    });
   


    //$(document).on('change', '#products', function (event) {
    //    let prodId = $(this).val();
    //    let x = productList;

    //    var prod = productList.filter(x => x.Id == prodId)[0];

    //    $("#produt-name").val(prod.Title);
    //    $("#product-unit").val(prod.Unit);
    //    $("#product-price").val(prod.RetailPrice);
    //    $("#produt-quantity").val(1)
    //    $("#product-discount").val(prod.RetailDiscount);

    //});

    function populateProductList(catId, callback) {

        $('.item-loading').show();
        $.ajax({
            dataType: "json",
            url: '/Home/GetHomePage_Products',
            method: 'GET',
            async: true,
            data: { categoryId: catId },
            //data: { branchId: branchId, fromDate: fromDate, toDate: toDate, orderStatus: orderStatus, orderMode: orderMode },
            success: function (recordSet) {

                $('.item-loading').hide();
                $('#delete-btn-container').show();


                var dataSet = [];
                if (recordSet.isSuccess && recordSet.Products.length > 0) {
                    productList = recordSet.Products;

                    for (var i = 0; i < recordSet.Products.length; i++) {
                        var record = [];
                        record.push(i + 1);
                        record.push(recordSet.Products[i].Id);
                        record.push(recordSet.Products[i].Title);
                        record.push(recordSet.Products[i].Weight);
                        record.push(recordSet.Products[i].Unit);
                        record.push(recordSet.Products[i].RetailPrice);

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


    function populateProducts(catId) {
        populateProductList(catId, function (records) {
            $('#data-table-admin-products').dataTable({
                "data": records,
                "bLengthChange": false,
                "bFilter": true,
                "pageLength": 50,
                "bDestroy": true,
                //"order": [[0, "asc"]],
                "columns": [
                    { "title": "S.N", "class": "center" },
                    { "title": "Id", "class": "center" },
                    { "title": "Product Name", "class": "center" },
                    { "title": "Weight", "class": "center" },
                    { "title": "Unit", "class": "center" },
                    { "title": "Retail Price", "class": "center" },
                    { "title": "Action", "class": "center" },
                ],
                "aoColumnDefs": [
                    {
                        "aTargets": [6],
                        "bSortable": false,
                        "mRender": function (data, type, row) {
                            var text = '<a id=' + row[1] + ' class="btn btn-success btn-show-modal" data-toggle="modal" data-target="#productModal">Select</a>';
                            return $("<div/>").append(text).html();
                        }
                    },
                ]
            });
        });
    }



    $('#orderDate').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });
    $("#orderDate").val(moment(new Date()).format("YYYY-MM-DD"))


    $(document).on('click', '#add-to-cart', function (event) {

        var productId = $("#product-id").val();


        var prod = productList.filter(x => x.Id == productId)[0];


        var name = $("#produt-name").val();
        var price = $("#product-price").val();
        var qty = $("#produt-quantity").val();
        var discount = $("#product-discount").val();
        var unit = $("#product-unit").val();
        var weight = $("#product-weight").val();

        let totalCost = price;
        if (prod.IsSoldbyweight) {
            totalCost = price * weight;
        }

        addToCart(productId, name, qty, totalCost, discount, unit, weight);
        event.preventDefault();
        $("#productModal").modal('hide');
        bootbox.alert("<h4>Item added into the cart!</h4>", function () { });
        animateAddToCart(this);
    });
    $(document).on("click", ".btn-show-modal", function () {

        let prodId = $(this).attr('id');

        var prod = productList.filter(x => x.Id == prodId)[0];

        $("#product-id").val(prod.Id);
        $("#produt-name").val(prod.Title);
        $("#product-unit").val(prod.Unit);
        $("#product-price").val(prod.RetailPrice);
        $("#produt-quantity").val(1)
        $("#product-discount").val(prod.RetailDiscount);
        $("#product-weight").val(prod.Weight);
    })



    $('#homepage-container-popular,#homepage-container-newarrival,#homepage-container-featured').on('click', '.txtQty', function (event) {
        event.preventDefault();
    });

}]);