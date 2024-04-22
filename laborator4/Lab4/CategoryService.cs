using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public  class CategoryService
    { 
        public async Task GetCategories()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = "https://localhost:44370/api/Category/categories";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var responseObject = JsonConvert.DeserializeObject<List<CategoryShortDto>>(content);

                        foreach (var category in responseObject)
                        {
                            Console.WriteLine($"Id: {category.Id}, Nume: {category.Name} , ItemsCount: {category.ItemsCount}");
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
        public async Task GetCategoryDetail(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = $"https://localhost:44370/api/Category/categories/{id}";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var responseObject = JsonConvert.DeserializeObject<List<CategoryShortDto>>(content);

                        foreach (var category in responseObject)
                        {
                            Console.WriteLine($"Id: {category.Id}, Nume: {category.Name} , ItemsCount: {category.ItemsCount}");
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
        public async Task AddCategory(CreateCategoryDto categorie)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = "https://localhost:44370/api/Category/categories";

                    string jsonContent = JsonConvert.SerializeObject(categorie);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Categorie adaugata cu succes!");
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
        public async Task DeleteCategory(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = $"https://localhost:44370/api/Category/categories/{id}";

                    HttpResponseMessage response = await client.DeleteAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Categorie stearsa cu succes!");
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
        public async Task UpdateTitle(string newTitle, long categoryId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = $"https://localhost:44370/api/Category/{categoryId}";
                    var updatedCategory = new
                    {
                        Title = newTitle
                    };
                    string jsonContent = JsonConvert.SerializeObject(updatedCategory);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                   
                    HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Titlu categorie actualizat cu succes!");
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

