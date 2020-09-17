using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace bfyoc_rating_api
{
    public static class ProcessBatchActivity
    {
        [FunctionName("ProcessBatchActivity")]
        public static async Task ProcessBatch([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var EntityId = context.GetInput<EntityId>();
            EntityId.ToString();
            // DO SOMETHING
        }

    }
}