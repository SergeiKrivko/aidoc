﻿using AiDoc.Core.Models;

namespace AiDoc.Core.Abstractions;

public interface IAiClient
{
    public Task<TResult?> ProcessAsync<TIn, TResult>(string url, TIn request);
    public Task<string?> ProcessAsync<TIn>(string url, TIn request);

    public void AddFunction<TIn, TOut>(string name, Func<TIn?, Task<TOut>> func);

    public Task<Stream> GenerateUml(string content);
}