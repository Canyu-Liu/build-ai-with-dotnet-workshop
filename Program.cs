using System.ComponentModel.DataAnnotations;
using Azure;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI;

namespace ChatApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var azureEndpoint = new Uri("your-azure-endpoint"); // Replace with your Azure OpenAI endpoint
            string deploymentName = "gpt-4.1-nano";
            var azureCredential = new AzureKeyCredential(config["AzureAiKey"]);

            IChatClient chatClient =
            new AzureOpenAIClient(azureEndpoint, azureCredential)
                .GetChatClient(deploymentName)
                .AsIChatClient();

            //如果您使用的是GitHub Models，请使用以下代码：
            // IChatClient client =
            //     new Azure.AI.Inference.ChatCompletionsClient(
            //     azureEndpoint,
            //     azureCredential)
            //     .AsIChatClient(deploymentName);
            
            Console.WriteLine(await chatClient.GetResponseAsync("什么是 Ai？"));
        }
    }
}