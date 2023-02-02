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
UiUtils.FormatNumber = function (text) {
    var tmp = ConvertToJs.Int(text) + '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(tmp)) {
        tmp = tmp.replace(rgx, '$1' + ',' + '$2');
    }
    return tmp;
}