$(function () {
    $('[data-toggle="popover"]').popover()
})

$(document).ready(function () {
    $(".update-image-viewer").click(function () {
        UpdateImage(this);
    });

    $("#registration_form").on("submit", SubmitRegistrationForm);
    $("#login_form").on("submit", SubmitLoginForm);
});

function UpdateImage(update) {
    var image = update.parentElement.parentElement.parentElement.parentElement.getElementsByClassName("image-viewer")[0];
    if (image != null) {
        var elem = image.parentElement.parentElement.parentElement.parentElement.getElementsByClassName('image-url-setter')[0];
        $(image).attr("src", $(elem).val())
    }
}

/**
 * Loading page Show/Hide
 */
function HoldOn() {
    var loader = document.getElementById("loading_screen");
    $(loader).show();
}

function EndHold() {
    var loader = document.getElementById("loading_screen");
    $(loader).hide();
}

/**
 * Submit the Registration form from the menu
 * @param {any} event
 */
function SubmitRegistrationForm(event) {
    event.preventDefault();

    HoldOn();

    var panel = document.getElementById('registration_panel');
    var that = $(this);
    var frmValues = that.serialize();
    $.ajax({
        type: that.attr('method'),
        url: that.attr('action'),
        data: frmValues,

        success: function (data) {
            $(panel).html(data);
            EndHold();
        },
        error: function (data) {
            $(panel).html(data.responseText);
            var newForm = document.getElementById("registration_form");
            $(newForm).on("submit", SubmitRegistrationForm);
            EndHold();
        }
    })
}

/**
 * Submit the Login form from the menu
 * @param {any} event
 */
function SubmitLoginForm(event) {
    event.preventDefault();

    HoldOn();

    var panel = document.getElementById('login_panel');
    var that = $(this);
    var frmValues = that.serialize();
    $.ajax({
        type: that.attr('method'),
        url: that.attr('action'),
        data: frmValues,

        success: function (data) {
            console.log(data);
            EndHold();   
            window.location.href = data;
        },
        error: function (data) {
            $(panel).html(data.responseText);
            var newForm = document.getElementById("login_form");
            $(newForm).on("submit", SubmitLoginForm);
            EndHold();
        }
    })
}