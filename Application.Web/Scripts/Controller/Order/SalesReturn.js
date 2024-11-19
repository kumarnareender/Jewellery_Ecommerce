function populateSalesReturnOrderDetails(callback) {

    var orderId = getParam('orderId');
    $('.item-loading').show();
    var messageType = getParam('Type');

    $.ajax({
        dataType: "json",
        url: '/Customer/GetOrderDetails',
        data: { orderId: orderId },
        success: function (order) {
            $('.item-loading').hide();

            window.order = order;
            renderSalesReturnOrderItems(window.order);
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

function printOrder() {
    var orderId = getParam('orderId');
    window.location.href = "/Order/PrintOrder?orderId=" + orderId;
}

function renderSalesReturnOrderItems(order) {

    var subTotal = 0;
    var vatAmount = 0;
    var shippingAmount = 0;
    var grandTotal = 0;
    var totalDiscount = 0;
    var changeAmount = 0;

    if (order) {
        var weight = order.TotalWeight ? order.TotalWeight + ' Kg' : '';
        var deliveryDateTime = (order.DeliveryDate && order.DeliveryTime) ? order.DeliveryDate + ' ' + (order.DeliveryTime === 'Not Specified' ? '' : order.DeliveryTime) : '';

        $('#orderCode').html(order.OrderCode);
        $('#orderMode').html(order.OrderMode);
        $('#orderStatus').html(order.OrderStatus);
        $('#totalWeight').html(weight);
        $('#deliveryDate').html(deliveryDateTime);
        $('#frozenItem').html(order.IsFrozen ? 'Yes' : 'No');
        $('#orderDate').html(moment(order.ActionDate, "x").format('DD MMM YYYY h:mm:ss a'));

        if (order.OrderMode === 'Store') {
            $('#trTotalWeight').hide();
            $('#trDeliveryDate').hide();
        }
    }

    var html = '<table class="tbl-shopping-cart-items">';

    html += '<tr class="shopping-cart-header">';
    html += '<td style="width:50px;text-align:center;">SL</td>';
    //html += '<td style="width:100px;">Image</td>';
    html += '<td class="left">Name</td>';
    html += '<td class="left" style="width: 5%;">Weight</td>';
    html += '<td class="center" style="width:150px;">Price</td>';
    html += '<td class="center" style="width:100px;">Qty</td>';
    html += '<td class="right" style="width:150px;">Total</td>';
    html += '<td class="center" style="width:20px;">Del</td>';
    html += '</tr>';

    for (var i = 0; i < order.OrderItems.length; i++) {

        var item = order.OrderItems[i];

        var itemTotal = (parseFloat(item.Price, 10) * parseInt(item.Quantity, 10));
        var quantityInputBoxId = 'txtQty_' + item.Id;
        var priceInputBoxId = 'txtPrice_' + item.Id;

        html += '<tr>';

        html += '<td style="text-align:center;">';
        html += '<span>' + (i + 1) + '</span>';
        html += '</td>';

        //html += '<td>';
        //html += '<img src="' + item.ImageUrl + '" style="height:80px;" class="img-orderItems" />';
        //html += '</td>';

        html += '<td class="left">';
        html += '<span>' + item.ProductName + '</span>';
        html += '</td>';

        html += '<td class="left">';
        html += '<span>' + item.Weight + item.Unit + '</span>';
        html += '</td>';

        //html += '<td class="left">';
        //html += '<span>' + item.Color + '</span>';
        //html += '</td>';

        html += '<td class="center">';
        html += '<span>' + siteCurrency() + '</span>&nbsp;';
        html += '<input type="number" min="0" class="dynamic-inputbox data-change" style="width:80px; text-align:center;" value="' + item.Price + '" itemid="' + item.Id + '" id="' + priceInputBoxId + '" />';
        html += '</td>';

        html += '<td class="center">';
        html += '<input type="number" min="0" class="dynamic-inputbox data-change" style="width:50px; text-align:center;" value="' + item.Quantity + '" itemid="' + item.Id + '" id="' + quantityInputBoxId + '" />';
        html += '</td>';

        html += '<td class="right">';
        html += '<span id="itemTotal_' + item.Id + '">' + siteCurrency() + itemTotal + '</span>';
        html += '</td>';

        html += '<td class="center">';
        html += '<img id="' + item.Id + '" class="delete-shopping-cart-item img-cart" src="/Images/cross.png" style="cursor:pointer;">';
        html += '</td>';

        html += '</tr>';
    }

    // Getting summary calculated amount
    var obj = getSummaryValues();
    subTotal = obj.SubTotal;
    vatAmount = obj.Vat;
    shippingAmount = obj.ShippingCost;
    grandTotal = obj.GrandTotal;
    totalDiscount = obj.TotalDiscount;
    changeAmount = grandTotal - order.PayAmount;

    // Summary row
    html += '<tr class="summary-row right">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Sub Total:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span id="grandSubTotal">' + siteCurrency() + subTotal + '</span>';
    html += '</td>';
    html += '<td></td>';

    html += '</tr>';

    // Discount row    
    html += '<tr class="summary-row">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">(-)Discount:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + '</span>&nbsp';
    html += '<input type="number" min="0" class="dynamic-inputbox discount-change" style="width:80px; text-align:right;" value="' + totalDiscount + '"/>';

    html += '</td>';
    html += '<td></td>';

    html += '</tr>';

    // Vat amount row    
    html += '<tr class="summary-row">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Vat:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span id="vat">' + siteCurrency() + vatAmount + '</span>';
    html += '</td>';
    html += '<td></td>';

    html += '</tr>';

    // Shipping amount row
    html += '<tr class="summary-row">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Shipping Charge:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span id="shippingCost">' + siteCurrency() + shippingAmount + '</span>';
    html += '</td>';
    html += '<td></td>';

    html += '</tr>';

    // Grand total row
    html += '<tr class="summary-row grand-total">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Grand Total:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span id="grandTotal">' + siteCurrency() + grandTotal + '</span>';
    html += '</td>';
    html += '<td></td>';

    html += '</tr>';

    // Paid amount
    html += '<tr class="summary-row">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Paid Amount:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + window.order.PayAmount + '</span>';
    html += '</td>';
    html += '<td></td>';

    html += '</tr>';

    // Amount adjust
    html += '<tr class="summary-row">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Adjust Amount:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span id="grandChangeAmount">' + siteCurrency() + changeAmount + '</span>';
    html += '</td>';
    html += '<td></td>';

    html += '</tr>';

    html += '</table>';

    $('#order-details-item-list').html(html);
}

$('#order-details-item-list').on('change', '.data-change', function () {

    var itemId = $(this).attr('itemid');
    var qty = $('#txtQty_' + itemId).val();
    var price = $('#txtPrice_' + itemId).val();

    for (var i = 0; i < window.order.OrderItems.length; i++) {
        var item = window.order.OrderItems[i];
        if (item.Id == itemId) {
            window.order.OrderItems[i].Quantity = parseInt(qty, 10);
            window.order.OrderItems[i].Price = parseFloat(price, 10);
            break;
        }
    }

    renderSalesReturnOrderItems(window.order);
});

$('#order-details-item-list').on('change', '.discount-change', function () {
    var totalDiscount = $(this).val();
    window.order.Discount = totalDiscount;

    renderSalesReturnOrderItems(window.order);
});

function getTotalWeight() {
    var totalWeight = 0; // TODO
    return totalWeight;
}

$('#btnUpdateOrder').click(function () {

    var order = {};
    order.OrderItems = [];

    var orderItems = window.order.OrderItems;
    var totalDiscount = 0;
    var val = 0;
    var shippingAmount = window.order.ShippingAmount;
    var grandTotal = 0;

    for (var i = 0; i < orderItems.length; i++) {

        var orderItem = {};

        var price = parseFloat(orderItems[i].Price, 10);
        var quantity = parseInt(orderItems[i].Quantity, 10);
        var discount = parseFloat(orderItems[i].Discount, 10);

        orderItem.ProductId = orderItems[i].ProductId;
        orderItem.Quantity = quantity;
        orderItem.Discount = orderItems[i].Discount; //0; // Always 0 in online version because discount is already reduced from price
        orderItem.Price = price;
        orderItem.TotalPrice = quantity * price;
        orderItem.ImageUrl = orderItems[i].ImageUrl;
        orderItem.Weight = orderItems[i].Weight;
        orderItem.Unit = orderItems[i].Unit;
        orderItem.Title = orderItems[i].ProductName;
        orderItem.CostPrice = orderItems[i].CostPrice;
        orderItem.ActionDate = moment(orderItems[i].ActionDate, "x").format('YYYY-MM-DD h:mm:ss a');
        order.OrderItems.push(orderItem);
    }

    // Getting summary calculated amount
    var obj = getSummaryValues();
    vat = obj.Vat;
    shippingAmount = obj.ShippingCost;
    grandTotal = obj.GrandTotal;
    totalDiscount = obj.TotalDiscount;
    changeAmount = grandTotal - order.PayAmount;

    order.Id = window.order.Id;
    order.UserId = window.order.UserId;
    order.BranchId = window.order.BranchId;
    order.OrderCode = window.order.OrderCode;
    order.DueAmount = window.order.DueAmount;
    order.ReceiveAmount = window.order.ReceiveAmount;
    order.ChangeAmount = window.order.ChangeAmount;
    order.OrderStatus = window.order.OrderStatus;
    order.PaymentStatus = window.order.PaymentStatus;
    order.PaymentType = window.order.PaymentType;
    order.ActionDate = moment(window.order.ActionDate, "x").format('YYYY-MM-DD h:mm:ss a');
    order.ActionType = window.order.ActionType;
    order.DeliveryDate = window.order.DeliveryDate;
    order.DeliveryTime = window.order.DeliveryTime;
    order.TotalWeight = getTotalWeight();
    order.IsFrozen = window.order.IsFrozen;
    order.OrderMode = window.order.OrderMode;
    order.PayAmount = grandTotal;
    order.Discount = totalDiscount;
    order.Vat = vat;
    order.ShippingAmount = shippingAmount;
    order.CustomerId = window.order.CustomerId;
    // Saving Records
    $.ajax({
        dataType: "json",
        contentType: 'application/json',
        url: '/Order/UpdateOrder',
        data: JSON.stringify(order),
        method: 'POST',
        success: function (data) {
            if (data) {
                bootbox.alert("<h4>Order updated successfully!</h4>", function () { });
            }
            else {
                bootbox.alert("<h4>Failed to update order!</h4>", function () { });
            }

            $('#updateStatus').html('');
        },
        error: function (xhr) {
            $('#updateStatus').html('');
            bootbox.alert("<h4>Error occured while updating order!</h4>", function () { });
        }
    });


});

function getSummaryValues() {

    var subtotal = 0;
    var totalDiscount = 0;
    var vat = 0;
    var shippingCost = 0;
    var grandTotal = 0;
    var changeAmount = 0;

    for (var i = 0; i < window.order.OrderItems.length; i++) {

        var item = window.order.OrderItems[i];

        var quantity = item.Quantity;
        var discount = item.Discount * quantity;
        var itemTotal = (parseFloat(item.Price, 10)) * parseInt(quantity, 10);

        subtotal += itemTotal;
        totalDiscount += discount;
    }

    if (window.order.Discount > totalDiscount) {
        var extraDiscountGiven = window.order.Discount - totalDiscount;
        totalDiscount += extraDiscountGiven;
    }

    vat = window.order.Vat > 0 ? Math.round(((subtotal - totalDiscount) * getVatPercentage()) / 100) : 0; // If Vat taken by the order then take the vat with changed items
    shippingCost = window.order.ShippingAmount > 0 ? Math.round(window.order.ShippingAmount) : 0; // TODO

    grandTotal = subtotal - totalDiscount + vat + shippingCost;
    changeAmount = grandTotal - window.order.PayAmount;

    var obj = {};
    obj.SubTotal = subtotal;
    obj.TotalDiscount = totalDiscount;
    obj.Vat = vat;
    obj.ShippingCost = shippingCost;
    obj.GrandTotal = grandTotal;
    obj.ChangeAmount = changeAmount;

    return obj;
}

app.controller('SalesReturnCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'service', function ($rootScope, $scope, $http, $filter, $location, Enum, service) {

    var orderId = getParam('orderId');

    if (orderId) {
        populateSalesReturnOrderDetails(orderId);
    }

    $('#printOrder').click(function () {
        printOrder();
    });

    $('#printCourierSlip').click(function () {
        printCourierSlip();
    });

    $('#order-details-item-list').on('click', '.delete-shopping-cart-item', function () {
        var id = $(this).attr('id');
        removeItem(id)
        var currentTr = $(this).closest("tr");
        $(currentTr).remove();
    });

    function removeItem(id) {

        if (id) {
            adjustDiscount(id);
            var orderItems = jQuery.removeFromArray(id, window.order.OrderItems);
            window.order.OrderItems = orderItems;
            renderSalesReturnOrderItems(window.order);
        }
        else {
            var orderItems = [];
        }
    }

    function adjustDiscount(id) {
        for (var i = 0; i < window.order.OrderItems.length; i++) {
            var item = window.order.OrderItems[i];
            if (item.Id == id) {
                var itemDiscount = item.Quantity * item.Discount;
                window.order.Discount = window.order.Discount - itemDiscount;
                break;
            }
        }
    }

}]);
