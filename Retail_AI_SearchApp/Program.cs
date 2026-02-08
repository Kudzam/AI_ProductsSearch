// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using Retail_AI_SearchApp.Models;
using System.ClientModel;

List<RetailProductsModel> productsModels =
[new(){
    ID = 1,
    ProductType = "Shoes",
    ProductLabel = "Nike",
    ProductModel = "Air Max 270",
    ProductDescription = "Nike Air Max 270 shoes feature a large air unit for exceptional cushioning, a breathable mesh upper for comfort, and a sleek, modern design suitable for both casual wear and athletic performance.\r\n"
},
new(){
    ID = 2,
    ProductType = "Shoes",
    ProductLabel = "Adidas",
    ProductModel = "Ultraboost 22",
    ProductDescription = "Adidas Ultraboost 22 shoes combine responsive Boost cushioning, a flexible Primeknit upper, and a supportive midsole, delivering an optimal running experience while maintaining a stylish, everyday-ready look.\r\n"
},
new(){
    ID = 3,
    ProductType = "Shoes",
    ProductLabel = "ASICS",
    ProductModel = "Gel-Kayano 28",
    ProductDescription = "ASICS Gel-Kayano 28 shoes offer advanced stability technology, GEL cushioning for shock absorption, and a lightweight, breathable mesh upper, making them perfect for long-distance running and active lifestyles.\r\n"
},
new(){
    ID = 4,
    ProductType = "Shoes",
    ProductLabel = "Puma",
    ProductModel = "Future Rider",
    ProductDescription = "Puma Future Rider shoes feature a soft EVA midsole for comfortable impact absorption, a durable rubber outsole for grip, and a retro-inspired design that combines style with everyday versatility.\r\n"
},
new(){
    ID = 5,
    ProductType = "Shoes",
    ProductLabel = "Reebok",
    ProductModel = "Nano X3",
    ProductDescription = "Reebok Nano X3 shoes are engineered for cross-training, offering a stable heel, flexible forefoot, and responsive cushioning, ensuring superior performance in diverse workout routines while maintaining comfort.\r\n"
},
new(){
    ID = 6,
    ProductType = "Shoes",
    ProductLabel = "New Balance",
    ProductModel = "990v5",
    ProductDescription = "New Balance 990v5 shoes combine premium pigskin and mesh materials with ENCAP midsole technology for cushioning and support, delivering a timeless style alongside reliable performance for running and casual wear.\r\n"
},
new(){
    ID = 7,
    ProductType = "Shoes",
    ProductLabel = "Converse",
    ProductModel = "Chuck Taylor All Star",
    ProductDescription = "Converse Chuck Taylor All Star shoes offer a classic canvas upper, durable rubber sole, and timeless high-top design, making them a versatile and iconic choice for casual fashion and everyday wear.\r\n"
},
new(){
    ID = 8,
    ProductType = "Shoes",
    ProductLabel = "Vans",
    ProductModel = "Old Skool",
    ProductDescription = "Vans Old Skool shoes feature a durable suede and canvas upper, signature rubber waffle outsole, and padded collar for support, blending skate-ready performance with casual streetwear style.\r\n"
},
new(){
    ID = 9,
    ProductType = "Shoes",
    ProductLabel = "Under Armour",
    ProductModel = "HOVR Phantom 3",
    ProductDescription = "Under Armour HOVR Phantom 3 shoes provide energy-returning cushioning, a lightweight breathable upper, and a secure fit, designed to enhance running performance while maintaining comfort for all-day wear.\r\n"
},
new(){
    ID = 10,
    ProductType = "Shoes",
    ProductLabel = "Levi’s",
    ProductModel = "Denim Runner",
    ProductDescription = "Levi’s Denim Runner shoes combine classic denim accents with a cushioned insole and durable rubber outsole, offering casual streetwear style alongside comfortable, everyday performance.\r\n"
}

    ];

// Load the configuration values.
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string? model = config["ModelName"];
string? key = config["OpenAIKey"];

if (string.IsNullOrWhiteSpace(model))
{
    throw new InvalidOperationException("ModelName configuration value is missing.");
}
if (string.IsNullOrWhiteSpace(key))
{
    throw new InvalidOperationException("OpenAIKey configuration value is missing.");
}

// Create the embedding generator.
IEmbeddingGenerator<string, Embedding<float>> generator =
    new OpenAIClient(new ApiKeyCredential(key))
      .GetEmbeddingClient(model: model)
      .AsIEmbeddingGenerator();

// Create and populate the vector store.
var vectorStore = new InMemoryVectorStore();
VectorStoreCollection<int, RetailProductsModel> productsvectorstore =
    vectorStore.GetCollection<int, RetailProductsModel>("cloudServices");
await productsvectorstore.EnsureCollectionExistsAsync();

foreach (RetailProductsModel service in productsModels)
{
    service.Vector = await generator.GenerateVectorAsync(service.ProductDescription);
    await productsvectorstore.UpsertAsync(service);
}

// Convert a search query to a vector
// and search the vector store.
Console.WriteLine("Are you looking for a new shoe ? \n\n Enter a description of the shoe you want ?");
string query = Console.ReadLine();
Console.WriteLine("\n\n This is your recommendation");
ReadOnlyMemory<float> queryEmbedding = await generator.GenerateVectorAsync(query);

IAsyncEnumerable<VectorSearchResult<RetailProductsModel>> results =
    productsvectorstore.SearchAsync(queryEmbedding, top: 1);

await foreach (VectorSearchResult<RetailProductsModel> result in results)
{
    Console.WriteLine($"Name: {result.Record.ProductLabel}");
    Console.WriteLine($"Model: {result.Record.ProductModel}");
    Console.WriteLine($"Description: {result.Record.ProductDescription}");
    Console.WriteLine($"Vector match score: {result.Score}");
    // Console.WriteLine($"Cost of the query: {result.} ");
}


