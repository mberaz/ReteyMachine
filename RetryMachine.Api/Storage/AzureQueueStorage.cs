﻿using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json;

namespace RetryMachine.Api.Storage;

public class AzureQueueStorage : IRetryStorage
{
    //https://learn.microsoft.com/en-us/azure/storage/queues/storage-quickstart-queues-dotnet?tabs=passwordless%2Croles-azure-portal%2Cenvironment-variable-windows%2Csign-in-azure-cli
    private const string queueName = "retry-q";
    private const string delimiter = ";";
    private const int maxRetryCounter = 5;
    private const int numberOfItemsToGet = 10;
    private readonly QueueClient _queueClient;

    public AzureQueueStorage()
    {
        var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
        _queueClient = new QueueClient(connectionString, queueName);
    }

    public Task Save(RetryTaskModel retryTaskModel)
    {
        return _queueClient.SendMessageAsync(JsonConvert.SerializeObject(retryTaskModel));
    }

    public async Task Update(RetryTaskModel retryTaskModel)
    {
        if (retryTaskModel.ExternalId != null)
        {
            var (messageId, popReceipt) = SplitExternalId(retryTaskModel.ExternalId);
            await _queueClient.DeleteMessageAsync(messageId, popReceipt);
        }

        //we don't want to re-queue the task if it is done, or we have reached the max retry counter
        if (retryTaskModel.Status != (int)RetryStatus.Done || retryTaskModel.RetryCount >= maxRetryCounter)
        {
            await _queueClient.SendMessageAsync(JsonConvert.SerializeObject(retryTaskModel));
        }
    }

    public async Task<List<RetryTaskModel>> Get()
    {
        QueueMessage[] messages = await _queueClient.ReceiveMessagesAsync(maxMessages: numberOfItemsToGet);

        return messages.Select(m =>
        {
            var item = m.Body.ToObjectFromJson<RetryTaskModel>();
            item.ExternalId = CreateExternalId(m);
            return item;
        }).ToList();
    }

    private string CreateExternalId(QueueMessage message) => message.MessageId + delimiter + message.PopReceipt;

    private (string messageId, string popReceipt) SplitExternalId(string externalId)
    {
        var parts = externalId.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        return (parts.First(), parts.Last());
    }
}