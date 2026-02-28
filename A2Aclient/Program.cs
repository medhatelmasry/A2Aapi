using A2A;

// Discover the agent endpoint and capabilities
var httpClient = new HttpClient()
{
    Timeout = TimeSpan.FromSeconds(30)
};

// Third argument is the relative path used when resolving the card URL.
var cardResolver = new A2ACardResolver(
    new Uri("http://127.0.0.1:5112/a2a/pirate/"),
    httpClient,
    "v1/card"  
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

string convertoutput = ((TextPart)response!.Parts[0]).Text;
Console.WriteLine($"Response: {convertoutput}");
Console.WriteLine("\nAll done. ");
