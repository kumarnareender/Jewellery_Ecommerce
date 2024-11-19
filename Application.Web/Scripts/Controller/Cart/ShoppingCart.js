$(document).ready(function () {
    getUserInformation();
    //builtShoppingCartItems();

    //confirmOrderList();


    //$("#div-place-order").hide();

    showStep();

    function showStep() {
        var param = getParam('step');
        if (param === 'confirm-order') {
            $('#div-confirm-order').show();
            $('#div-place-order').hide();
        }
    }

    $(document).on('click', '.delete-shopping-cart-item', function () {
        var id = $(this).attr('id');
        let pk = $(this).attr('pk');

        removeCartItem(id, pk)

        var currentTr = $(this).closest("tr");
        $(currentTr).remove();

        builtShoppingCartItems();
    });

    $('#btnClearCart').click(function () {

        bootbox.confirm("<h3 class='text-danger'>Clear Cart Items</h3> " +
            "<br/><h4 class='text-info'> Are you sure to clear the cart?</h4>",
            function (result) {
                if (result) {
                    clearCart();
                    builtShoppingCartItems();
                }
            });

    });

    $('#btnUpdateCart').click(function () {

        var cart = getCart();
        for (var i = 0; i < cart.length; i++) {
            var quantityInputBoxId = 'txtQty_' + cart[i].Id;
            cart[i].Quantity = $('#' + quantityInputBoxId).val();
        }

        updateCart(cart);
        builtShoppingCartItems();
        confirmOrderList();
    });
    //$(document).on("change", "#order-type-shopping-cart", function () {

    //    let grandTotal = +$('#grandTotal').val();
    //    let shippingCharges = +$("#shippingAmount").val();

    //    let newGrandTotal = grandTotal;

    //    if ($(this).val() == 'Wholesale') {
    //        $("#wholesale-customer-div").removeClass('hide');//addClass("d-none");
    //        $("#purchase-supplier-div").addClass('hide');
    //        $("#btnPlaceOrder").removeClass('hide');
    //        $("#btnPurchaseOrder").addClass('hide');
    //        $(".p-order").removeClass('hide');
    //        newGrandTotal = grandTotal + shippingCharges;
    //    }
    //    else if ($(this).val() == 'Purchase') {
    //        $("#wholesale-customer-div").addClass('hide');//remvoeClass("d-none");
    //        $("#purchase-supplier-div").removeClass('hide');
    //        $("#btnPlaceOrder").addClass('hide');
    //        $("#btnPurchaseOrder").removeClass('hide');
    //        $(".p-order").addClass('hide');
    //        //$("#shippingAmount").val(0);
    //        $("#btnUpdateAddress").addClass('hide');

    //    } else {
    //        $("#wholesale-customer-div").addClass('hide');//remvoeClass("d-none");
    //        $("#purchase-supplier-div").addClass('hide');

    //        $("#btnPlaceOrder").removeClass('hide');
    //        $("#btnPurchaseOrder").addClass('hide');
    //        $(".p-order").removeClass('hide');
    //        newGrandTotal = grandTotal + shippingCharges;
    //    }
    //    $(".checkout-grandTotal").html(siteCurrency() + newGrandTotal);

    //})

    $(document).on("change", "#wholesale-customer, #purchase-supplier", function () {
        let zipCode = $('option:selected', this).attr('postal-code');
        let firstName = $('option:selected', this).attr('name');
        let address = $('option:selected', this).attr('address');
        let mobile = $('option:selected', this).attr('mobile');
        let country = $('option:selected', this).attr('country');
        let city = $('option:selected', this).attr('city');

        $('#firstName').val(firstName);
        $('#mobile').val(mobile);
        $('#address').val(address);
        $('#zip').val(zipCode);
        $('#city').val(city);
        $('#state').val(country);
        //$('#email').val(email);

        // Show in confirm order section
        $('#showFirstName').html(firstName);
        $('#showMobile').html(mobile);
        $('#showAddress').html(address);
        $('#showZipCode').html(zipCode);
        $('#showCity').html(city);
        $('#showState').html(country);

    })

    function showLoader() {
        $('#order-loader').show();
        $('#btnBack').show();
        $("#btnConfirmOrder").prop("disabled", true);
        $("#btnBack").prop("disabled", true);
    }

    function hideLoader() {
        $('#order-loader').hide();
        $('#btnBack').hide();
        $("#btnConfirmOrder").prop("disabled", false);
        $("#btnBack").prop("disabled", false);
    }

    function isCodPayment() {

        // COD or Card payment
        var isCod = false;
        if ($("#rbCod").is(":checked")) {
            isCod = true;
        }

        return isCod;
    }

    //function getPaymentType() {
    //    return document.querySelector('input[name="paymentBy"]:checked').value;
    //}

    // Here order records are save
    $('#btnConfirmOrder').click(function () {
        var userStatus = getUserStatus();
        if (!userStatus.isLoggedIn) {
            window.location.href = '/Security/Login/?returnUrl=/cart';
            return;
        }

        // COD or Card
        //var paymentType = getPaymentType();//isCodPayment();

        var order = {};
        order.OrderItems = [];

        var cart = getCart();
        var totalAmount = 0;
        var vatAmount = 0;
        var val = 0;
        var shippingAmount = +$("#shippingAmount").val();
        var grandTotal = 0;

        for (var i = 0; i < cart.length; i++) {

            var orderItem = {};

            var price = parseFloat(cart[i].OnlinePrice, 10);
            var quantity = parseInt(cart[i].Quantity, 10);

            orderItem.Title = cart[i].Name;
            orderItem.ProductId = cart[i].Id;
            orderItem.Quantity = quantity;
            orderItem.Discount = 0;
            orderItem.Price = price;
            orderItem.TotalPrice = quantity * price;
            orderItem.ImageUrl = cart[i].ImageUrl;
            
            orderItem.Weight = cart[i].Weight;
            orderItem.Unit = cart[i].Unit;


            order.OrderItems.push(orderItem);
            totalAmount += orderItem.TotalPrice;

            vatAmount += +((orderItem.TotalPrice * Number(cart[i].Gst)) / 100);
        }

        //vat = Math.round((totalAmount * getVatPercentage()) / 100);
        grandTotal = totalAmount + shippingAmount;

        order.OrderMode = $('input[name="Ordertype"]:checked').val(); //localStorage.getItem("OrderType");
        //$("#order-type-shopping-cart").val();

        order.OrderStatus = 'store';
        order.PaymentStatus = '';
        //order.PaymentType = paymentType;//isCOD === true ? 'COD' : 'Card';
        order.Ordertype = $('input[name="Ordertype"]:checked').val();

        order.Vat = vatAmount;
        order.Discount = $("#discount").val() == undefined || $("#discount").val() == null ? 0 : $("#discount").val();
        order.ShippingAmount = shippingAmount;
        order.PayAmount = grandTotal - order.Discount;
        order.IsWholeSaleOrder = order.OrderMode == "Wholesale" ? true : false;
        /*$("#order-type-shopping-cart").val()*/;
        order.CustomerId = $("#customers").val();


        showLoader();

        // Saving Records
        $.ajax({
            dataType: "json",
            contentType: 'application/json',
            url: '/Customer/PlaceOrder',
            data: JSON.stringify(order),
            method: 'POST',
            success: function (data) {
                if (data.isSuccess) {

                    clearCart();

                    window.location.href = '/Order/OrderList';

                    //if (order.OrderMode == "Wholesale") {
                    //    window.location.href = '/Wholesale/OrderConfirm?orderCode=' + data.orderCode;
                    //    return;
                    //}

                    //if (paymentType != "Online") {
                    //    window.location.href = '/Customer/OrderConfirm?orderCode=' + data.orderCode;
                    //}
                    //else {
                    //    proceedToCardPayment(data.orderId, data.orderCode, grandTotal);
                    //}
                }
                else {
                    hideLoader();
                    bootbox.alert("<h4>Failed to place your order!</h4>", function () { });
                }

                $('#updateStatus').html('');
            },
            error: function (xhr) {
                hideLoader();
                $('#updateStatus').html('');
                bootbox.alert("<h4>Error occured while placing your order!</h4>", function () { });
            }
        });

    });


    function proceedToCardPayment(orderId, orderCode, amount) {

        $.ajax({
            dataType: "json",
            url: '/Customer/CardPayment',
            type: 'POST',
            data: { orderId: orderId, orderCode: orderCode, amount: amount },
            success: function (data) {
                if (data.isSuccess) {

                    var stripeKey = getStripeKey();
                    var stripe = Stripe(stripeKey);

                    stripe.redirectToCheckout({
                        sessionId: data.sessionId
                    }).then(function (result) {
                        bootbox.alert("<h4>" + result.error.message + "</h4>", function () { });
                    });

                }
                else {
                    hideLoader();
                    bootbox.alert("<h4>Failed to initiate your order!</h4>", function () { });
                }
            },
            error: function (xhr) {
                hideLoader();
                bootbox.alert("<h4>Error occured while initiating your order!</h4>", function () { });
            }
        });
    }
    $("#shippingAmount").on("keyup", function () {


        let shippingAmount = +$(this).val();

        $(".checkout-shippingAmount").html(siteCurrency() + shippingAmount);

        let grandTotal = +$("#grandTotal").val();
        $(".checkout-grandTotal").html(siteCurrency() + (grandTotal + shippingAmount).toFixed(2));

    })
    $('#btnPlaceOrder').click(function () {

        let orderMode = localStorage.getItem("OrderType");

        let wholesaleCustomer = $("#wholesale-customer").val()
        if (orderMode == "Wholesale") {
            if (wholesaleCustomer == "0") {
                bootbox.alert("<h4>Please Select Wholesale Customer!</h4>");
                return;
            }
        }


        var userStatus = getUserStatus();
        if (!userStatus.isLoggedIn) {
            window.location.href = '/Security/Login/?returnUrl=/cart';
            return;
        }

        //var paymentType = getPaymentType();//isCodPayment();
        //if (paymentType != "Online") {
        //    $('#btnConfirmOrder').html('Confirm Order');
        //}
        //else {
        //    $('#btnConfirmOrder').html('Confirm Order & Proceed to Pay');
        //}

        if ($("#customers").val() == 0) {
            bootbox.alert("Please select customer");
            return;
        }

        $('#div-confirm-order').show();
        $('#div-place-order').hide();

    });

    $('#btnBack,#btnEditAddr').click(function () {
        $('#div-place-order').show();
        $('#div-confirm-order').hide();
    });

    $("#city").on("change", function () {
        let postcode = $('option:selected', this).attr('postcode');
        $("#zip").val(postcode);
        let shippingAmount = +$('option:selected', this).attr('shipping-charges');
        $("#shippingAmount").val(shippingAmount);

        if (/*$("#order-type-shopping-cart").val()*/
            localStorage.getItem("OrderType") != "Purchase") {


            $(".checkout-shippingAmount").html(siteCurrency() + shippingAmount);

            let grandTotal = +$("#grandTotal").val();

            $(".checkout-grandTotal").html(siteCurrency() + (grandTotal + shippingAmount));
        }

    })


    $('#btnUpdateAddress').click(function () {

        $('#updateStatus').html('Updating your address...');

        var mobile = $('#mobile').val();
        var firstName = $('#firstName').val();
        var lastName = $('#lastName').val();
        var address = $('#address').val();
        var zipCode = $('#zip').val();
        var city = $('#city').val();
        var state = $('#state').val();
        var country = $('#country').val();
        var email = $("#email").val();
        if (!zipCode) {
            bootbox.alert("<h4>Please enter zipcode!</h4>", function () { });
            return;
        }
        else if (!state) {
            bootbox.alert("<h4>Please enter prefecture!</h4>", function () { });
            return;
        }
        else if (!city) {
            bootbox.alert("<h4>Please enter city!</h4>", function () { });
            return;
        }
        else if (!firstName) {
            bootbox.alert("<h4>Please enter your name!</h4>", function () { });
            return;
        }

        $.ajax({
            dataType: "json",
            url: '/Account/UpdateUserAddress',
            data: { mobile: mobile, firstName: firstName, lastName: lastName, address: address, zipCode: zipCode, city: city, state: state, country: country, email: email },
            method: 'POST',
            success: function (data) {
                if (data.isSuccess) {

                    // Show in confirm order section
                    $('#showFirstName').html(firstName);
                    $('#showMobile').html(mobile);
                    $('#showAddress').html(address);
                    $('#showZipCode').html(zipCode);
                    $('#showCity').html(city);
                    $('#showState').html(state);

                    bootbox.alert("<h4>Your address has been updated sucessfully!</h4>", function () { });
                }
                else {
                    if (data.message) {
                        bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                    }
                    else {
                        bootbox.alert("<h4>Failed to update!</h4>", function () { });
                    }
                }

                $('#updateStatus').html('');
            },
            error: function (xhr) {
                $('#updateStatus').html('');
                bootbox.alert("<h4>Error occured while updating your address!</h4>", function () { });
            }
        });
    });

    $('#btn-apply-coupon').click(function () {

        let coupon = $("#coupon").val();
        if (coupon == "" || coupon == undefined || coupon == null) {
            bootbox.alert("<h4>Please add coupon!</h4>", function () { });
            return;
        }

        $.ajax({
            dataType: "json",
            url: '/Product/GetCouponDiscount',
            data: { coupon: coupon },
            method: 'POST',
            success: function (data) {


                if (data.IsSuccess) {

                    let subTotal = $("#btn-apply-coupon").attr('checkout-grandTotal');
                    let shippingAmount = +$("#shippingAmount").val();
                    let discount = (subTotal * data.Value) / 100;
                    let grandTotal = subTotal - discount + shippingAmount;

                    $(".discount").html(siteCurrency() + discount);
                    $("#discount").val(discount);
                    $(".checkout-grandTotal").html(siteCurrency() + grandTotal);

                    bootbox.alert("<h4>Coupon added sucessful!</h4>", function () { });
                }
                else {
                    if (data.message) {
                        bootbox.alert("<h4>" + data.message + "</h4>", function () { });
                    }
                    else {
                        bootbox.alert("<h4>Failed to update!</h4>", function () { });
                    }
                }

                $('#updateStatus').html('');
            },
            error: function (xhr) {
                $('#updateStatus').html('');
                bootbox.alert("<h4>Error occured while updating your address!</h4>", function () { });
            }
        });

    });
});



