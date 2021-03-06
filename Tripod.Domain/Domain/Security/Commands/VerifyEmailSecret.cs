﻿using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class VerifyEmailSecret : IDefineCommand
    {
        public string Secret
        {
            get { return _secret; }
            [UsedImplicitly] set { _secret = value != null ? value.Trim() : null; }
        }
        private string _secret;
        public string Ticket { get; [UsedImplicitly] set; }
        public EmailVerificationPurpose Purpose { get; [UsedImplicitly] set; }
        public string Token { get; internal set; }
    }

    [UsedImplicitly]
    public class ValidateVerifyEmailSecretCommand : AbstractValidator<VerifyEmailSecret>
    {
        public ValidateVerifyEmailSecretCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Ticket)
                .MustBeRedeemableVerifyEmailTicket(queries)
                .MustBePurposedVerifyEmailTicket(queries, x => x.Purpose)
                    .WithName(EmailVerification.Constraints.Label);

            RuleFor(x => x.Secret)
                .NotEmpty()
                .MustBeVerifiedEmailSecret(queries, x => x.Ticket)
                    .WithName(EmailVerification.Constraints.SecretLabel);
        }
    }

    [UsedImplicitly]
    public class HandleVerifyEmailSecretCommand : IHandleCommand<VerifyEmailSecret>
    {
        private readonly IReadEntities _entities;

        public HandleVerifyEmailSecretCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(VerifyEmailSecret command)
        {
            var entity = await _entities.Query<EmailVerification>().ByTicketAsync(command.Ticket);
            command.Token = entity.Token;
        }
    }
}
