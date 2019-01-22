$('.close-btn').on("click", removePicture);

$('#file-input').on('change',
    function (e) {
        var files = e.target.files;
        var myId = $('#Vehicle_Id').val();
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                data.append('__RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
                $.ajax({
                    type: "POST",
                    url: '/Ad/UploadFile?id=' + myId,
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        preview(result);
                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                    }
                });
            }
        }
    });

function preview(data) {
    $('[value="/images/vehicles/default.jpg"]').closest('li').remove();

    var gallery = $('#gallery');
    var count = gallery.children().length;

    for (var i = 0; i < data.urls.length; i++) {
        var url = data.urls[i];

        var li = $('<li>')
            .append($('<a>')
                .append($('<img>')
                    .attr("src", url)))
            .append($('<input>')
                .addClass('vehicle-picture')
                .attr("value", url)
                .attr("hidden", "hidden")
                .attr("id", `urls[${i + count}]`)
                .attr("name", `urls[${i + count}]`))
            .append($('<a>')
                .addClass("close-btn")
                .text('x')
                .on("click", removePicture));

        gallery.append(li);
    }
}

function removePicture() {
    $(this).closest('li').remove();

    var $inputs = $(".vehicle-picture");

    $inputs.each(function (index, element) {
        $(this)
            .attr("id", `urls[${index}]`)
            .attr("name", `urls[${index}]`);
    });

    if ($inputs.length === 0) {
        var li = $('<li>')
            .append($('<a>')
                .append($('<img>')
                    .attr("src", "/images/vehicles/default.jpg")))
            .append($('<input>')
                .addClass('vehicle-picture')
                .attr("value", '/images/vehicles/default.jpg')
                .attr("hidden", "hidden")
                .attr("id", 'urls[0]')
                .attr("name", 'urls[0]'))
            .append($('<a>')
                .addClass("close-btn")
                .text('x')
                .on("click", removePicture));

        $('#gallery').append(li);
    }
}