﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Tripod.Services.Net
{
    public class InterceptMailDeliveryDecorator : IDeliverMailMessage
    {
        private readonly IDeliverMailMessage _decorated;
        private readonly AppConfiguration _appConfiguration;

        public InterceptMailDeliveryDecorator(
              IDeliverMailMessage decorated
            , AppConfiguration appConfiguration
        )
        {
            _decorated = decorated;
            _appConfiguration = appConfiguration;
        }

        public void Deliver(MailMessage message, SendCompletedEventHandler sendCompleted = null, object userState = null)
        {
            const string messageFormat =
@"
***********************************************
* This message was intercepted before it was
* sent over the network. The intended
* recipients were:
* {0}
***********************************************
";
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("TO:");
            AppendIntendedRecipients(message.To, messageBuilder);

            if (message.CC.Any())
            {
                messageBuilder.AppendLine("* CC:");
                AppendIntendedRecipients(message.CC, messageBuilder);
            }

            if (message.Bcc.Any())
            {
                messageBuilder.AppendLine("* BCC:");
                AppendIntendedRecipients(message.Bcc, messageBuilder);
            }

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            foreach (var interceptor in _appConfiguration.MailInterceptors)
                message.To.Add(interceptor);

            var formattedMessage = string.Format(messageFormat, messageBuilder.ToString().Trim());
            message.Body = string.Format("{0}{1}", formattedMessage, message.Body);

            _decorated.Deliver(message, sendCompleted, userState);
        }

        private static void AppendIntendedRecipients(IEnumerable<MailAddress> recipients, StringBuilder messageBuilder)
        {
            foreach (var recipient in recipients)
            {
                if (!string.IsNullOrWhiteSpace(recipient.DisplayName) && recipient.DisplayName != recipient.Address)
                    messageBuilder.AppendLine(string.Format("* {0} <{1}>", recipient.DisplayName, recipient.Address));
                else
                    messageBuilder.AppendLine(string.Format("* {0}", recipient.Address));
            }
        }
    }
}
