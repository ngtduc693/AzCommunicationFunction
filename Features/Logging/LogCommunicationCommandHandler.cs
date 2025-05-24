using CommunicationFunction.Core.Interfaces.CommandHandler;
using CommunicationFunction.Core.Interfaces.Repository;
using CommunicationFunction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Features.Logging
{
    /// <summary>
    /// Handles logging communication to the storage system.
    /// </summary>
    public class LogCommunicationCommandHandler : ILogCommunicationCommandHandler
    {
        private readonly ILogRepository _logRepository;

        public LogCommunicationCommandHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        /// <summary>
        /// Logs communication based on the received command.
        /// </summary>
        public async Task<ProcessingResult> HandleAsync(LogCommunicationCommand command)
        {
            var result = new ProcessingResult { MessageId = command.CommandId };

            try
            {
                await _logRepository.SaveLogAsync(command.Log);
                result.Success = true;
                result.Metadata["LogId"] = command.Log.id;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
