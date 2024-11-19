function getAllTestimonies(callback) {

    $('.item-loading').show();
    $.ajax({
        dataType: "json",
        url: '/Testimony/GetAllTestimonies',
        success: function (recordSet) {
            $('.item-loading').hide();
            $('#delete-btn-container').show();

            var dataSet = [];
            if (recordSet.length > 0) {
                //for (var i = 0; i < recordSet.length; i++) {
                //    var record = [];
                //    record.push(recordSet[i].Id);
                //    record.push(recordSet[i].Description);
                //    record.push(recordSet[i].IsActive);

                //    dataSet.push(record);

                //}
            }

            callback(recordSet);
        },
        error: function (xhr) {
            $('.item-loading').hide();
        }
    });
}

$(document).ready(function () {

});

app.controller('TestimonyListCtrl', ['$rootScope', '$scope', '$http', '$filter', '$location', 'Enum', 'orderListService', function ($rootScope, $scope, $http, $filter, $location, Enum, orderListService) {

    populateTestimonies();

    function populateTestimonies() {
        getAllTestimonies(function (records) {
            let html = '';
            for (let i = 0; i < records.length; i++) {
                html += '<tr>' +
                    '<td><input class="check-box" type="checkbox"' + (records[i].IsActive ? 'checked' : '') + ' id="' + records[i].Id + '" value="' + records[i].Id + '"></td>' +
                    '<td>' + records[i].Description + '</td>';

            }
            $("#testimoniesBody").html(html);
            //$('#data-table-admin-orderlist').dataTable({
            //    "data": records,
            //    "bLengthChange": false,
            //    "bFilter": true,
            //    "pageLength": 50,
            //    "bDestroy": true,
            //    "order": [[1, "desc"]],
            //    "columns": [
            //        { "title": "IsActive", "class": "center" },
            //        { "title": "Description", "class": "center" },
            //    ],
            //    "aoColumnDefs": [
            //        {
            //            "aTargets": [0],
            //            "visible": true,
            //            "bSortable": false,
            //            "mRender": function (data, type, row) {
            //                var text = '<input class="check-box" type="checkbox" id="' + row[0] + '" value="' + row[0] + '">';
            //                return $("<div/>").append(text).html();
            //            }
            //        },
            //        {
            //            "aTargets": [1],
            //            "mRender": function (data, type, row) {
            //                var text = row[1];
            //                return $("<div/>").append(text).html();
            //            }
            //        }
            //    ]
            //});

            // Hide the action button column in completed list
            //if (orderStatus === 'Completed') {
            //    var table = $('#data-table-admin-orderlist').DataTable();
            //    var column = table.column(8);
            //    column.visible(false);
            //}


        });
    }

    $('#btn-update').click(function () {

        var checkIds = '';
        var count = 0;

        $('.check-box:checkbox:checked').each(function () {
            var id = (this.checked ? $(this).val() : "");

            if (id) {
                checkIds += id + ',';
                count++;
            }
        });

        if (count === 0) {
            bootbox.alert("<h4>Please select a testimony to update status!</h4>", function () { });
            return;
        }


        bootbox.confirm("<h4>Are you sure to update status?</h4>",
            function (result) {
                if (result) {
                    UpdateStatus(checkIds, function () {
                    });
                }
            });

    });

    function UpdateStatus(ids, callback) {
        $.ajax({
            dataType: "json",
            url: '/Testimony/UpdateTestimonies',
            data: { testimoniesId: ids },
            method:"post",
            success: function (data) {

                if (data.IsSuccess) {
                    bootbox.alert("<h4>Update successfully!</h4>", function () { });
                }
                else {
                    bootbox.alert("<h4>Not Updated, some error occured!</h4>", function () { });

                }
            },
            error: function (xhr) {
                bootbox.alert("<h4>Error!</h4>", function () { });
            }
        });
    }

}]);
