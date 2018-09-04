//Based on http://blog.rebuildall.net/2011/03/02/jquery_validate_and_the_comma_decimal_separator
$.validator.methods.range = function (value, element, params) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= params[0] && globalizedValue <= params[1]);
}

$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
}


//Based on http://www.macaalay.com/2014/02/25/unobtrusive-client-and-server-side-not-equal-to-validation-in-mvc-using-custom-data-annotations/
$.validator.addMethod('unlike', function (value, element, params) {
    var otherProperty = $('#' + params.otherproperty);
    return this.optional(element) || (otherProperty.val() != value);
});

$.validator.unobtrusive.adapters.add('unlike', ['otherproperty', 'otherpropertyname'], function (options) {
    var params = {
        otherproperty: options.params.otherproperty,
        otherpropertyname: options.params.otherpropertyname
    };
    options.rules['unlike'] = params;
    options.messages['unlike'] = options.message;
});