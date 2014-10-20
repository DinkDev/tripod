﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindEmailVerificationByTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustFindEmailVerificationByTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindEmailVerificationByTicket(queries));
        }
    }

    internal class MustFindEmailVerificationByTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindEmailVerificationByTicket(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist_NoValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(ticket)) return false;

            var entity = _queries.Execute(new EmailVerificationBy(ticket)).Result;
            return entity != null;
        }
    }
}
