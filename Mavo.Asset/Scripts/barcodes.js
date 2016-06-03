// ---------------------------------------- barcode scanning recognition

// receive barcodes using:
//   $(window).on('barcode', function (e, barcode) { });

var control = ['{', '}'];
var buf = null;
var timer = null;
var timeout = function () {
    buf = null;
    timer = null;
};
$(window).keypress(function (e) {
    var handled = false;
    var key = String.fromCharCode(e.charCode);
    if (key === control[0]) {

        // start marker
        buf = [];
        handled = true;

        // reset timeout
        if (timer) clearTimeout(timer);
        timer = setTimeout(timeout, 1000);
    }
    else if (key === control[1]) {

        // end marker
        var barcode = buf ? buf.join('') : null;
        buf = null;
        handled = true;

        // clear existing timeout
        if (timer) clearTimeout(timer);
        timer = null;

        if (barcode) {

            // try to send to a receiver textbox if one is focused
            var receiver = $(':input[data-barcode-receiver]');
            if (receiver.length > 0) {
                receiver.val(barcode);
                if (receiver.is('[data-barcode-submit]')) {
                    receiver.closest('form').submit();
                }
                var modal = receiver.closest('.modal');
                if (modal) modal.modal('show');
            }
            else {
                $(window).trigger('barcode', barcode);
            }
        }
    }
    else if (buf !== null) {

        // barcode character
        handled = true;
        buf.push(key.trim())

        // reset timeout
        if (timer) clearTimeout(timer);
        timer = setTimeout(timeout, 1000);
    }
    if (handled) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
    }
});
