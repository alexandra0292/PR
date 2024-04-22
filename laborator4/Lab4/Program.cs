using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lab4;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static void Meniu()
    {
        Console.WriteLine("1.Aplicatia poate enumera lista de categorii");
        Console.WriteLine("2.Aplicatia poate arăta detalii despre o categorie");
        Console.WriteLine("3.Aplicatia poate poate crea o categorie nouă");
        Console.WriteLine("4.Aplicatia poate sterge o categorie");
        Console.WriteLine("5.Aplicatia poate modifica titul unei categorii ");
        Console.WriteLine("6.Aplicatia poate crea produse noi intr-o categorie");
        Console.WriteLine("7.Aplicatia poate vedea lista produselor dintr-o categorie");
    }
    static long ConvertLong(string value)
    {
        long categoryId=0;
        // Încercarea conversiei
        if (long.TryParse(value, out categoryId))
        {
            Console.WriteLine($"Ati introdus: {categoryId}");
            return categoryId;
        }
        else
        {
            Console.WriteLine("Valoarea introdusa nu este valida pentru un numar long.");
        }
        return 0;

    }
    static decimal ConvertDecimal(string value)
    {
        decimal price=0;
        if (decimal.TryParse(value, out price))
        {
            Console.WriteLine($"Ati introdus: {price}");
            
        }
        else
        {
            Console.WriteLine("Valoarea introdusa nu este valida pentru un numar decimal.");
        }
        return price;
    }
    static async Task Main()
    {


        CategoryService categoryService = new CategoryService();
        ProductService productService = new ProductService();
        while (true)
        {
            Console.WriteLine("\n");
            Meniu();
            Console.WriteLine("\n");
            Console.WriteLine("\nIntrodu o optiune dorita:");
            string optiune = Console.ReadLine();

            switch (optiune)
            {
                case "1":
                    await categoryService.GetCategories();
                    break;
                case "2":
                    Console.WriteLine("Introdu Id pentru categoria dorita");
                    string value1 = Console.ReadLine();
                    await categoryService.GetCategoryDetail(ConvertLong(value1));
                    break;
                case "3":
                    CreateCategoryDto categoryDto = new CreateCategoryDto();
                    Console.WriteLine("Intro titlu la noua categorie");
                    categoryDto.Title = Console.ReadLine();
                    await categoryService.AddCategory(categoryDto);
                    break;
                case "4":
                    Console.WriteLine("Introdu Id pentru categoria pentru a o sterge");
                    string value = Console.ReadLine();
                    await categoryService.DeleteCategory(ConvertLong(value));
                    break;
                case "5":
                    Console.WriteLine("Intro Id pentru catgeoria pentru ai schimba titlul");
                    string value3 = Console.ReadLine();
                    Console.WriteLine("Introdu noul titlu pentru categorie");
                    string title = Console.ReadLine();
                    await categoryService.UpdateTitle(title, ConvertLong(value3));
                    break;
                case "6":
                    ProductShortDto product = new ProductShortDto();
                    Console.WriteLine("Introduceti denumirea produsului:");
                    product.Title = Console.ReadLine();
                    Console.WriteLine("Introduceti pretul produsului");
                    string Price = Console.ReadLine();
                    product.Price = ConvertDecimal(Price);
                    Console.WriteLine("Introdu Id categoriei din care sa faca parte produsul ");
                    value = Console.ReadLine();
                    await productService.AddProductinCategory(product, ConvertLong(value));
                    break;
                case "7":
                    Console.WriteLine("Introduceti id categoriei din care sa afisam produsele");
                    value = Console.ReadLine();
                    await productService.GetProductsCategory(ConvertLong(value));
                    break;
                default:
                    Console.WriteLine("Optiune invalida.");
                    return;

            }
        }
    }
    
}

public class Product
{
    public long Id { get; set; }
    public string Title { get; set; }
    public Decimal Price { get; set; }
    public Category ParentCategory { get; set; }
}
public class Category
{
    public long Id { get; set; }
    public string Title { get; set; }
    public ICollection<Product> Products { get; set; } = new HashSet<Product>();
}
public class CategoryShortDto
{
    [JsonProperty("id")]
    public long Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("itemsCount")]
    public long ItemsCount { get; set; }
}
public class CreateCategoryDto
{
    public string Title { get; set; }
}
public class ProductShortDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public Decimal Price { get; set; }
    public long CategoryId { get; set; }
}