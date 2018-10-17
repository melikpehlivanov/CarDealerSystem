var notifier = notifier || {};

$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
});


notifier.showMessage = function (message, type) {
    switch (type.toLowerCase()) {
        case "warning":
            notifier.showWarning(message);
            break;
        case "error":
            notifier.showError(message);
            break;
        case "success":
            notifier.showSuccess(message);
            break;
        case "info":
            notifier.showInfo(message);
            break;
        default:
            notifier.showInfo(message);
            break;
    }
};
notifier.showInfo = function (message) {
    toastr.info(message);
};
notifier.showError = function (message) {
    toastr.error(message);
};
notifier.showSuccess = function (message) {
    toastr.success(message);
};
notifier.showWarning = function (message) {
    toastr.warning(message);
};