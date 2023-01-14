////ConvertToJs
//class ConvertToJs {
//    static Int(text) {
//        if (text == null || text == undefined || text.trim() == '') return 0;
//        var tmp = (text + '').trim();
//        while (tmp.indexOf(',') >= 0) {
//            tmp = tmp.replace(',', '');
//        }
//        return parseInt(tmp);
//    }

//    static ToJson(form) {
//        var loginForm = form.serializeArray();
//        var loginFormObject = {};
//        $.each(loginForm,
//            function (i, v) {
//                loginFormObject[v.name] = v.value;
//            });
//        return loginFormObject;
//    }
//}

////UiUtils
//class UiUtils {
//    static formatNumber(text) {
//        var tmp = ConvertTo.Int(text) + '';
//        var rgx = /(\d+)(\d{3})/;
//        while (rgx.test(tmp)) {
//            tmp = tmp.replace(rgx, '$1' + ',' + '$2');
//        }
//        return tmp;
//    }
//}

//ConvertToJs
export function ConvertToInt(text) {
    if (text == null || text == undefined || text.trim() == '') return 0;
    var tmp = (text + '').trim();
    while (tmp.indexOf(',') >= 0) {
        tmp = tmp.replace(',', '');
    }
    return parseInt(tmp);
}
export function FormToJson(form) {
    var loginForm = form.serializeArray();
    var loginFormObject = {};
    $.each(loginForm,
        function (i, v) {
            loginFormObject[v.name] = v.value;
        });
    return loginFormObject;
}

//UiUtils
export function FormatNumber(text) {
    var tmp = ConvertTo.Int(text) + '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(tmp)) {
        tmp = tmp.replace(rgx, '$1' + ',' + '$2');
    }
    return tmp;
}