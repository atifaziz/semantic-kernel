﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
/**
 * Logit_bias is an optional parameter that modifies the likelihood of specified tokens appearing in a Completion.
 * When using the Token Selection Biases parameter, the bias is added to the logits generated by the model prior to sampling.
 */
// ReSharper disable once InconsistentNaming
public static class Example49_LogitBias
{
    public static async Task RunAsync()
    {
        OpenAIChatCompletionService chatCompletionService = new(TestConfiguration.OpenAI.ChatModelId, TestConfiguration.OpenAI.ApiKey);

        // To use Logit Bias you will need to know the token ids of the words you want to use.
        // Getting the token ids using the GPT Tokenizer: https://platform.openai.com/tokenizer

        // The following text is the tokenized version of the book related tokens
        // "novel literature reading author library story chapter paperback hardcover ebook publishing fiction nonfiction manuscript textbook bestseller bookstore reading list bookworm"
        var keys = new[] { 3919, 626, 17201, 1300, 25782, 9800, 32016, 13571, 43582, 20189, 1891, 10424, 9631, 16497, 12984, 20020, 24046, 13159, 805, 15817, 5239, 2070, 13466, 32932, 8095, 1351, 25323 };

        var settings = new OpenAIPromptExecutionSettings();

        // This will make the model try its best to avoid any of the above related words.
        //-100 to potentially ban all the tokens from the list.
        settings.TokenSelectionBiases = keys.ToDictionary(key => key, key => -100);

        Console.WriteLine("Chat content:");
        Console.WriteLine("------------------------");

        var chatHistory = new ChatHistory("You are a librarian expert");

        // First user message
        chatHistory.AddUserMessage("Hi, I'm looking some suggestions");
        await MessageOutputAsync(chatHistory);

        var replyMessage = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings);
        chatHistory.AddAssistantMessage(replyMessage.Content!);
        await MessageOutputAsync(chatHistory);

        chatHistory.AddUserMessage("I love history and philosophy, I'd like to learn something new about Greece, any suggestion");
        await MessageOutputAsync(chatHistory);

        replyMessage = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings);
        chatHistory.AddAssistantMessage(replyMessage.Content!);
        await MessageOutputAsync(chatHistory);

        /* Output:
        Chat content:
        ------------------------
        User: Hi, I'm looking some suggestions
        ------------------------
        Assistant: Sure, what kind of suggestions are you looking for?
        ------------------------
        User: I love history and philosophy, I'd like to learn something new about Greece, any suggestion?
        ------------------------
        Assistant: If you're interested in learning about ancient Greece, I would recommend the book "The Histories" by Herodotus. It's a fascinating account of the Persian Wars and provides a lot of insight into ancient Greek culture and society. For philosophy, you might enjoy reading the works of Plato, particularly "The Republic" and "The Symposium." These texts explore ideas about justice, morality, and the nature of love.
        ------------------------
        */
    }

    /// <summary>
    /// Outputs the last message of the chat history
    /// </summary>
    private static Task MessageOutputAsync(ChatHistory chatHistory)
    {
        var message = chatHistory.Last();

        Console.WriteLine($"{message.Role}: {message.Content}");
        Console.WriteLine("------------------------");

        return Task.CompletedTask;
    }
}