// Get user information
function getUserInformation() {

    var isLoggedIn = false;
    var userStatus = getUserStatus();
    if (userStatus.isLoggedIn) {
        isLoggedIn = true;
    }

    if (isLoggedIn) {
        $('.item-loading').show();
        $.ajax({
            dataType: "json",
            url: '/City/GetCities',
            success: function (data) {
                if (data) {
                    let html = "";
                    html += "<option value='0'>Select City</option>";
                    for (let i = 0; i < data.length; i++) {
                        html += "<option value='" + data[i].Name + "' postcode='" + data[i].Postcode + "' name='" + data[i].Postcode + "' shipping-charges='" + data[i].ShippingCharge + "'>" + data[i].Name + " </option>";
                    }

                    $("#city").html(html);


                    $.ajax({
                        dataType: "json",
                        url: '/Account/GetLoggedInUserAddress',
                        success: function (data) {
                            $('.item-loading').hide();
                            if (data) {
                                if (data.IsAdmin) {
                                    $(".wholesale").removeClass('hide');
                                    $("#shippingAmount").removeClass('hide');
                                    $(".checkout-shippingAmount").addClass('hide');

                                    let orderType = localStorage.getItem("OrderType");

                                    let grandTotal = +$('#grandTotal').val();
                                    let shippingCharges = +$("#shippingAmount").val();

                                    let newGrandTotal = grandTotal;

                                    if (orderType == 'Wholesale') {
                                        $("#cart-text").text("Shopping Cart - " + orderType + " Order");
                                        $("#wholesale-customer-div").removeClass('hide');//addClass("d-none");
                                        $("#purchase-supplier-div").addClass('hide');
                                        $("#btnPlaceOrder").removeClass('hide');
                                        $("#btnPurchaseOrder").addClass('hide');
                                        $(".p-order").removeClass('hide');
                                        newGrandTotal = grandTotal + shippingCharges;
                                    }
                                    else if (orderType == 'Purchase') {
                                        $("#cart-text").text("Shopping Cart - " + orderType + " Order");
                                        $("#wholesale-customer-div").addClass('hide');//remvoeClass("d-none");
                                        $("#purchase-supplier-div").removeClass('hide');
                                        $("#btnPlaceOrder").addClass('hide');
                                        $("#btnPurchaseOrder").removeClass('hide');
                                        $(".p-order").addClass('hide');
                                        //$("#shippingAmount").val(0);
                                        $("#btnUpdateAddress").addClass('hide');

                                    } else {
                                        $("#cart-text").text("Shopping Cart - Online Order");
                                        $("#wholesale-customer-div").addClass('hide');//remvoeClass("d-none");
                                        $("#purchase-supplier-div").addClass('hide');

                                        $("#btnPlaceOrder").removeClass('hide');
                                        $("#btnPurchaseOrder").addClass('hide');
                                        $(".p-order").removeClass('hide');
                                        newGrandTotal = grandTotal + shippingCharges;
                                    }
                                    $(".checkout-grandTotal").html(siteCurrency() + newGrandTotal);


                                }


                                $('#firstName').val(data.FirstName);
                                $('#mobile').val(data.Mobile);
                                $('#address').val(data.ShipAddress);
                                $('#zip').val(data.ShipZipCode);
                                $('#city').val(data.ShipCity);
                                $('#state').val(data.ShipState);
                                $('#email').val(data.Email);

                                // Show in confirm order section
                                $('#showFirstName').html(data.FirstName);
                                $('#showMobile').html(data.Username);
                                $('#showAddress').html(data.ShipAddress);
                                $('#showZipCode').html(data.ShipZipCode);
                                $('#showCity').html(data.ShipCity);
                                $('#showState').html(data.ShipState);

                                $(".checkout-shippingAmount").html(siteCurrency() + $('option:selected', "#city").attr('shipping-charges'));
                                $("#shippingAmount").val($('option:selected', "#city").attr('shipping-charges'));
                                builtShoppingCartItems();
                                confirmOrderList();
                            }
                        },
                        error: function (xhr) {
                            $('.item-loading').hide();
                        }
                    });
                    $.ajax({
                        dataType: "json",
                        url: '/Home/GetCustomerList',
                        data: {},
                        success: function (recordSet) {
                            let html = "";

                            html += "<option value='0'>Select Customer</option>";
                            for (let i = 0; i < recordSet.length; i++) {
                                html += "<option value='" + recordSet[i].Id + "' mobile=" + recordSet[i].Mobile + " country='" + recordSet[i].ShipCountry + "' postal-code='" + recordSet[i].ShipZipCode + "' address=" + recordSet[i].ShipAddress + " name=" + recordSet[i].FirstName + " city='" + recordSet[i].ShipCity + "'>" + recordSet[i].CustName + " </option>";
                            }

                            $("#customers").html(html);

                        },
                        error: function (xhr) {
                            $('.item-loading').hide();
                        }
                    });
                    $.ajax({
                        dataType: "json",
                        url: '/Supplier/GetSupplierList',
                        data: {},
                        success: function (recordSet) {
                            let html = "";

                            html += "<option value='0'>Select Supplier</option>";
                            for (let i = 0; i < recordSet.length; i++) {
                                html += "<option value='" + recordSet[i].Id + "' mobile=" + recordSet[i].Mobile + " country='" + recordSet[i].State + "' postal-code='" + recordSet[i].Postcode + "' address=" + recordSet[i].Address + " name=" + recordSet[i].Name + " city='" + recordSet[i].City + "'>" + recordSet[i].Name + " </option>";
                            }

                            $("#purchase-supplier").html(html);

                        },
                        error: function (xhr) {
                            $('.item-loading').hide();
                        }
                    });
                }
            },
            error: function (xhr) {
                $('.item-loading').hide();
            }
        });





    }
    else {
        builtShoppingCartItems();
        confirmOrderList();
        $('.customer-address').hide();
    }
}

