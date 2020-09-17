using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace bfyoc_rating_api
{
    public  class ProcessBatchActivity
    {
        [FunctionName("ProcessBatchActivity")]
        public static async Task ProcessBatch([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var EntityId = context.GetInput<EntityId>();
            log.LogInformation($"Processing batch: {EntityId.EntityKey}");

            await context.CallActivityAsync<string>("ProcessBatchLogic", EntityId.EntityKey);
        }

        [FunctionName("ProcessBatchLogic")]
        public static async Task ProcessBatchLogic([ActivityTrigger] string batchId, [CosmosDB(databaseName: "bfyoc", collectionName: "Orders", ConnectionStringSetting = "CosmosDBConnectionString", CreateIfNotExists = true)] IAsyncCollector<dynamic> documents, ILogger log)
        {
            string orderDetailsURL = "https://bfyocorders.blob.core.windows.net/orders/" + batchId + "-OrderHeaderDetails.csv";
            string prodInfoURL = "https://bfyocorders.blob.core.windows.net/orders/" + batchId + "-ProductInformation.csv";
            string orderLineURL = "https://bfyocorders.blob.core.windows.net/orders/" + batchId + "-OrderLineItems.csv";

            var requestObj = new
            {
                orderHeaderDetailsCSVUrl = orderDetailsURL,
                orderLineItemsCSVUrl = orderLineURL,
                productInformationCSVUrl = prodInfoURL
            };

            using var client = new HttpClient();
            var response = await client.PostAsync("https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent", new StringContent(JsonConvert.SerializeObject(requestObj), Encoding.UTF8, "application/json"));
            string responseJSON = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(responseJSON);
            foreach (var r in data)
            {
                await documents.AddAsync(r);
            }
        }
    }
}