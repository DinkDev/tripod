'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUp) {
            (function (EmailForm) {
                var Controller = (function () {
                    function Controller(scope) {
                        this.scope = scope;
                        this.emailAddress = '';
                        this.isExpectingEmail = false;
                        scope.vm = this;
                    }
                    Controller.prototype.emailAddressInputGroupValidationAddOnCssClass = function () {
                        return this.scope.signUpCtrb.emailAddress.hasFeedback() ? null : 'hide';
                    };

                    Controller.prototype.isEmailAddressRequiredError = function () {
                        return this.scope.signUpForm.emailAddress.$error.required && this.scope.signUpCtrb.emailAddress.hasError;
                    };

                    Controller.prototype.isEmailAddressPatternError = function () {
                        return this.scope.signUpForm.emailAddress.$error.email && this.scope.signUpCtrb.emailAddress.hasError;
                    };

                    Controller.prototype.isEmailAddressServerError = function () {
                        return this.scope.signUpForm.emailAddress.$error.server && this.scope.signUpCtrb.emailAddress.hasError;
                    };

                    Controller.prototype.isExpectingEmailError = function () {
                        return this.scope.signUpCtrb.isExpectingEmail.hasError;
                    };

                    Controller.prototype.isExpectingEmailRequiredError = function () {
                        return this.scope.signUpForm.isExpectingEmail.$error.required && this.isExpectingEmailError();
                    };

                    Controller.prototype.isExpectingEmailServerError = function () {
                        return this.scope.signUpForm.isExpectingEmail.$error.server && this.isExpectingEmailError();
                    };

                    Controller.prototype.isSubmitWaiting = function () {
                        return this.scope.signUpCtrb.isSubmitWaiting;
                    };

                    Controller.prototype.isSubmitError = function () {
                        return !this.isSubmitWaiting() && this.scope.signUpCtrb.hasError;
                    };

                    Controller.prototype.isSubmitReady = function () {
                        return !this.isSubmitWaiting() && !this.isSubmitError();
                    };

                    Controller.prototype.isSubmitDisabled = function () {
                        return this.isSubmitWaiting() || this.isSubmitError();
                    };

                    Controller.prototype.submitCssClass = function () {
                        return this.isSubmitError() ? 'btn-danger' : null;
                    };
                    Controller.$inject = ['$scope'];
                    return Controller;
                })();
                EmailForm.Controller = Controller;

                EmailForm.moduleName = 'sign-up-form';

                EmailForm.ngModule = angular.module(EmailForm.moduleName, [App.Modules.Tripod.moduleName]);
            })(SignUp.EmailForm || (SignUp.EmailForm = {}));
            var EmailForm = SignUp.EmailForm;
        })(Security.SignUp || (Security.SignUp = {}));
        var SignUp = Security.SignUp;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));