function builtShoppingCartItems() {

    var subTotal = 0;
    var vatAmount = 0;
    var shippingAmount = getShippingCharge();
    var grandTotal = 0;

    var cart = getCart();

    var html = '<table class="tbl-shopping-cart-items">';

    html += '<tr class="shopping-cart-header">';
    html += '<td>SN</td>';
    html += '<td>Name</td>';
    html += '<td>Weight</td>';
    html += '<td class="center">Price</td>';
    html += '<td class="center">Qty</td>';
    html += '<td class="center">Discount</td>';
    html += '<td class="center">Total</td>';
    //html += '<td class="center">Gst</td>';
    html += '<td class="center">Remove</td>';
    html += '</tr>';

    for (var i = 0; i < cart.length; i++) {

        let pk = cart[i].Id + "_" + cart[i].Size + "_" + cart[i].Color;


        var itemTotal = (parseFloat(cart[i].OnlinePrice - cart[i].Discount, 10) * parseInt(cart[i].Quantity, 10));
        var quantityInputBoxId = 'txtQty_' + cart[i].Id;

        html += '<tr>';

        html += '<td>';
        html += i + 1;
        //html += '<img src="' + cart[i].ImageUrl + '" />';
        html += '</td>';

        html += '<td>';
        html += '<a href="/Product/Details?id=' + cart[i].Id + '">' + cart[i].Name + '</a>';
        html += '</td>';

        html += '<td class="center">';
        html += '<span>' + cart[i].Weight + cart[i].Unit + '</span>';
        html += '</td>';
        html += '<td class="center">';
        html += '<span>' + siteCurrency() + cart[i].OnlinePrice + '</span>';
        html += '</td>';

        html += '<td>';
        html += '<input type="number" class="font-control" style="width:50px; text-align:center;" value="' + cart[i].Quantity + '" id="' + quantityInputBoxId + '" />';
        html += '</td>';

        html += '<td class="center">';
        html += '<span>' + siteCurrency() + (cart[i].Discount * cart[i].Quantity).toFixed(2) + '</span>';
        html += '</td>';

        html += '<td class="center">';
        html += '<span>' + siteCurrency() + itemTotal.toFixed(2) + '</span>';
        html += '</td>';


        //html += '<td class="center">';
        //html += '<span>' + calculateGst(itemTotal, cart[i].Gst) + '</span>';
        //html += '</td>';

        html += '<td class="center">';
        html += '<img id="' + cart[i].Id + '" pk=' + pk + ' class="delete-shopping-cart-item img-cart" src="/Images/cross.png" style="cursor:pointer;">';
        html += '</td>';

        html += '</tr>';
    }

    html += '</table>';

    // Getting summary calculated amount
    var obj = getSummaryAmount();
    subTotal = obj.subTotal;
    vatAmount = obj.vatAmount;
    shippingAmount = +$("#shippingAmount").val();
    grandTotal = obj.grandTotal.toFixed(2);

    $("#btn-apply-coupon").attr("checkout-grandTotal", grandTotal);

    $('#checkout-subTotal').html(siteCurrency() + subTotal.toFixed(2));
    //$('#vatPerc').html('(' + getVatPercentage() + '%)');
    $('#vatPerc').html('');
    $('#checkout-vatAmount').html(siteCurrency() + vatAmount);
    $('.checkout-shippingAmount').html(siteCurrency() + shippingAmount.toFixed(2));
    $('.checkout-grandTotal').html(siteCurrency() + (+grandTotal + shippingAmount).toFixed(2));
    $("#grandTotal").val(grandTotal);
    $('.shopping-cart-container').html(html);

    $("#div-place-order").show();

}

