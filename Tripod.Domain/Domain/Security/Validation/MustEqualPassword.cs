﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustEqualPassword<T> : PropertyValidator
    {
        private readonly Func<T, string> _password;
        private readonly string _matchLabel;

        internal MustEqualPassword(Func<T, string> password, string matchLabel)
            : base(() => Resources.Validation_PasswordDoesNotEqualConfirmation)
        {
            if (password == null) throw new ArgumentNullException("password");
            _password = password;
            _matchLabel = matchLabel;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var password2 = (string)context.PropertyValue;
            var password = _password((T)context.Instance);

            if (password.Equals(password2)) return true;

            context.MessageFormatter.AppendArgument("PasswordLabel", _matchLabel ?? LocalMembership.Constraints.PasswordLabel.ToLower());
            return false;
        }
    }

    public static class MustEqualPasswordExtensions
    {
        public static IRuleBuilderOptions<T, string> MustEqualPassword<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, string> password, string matchLabel = null)
        {
            return ruleBuilder.SetValidator(new MustEqualPassword<T>(password, matchLabel));
        }
    }
}