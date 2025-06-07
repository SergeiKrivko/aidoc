using System.Net.Http.Json;
using System.Text.Json;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Application;

public class AiClient : IAiClient
{
    private readonly HttpClient _httpClient = new();

    private const int MaxToolCalls = 100;
    private const int MaxRetries = 3;

    private record Function(string Name, Func<string, Task<object?>> Func);

    private readonly List<Function> _functions = [];

    public async Task<TResult?> ProcessAsync<TIn, TResult>(string url, TIn request)
    {
        var resp = await SendInitAsync(url, request);
        for (int i = 0; i < MaxToolCalls; i++)
        {
            var retry = 0;
            while (true)
            {
                try
                {
                    var messages = resp.Messages.ToList();
                    var lastMessage = messages.Last();
                    if (lastMessage.ToolCalls == null || lastMessage.ToolCalls.Length == 0)
                    {
                        if (lastMessage.Content == null)
                            throw new Exception("Invalid response from AI");
                        return JsonSerializer.Deserialize<TResult>(lastMessage.Content);
                    }

                    resp = await SendAsync(url, new AiRequestModel
                    {
                        Messages = messages.Concat(await CallToolsAsync(lastMessage)).ToArray()
                    });
                    break;
                }
                catch (Exception)
                {
                    retry++;
                    if (retry > MaxRetries)
                        throw;
                }
            }
        }
        throw new Exception("Max calls reached");
    }

    private async Task<AiResponseModel> SendAsync(string url, AiRequestModel request)
    {
        var content = JsonContent.Create(request);
        var resp = await _httpClient.PostAsync(url + "/request", content);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<AiResponseModel>() ??
                     throw new Exception("Failed to send request");
        return result;
    }

    private async Task<AiResponseModel> SendInitAsync<TIn>(string url, TIn request)
    {
        var content = JsonContent.Create(request);
        var resp = await _httpClient.PostAsync(url + "/init", content);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<AiResponseModel>() ??
                     throw new Exception("Failed to send request");
        return result;
    }

    private async Task<List<AiMessage>> CallToolsAsync(AiMessage message)
    {
        var messages = new List<AiMessage>();
        if (message.ToolCalls == null)
            return messages;
        foreach (var toolCall in message.ToolCalls)
        {
            var function = _functions.Single(f => f.Name == toolCall.Function.Name);
            var res = await function.Func(toolCall.Function.Arguments);
            messages.Add(new AiMessage
            {
                Role = "tool",
                Content = JsonSerializer.Serialize(res),
                ToolCallId = toolCall.Id,
            });
        }
        return messages;
    }

    public void AddFunction<TIn, TOut>(string name, Func<TIn?, Task<TOut>> func)
    {
        _functions.Add(new Function(name, async data =>
        {
            var param = JsonSerializer.Deserialize<TIn>(data);
            return await func(param);
        }));
    }
}