function calculateGst(totalAmount, gst) {
    return ((totalAmount * gst) / 100).toFixed(2);
}

function confirmOrderList() {

    var subTotal = 0;
    var vatAmount = 0;
    var shippingAmount = getShippingCharge();
    var grandTotal = 0;
    var totalQuantity = 0

    var cart = getCart();

    var html = '<table class="tbl-shopping-cart-items">';

    html += '<tr class="shopping-cart-header">';
    html += '<td>SL</td>';
    //html += '<td>Image</td>';
    html += '<td class="left">Name</td>';
    html += '<td class="left">Weight</td>';
    html += '<td class="center">Price</td>';
    html += '<td class="center">Qty</td>';
    //html += '<td class="center">GST</td>';
    html += '<td class="right">Total</td>';
    html += '</tr>';

    for (var i = 0; i < cart.length; i++) {

        var itemTotal = (parseFloat(cart[i].OnlinePrice, 10) * parseInt(cart[i].Quantity, 10));

        html += '<tr>';

        html += '<td>';
        html += '<span>' + (i + 1) + '</span>';
        html += '</td>';

        //html += '<td>';
        //html += '<img src="' + cart[i].ImageUrl + '" class="img-cart" />';
        //html += '</td>';

        html += '<td class="left">';
        html += '<a href="/Product/Details?id=' + cart[i].Id + '">' + cart[i].Name + '</a>';
        html += '</td>';

        html += '<td class="center">';
        html += '<span>' + cart[i].Weight + cart[i].Unit + '</span>';
        html += '</td>';


        html += '<td class="center">';
        html += '<span>' + siteCurrency() + cart[i].OnlinePrice + '</span>';
        html += '</td>';

        html += '<td>';
        html += '<span>' + cart[i].Quantity + '</span>';
        html += '</td>';

        //html += '<td>';
        //html += '<span>' + calculateGst(itemTotal, cart[i].Gst) + '</span>';
        //html += '</td>';

        html += '<td class="right">';
        html += '<span>' + siteCurrency() + itemTotal.toFixed(2) + '</span>';
        html += '</td>';

        html += '</tr>';
    }

    // Getting summary calculated amount
    var obj = getSummaryAmount();
    subTotal = obj.subTotal;
    vatAmount = obj.vatAmount;
    shippingAmount = +$("#shippingAmount").val();
    grandTotal = (obj.grandTotal + shippingAmount).toFixed(2);
    totalQuantity = obj.totalQuantity;

    // Summary row
    html += '<tr class="summary-row right">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Sub Total (' + totalQuantity + ' items):</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + subTotal.toFixed(2) + '</span>';
    html += '</td>';

    html += '</tr>';

    // Vat amount row    
    //html += '<tr class="summary-row">';

    //html += '<td colspan="6">';
    ////html += '<span style="float:right;">Vat (' + getVatPercentage() + '%):</span>';
    //html += '<span style="float:right;">Total Gst:</span>';
    //html += '</td>';

    //html += '<td class="right">';
    //html += '<span>' + siteCurrency() + vatAmount + '</span>';
    //html += '</td>';

    //html += '</tr>';

    // Discount row
    html += '<tr class="summary-row">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Discount:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span class="discount">' + siteCurrency() + 0 + '</span>';
    html += '</td>';

    html += '</tr>';

    // Shipping amount row
    html += '<tr class="summary-row /*shipping-cost"*/>';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Shipping Cost:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span class="checkout-shippingAmount">' + siteCurrency() + (+$("#shippingAmount").val()).toFixed(2) + '</span>';
    html += '</td>';

    html += '</tr>';

    // Grand total row
    html += '<tr class="summary-row grand-total">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Grand Total:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span class="checkout-grandTotal">' + siteCurrency() + grandTotal + '</span>';
    html += '</td>';

    html += '</tr>';

    // Shipping charge note
    html += '<tr class="summary-row shipping-cost-note">';
    html += '<td colspan="7">';
    html += '<span class="" style="float:right; font-weight:400;">Note: Shipping charge will be added based on location and weight</span>';
    html += '</td>';
    html += '</tr>';

    html += '</table>';

    $('#order-item-list').html(html);
}