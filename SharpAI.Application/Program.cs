// See https://aka.ms/new-console-template for more information
using SharpAI;
using SharpAI.API;
using SharpAI.Configuration;
using System.Text.Json;
using static SharpAI.API.AIModules;

var data = JsonSerializer.Serialize(new APIConfiguration() { });
File.WriteAllText("SharpAI.json", data);

SharpOpenAI openAi = new SharpOpenAI();

Completion dataToSend = new()
{
    Prompt = "Pourquoi Hipolee est un lâche ?",
    MaxTokens = 2048,
    Temperature = 0,
    TopP = 1,
    Stop = null

};

Image image = new Image() { Number = 1, Prompt = "Darth Vader but without the armor", ResponseFormat = "url", Size = "512x512" };

var result = await openAi.AskToAI(TextGenerationModule, dataToSend);
var url = await openAi.AskToAI(ImageGenerationModule, image);


Console.WriteLine(url);