using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace bfyoc_rating_api
{
    public static class BatchTrackerIncrementEndpoint
    {
        [FunctionName("BatchTrackerIncrementEndpoint")]
        public static async Task<IActionResult> HttpStart([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Tracker/{batchId}")] HttpRequest req,
            [DurableClient] IDurableClient context,
            string batchId)
        {
            var entityId = new EntityId("BatchTracker", batchId);
            context.SignalEntityAsync(entityId, "Add").Wait();

            var currentValue = await context.ReadEntityStateAsync<BatchTracker>(entityId);
            return new OkObjectResult(currentValue);
        }
    }
}
