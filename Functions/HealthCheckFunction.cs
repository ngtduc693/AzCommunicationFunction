using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFunction.Functions
{
    /// <summary>
    /// Azure Function to health check.
    /// </summary>
    public class HealthCheckFunction
    {
        /// <summary>
        /// API for health check.
        /// </summary>
        [Function("HealthCheck")]
        public Task<string> HealthCheck(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")]
            HttpRequestData req)
        {
            return Task.FromResult("Healthy");
        }
    }
}
