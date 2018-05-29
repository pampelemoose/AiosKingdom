$(function () {
    $('[data-toggle="popover"]').popover()
})

$(document).ready(function () {
    $(".update-image-viewer").click(function () {
        var image = this.parentElement.parentElement.getElementsByClassName("image-viewer")[0];
        if (image != null) {
            var elem = image.parentElement.parentElement.getElementsByClassName('image-url-setter')[0];
            $(image).attr("src", $(elem).val())
        }
    });
});