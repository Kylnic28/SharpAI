// See https://aka.ms/new-console-template for more information
using SharpAI;
using SharpAI.API;
using static SharpAI.API.AIModules;

SharpOpenAI ai = new();

Completion completion = new Completion()
{
    Prompt = "Why StackOverflow is considered as a Bible for developers?",
    MaxTokens = 2048/2,
    Temperature = 0,
    TopP = 1,
    FrequencePenalty = 0,
    PresencePenalty = 0,
    Stop = null
};


var answer = await ai.AskToAI(TextGenerationModule, completion);
Console.WriteLine(answer);