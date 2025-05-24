using CommunicationFunction.Features.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Interfaces.CommandHandler
{
    public interface ISendEmailCommandHandler : ICommandHandler<SendEmailCommand> { }
}
