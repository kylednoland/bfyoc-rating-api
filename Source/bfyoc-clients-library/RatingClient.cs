using bfyoc_objects_library;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bfyoc_clients_library
{
    public class RatingClient
    {
        private static string cosmosEndpointUri = "https://serverlessopenhack.documents.azure.com:443/";

        private static string primaryKey = "G0INAL2odWOocg60rgmogpJ4hxug2oY173JY6b3fGjtHAHQpQoWr9xlZnpqoMqrmdzZXtRscS23BUopcHk3L5w==";

        private static string databaseID = "bfyoc";

        private static string ratingContainerID = "Rating";

        private CosmosClient cosmosClient;
        public async Task CreateRatingAsync(Rating rating)
        {
            this.cosmosClient = new CosmosClient(cosmosEndpointUri, primaryKey);
            var container = cosmosClient.GetContainer(databaseID, ratingContainerID);
            await container.CreateItemAsync<Rating>(rating);
        }

        public async Task<Rating> RetrieveRating(Guid ratingId)
        {
            return null;
        }

        public async Task<List<Rating>> RetrieveRatings(Guid routeId)
        {
            return null;
        }
    }
}
