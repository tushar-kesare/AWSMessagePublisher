using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace AWSMessagePublisher
{
    public class SNSMessagePublisher
    {
        public IAmazonSimpleNotificationService Client { get; private set; }

        public SNSMessagePublisher(RegionEndpoint region)
        {
            Client = GetSnsClient(region);
        }

        public SNSMessagePublisher(string accessKey, string secretKey, RegionEndpoint region)
        {
            Client = GetSnsClient(accessKey, secretKey, region);
        }

        public SNSMessagePublisher(string accessKey, string secretKey, string sessionToken, RegionEndpoint region)
        {
            Client = GetSnsClient(accessKey, secretKey, sessionToken, region);
        }

        public void Publish(string topicName, string messageType, string message)
        {
            var topic = Client.CreateTopic(new CreateTopicRequest(topicName));
            PublishRequest request = new PublishRequest();
            request.Subject = messageType;
            request.Message = message;
            request.TopicArn = topic.TopicArn;
            Client.Publish(request);
        }

        private IAmazonSimpleNotificationService GetSnsClient(RegionEndpoint region)
        {
            var client = AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(new AmazonSimpleNotificationServiceConfig(){RegionEndpoint = region});
            return client;
        }

        private IAmazonSimpleNotificationService GetSnsClient(string accessKey, string secretKey, RegionEndpoint region)
        {
            return AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(new BasicAWSCredentials(accessKey, secretKey), region);
        }

        private IAmazonSimpleNotificationService GetSnsClient(string accessKey, string secretKey, string sessionToken, RegionEndpoint region)
        {
            return AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(new SessionAWSCredentials(accessKey, secretKey, sessionToken), region);
        }
    }
}
