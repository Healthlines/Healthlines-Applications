/// <reference path="../jquery-1.5.1-vsdoc.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />

(function ($) {
    /*
    "Required If" Validation
    *** mvcvtkrequiredif ***
    */
    $.validator.addMethod('mvcvtkrequiredif',
        function (value, element, parameters) {
            var id = '#' + parameters['dependentproperty'];

            // get the target value (as a string, 
            // as that's what actual value will be)
            var targetvalue = parameters['targetvalue'];
            targetvalue =
              (targetvalue == null ? '' : targetvalue).toString();

            // get the actual value of the target control
            // note - this probably needs to cater for more 
            // control types, e.g. radios
            var control = $(id);
            //var control = $(element).closest("form").find(id);
            var controltype = control.attr('type');
            var actualvalue =
                controltype === 'checkbox' ?
                control.attr('checked').toString() :
                control.val();

            // if the condition is true, reuse the existing 
            // required field validator functionality
            if (targetvalue === actualvalue)
                return $.validator.methods.required.call(
                  this, value, element, parameters);

            return true;
        }
    );

    $.validator.unobtrusive.adapters.add(
        'mvcvtkrequiredif',
        ['dependentproperty', 'targetvalue'],
        function (options) {
            options.rules['mvcvtkrequiredif'] = {
                dependentproperty: options.params['dependentproperty'],
                targetvalue: options.params['targetvalue']
            };
            options.messages['mvcvtkrequiredif'] = options.message;
        }
    );


    /*
    "Required Empty If" Validation
    *** mvcvtkrequiredemptyif ***
    */
    $.validator.addMethod('mvcvtkrequiredemptyif',
        function (value, element, parameters) {
            var id = '#' + parameters['dependentproperty'];

            // get the target value (as a string, 
            // as that's what actual value will be)
            var targetvalue = parameters['targetvalue'];
            targetvalue =
              (targetvalue == null ? '' : targetvalue).toString();

            // get the actual value of the target control
            // note - this probably needs to cater for more 
            // control types, e.g. radios
            var control = $(id);
            var controltype = control.attr('type');
            var actualvalue =
                controltype === 'checkbox' ?
                control.attr('checked').toString() :
                control.val();

            // if the condition is true, reuse the existing 
            // required field validator functionality
            // AND apply an inverse (i.e. ! not operator)
            // to ensure the logic is reversed
            if (targetvalue === actualvalue)
                return !$.validator.methods.required.call(
                  this, value, element, parameters);

            return true;
        }
    );

    $.validator.unobtrusive.adapters.add(
        'mvcvtkrequiredemptyif',
        ['dependentproperty', 'targetvalue'],
        function (options) {
            options.rules['mvcvtkrequiredemptyif'] = {
                dependentproperty: options.params['dependentproperty'],
                targetvalue: options.params['targetvalue']
            };
            options.messages['mvcvtkrequiredemptyif'] = options.message;
        }
    );


    /*
    "Range Defined By Fields" Validation
    *** mvcvtkrangedefinedbyfields ***
    */
    $.validator.addMethod('mvcvtkrangedefinedbyfields',
        function (value, element, parameters) {
            var minid = '#' + parameters['dependentpropertyminimum'];
            var maxid = '#' + parameters['dependentpropertymaximum'];

            // get the controls we're comparing against
            var minvalue = new Number($(minid).val());
            var maxvalue = new Number($(maxid).val());

            return this.optional(element) || ( value >= minvalue && value <= maxvalue );
        }
    );

    $.validator.unobtrusive.adapters.add(
        'mvcvtkrangedefinedbyfields',
        ['dependentpropertyminimum', 'dependentpropertymaximum'],
        function (options) {
            options.rules['mvcvtkrangedefinedbyfields'] = {
                dependentpropertyminimum: options.params['dependentpropertyminimum'],
                dependentpropertymaximum: options.params['dependentpropertymaximum']
            };
            options.messages['mvcvtkrangedefinedbyfields'] = options.message;
        }
    );
})(jQuery);
