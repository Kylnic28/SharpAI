// See https://aka.ms/new-console-template for more information
using SharpAI;
using SharpAI.API;

SharpOpenAI ai = new();


Completion completion = new Completion()
{
    Model = "text-davinci-003",
    Prompt = "Explain me what is Stackoverflow.",
    MaxTokens = 4000,
    Temperature = 0.9f,
    TopP = 0,
    FrequencePenalty = 0,
    PresencePenalty = 0,
    Stop = nulls
};

Image image = new()
{
    Prompt = "Anakin Skywalker as a communist, under the red flag, realistic",
    Number = 1,
    ResponseFormat = "url",
    Size = "1024x1024"
};


var answer = await ai.AskToAI(completion);
Console.WriteLine(answer);