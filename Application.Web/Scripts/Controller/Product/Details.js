
// SERVICE CALL
app.factory('productDetailsService', [
    '$http', function ($http) {

        return {
            getProduct: function (id) {
                return $http({
                    url: '/Product/GetProduct',
                    method: 'GET',
                    async: true,
                    params: { id: id }
                });
            },

            getRelatedProductList: function (categoryId, excludeProductId) {
                return $http({
                    url: '/Product/GetRelatedProducts',
                    method: 'GET',
                    async: true,
                    params: { categoryId: categoryId, excludeProductId: excludeProductId }
                });
            }
        };

    }
]);

// CONTROLLER
app.controller('ProductDetailCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', '$compile', 'Enum', '$sce', 'productDetailsService', function ($rootScope, $scope, $http, $filter, $location, $compile, Enum, $sce, productDetailsService) {


    var productId = getParam('id');
    if (productId) {
        getProduct(productId);
    }

    function getRelatedProductList(categoryId, productId) {
        productDetailsService.getRelatedProductList(categoryId, productId)
            .success(function (rpList) {

                for (var i = 0; i < rpList.length; i++) {
                    $('.related-product-slider').append('<a href="/Product/Details?id=' + rpList[i].Id + '"><div data-index="' + i + 1 + '" class="rp-item"> ' +

                        '<div class="rp-img"> ' +
                        '<img src="' + rpList[i].PrimaryImageName + '" alt="One">' +
                        '</div>' +
                        '<span class="rp-old-price">' + rpList[i].PriceTextOld + '</span>' + '<span class="rp-price theme-text-color">' + rpList[i].PriceText + '</span>' +
                        '<div class="rp-info">' +
                        '<div class="rp-info-title">' + rpList[i].Title + '</div>' +
                        '</div>' +
                        '</div></a>');

                }

                $('.related-product-slider').slick({
                    slidesToShow: 6,
                    slidesToScroll: 1,
                    arrows: true,
                    prevArrow: "<img class='a-left control-c prev slick-prev' src='/images/left-arrow.png'>",
                    nextArrow: "<img class='a-right control-c next slick-next' src='/images/right-arrow.png'>",
                    fade: false,
                    speed: 300,
                    responsive: [
                        {
                            breakpoint: 1024,
                            settings: {
                                slidesToShow: 4,
                                slidesToScroll: 1
                            }
                        },
                        {
                            breakpoint: 600,
                            settings: {
                                slidesToShow: 2,
                                slidesToScroll: 1
                            }
                        },
                        {
                            breakpoint: 300,
                            settings: "unslick"
                        }
                    ]
                });
            });
    }

    $scope.closeModal = function () {
        $('.slider-modal').hide();
        $('.slider-modal-close').hide();
        $('.max-view-icon').hide();
    }

    $scope.clickChoice = function (value) {

        let p = +window.Choices.find(x => x.Id == value).Amount;

        if ($("#choice_" + value).is(":checked")) {
            if (window.Description == undefined)
                window.Description = "," + $("#choice_" + value).val();
            else
                window.Description = window.Description + "," + $("#choice_" + value).val();

            window.price = window.price + p;

        } else {
            window.Description = window.Description.replace("," + $("#choice_" + value).val(), "");
            window.price = window.price - p;
        }

        $("#price").text(siteCurrency() + window.price);

        console.log(window.Description);
    }

    $scope.clickSize = function (value) {

        let colors = window.SizeColor.filter(x => x.Size.trim() == value);
        let html = '<span style="font-size: 15px;">Color:</span>';
        let color = '';
        for (let i = 0; i < colors.length; i++) {
            if (i == 0)
                color = colors[i].Color.trim();
            window.price = colors[i].Price;

            html += ' <label class="padd-left-10">'
                + '   <input id="rbColor" type="radio" ng-click="clickColor(' + colors[i].Color.trim() + ')" name="color" class="form-check-input color" ' + (i == 0 ? 'checked="checked"' : "") + ' value="' + colors[i].Color.trim() + '" required>  '
                + '  ' + colors[i].Color.trim()
                + '</label>';
        }
        $("#price").text(siteCurrency() + window.price);
        $("#color").html(html);
        $("#showColor").text(color);
        $("#showColorDiv").css("background-color", color);
    }

    //$scope.clickColor = function (value) {
    //    $("#showColor").text(value);
    //    $("#showColorDiv").css("background-color", value);
    //}


    function getProduct(id) {
        productDetailsService.getProduct(id)
            .success(function (data) {
                console.log(data);
                $('#product-description').html(data.Description ? data.Description.replace(/\n/g, '<br>') : "");
                $scope.product = data;
                // For shopping cart
                window.productId = data.Id;
                window.imageUrl = data.ImageList.length > 0 ? data.ImageList[0].ThumbImageName : '';
                window.price = data.OnlinePrice;
                window.name = data.Title;
                window.Gst = data.Gst;
                window.SizeColor = data.SizeColors;
                window.Choices = data.Choices;

                if (data.Sizes != null && data.Sizes != undefined && data.Sizes.length > 0) {
                    let size = data.Sizes[data.Sizes.length - 1].Size;


                    let colors = data.SizeColors.filter(x => x.Size.trim() == size);
                    let html = '<span style="font-size: 15px;">Color:</span>';
                    let color = '';
                    for (let i = 0; i < colors.length; i++) {
                        if (i == 0) color = colors[i].Color.trim();
                        window.price = colors[i].Price;
                        html += ' <label class="padd-left-10">'
                            + '   <input id="rbColor" type="radio" name="color" class="form-check-input color" ' + (i == 0 ? 'checked="checked"' : "") + ' value="' + colors[i].Color.trim() + '" required>'
                            + '  ' + colors[i].Color.trim()
                            + '</label>';
                    }

                    $("#price").text(siteCurrency() + window.price);
                    $("#color").html(html);
                    $("#showColor").text(color);
                    $("#showColorDiv").css("background-color", color);
                }
                var imageList = data.ImageList;

                // Bread crumb
                var breadCrumb = '<a class="bc-item" style="position:relative; left: -3px;" href=/Home><i class="icon-home-1"></i>Home</a> <i class="icon-right-open-big"></i>';
                for (var i = 0; i < data.BreadCrumbList.length; i++) {
                    var bc = data.BreadCrumbList[i];
                    if (bc.Id) {
                        breadCrumb = breadCrumb + '<a class="bc-item" href=/Product/Search?cat=' + bc.Id + '>' + bc.Name + '</a> <i class="icon-right-open-big"></i>';
                    }
                    else {
                        breadCrumb = breadCrumb + '<span>' + bc.Name + '</span>';
                    }
                }
                $('#bread-crumb').append(breadCrumb);

                var windowHeight = $(window).height();

                // Slick slider
                $('.slider').html('');
                for (var i = 0; i < imageList.length; i++) {
                    $('.slider').append('<div class="slider-container" data-index="' + i + 1 + '"><span class="max-view-icon icon-search-4"></span><div style="display:table; height: 488px;"><div style="display:table-cell;vertical-align: middle;"><img class="slider-img" src="' + imageList[i].ImageName + '" alt="' + i + 1 + '"></div></div></div>');
                    $('.slider-nav-thumbnails').append('<div data-index="' + i + 1 + '"><img src="' + imageList[i].ThumbImageName + '" alt="' + i + 1 + '"></div>');
                    $('.slider-modal').append('<div data-index="' + i + 1 + '"><div style="display:table; height: ' + windowHeight + 'px;"><div style="display:table-cell;vertical-align: middle;"><img class="modal-details-slide-img" src="' + imageList[i].MaxViewImageName + '" alt="' + i + 1 + '"></div></div></div>');
                }

                $('.slider').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: true,
                    fade: false,
                    speed: 1,
                    prevArrow: "<a class='a-left control-c prev slick-prev'></a>",
                    nextArrow: "<a class='a-right control-c next slick-next'></a>",
                    asNavFor: '.slider-nav-thumbnails',
                });

                $('.slider-nav-thumbnails').slick({
                    slidesToShow: 6,
                    slidesToScroll: 0,
                    asNavFor: '.slider',
                    dots: false,
                    arrows: false,
                    focusOnSelect: true
                });

                $('.slider-modal').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: true,
                    fade: false,
                    speed: 1,
                    prevArrow: "<a class='a-left control-c prev slick-prev'></a>",
                    nextArrow: "<a class='a-right control-c next slick-next'></a>"
                });

                // If there are only one image then hide the thumb view
                if (imageList.length === 1) {
                    $('.slider-nav-thumbnails').hide();
                }

                // If no logo available then hide it
                if (data.Seller.Store && !data.Seller.Store.LogoName) {
                    $('#store-logo').css('border', '0');
                    $('.company-title').css('padding-left', '0');
                }

                // Load related products
                getRelatedProductList(data.Category.Id, data.Id);
                //console.log($(".sizes:checked").val());
            })
            .error(function (xhr) {
            });
    }

    function animateAddToCart_DetailPage(obj) {
        var cart = $('.top-shopping-cart');
        var img = $('.slider-img').eq(0);

        var imgtodrag = img;
        if (imgtodrag) {
            var imgclone = imgtodrag.clone()
                .offset({
                    top: imgtodrag.offset().top,
                    left: imgtodrag.offset().left
                })
                .css({
                    'opacity': '0.5',
                    'position': 'absolute',
                    'height': '150px',
                    'width': '150px',
                    'z-index': '100'
                })
                .appendTo($('body'))
                .animate({
                    'top': cart.offset().top + 10,
                    'left': cart.offset().left + 10,
                    'width': 75,
                    'height': 75
                }, 1000, 'easeInOutExpo');

            setTimeout(function () {
                cart.effect("shake", {
                    times: 2
                }, 200);
            }, 1500);

            imgclone.animate({
                'width': 0,
                'height': 0
            }, function () {

            });
        }
    }


    $(document).ready(function () {

        // For shopping cart
        window.productId = '';
        window.imageUrl = '';
        window.price = '';
        window.name = '';

        $('.btn-add-to-cart').click(function () {

            let orderType = localStorage.getItem("OrderType");
            if (orderType == null || orderType != "Online") {
                localStorage.removeItem("cart");
                localStorage.setItem("OrderType", "Online")
            } else {
                localStorage.setItem("OrderType", "Online");
            }

            var qty = $('#cartQuantity').val();

            let color = document.querySelector('input[name="color"]:checked') != null ? document.querySelector('input[name="color"]:checked').value : "";
            let size = document.querySelector('input[name="size"]:checked') != null ? document.querySelector('input[name="size"]:checked').value : "";
            let option = document.querySelector('input[name="option"]:checked') != null ? document.querySelector('input[name="option"]:checked').value : "";
            let name = window.name;
            //if (color != "")
            //    name = name + " - " + color;
            //if (size != "")
            //    name = name + " - " + size;
            //if (option != "")
            //    name = name + " - " + option;

            if (window.Description == undefined) {
                window.Description = ''
            }
            addToCart(window.productId, name, qty, window.price, window.imageUrl, window.Gst, 0, color, size, window.Description, option);

            animateAddToCart_DetailPage(this);
        });


        $('#btnPlus').click(function () {
            var qty = $('#cartQuantity').val();
            $('#cartQuantity').val(parseInt(qty, 10) + 1);
        });

        $(document).on("click", ".color", function () {

            let value = $(this).val();

            $("#showColor").text(value);
            $("#showColorDiv").css("background-color", value);
        });

        $('#btnMinus').click(function () {
            var qty = $('#cartQuantity').val();
            if (parseInt(qty, 10) > 1) {
                $('#cartQuantity').val(parseInt(qty, 10) - 1);
            }
        });


        $(document).on("mouseenter", ".slider-img", function () {
            $('.max-view-icon').css('display', 'inline-table');
        });

        $(document).on("mouseleave", ".slider-img", function () {
            $('.max-view-icon').hide();
        });

        $(document).on("mouseenter", ".max-view-icon", function () {
            $('.max-view-icon').css('display', 'inline-table');
        });

        $(".max-view-icon").live("click", function () {
            var dataIndex = $(this).parent().closest('.slick-slide').attr('data-slick-index');
            $('.slider-modal').slick('slickGoTo', dataIndex)
            $('.slider-modal').show();
            $('.slider-modal-close').show();

            $(".slider-modal").slick("refresh");
        });


        $(document).ready(function ($) {
            $('.tab_content').hide();
            $('.tab_content:first').show();
            $('.tabs li:first').addClass('active');
            $('.tabs li').click(function (event) {
                $('.tabs li').removeClass('active');
                $(this).addClass('active');
                $('.tab_content').hide();

                var selectTab = $(this).find('a').attr("href");

                $(selectTab).fadeIn();
            });
        });
    });

}]);