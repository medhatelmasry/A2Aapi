using OpenAI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Azure;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

string githubToken = builder.Configuration["GitHub:Token"]
    ?? throw new InvalidOperationException("GitHub:Token is not set.");
string apiEndpoint = builder.Configuration["GitHub:ApiEndpoint"]
    ?? throw new InvalidOperationException("GitHub:ApiEndpoint is not set.");
string model = builder.Configuration["GitHub:Model"]
    ?? throw new InvalidOperationException("GitHub:Model is not set.");

// Register the chat client
IChatClient chatClient = new ChatClient(
    model,
    new AzureKeyCredential(githubToken),
    new OpenAIClientOptions
    {
        Endpoint = new Uri(apiEndpoint)
    }
)
.AsIChatClient();

builder.Services.AddSingleton(chatClient);

// Register agents
var pirateAgent = builder.AddAIAgent("pirate", instructions: "You are a pirate. Speak like a pirate.");

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

// Expose the agent via A2A protocol. You can also customize the agentCard
app.MapA2A(pirateAgent, path: "/a2a/pirate", agentCard: new()
{
    Name = "Pirate Agent",
    Description = "An agent that speaks like a pirate.",
    Version = "1.0"
});

app.Run();