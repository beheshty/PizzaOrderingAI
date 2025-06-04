using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PizzaOrderingAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var modelId = configuration["OpenAI:ModelId"];
var endpoint = configuration["OpenAI:Endpoint"];
var apiKey = configuration["OpenAI:ApiKey"];

var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromType<PizzaOrdering>("PizzaOrdering");

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    ChatSystemPrompt = "You are a helpful assistant that can help with pizza ordering. You can provide a list of pizzas, place an order, and retrieve the current order, or clear the current Order. You try to get the order from the user and always ask the user if they need more to add to their order",
};

var history = new ChatHistory();

string? userInput;
do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    history.AddUserMessage(userInput);

    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    Console.WriteLine("Assistant > " + result);

    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);