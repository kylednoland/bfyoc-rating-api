using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using bfyoc_clients_library;

namespace bfyoc_rating_api
{
    public static class Function1
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string userId = data?.userId;
            string productId = data?.productId;
            string locationName = data?.locationName;
            string rating = data?.rating;
            string userNotes = data?.userNotes;


            string responseMessage = 
            string.IsNullOrEmpty(userId) || 
            string.IsNullOrEmpty(productId) || 
            string.IsNullOrEmpty(locationName) ||  
            string.IsNullOrEmpty(rating) ||
            string.IsNullOrEmpty(userNotes)
                ? "This HTTP triggered function executed successfully. Pass UserId, ProductID, LocationName, Rating and UserNotes in the request body for a personalized response."
                : $"{userId} {productId} {locationName} {rating} {userNotes}. This HTTP triggered function executed successfully.";

            var productClient = new ProductClient();
            var userClient = new UserClient();
            var products = await productClient.GetProductsAsync();
            var product = await productClient.GetProductAsync(new Guid(productId));
            var user = await userClient.GetUserAsync(new Guid(userId));
            return new OkObjectResult(responseMessage);
        }
    }
}
