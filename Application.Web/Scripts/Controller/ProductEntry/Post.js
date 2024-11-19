$(document).ready(function () {

    var isLoad = getParam('load');
    if (isLoad) {
        loadCategoryTree();      
    }

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
        });
    }    
        
    $("#inputFile1").change(function () {                
        readURL11(this);
    });

    $("#inputFile2").change(function () {
        readURL11(this);
    });

    $("#inputFile3").change(function () {
        readURL11(this);
    });

    $("#inputFile4").change(function () {
        readURL11(this);
    });

    $("#inputFile5").change(function () {
        readURL11(this);
    });

    $("#inputFile6").change(function () {
        readURL11(this);
    });

    $("#btnPhotoCancel1").click(function () {
        $('#productImage1').attr('src', '');
        $('#productImage1').hide();
        $("#btnPhotoCancel1").hide();
        $('.btn-upload-1').css('visibility', 'visible')
    });

    $("#btnPhotoCancel2").click(function () {
        $('#productImage2').attr('src', '');
        $('#productImage2').hide();
        $("#btnPhotoCancel2").hide();
        $('.btn-upload-2').css('visibility', 'visible')
    });

    $("#btnPhotoCancel3").click(function () {
        $('#productImage3').attr('src', '');
        $('#productImage3').hide();
        $("#btnPhotoCancel3").hide();
        $('.btn-upload-3').css('visibility', 'visible')
    });

    $("#btnPhotoCancel4").click(function () {
        $('#productImage4').attr('src', '');
        $('#productImage4').hide();
        $("#btnPhotoCancel4").hide();
        $('.btn-upload-4').css('visibility', 'visible')
    });

    $("#btnPhotoCancel5").click(function () {
        $('#productImage5').attr('src', '');
        $('#productImage5').hide();
        $("#btnPhotoCancel5").hide();
        $('.btn-upload-5').css('visibility', 'visible')
    });

    $("#btnPhotoCancel6").click(function () {
        $('#productImage6').attr('src', '');
        $('#productImage6').hide();
        $("#btnPhotoCancel6").hide();
        $('.btn-upload-6').css('visibility', 'visible')
    });

    function readURL(input, imageId) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#' + imageId).attr('src', e.target.result);
            }
            reader.readAsDataURL(input.files[0]);            
        }
    }

    function readURL11(input) {
        if ($('#productImage1').attr('src') === '') {
            setImage(input, 'productImage1');
            $('.btn-upload-1').css('visibility', 'hidden')
        }
        else if ($('#productImage2').attr('src') === '') {
            setImage(input, 'productImage2');
            $('.btn-upload-2').css('visibility', 'hidden')
        }
        else if ($('#productImage3').attr('src') === '') {
            setImage(input, 'productImage3');
            $('.btn-upload-3').css('visibility', 'hidden')
        }
        else if ($('#productImage4').attr('src') === '') {
            setImage(input, 'productImage4');
            $('.btn-upload-4').css('visibility', 'hidden')
        }
        else if ($('#productImage5').attr('src') === '') {
            setImage(input, 'productImage5');
            $('.btn-upload-5').css('visibility', 'hidden')
        }
        else if ($('#productImage6').attr('src') === '') {
            setImage(input, 'productImage6');
            $('.btn-upload-6').css('visibility', 'hidden')
        }
    }

    function setImage(input, imageId) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#' + imageId).attr('src', e.target.result);
            }
            reader.readAsDataURL(input.files[0]);

            // Show cancel button
            $('#' + imageId.replace('productImage', 'btnPhotoCancel')).show();

            // Show the image
            $('#' + imageId).show();
        }
    }    

    $('#oneSelect').on('change', function () {
        clearList(1, true);
        getChildList(parseInt(this.value, 10), true);
        if (window.subCatList && window.subCatList.length > 0) {
            populateListControls('twoSelect', window.subCatList);
        }
        else {
            setSelectedText(1, true);            
            window.selectedCategoryId = this.value;
        }
    });

    $('#twoSelect').on('change', function () {
        clearList(2, true);
        getChildList(parseInt(this.value, 10), true);
        if (window.subCatList && window.subCatList.length > 0) {
            populateListControls('threeSelect', window.subCatList);
        }
        else {
            setSelectedText(2, true);            
            window.selectedCategoryId = this.value;
        }
    });

    $('#threeSelect').on('change', function () {
        clearList(3, true);
        getChildList(parseInt(this.value, 10), true);
        if (window.subCatList && window.subCatList.length > 0) {
            populateListControls('fourSelect', window.subCatList);
        }
        else {
            setSelectedText(3, true);            
            window.selectedCategoryId = this.value;
        }
    });

    $('#fourSelect').on('change', function () {
        setSelectedText(4, true);        
        window.selectedCategoryId = this.value;
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
                $('#twoSelect').empty();
                $('#threeSelect').empty();
                $('#fourSelect').empty();
            }
            else if (number === 2) {
                $('#threeSelect').empty();
                $('#fourSelect').empty();
            }
            else if (number === 3) {
                $('#fourSelect').empty();
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
                window.selectedCategoryText = $("#oneSelect option:selected").text();
            }
            else if (controlId === 2) {
                window.selectedCategoryText = $("#oneSelect option:selected").text() + ' / ' + $("#twoSelect option:selected").text();
            }
            else if (controlId === 3) {
                window.selectedCategoryText = $("#oneSelect option:selected").text() + ' / ' + $("#twoSelect option:selected").text() + ' / ' + $("#threeSelect option:selected").text();
            }
            else if (controlId === 4) {
                window.selectedCategoryText = $("#oneSelect option:selected").text() + ' / ' + $("#twoSelect option:selected").text() + ' / ' + $("#threeSelect option:selected").text() + ' / ' + $("#fourSelect option:selected").text();
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
        var catBox = $("#oneSelect");
        $.each(catItems, function (index, item) {
            catBox.append(new Option(item.name, item.id));
        });        
    }

    window.subCatList = [];
    window.selectedCategoryText = '';
    window.selectedCategoryId = '';
});

