using System;
using System.Collections.Generic;
using Amazon.SQS.Model;

namespace EventBus.Sqs.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static void BuildToMessageAttribute(this Dictionary<string, string> keyValues, Dictionary<string, MessageAttributeValue> messagesAttribute)
        {
            if (keyValues != null)
            {
                foreach (KeyValuePair<string, string> entry in keyValues)
                {
                    messagesAttribute.Add(entry.Key, new MessageAttributeValue()
                    {
                        DataType = "String",
                        StringValue = entry.Value
                    });
                }
            }
        }
        internal static Dictionary<string, string> BuildToMessageAttribute(this Dictionary<string, MessageAttributeValue> keyValues)
        {
            var messageToReturn = new Dictionary<string, string>();

            if (keyValues != null)
            {
                foreach (KeyValuePair<string, MessageAttributeValue> entry in keyValues)
                {
                    messageToReturn.Add(entry.Key, entry.Value.StringValue);
                }
            }
            return messageToReturn;
        }
    }
}
