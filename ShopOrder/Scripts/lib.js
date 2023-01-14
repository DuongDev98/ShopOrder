//thư viện
$(document).ready(function () {
    $('html').on("keypress", ".number-input", function (e) {
        //chỉ nhập được số
        if (!$.isNumeric(e.key)) e.preventDefault();
    });

    $('html').on("keyup", ".number-input", function (e) {
        //hiển thị giá trị luôn
        if ($(this).val() != "") {
            var temp = UiUtils.formatNumber($(this).val());
            if (temp != $(this).val()) $(this).val(temp);
        }

        //tìm điền vào giá trị
        $(this).parent().find('.input-number-custom').val(ConvertTo.Int($(this).val()));
    });

    $('html').on("click", ".number-view", function (e) {
        if ($(this).find(".number-input").length == 0) {
            var text = $(this).text();
            $(this).text("");
            $(this).append('<input type="text" class="form-control number-input" value="' + UiUtils.formatNumber(text) + '">');
            $(this).find(".number-input").focus().select();
        }
    });

    $('html').on("focusout", ".number-view > .number-input", function () {
        $(this).parent().text($(this).val());
        $(this).remove();
    });

    $('.input-number-custom').each(function () {
        $(this).parent().append('<input type="text" class="form-control number-input" value="' + UiUtils.formatNumber($(this).val()) + '">');
        $(this).css("display", "none");
    });
});

////ConvertToJs
//export function ConvertToInt(text) {
//    if (text == null || text == undefined || text.trim() == '') return 0;
//    var tmp = (text + '').trim();
//    while (tmp.indexOf(',') >= 0) {
//        tmp = tmp.replace(',', '');
//    }
//    return parseInt(tmp);
//}
//export function FormToJson(form) {
//    var loginForm = form.serializeArray();
//    var loginFormObject = {};
//    $.each(loginForm,
//        function (i, v) {
//            loginFormObject[v.name] = v.value;
//        });
//    return loginFormObject;
//}

////UiUtils
//export function FormatNumber(text) {
//    var tmp = ConvertTo.Int(text) + '';
//    var rgx = /(\d+)(\d{3})/;
//    while (rgx.test(tmp)) {
//        tmp = tmp.replace(rgx, '$1' + ',' + '$2');
//    }
//    return tmp;
//}