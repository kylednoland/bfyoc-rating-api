

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace bfyoc_rating_api
{
    public static class BlobEventProcessor
    {
        [FunctionName("BlobEventProcessor")]
        public static async Task Run([EventGridTrigger] EventGridEvent blobCreatedEvent, [DurableClient] IDurableClient context, ILogger log)
        {
            log.LogInformation(blobCreatedEvent.Data.ToString());
            StorageBlobCreatedEventData createdEvent = ((JObject)blobCreatedEvent.Data).ToObject<StorageBlobCreatedEventData>();

            var batchId = ParseBatchId(createdEvent.Url);

            var entityId = new EntityId("BatchTracker", batchId);
            context.SignalEntityAsync(entityId, "Add").Wait();
        }

        private static string ParseBatchId(string url)
        {
            var uri = new Uri(url);
            var fileName = uri.Segments[uri.Segments.Length - 1];
            return fileName.Split('-')[0];
        }
    }
}
