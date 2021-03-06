﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustNotBeVerifiedEmailAddressExtensions
    {
        /// <summary>
        /// Validates that this EmailAddress is not already verified.
        /// </summary>
        /// <typeparam name="T">The command with the EmailAddress Value to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailAddress by Value.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustNotBeVerifiedEmailAddress<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeVerifiedEmailAddress(queries));
        }
    }

    internal class MustNotBeVerifiedEmailAddress : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeVerifiedEmailAddress(IProcessQueries queries)
            : base(() => Resources.Validation_EmailAddress_IsAlreadyVerified)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var email = (string)context.PropertyValue;
            var entity = _queries.Execute(new EmailAddressBy(email)).Result;
            if (entity == null || !entity.IsVerified) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
