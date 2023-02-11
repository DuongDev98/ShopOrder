//thư viện
$(document).ready(function () {
    $('html').on("keypress", ".number-input", function (e) {
        let numberInput = $(this);
        if (e.which == '13') {
            numberInput.focusout();
        }
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
        $(this).parent().find('.number-input-custom').val(ConvertToJs.Int($(this).val()));
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

    UiUtils.InputNumberCustom();

    $('html').on("change", ".custom-checkbox", function () {
        let checked = $(this).is(":checked");
        $(this).parent().find('input').val(checked ? 1 : 0);
    });
});

//ConvertToJs
var ConvertToJs = function () { };

ConvertToJs.Int = function (text) {
    if (text == null || text == undefined || (text + '').trim() == '') return 0;
    var tmp = (text + '').trim();
    while (tmp.indexOf(',') >= 0) {
        tmp = tmp.replace(',', '');
    }
    return parseInt(tmp);
}

ConvertToJs.Decimal = function (text) {
    if (text == null || text == undefined || (text + '').trim() == '') return 0;
    var tmp = (text + '').trim();
    while (tmp.indexOf(',') >= 0) {
        tmp = tmp.replace(',', '');
    }
    return parseFloat(tmp);
}

ConvertToJs.FormToJson = function (form) {
    var loginForm = form.serializeArray();
    var loginFormObject = {};
    $.each(loginForm,
        function (i, v) {
            loginFormObject[v.name] = v.value;
        });
    return loginFormObject;
}

//UiUtils
var UiUtils = function () { };

UiUtils.InputNumberCustom = function () {
    $('body').find('.number-input-custom').each(function () {
        $(this).parent().append('<input type="text" class="form-control number-input" value="' + UiUtils.FormatNumber($(this).val()) + '">');
        $(this).css("display", "none");
    });
}

UiUtils.FormatNumber = function (text) {
    var tmp = ConvertToJs.Int(text) + '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(tmp)) {
        tmp = tmp.replace(rgx, '$1' + ',' + '$2');
    }
    return tmp;
}

var formModalHtml =
    '<div class="modal myModal" tabindex = "-1" role = "dialog">' +
    '   <div class="modal-dialog" role="document">' +
    '       <div class="modal-content">' +
    '           <div class="modal-header">' +
    '               <h5 class="modal-title"></h5>' +
    '               <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
    '                   <span aria-hidden="true">&times;</span>' +
    '               </button>' +
    '           </div>' +
    '           <div class="modal-body"></div>' +
    '           <div class="modal-footer">' +
    '               <button type="button" class="btn btn-primary">Ok</button>' +
    '               <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>' +
    '           </div>' +
    '       </div>' +
    '   </div>' +
    '</div>';

UiUtils.ShowForm = function (title, formHtml, success) {
    let form = $(formModalHtml);
    form.find('.modal-title').text(title);
    form.find('.modal-body').append(formHtml);
    form.find('.btn-primary').click(function () {
        if (success != undefined && success != null) success();
        form.modal('hide');
        form.remove();
    });
    $('body').append(form);
    UiUtils.InputNumberCustom();
    form.modal('show');
    return form;
}

UiUtils.ShowInfo = function (title, msg, success) {
    swal(title, msg, "info").then(function () {
        if (success != undefined && success != null) success();
    });
}

UiUtils.ShowSuccess = function (title, msg, success) {
    swal(title, msg, "success").then(function () {
        if (success != undefined && success != null) success();
    });
}

UiUtils.ShowError = function (title, msg, success) {
    swal(title, msg, "error").then(function () {
        if (success != undefined && success != null) success();
    });
}