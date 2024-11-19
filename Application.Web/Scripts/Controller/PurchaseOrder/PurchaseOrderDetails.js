var statuses = [];
function populatePurchaseOrderDetails(callback) {

    var orderId = getParam('orderId');

    $('.item-loading').show();


    $.ajax({
        dataType: "json",
        url: '/Order/GetStatuses',
        success: function (result) {
            if (result.IsSuccess) {
                statuses = result.data;
                let html = "";
                html += "<option value='0'>Select Status</option>";
                for (let i = 0; i < result.data.length; i++) {
                    html += "<option value='" + result.data[i].Id + "' data-id='" + result.data[i].Name + "'>" + result.data[i].Name + " </option>";
                }

                $("#purchaseStatus").html(html);
                populatePurchaseHistory(statuses, orderId);
                populatePurchaseOrder();
            }
        }
    });
    //populatePurchaseOrder();

}

function populatePurchaseOrder() {

    $.ajax({
        dataType: "json",
        url: '/PurchaseOrder/GetOrderDetails',
        data: { orderId: getParam('orderId') },
        success: function (order) {
            $('.item-loading').hide();
            renderPurchaseOrderItems(order);

        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });

}

function populatePurchaseHistory(statuses) {
    $("#purchase-order-table").DataTable().destroy();
    $.ajax({
        dataType: "json",
        url: '/OrderStatus/GetOrderStatusByOrder',
        data: { OrderId: getParam('orderId') },
        success: function (result) {
            if (result.IsSuccess) {

                let html = "";

                for (let i = 0; i < result.Data.length; i++) {
                    html += "<tr>" +
                        "<td>" + moment(result.Data[i].CreatedDate, "x").format('DD MMM YYYY h:mm:ss a') + "</td>" +
                        "<td>" + result.Data[i].CreatedBy + "</td>" +
                        "<td>" + getStatusName(statuses, result.Data[i].NewStatusId) + "</td>" +
                        "<td>" + result.Data[i].Description + "</td>" +
                        "</tr> ";
                }
                $("#history").html(html);
                $("#purchase-order-table").dataTable({
                    //"data": records,
                    "bLengthChange": false,
                    //"bFilter": true,
                    sorting: false,
                    "pageLength": 5,
                    "bDestroy": true,
                    "responsive": true
                });
            }
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

function getStatusName(statuses, id) {
    return statuses.filter(x => x.Id == id)[0].Name;
}

function populateCustomerAddress(userId) {

    $.ajax({
        dataType: "json",
        url: '/Admin/GetCustomer',
        data: { id: userId },
        success: function (customer) {
            if (customer) {
                $('#txtPhone').val(customer.Username);
                $('#txtFirstName').val(customer.FirstName);
                $('#txtLastName').val(customer.LastName);
                $('#txtCity').val(customer.ShipCity);
                $('#ddlPrefecture').val(customer.ShipState);
                $('#txtPostCode').val(customer.ShipZipCode);
                $('#txtAddress').val(customer.ShipAddress);
            }
        },
        error: function (xhr) {

        }
    });
}

function printPurchaseOrder() {

    var orderId = getParam('orderId');
    window.location.href = "/PurchaseOrder/PrintOrder?orderId=" + orderId;
}

function printInvoice() {

    var orderId = getParam('orderId');
    window.location.href = "/Order/PrintInvoice?orderId=" + orderId;
}

function renderPurchaseOrderItems(order) {

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
        $("#purchaseOrderStatusId").val(order.StatusId);
        $("#purchaseStatus").val(order.StatusId);
        localStorage.setItem("purchaseOrderStatusId", order.StatusId);
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
    //discount = order.Discount.toFixed(2);
    //vatAmount = order.Vat.toFixed(2);
    shippingAmount = order.ShippingAmount ? order.ShippingAmount : 0;
    grandTotal = order.PayAmount.toFixed(2);
    totalQuantity = order.OrderItems.length;

    // Summary row
    html += '<tr class="summary-row right">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Sub Total:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + subTotal.toFixed(2) + '</span>';
    html += '</td>';

    html += '</tr>';

    // Vat amount row    
    html += '<tr class="summary-row">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Gst:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + vatAmount + '</span>';
    html += '</td>';

    html += '</tr>';

    // Discount row    
    if (order.Discount > 0) {
        html += '<tr class="summary-row">';

        html += '<td colspan="5">';
        html += '<span style="float:right;">(-)Discount:</span>';
        html += '</td>';

        html += '<td class="right">';
        html += '<span>' + siteCurrency() + discount + '</span>';
        html += '</td>';

        html += '</tr>';
    }


    // Shipping amount row
    html += '<tr class="summary-row /*shipping-cost*/">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Shipping Charge:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + shippingAmount.toFixed(2) + '</span>';
    html += '</td>';

    html += '</tr>';

    // Grand total row
    html += '<tr class="summary-row grand-total">';

    html += '<td colspan="5">';
    html += '<span style="float:right;">Grand Total:</span>';
    html += '</td>';

    html += '<td class="right">';
    html += '<span>' + siteCurrency() + grandTotal + '</span>';
    html += '</td>';

    html += '</tr>';

    // Shipping charge note
    html += '<tr class="summary-row shipping-cost-note">';
    html += '<td colspan="6">';
    html += '<span class="" style="float:right; font-weight:400;">Note: Shipping charge will be added based on location and weight</span>';
    html += '</td>';
    html += '</tr>';

    html += '</table>';

    $('#order-details-item-list').html(html);
}

function updatePurchaseOrderStatus(orderStatusName) {

    let oldStatusId = $("#purchaseOrderStatusId").val();
    let description = $("#purchaseDescription").val();

    $.ajax({
        dataType: "json",
        url: '/OrderStatus/CreateOrderStatusHistory',
        data: { OrderId: getParam('orderId'), Description: description, OldStatusId: oldStatusId, NewStatusId: $("#purchaseStatus").val(), NewStatus: orderStatusName, IsPurchaseOrder: true },
        success: function (data) {
            if (data.IsSuccess) {
                bootbox.alert("<h4>Successfully updated the status!</h4>", function () { });
                $("#purchaseDescription").val('');
                populatePurchaseHistory(statuses);
                populatePurchaseOrder();
            }
            else {
                bootbox.alert("<h4>Failed to update the status!</h4>", function () { });
            }
        },
        error: function (xhr) {
            bootbox.alert("<h4>Error to update the status!</h4>", function () { });
        }
    });
}

app.controller('AdminPurchaseOrderDetailsCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'service', function ($rootScope, $scope, $http, $filter, $location, Enum, service) {

    var orderId = getParam('orderId');

    if (orderId) {
        getOrder(orderId);
    }

    function getOrder(orderId) {
        populatePurchaseOrderDetails(orderId);
    }

    $('#printOrder').click(function () {
        printPurchaseOrder();
    });

    $('#printInvoice').click(function () {
        printInvoice();
    });

    $('#updatePurchaseOrderStatus').click(function () {
        updatePurchaseOrderStatus($('option:selected', "#purchaseStatus").attr('data-id'));
    });


}]);
