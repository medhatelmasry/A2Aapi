using A2A;
using A2Aclient;

// Discover the agent endpoint and capabilities
// NOTE: trailing slash is significant for relative URI resolution.  Without it
// the resolver may drop the last segment ("pirate") when appending "/v1/card".
// We'll supply a custom HttpClient so we can log the actual request URI.
var httpClient = new HttpClient()
{
    Timeout = TimeSpan.FromSeconds(30)
};

// The library defaults to ".well-known/agent-card.json"; override it to match
// the server's swagger route.  Third argument is the relative path used when
// resolving the card URL.
var cardResolver = new A2ACardResolver(
    new Uri("http://127.0.0.1:5112/a2a/pirate/"),
    httpClient,
    "v1/card"                // <- use the swagger-defined endpoint
);

var agentCard = await cardResolver.GetAgentCardAsync();
Console.WriteLine($"Connected to: {agentCard.Name}");

Console.WriteLine($"Supports streaming: {agentCard.Capabilities?.Streaming}");

// Initialize the A2AClient
var client = new A2AClient(new Uri(agentCard.Url));

// Construct a message
var message = new AgentMessage
{
    Role = MessageRole.User,
    MessageId = Guid.NewGuid().ToString(),
    Parts = [new TextPart { Text = "Hello, agent! What can you do?" }]
};

// Send the message and receive the response
AgentMessage? response = await client.SendMessageAsync(
    new MessageSendParams { Message = message }
) as AgentMessage;

// Process the response
// if (response?.Message?.Parts != null)
// {
//     var responseText = response.Message.Parts.OfType<TextPart>().FirstOrDefault()?.Text;
//     Console.WriteLine($"Agent response: {responseText}");
// }

string convertoutput = ((TextPart)response!.Parts[0]).Text;
Console.WriteLine($"Response: {convertoutput}");
Console.WriteLine("\nAll done. ");

/*

// Agent endpoints (could be read from config)
// var currencyAgentUrl = new Uri("http://localhost:5112/a2a/pirate");
var currencyAgentUrl = new Uri("http://localhost:5112/a2a/pirate/v1/card");

// 1. Discover agents via Agent Cards
var resolver1 = new A2ACardResolver(currencyAgentUrl);

AgentCard currencyCard = await resolver1.GetAgentCardAsync();
Console.WriteLine($"Discovered Agent: {currencyCard.Name} - {currencyCard.Description}");
Console.WriteLine($"Endpoint: {currencyCard.Url}, Streaming: {currencyCard.Capabilities?.Streaming}");

// 2. Create A2AClient instances for the agent
var currencyClient = new A2AClient(new Uri(currencyCard.Url));

// 3. Prepare request the agent
var convertRequest = new AgentMessage
{
    Role = MessageRole.User,
    MessageId = Guid.NewGuid().ToString(),
    Parts = new List<Part> { new TextPart { Text = "100 USD to EUR" } }
};

// 4. Call Currency Converter agent (standard request-response)
Console.WriteLine($"\nQuerying Currency Agent: {((TextPart)convertRequest.Parts[0]).Text}");
var convertParams = new MessageSendParams { Message = convertRequest };
AgentMessage convertResponse = (AgentMessage)await currencyClient.SendMessageAsync(convertParams);
string convertoutput = ((TextPart)convertResponse.Parts[0]).Text;
Console.WriteLine($"Response: {convertoutput}");
Console.WriteLine("\nAll done. ");

*/
