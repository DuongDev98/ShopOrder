//thư viện
$(document).ready(function () {
    $('html').on("keypress", ".number-input", function (e) {
        //chỉ nhập được số
        if (!$.isNumeric(e.key)) e.preventDefault();
    });

    $('html').on("keyup", ".number-input", function (e) {
        //hiển thị giá trị luôn
        if ($(this).val() != "") {
            var temp = UiUtils.FormatNumber($(this).val());
            if (temp != $(this).val()) $(this).val(temp);
        }

        //tìm điền vào giá trị
        $(this).parent().find('.input-number-custom').val(ConvertToJs.Int($(this).val()));
    });

    $('html').on("click", ".number-view", function (e) {
        if ($(this).find(".number-input").length == 0) {
            var text = $(this).text();
            $(this).text("");
            $(this).append('<input type="text" class="form-control number-input" value="' + UiUtils.FormatNumber(text) + '">');
            $(this).find(".number-input").focus().select();
        }
    });

    $('html').on("focusout", ".number-view > .number-input", function () {
        $(this).parent().text($(this).val());
        $(this).remove();
    });

    $('.input-number-custom').each(function () {
        $(this).parent().append('<input type="text" class="form-control number-input" value="' + UiUtils.FormatNumber($(this).val()) + '">');
        $(this).css("display", "none");
    });

    $('html').on("change", ".custom-checkbox", function () {
        let checked = $(this).is(":checked");
        $(this).parent().find('input').val(checked ? 1 : 0);
    });
});