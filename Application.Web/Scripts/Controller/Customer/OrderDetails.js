function populateCustomerOrderDetails(callback) {

    var orderId = getParam('orderId');

    $('.item-loading').show();    
    $.ajax({
        dataType: "json",
        url: '/Customer/GetOrderDetails',
        data: {orderId: orderId},
        success: function (order) {
            $('.item-loading').hide();
            renderOrderItems(order);
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

function cusPrintOrder() {

    var orderId = getParam('orderId');
    window.location.href = "/Order/PrintOrder?orderId=" + orderId;
}

function renderOrderItems(order) {

    //var subTotal = 0;
    //var vatAmount = 0;
    //var shippingAmount = 0;
    //var grandTotal = 0;
    //var totalQuantity = 0

    //if (order) {
    //    $('#orderId').html(order.Id);
    //    $('#orderStatus').html(order.OrderStatus);
    //    $('#orderDate').html(moment(order.ActionDate, "x").format('DD MMM YYYY h:mm:ss a'));
    //}

    //var html = '<table class="tbl-shopping-cart-items">';

    //html += '<tr class="shopping-cart-header">';
    //html += '<td>SL</td>';
    //html += '<td>Image</td>';
    //html += '<td class="left">Name</td>';
    //html += '<td class="center">Price</td>';
    //html += '<td class="center">Qty</td>';
    //html += '<td class="right">Total</td>';
    //html += '</tr>';

    //for (var i = 0; i < order.OrderItems.length; i++) {

    //    var item = order.OrderItems[i];

    //    var itemTotal = (parseFloat(item.Price, 10) * parseInt(item.Quantity, 10));

    //    html += '<tr>';

    //    html += '<td>';
    //    html += '<span>' + (i + 1) + '</span>';
    //    html += '</td>';

    //    html += '<td>';
    //    html += '<img src="' + item.ImageUrl + '" class="img-cart" />';
    //    html += '</td>';

    //    html += '<td class="left">';
    //    html += '<span>' + item.ProductName + '</span>';
    //    html += '</td>';

    //    html += '<td class="center">';
    //    html += '<span>' + siteCurrency() + item.Price + '</span>';
    //    html += '</td>';

    //    html += '<td>';
    //    html += '<span>' + item.Quantity + '</span>';
    //    html += '</td>';

    //    html += '<td class="right">';
    //    html += '<span>' + siteCurrency() + itemTotal + '</span>';
    //    html += '</td>';

    //    html += '</tr>';
    //}

    //// Getting summary calculated amount
    //subTotal = order.PayAmount - order.Vat - order.ShippingAmount;
    //vatAmount = order.Vat;
    //shippingAmount = order.ShippingAmount ? order.ShippingAmount : 0;
    //grandTotal = order.PayAmount;
    //totalQuantity = order.OrderItems.length;

    //// Summary row
    //html += '<tr class="summary-row right">';

    //html += '<td colspan="5">';
    //html += '<span style="float:right;">Sub Total:</span>';
    //html += '</td>';

    //html += '<td class="right">';
    //html += '<span>' + siteCurrency() + subTotal + '</span>';
    //html += '</td>';

    //html += '</tr>';

    //// Vat amount row
    //html += '<tr class="summary-row">';

    //html += '<td colspan="5">';
    //html += '<span style="float:right;">Vat (' + getVatPercentage() + '%):</span>';
    //html += '</td>';

    //html += '<td class="right">';
    //html += '<span>' + siteCurrency() + vatAmount + '</span>';
    //html += '</td>';

    //html += '</tr>';

    //// Shipping amount row
    //html += '<tr class="summary-row">';

    //html += '<td colspan="5">';
    //html += '<span style="float:right;">Shipping Charge:</span>';
    //html += '</td>';

    //html += '<td class="right">';
    //html += '<span>' + siteCurrency() + shippingAmount + '</span>';
    //html += '</td>';

    //html += '</tr>';

    //// Grand total row
    //html += '<tr class="summary-row grand-total">';

    //html += '<td colspan="5">';
    //html += '<span style="float:right;">Grand Total:</span>';
    //html += '</td>';

    //html += '<td class="right">';
    //html += '<span>' + siteCurrency() + grandTotal + '</span>';
    //html += '</td>';

    //html += '</tr>';

    //html += '</table>';

    var subTotal = 0;
    var discount = 0;
    var vatAmount = 0;
    var shippingAmount = 0;
    var grandTotal = 0;
    var totalQuantity = 0

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
        $('#orderType').html(order.OrderType);
        $("#orderStatusId").val(order.StatusId);
        $("#status").val(order.StatusId);
        if (order.OrderMode === 'Store') {
            $('#trTotalWeight').hide();
            $('#trDeliveryDate').hide();
        }
    }

    var html = '<table class="tbl-shopping-cart-items">';

    html += '<tr class="shopping-cart-header">';
    html += '<td style="width:50px;text-align:center;">SL</td>';
    html += '<td style="width:100px;">Image</td>';
    html += '<td class="left">Name</td>';
    html += '<td class="left" style="width: 5%;">Size</td>';
    html += '<td class="left" style="width: 5%;">Color</td>';
    html += '<td class="center" style="width:150px;">Price</td>';
    html += '<td class="center" style="width:100px;">Qty</td>';
    html += '<td class="right" style="width:150px;">Total</td>';
    html += '</tr>';

    for (var i = 0; i < order.OrderItems.length; i++) {

        var item = order.OrderItems[i];

        var itemTotal = (parseFloat(item.Price, 10) * parseInt(item.Quantity, 10));

        html += '<tr>';

        html += '<td style="text-align:center;">';
        html += '<span>' + (i + 1) + '</span>';
        html += '</td>';

        html += '<td>';
        html += '<img src="' + item.ImageUrl + '" style="height:80px;" class="img-cart" />';
        html += '</td>';

        html += '<td class="left">';
        html += '<span>' + item.ProductName + '</span>';
        html += '</td>';
        html += '<td class="left">';
        html += '<span>' + item.Size + '</span>';
        html += '</td>';
        html += '<td class="left">';
        html += '<span>' + item.Color + '</span>';
        html += '</td>';

        html += '<td class="center">';
        html += '<span>' + siteCurrency() + item.Price + '</span>';
        html += '</td>';

        html += '<td class="center">';
        html += '<span>' + item.Quantity + '</span>';
        html += '</td>';

        html += '<td class="right">';
        html += '<span>' + siteCurrency() + itemTotal.toFixed(2) + '</span>';
        html += '</td>';

        html += '</tr>';
    }

    // Getting summary calculated amount
    subTotal = order.PayAmount + order.Discount - order.ShippingAmount;
    discount = order.Discount.toFixed(2);
    vatAmount = order.Vat.toFixed(2);
    shippingAmount = order.ShippingAmount ? order.ShippingAmount : 0;
    grandTotal = order.PayAmount.toFixed(2);
    totalQuantity = order.OrderItems.length;

    // Summary row
    html += '<tr class="summary-row right">';

    html += '<td colspan="7">';
    html += '<span style="float:right;">Sub Total:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + subTotal.toFixed(2) + '</span>';
    html += '</td>';

    html += '</tr>';

    // Vat amount row    
    html += '<tr class="summary-row">';

    html += '<td colspan="7">';
    html += '<span style="float:right;">Gst:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + vatAmount + '</span>';
    html += '</td>';

    html += '</tr>';

    // Discount row    
    if (order.Discount > 0) {
        html += '<tr class="summary-row">';

        html += '<td colspan="7">';
        html += '<span style="float:right;">(-)Discount:</span>';
        html += '</td>';

        html += '<td class="right">';
        html += '<span>' + siteCurrency() + discount + '</span>';
        html += '</td>';

        html += '</tr>';
    }


    // Shipping amount row
    html += '<tr class="summary-row /*shipping-cost*/">';

    html += '<td colspan="7">';
    html += '<span style="float:right;">Shipping Charge:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + shippingAmount.toFixed(2) + '</span>';
    html += '</td>';

    html += '</tr>';

    // Grand total row
    html += '<tr class="summary-row grand-total">';

    html += '<td colspan="7">';
    html += '<span style="float:right;">Grand Total:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + grandTotal + '</span>';
    html += '</td>';

    html += '</tr>';

    // Shipping charge note
    html += '<tr class="summary-row shipping-cost-note">';
    html += '<td colspan="8">';
    html += '<span class="" style="float:right; font-weight:400;">Note: Shipping charge will be added based on location and weight</span>';
    html += '</td>';
    html += '</tr>';

    html += '</table>';

    $('#customer-order-details-item-list').html(html);
}

app.factory('service', [
    '$http', function ($http) {

        return {
            
        };
    }
]);

app.controller('CustomerOrderDetailsCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'service', function ($rootScope, $scope, $http, $filter, $location, Enum, service) {

    var orderId = getParam('orderId');

    if (orderId) {
        getOrder(orderId);
    }

    function getOrder(orderId) {
        populateCustomerOrderDetails(orderId);        
    }

    $('#cusPrintOrder').click(function () {
        cusPrintOrder();
    });

}]);
