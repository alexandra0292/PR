using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class ProductService
    {
        public async Task AddProductinCategory(ProductShortDto product, long id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {

                    string apiUrl = $"https://localhost:44370/api/Category/categories/{id}/products";

                    string jsonContent = JsonConvert.SerializeObject(product);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Produs nou adaugat cu succes!");
                    }
                    else
                    {
                        Console.WriteLine($"Eroare: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare: {ex.Message}");
                }
            }

        }
        public async Task GetProductsCategory(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = $"https://localhost:44370/api/Category/categories/{id}/products";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var responseObject = JsonConvert.DeserializeObject<List<ProductShortDto>>(content);

                        foreach (var category in responseObject)
                        {
                            Console.WriteLine($"Id: {category.Id}, Nume: {category.Title} , ItemsCount: {category.Price}, CategoryId: {category.CategoryId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Eroare: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare: {ex.Message}");
                }
            }
        }



    }
}