// SERVICE
app.factory('postProductService', ['$http', function ($http) {

    return {
        postProduct: function (product, formData) {

            formData.append('product', JSON.stringify(product));

            return $http.post('/ProductEntry/PostProduct', formData, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            });
        },

        getPhotoList: function () {
            return $http({
                url: '/Photo/GetPhotoList',
                method: 'GET',
                cache: false
            });
        },

        setProfilePhoto: function (photoId) {
            return $http({
                url: '/Photo/SetProfilePhoto',
                method: 'POST',
                data: { photoId: photoId }
            });
        },

        deletePhoto: function (photoId) {
            return $http({
                url: '/Photo/DeletePhoto',
                method: 'POST',
                data: { photoId: photoId }
            });
        },
        getBranchList: function () {
            return $http.get('/Branch/GetUserBranchList');
        },
        getSupplierList: function () {
            return $http.get('/Supplier/GetSupplierList');
        },
        getItemtypeList: function () {
            return $http.get('/ItemType/GetItemTypeList');
        },
        getColorList: function () {
            return $http.get('/Lookup/GetLookups?name=color');
        },
        getConditionList: function () {
            return $http.get('/Lookup/GetLookups?name=condition');
        },
        getCapacityList: function () {
            return $http.get('/Lookup/GetLookups?name=capacity');
        },
        getManufacturerList: function () {
            return $http.get('/Lookup/GetLookups?name=manufacturer');
        },
        isBarcodeExists: function (barcode) {
            return $http.get('/ProductEntry/IsBarcodeExists?barcode=' + barcode);
        },
        getGeneratedBarcode: function () {
            return $http.get('/ProductEntry/GetGeneratedBarcode');
        },
        copyProduct: function (barcode) {
            return $http.get('/ProductEntry/CopyProduct?barcode=' + barcode);
        }
    };

}]);

