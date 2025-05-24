using CommunicationFunction.Infrastructure.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Core.Interfaces.QueryHandler
{
    public interface IProcessTemplateQueryHandler : IQueryHandler<ProcessTemplateQuery, string> { }
}
