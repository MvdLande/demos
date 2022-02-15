using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;

namespace toit.demos.api.csharp.pubsubpublish
{
    public class Program
    {
        // 
        //private static string apikey = "enter API key secret here"; // From: https://console.toit.io/project/apikeys
        private static string apikey = "xxxxxx"; // From: https://console.toit.io/project/apikeys
        static async Task Main(string[] args)
        {
            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("Authorization", $"Bearer {apikey}");
                return Task.CompletedTask;
            });
            var channelCredentials = ChannelCredentials.Create(new SslCredentials(), callCredentials);
            using var channel = GrpcChannel.ForAddress("https://api.toit.io:443", new GrpcChannelOptions
            {
                Credentials = channelCredentials
            });

            var publisher = new Toit.Proto.API.PubSub.Publish.PublishClient(channel);

            var request = new Toit.Proto.API.PubSub.PublishRequest
            {
                Topic = "cloud:hello-world",
                PublisherName = "C#",
                Data = { ByteString.CopyFromUtf8("This is the message!") }
            };
            await publisher.PublishAsync(request);

            // create a subscription

            var subscriber = new Toit.Proto.API.PubSub.Subscribe.SubscribeClient(channel);

            var subscription = new Toit.Proto.API.PubSub.Subscription
            {
                Topic = "cloud:hello-world",
                Name = "C#"
            };

            var subscriptionrequest = new Toit.Proto.API.PubSub.CreateSubscriptionRequest 
            { 
                Subscription = subscription 
            };

            //var SubscriptionResponse = new Toit.Proto.API.PubSub.CreateSubscriptionResponse();

            //SubscriptionResponse = subscriber.CreateSubscription(subscriptionrequest);
            await subscriber.CreateSubscriptionAsync(subscriptionrequest);

        }
    }
}
