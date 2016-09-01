// Wrap code in an self-executing anonymous function and 
// pass jQuery into it so we can use the "$" shortcut without 
// causing potential conflicts with already existing functions.
(function ($) {
    /*
    Validation Singleton
    */
    var QValidation = function () {
        //define some default validation rules for questions
        var rules = {
            required: {
                //check that at least one answer option has been provided
                check: function (qelement) {
                    var i = 0;
                    var values = qelement.find("input,textarea").each(function () {
                        var type = $(this).attr("type");
                        if (type == "checkbox" || type == "radio") {
                            if ($(this).attr("checked"))
                                i++;
                        }
                        else if ($(this).val()) {
                            var t = $(this).val();
                            if($.trim(t).length >0)
                            //if ($(this).val().trim().length > 0)
                                i++;
                        }
                    });
                    if (i > 0)
                        return true;
                    else
                        return false;
                },
                msg: "This question requires an answer."
            },
            email: {
                check: function (value) {

                    if (value)
                        return testPattern(value, "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])");
                    return true;
                },
                msg: "Enter a valid e-mail address."
            }
        }
        var testPattern = function (value, pattern) {

            var regExp = new RegExp(pattern, "");
            return regExp.test(value);
        }
        return {

            addRule: function (name, rule) {

                rules[name] = rule;
            },
            getRule: function (name) {

                return rules[name];
            }
        }
    }

    /* 
    Form factory 
    */
    var Form = function (form) {

        var questions = [];

        form.find(".quesionSetQuestion:.required").each(function () {
            var question = $(this);
            questions.push(new QField(question));
        });
        this.questions = questions;
    }
    Form.prototype = {
        //validate all the questions
        validate: function () {

            for (field in this.questions) {

                this.questions[field].validate();
            }
        },
        //check if all question answers are valid
        isValid: function () {

            for (field in this.questions) {

                if (!this.questions[field].valid) {
                    /*go to the first question thats invalid*/
                    var eml = this.questions[field].element.find("input:visible:first,textarea:visible:first").first();
                    var emlp = this.questions[field].element;
                    $('html, body').animate({
                        scrollTop: emlp.offset().top
                    }, 800);

                    eml.focus();
                    return false;
                }
            }
            return true;
        }
    }

    /* 
    Field factory 
    */
    var QField = function (element) {

        this.element = element;
        this.valid = false;
        //un rem the line below to inable change event validation from load
        //this.attach("change");
    }
    QField.prototype = {
        //attach validation to the provided event
        attach: function (event) {

            var obj = this;
            if (event == "change") {
                obj.element.find("input,textarea").change("change", function () {
                    return obj.validate();
                });
            }
            if (event == "keyup") {
                obj.element.find("input,textarea").bind("keyup", function (e) {
                    return obj.validate();
                });
            }
        },
        //unattach events 
        unattach: function (event) {

            var obj = this;
            if (event == "change") {
                obj.element.find("input,textarea").unbind("change");
            }
            if (event == "keyup") {
                obj.element.find("input,textarea").unbind("keyup");
            }
        },
        // Method that runs validation on a question field set
        validate: function () {
            // Create an internal reference to the Field set object. 
            var obj = this,
                element = obj.element,
                types = element.attr("class").split(" "),
                errors = [];
            // If there is an errorlist already present
            // clear it before performing additional validation
            var err = element.find(".errorList");
            err.empty();
            // Iterate over validation types
            for (var type in types) {

                var rule = $.QValidation.getRule(types[type]);
                if (rule) {
                    if (!rule.check(element)) {

                        element.addClass("questionerror");
                        errors.push(rule.msg);
                    }
                }
            }
            if (errors.length) {
                obj.unattach("keyup");
                obj.unattach("change");
                obj.attach("keyup");
                obj.attach("change");
                element.find(".errorList").show();
                for (error in errors) {
                    element.find(".errorList").append("<p>" + errors[error] + "</p>");
                }
                obj.valid = false;
            }
            else {
                element.find(".errorList").hide();
                element.removeClass("questionerror");
                obj.valid = true;
            }
        }
    }

    /*
    Validation extends jQuery prototype
    */
    $.extend($.fn, {

        questionvalidation: function () {

            var validator = new Form($(this));
            $.data($(this)[0], 'qvalidator', validator);

            $(this).bind("submit", function (e) {
                validator.validate();
                if (!validator.isValid()) {
                    e.preventDefault();
                }
            });
        },
        questionvalidate: function () {

            var validator = $.data($(this)[0], 'qvalidator');
            validator.validate();
            return validator.isValid();

        }
    });
    $.QValidation = new QValidation();
})(jQuery);