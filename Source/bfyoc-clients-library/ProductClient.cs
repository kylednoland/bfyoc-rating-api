using bfyoc_objects_library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace bfyoc_clients_library
{
    public class ProductClient
    {
        private string productsUrl = "https://serverlessohproduct.trafficmanager.net/api/GetProducts";
        private string productUrl = "https://serverlessohproduct.trafficmanager.net/api/GetProduct";
        private static HttpClient httpClient = new HttpClient();
        public async Task<List<Product>> GetProductsAsync()
        {
            var responseText = await httpClient.GetStringAsync(this.productsUrl);
            var productList = JsonConvert.DeserializeObject<List<Product>>(responseText);
            return productList;
        }

        public async Task<Product> GetProductAsync(Guid productId)
        {
            var finalProductUrl = string.Format("{0}?productId={1}", productUrl, productId.ToString());
            var responseText = await httpClient.GetStringAsync(finalProductUrl);
            var product = JsonConvert.DeserializeObject<Product>(responseText);
            return product;
        }
    }
}
