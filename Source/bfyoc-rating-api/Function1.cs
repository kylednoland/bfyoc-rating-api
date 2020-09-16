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
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string userId = data?.userId;
            string productId = data?.productId;
            string locationName = data?.locationName;
            string rating = data?.rating;
            string userNotes = data?.userNotes;


            if(
            string.IsNullOrEmpty(userId) || 
            string.IsNullOrEmpty(productId) || 
            string.IsNullOrEmpty(locationName) ||  
            string.IsNullOrEmpty(rating) ||
            string.IsNullOrEmpty(userNotes)
            )
            {
                var message = "Missing paramaters. Pass UserId, ProductID, LocationName, Rating and UserNotes in the request body for a personalized response.";
                return new BadRequestObjectResult(message);
            }

            //Make sure Product ID is valid
            var productClient = new ProductClient();
            var productIdGuid = new Guid(productId);
            var product = await productClient.RetrieveProductAsync(productIdGuid);
            if(product == null)
            {
                var responseMessage = "Invalid Product ID. Please try again with a valid Product ID";
                return new BadRequestObjectResult(responseMessage);
            }

            //Make sure User ID is valid
            var userClient = new UserClient();
            var userIdGuid = new Guid(userId);
            var user = await userClient.RetrieveUserAsync(userIdGuid);
            if(user == null)
            {
                var responseMessage = "Invalid User ID. Please try again with a valid User ID";
                return new BadRequestObjectResult(responseMessage);
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
                var responseMessage = "Invalid Rating. Please try again with a rating that is a number between 1 and 5";
                return new BadRequestObjectResult(responseMessage);
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


        [FunctionName("GetRating")]
        public static async Task<IActionResult> GetRatingById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string ratingId = req.Query["ratingId"];

            if (Guid.TryParse(ratingId, out Guid id))
            {
                var ratingClient = new RatingClient();
                try
                {
                    var rating = await ratingClient.RetrieveRatingAsync(id);

                    if (rating == null)
                    {
                        return new BadRequestObjectResult($"There is no rating with Id {ratingId}.");
                    }
                    return new OkObjectResult(rating);
                }
                catch
                {
                    return new BadRequestObjectResult($"There is no rating with Id {ratingId}.");
                }
            }
            else
            {
                return new BadRequestObjectResult($"There is no rating with Id {ratingId}.");

            }
        }

        [FunctionName("GetRatings")]
        public static async Task<IActionResult> GetUserRatings(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id}/ratings")] HttpRequest req,
            ILogger log,
            string id)
        {
            if(Guid.TryParse(id, out Guid userId))
            {
                // Check user Id.
                var userClient = new UserClient();
                var user = await userClient.RetrieveUserAsync(userId);

                if(user == null)
                {
                    return new BadRequestObjectResult($"There is no user with Id {userId}.");
                }

                // Retrieve ratings
                var ratingsClient = new RatingClient();
                var ratings = await ratingsClient.RetrieveRatingsAsync(userId);
                return new OkObjectResult(ratings);
            }
            else
            {
                return new BadRequestObjectResult("The user Id is not a GUID.");
            }
        }
    }
}