// CONTROLLER
app.controller('postProductCtrl', ['$rootScope', '$scope', '$http', '$window', '$filter', '$location', 'Enum', 'postProductService', function ($rootScope, $scope, $http, $window, $filter, $location, Enum, postProductService) {

    $scope.IsFileValid = true;    
    $scope.sellType = 1;
    $scope.isAuction = false;
    $scope.discount = 0;
    $scope.quantity = 1;
    $scope.lowStockAlert = 5;
    //$scope.itemType = "Dry";
    $scope.unit = "gm";
    var formData = new FormData();

    // Load branch, supplier & item type list
    getBranchList();
    getSupplierList();
    getItemTypeList();
    getColorList();
    getCapacityList();
    getManufacturerList();
    getConditionList();

    // Initialize date picker
    $('#expireDate').datepicker({ autoclose: true, todayHighlight: true }).next().on(ace.click_event, function () { $(this).prev().focus(); });

    $("#title").blur(function () {
        var shortCode = '';
        var data = $('#title').val();
        if (data) {
            var titleParts = data.split(' ');
            for (var i = 0; i < titleParts.length; i++) {
                shortCode += titleParts[i].charAt(0);
            }
        }

        $('#shortCode').val(shortCode);
    });

    $scope.numberOnly = function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    }

    $('#barcode').keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            checkBarcode();
        }
    });

    $scope.checkBarcodeDuplicacy = function () {
        checkBarcode();
    }

    $scope.showHideBarcodeScanner = function () {

        var isVisible = $('#barcode-reader-container').is(":visible");
        if (isVisible) {
            $('#barcode-reader-container').hide();
        }
        else {
            $('#barcode-reader-container').show();

            var html5QrcodeScanner = new Html5QrcodeScanner("barcode-reader", { fps: 10, qrbox: 250 });
            var lastResult, countResults = 0;

            html5QrcodeScanner.render(onScanSuccess, onScanError);

            $('a', '#barcode-reader').hide();

            function onScanSuccess(decodedText, decodedResult) {

                if (decodedText !== lastResult) {
                    lastResult = decodedText;

                    $('#barcode').val(decodedText);
                    document.getElementById("barcode-reader-container").style.display = "none";

                    // Optional: To close the QR code scannign after the result is found
                    //html5QrcodeScanner.clear();     
                }
            }

            function onScanError(qrCodeError) {
            }
        }
    }

    function fillProduct(barcode) {

        // If barcode not exists then pick product from common DB
        $('#barcodeStatus').html('Getting product information...');
        postProductService.copyProduct(barcode)
            .success(function (data) {
                if (data) {

                    $('#barcodeStatus').html('');

                    // Product information
                    $scope.title = data.Title;
                    $scope.shortCode = data.ShortCode;
                    $scope.description = data.Description;
                    $scope.retailPrice = data.RetailPrice;
                    $scope.wholesalePrice = data.WholesalePrice;
                    $scope.onlinePrice = data.OnlinePrice;
                    $scope.weight = data.Weight;
                    $scope.unit = data.Unit;
                    $scope.isFrozen = data.IsFrozen;
                    $scope.isFeatured = data.IsFeatured;
                    $scope.isInternal = data.IsInternal;
                    $scope.isFastMoving = data.IsFastMoving;
                    $scope.isMainItem = data.IsMainItem;
                    $scope.supplierId = data.SupplierId;

                    // Product images
                    if (data.ImageList) {
                        for (var i = 1; i <= data.ImageList.length; i++) {
                            $('#copyImgContainer' + i).show();
                            $('#copyImage' + i).attr('src', data.ImageList[i - 1].ImageName);
                        }
                    }
                }
                else {
                    $('#barcodeStatus').html('');
                }
            })
            .error(function (data) {
                $('#barcodeStatus').html('');
            });

    }

    function checkBarcode() {

        $('#barcodeStatus').html('Checking barcode...');

        // Check barcode is exists or not        
        postProductService.isBarcodeExists($scope.barcode)
            .success(function (data) {
                if (data.isExists) {
                    $('#barcode').css('border', '2px solid red');
                    $('#barcodeStatus').html('Barcode already exists!').css('color', 'red');                    
                }
                else {
                    $('#barcode').css('border', '2px solid green');
                    $('#barcodeStatus').html('').css('color', 'black');
                    
                    fillProduct($scope.barcode);
                }
            })
            .error(function(data){

            });        
    }

    $scope.generateBarcode = function () {
        postProductService.getGeneratedBarcode()
            .success(function (data) {
                if (data) {
                    $('#barcode').val(data.barcode);
                }                
            })
            .error(function (data) {

            });
    }

    var isWholesalePriceEmpty = false;
    var isOnlinePriceEmpty = false;
    $scope.retailPriceChange = function () {
        
        if (!$scope.wholesalePrice) {
            isWholesalePriceEmpty = true;            
        }

        if (!$scope.onlinePrice) {
            isOnlinePriceEmpty = true;
        }

        if (isWholesalePriceEmpty) {
            $scope.wholesalePrice = $scope.retailPrice;
        }

        if (isOnlinePriceEmpty) {
            $scope.onlinePrice = $scope.retailPrice;
        }
    }

    function showHideLoading(show) {
        if (show) {
            $('.post-product-status').show();
            $('#btnPostProduct').attr('disabled', 'disabled');
        }
        else {
            $('.post-product-status').hide();
            $('#btnPostProduct').removeAttr('disabled');
        }
    }    

    $scope.postProduct = function () {

        $scope.submitted = true;        
        if ($scope.myForm.$invalid) {
            return false;
        }

        formData = new FormData();

        $scope.shortCode = $('#shortCode').val();

        // Validation
        if (!window.selectedCategoryId) {
            bootbox.alert("<h4>Please select a product category!</h4>", function () { });
            return;
        }
        else if (!$scope.branchId) {
            bootbox.alert("<h4>Please select a branch!</h4>", function () { });
            return;
        }
        else if ($scope.Barcode) {
            if ($scope.Barcode.length < 6) {
                bootbox.alert("<h4>Barcode must be greater than 6 characters!</h4>", function () { });
                return;
            }
        }
        else if (!$scope.title) {
            bootbox.alert("<h4>Please select product title!</h4>", function () { });
            return;
        }
        else if (!$scope.shortCode) {
            bootbox.alert("<h4>Please select product short code!</h4>", function () { });
            return;
        }
        else if (!$scope.description) {
            bootbox.alert("<h4>Please select product description!</h4>", function () { });
            return;
        }
        else if (!$scope.supplierId) {
            bootbox.alert("<h4>Please select product supplier!</h4>", function () { });
            return;
        }
        else if (!$scope.costPrice) {
            bootbox.alert("<h4>Please enter product cost price!</h4>", function () { });
            return;
        }
        else if (!$scope.retailPrice) {
            bootbox.alert("<h4>Please enter product retail price!</h4>", function () { });
            return;
        }

        // Image1
        var image1 = $scope.productImage1;
        $scope.CheckFile(image1);
        if (!$scope.IsFileValid) {
            return false;
        }
        else {
            if (image1) {
                formData.append("image1", image1);
            }
        }

        // Image2
        var image2 = $scope.productImage2;
        $scope.CheckFile(image2);
        if (!$scope.IsFileValid) {
            return false;
        }
        else {
            if (image2) {
                formData.append("image2", image2);
            }
        }

        // Image3
        var image3 = $scope.productImage3;
        $scope.CheckFile(image3);
        if (!$scope.IsFileValid) {
            return false;
        }
        else {
            if (image3) {
                formData.append("image3", image3);
            }
        }

        // Image4
        var image4 = $scope.productImage4;
        $scope.CheckFile(image4);
        if (!$scope.IsFileValid) {
            return false;
        }
        else {
            if (image4) {
                formData.append("image4", image4);
            }
        }

        // Image5
        var image5 = $scope.productImage5;
        $scope.CheckFile(image5);
        if (!$scope.IsFileValid) {
            return false;
        }
        else {
            if (image5) {
                formData.append("image5", image5);
            }
        }

        // Image6
        var image6 = $scope.productImage6;
        $scope.CheckFile(image6);
        if (!$scope.IsFileValid) {
            return false;
        }
        else {
            if (image6) {
                formData.append("image6", image6);
            }
        }

        showHideLoading(true);

        var retailPrice = parseFloat($scope.retailPrice, 10);
        var wholesalePrice = !$scope.wholesalePrice ? retailPrice : parseFloat($scope.wholesalePrice, 10);
        var onlinePrice = !$scope.onlinePrice ? retailPrice : parseFloat($scope.onlinePrice, 10);

        var product = {};

        product.Barcode = $('#barcode').val();
        product.BranchId = $scope.branchId;
        product.Title = $scope.title;
        product.ShortCode = $scope.shortCode;
        product.Description = $scope.description;
        product.CategoryId = parseInt(window.selectedCategoryId, 10);
        product.CostPrice = parseFloat($scope.costPrice, 10);
        product.RetailPrice = retailPrice;
        product.WholesalePrice = wholesalePrice;
        product.OnlinePrice = onlinePrice; 

        product.RetailDiscount = !$scope.retailDiscount ? 0 : parseFloat($scope.retailDiscount, 10);
        product.WholesaleDiscount = !$scope.wholesaleDiscount ? 0 : parseFloat($scope.wholesaleDiscount, 10);
        product.OnlineDiscount = !$scope.onlineDiscount ? 0 : parseFloat($scope.onlineDiscount, 10);

        product.Weight = !$scope.weight ? 0 : parseFloat($scope.weight, 10);
        product.Unit = $scope.unit;
        product.Quantity = !$scope.quantity ? 0 : parseInt($scope.quantity, 10);
        product.LowStockAlert = !$scope.lowStockAlert ? 0 : parseInt($scope.lowStockAlert, 10);
        product.IsFrozen = !$scope.isFrozen ? 0 : 1;
        product.IsFeatured = !$scope.isFeatured ? 0 : 1;
        product.IsInternal = !$scope.isInternal ? 0 : 1;
        product.IsFastMoving = !$scope.isFastMoving ? 0 : 1;
        product.IsMainItem = !$scope.isMainItem ? 0 : 1;
        product.ItemTypeId = $scope.itemTypeId;
        product.SupplierId = $scope.supplierId;        
        product.ExpireDate = $('#expireDate').val();

        product.Color = $scope.color;
        product.Condition = $scope.condition;
        product.Capacity = $scope.capacity;
        product.Manufacturer = $scope.manufacturer;
        product.IMEI = $scope.imei;
        product.ModelNumber = $scope.modelNumber;
        product.WarrantyPeriod = $scope.warrantyPeriod;

        postProductService.postProduct(product, formData)
            .success(function (data) {
                showHideLoading(false);

                if (data.isSuccess) {                    
                    window.location.href = "/ProductEntry/PostProductMessage";
                }
                else {
                    if (data.message) {
                        bootbox.alert("<h4>"+ data.message +"</h4>", function () { });
                    }
                    else {
                        bootbox.alert("<h4>Something wrong! Failed to post the product</h4>", function () { });
                    }
                }                
            })
            .error(function (exception) {
                showHideLoading(false);
                bootbox.alert("<h4>System Error Occured!</h4>", function () { });
            });
    }

    function getStringToDate(ddmmyyyy) {
        var from = ddmmyyyy.split("-");
        var dt = new Date(from[2], from[1] - 1, from[0]);
        return dt;
    }

    function getBranchList() {
        postProductService.getBranchList()
        .success(function (data) {
            $scope.branchList = data;

            if ($scope.branchList && $scope.branchList.length == 1) {
                $scope.branchId = $scope.branchList[0].Id;
            }
        })
        .error(function (xhr) {
            ShowError('Error to get branches');
        });
    }

    function getSupplierList() {
        postProductService.getSupplierList()
        .success(function (data) {
            $scope.supplierList = data;
        })
        .error(function (xhr) {
            ShowError('Error to get suppliers');
        });
    }

    function getItemTypeList() {
        postProductService.getItemtypeList()
        .success(function (data) {
            $scope.itemTypeList = data;
        })
        .error(function (xhr) {
            ShowError('Error to get suppliers');
        });
    }

    function getColorList() {
        postProductService.getColorList()
        .success(function (data) {
            $scope.colorList = data;
        })
        .error(function (xhr) {            
        });
    }

    function getCapacityList() {
        postProductService.getCapacityList()
        .success(function (data) {
            $scope.capacityList = data;
        })
        .error(function (xhr) {
        });
    }

    function getConditionList() {
        postProductService.getConditionList()
        .success(function (data) {
            $scope.conditionList = data;
        })
        .error(function (xhr) {
        });
    }

    function getManufacturerList() {
        postProductService.getManufacturerList()
        .success(function (data) {
            $scope.manufacturerList = data;
        })
        .error(function (xhr) {
        });
    }
    
    $scope.selectPhoto1 = function (files) {
        $scope.productImage1 = files[0];
    };

    $scope.selectPhoto2 = function (files) {
        $scope.productImage2 = files[0];
    };

    $scope.selectPhoto3 = function (files) {
        $scope.productImage3 = files[0];
    };

    $scope.selectPhoto4 = function (files) {
        $scope.productImage4 = files[0];
    };

    $scope.selectPhoto5 = function (files) {
        $scope.productImage5 = files[0];
    };

    $scope.selectPhoto6 = function (files) {
        $scope.productImage6 = files[0];
    };
    
    $scope.CheckFile = function (file) {        
        if (file != null) {
            if ((file.type == 'image/gif' || file.type == 'image/png' || file.type == 'image/jpeg') && file.size <= (4096 * 1024)) { // limit photo size to 4 mb
                $scope.FileInvalidMessage = "";
                $scope.IsFileValid = true;
            }
            else {
                $scope.IsFileValid = false;
                bootbox.alert("<h4>Invalid file is selected. (File format must be gif or png or jpeg. Maximum file size 4 mb)</h4>", function () { });
            }
        }        
    };

    $("#isElectronicsItem").change(function () {
        if (this.checked) {
            $('.electronics-item-body').show();
        }
        else {
            $('.electronics-item-body').hide();
        }
    });

}]);