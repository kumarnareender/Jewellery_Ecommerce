var freeText = getParam('search');
if (freeText) {
    $('#header-search-box').val(freeText);
}

loadCategoryTree();

$('#header-search-box').keydown(function (event) {
    var keyCode = (event.keyCode ? event.keyCode : event.which);
    if (keyCode == 13) {
        $('#header-search-btn').trigger('click');
    }
});

$("#header-search-btn").click(function () {

    var catId = $('#header-category').val();
    var locId = '0';
    var searchText = $('#header-search-box').val();

    if (!catId) catId = 0;

    window.location.href = '/Product/Search?cat=' + catId + "&loc=" + locId + "&search=" + searchText;
});

function loadCategoryTree() {
    $.ajax({
        dataType: "json",
        contentType: 'application/json',
        url: '/Category/GetParentCategoryList',
        method: 'GET',
        async: true,
        data: {},
        success: function (itemList) {
            for (var i = 0; i < itemList.length; i++) {
                $('#header-category').append('<option value="' + itemList[i].Id + '">' + itemList[i].Name + '</option>');
            }

            // If query string has category then select it
            var catId = getParam('cat');
            if (catId && catId !== "0") {
                $('#header-category').val(catId);
            }

        },
        error: function (xhr) {
        }
    });
}