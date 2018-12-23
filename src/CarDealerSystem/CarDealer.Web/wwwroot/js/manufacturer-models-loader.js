$(document).ready(function () {
    var items2 = "<option>--- All ---</option>";
    var items = '<li data-value="--- All ---" class="option disabled">--- All ---</li>';
    $('#ModelName').next('div').find('ul').html(items);
    $('#ModelName').html(items2);

    $('#ManufacturerId').on('change', function () {
        var url = '/Vehicle/Vehicle/GetModelsByManufacturerId';
        var ddlSource = "#ManufacturerId";
        $.getJSON(url, { manufacturerId: $(ddlSource).val() }, function (data) {
            var items = '<li data-value="--- All ---" class="option disabled">--- All ---</li>';
            var items2 = "<option disabled>--- All ---</option>";
            $('#ModelName').next('div').find('ul').empty();
            for (var i = 0; i < data.length; i++) {
                var item = `<li data-value="${data[i].text}" class="option focus">${data[i].text}</li\>`;
                items += item;
            }
            $.each(data, function (i, vehModel) {
                items2 += "<option>" + vehModel.text + "</option>";
            });
            $('#ModelName').next('div').find('ul').html(items);
            $('#ModelName').html(items2);
        });
    });
});