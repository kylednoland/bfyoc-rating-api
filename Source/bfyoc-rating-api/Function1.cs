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
using bfyoc_objects_library;

namespace bfyoc_rating_api
{
    public static class Function1
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
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

            //Make sure Product ID is valid
            var productClient = new ProductClient();
            var productIdGuid = new Guid(productId);
            var product = await productClient.RetrieveProductAsync(productIdGuid);
            if(product == null)
            {
                responseMessage = "Invalid Product ID. Please try again with a valid Product ID";
                return new OkObjectResult(responseMessage);
            }

            //Make sure User ID is valid
            var userClient = new UserClient();
            var userIdGuid = new Guid(userId);
            var user = await userClient.RetrieveUserAsync(userIdGuid);
            if(user == null)
            {
                responseMessage = "Invalid User ID. Please try again with a valid User ID";
                return new OkObjectResult(responseMessage);
            }

            //Make sure rating is a number between 1 and 5
            var intRating = -1;
            try
            {
                intRating = int.Parse(rating);
                if(intRating<1 || intRating >5)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                responseMessage = "Invalid Rating. Please try again with a rating that is a number between 1 and 5";
                return new OkObjectResult(responseMessage);
            }

            //All Parameters are valid, build the rating object
            var ratingObject = new Rating();
            ratingObject.id = Guid.NewGuid();
            ratingObject.userId = userIdGuid;
            ratingObject.productId = productIdGuid;
            ratingObject.timestamp = DateTime.UtcNow;
            ratingObject.locationName = locationName;
            ratingObject.rating = intRating;
            ratingObject.usernotes = userNotes;

            //Create The Rating Object in the CosmosDB
            var ratingClient = new RatingClient();
            await ratingClient.CreateRatingAsync(ratingObject);

            return new OkObjectResult(ratingObject);
        }
    }
}
