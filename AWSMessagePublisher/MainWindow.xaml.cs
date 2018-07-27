using System;
using System.Configuration;
using System.Windows;
using Amazon;
using Newtonsoft.Json;

namespace AWSMessagePublisher
{
    public class ViewModel
    {
        public bool UseDefaultCredentials { get; set; }

        public string AWSAccessKey { get; set; }

        public string AWSSecretKey { get; set; }

        public string SessionToken { get; set; }

        public string TopicName { get; set; }

        public string MessageType { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public string Region { get; set; }
    }

    public partial class MainWindow : Window
    {
        public ViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            DataContext = ViewModel;
        }

        private void Initialize()
        {
            ViewModel = new ViewModel()
                        {
                            AWSAccessKey = ConfigurationManager.AppSettings["AccessKey"],
                            AWSSecretKey = ConfigurationManager.AppSettings["SecretKey"],
                            TopicName = ConfigurationManager.AppSettings["TopicName"],
                            MessageType = ConfigurationManager.AppSettings["MessageType"],
                            Region = ConfigurationManager.AppSettings["Region"],
                        };
        }

        private void SendMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var publisher = GetPublisher();
                publisher.Publish(ViewModel.TopicName, ViewModel.MessageType, ViewModel.Message);

                ViewModel.Status = $"{ViewModel.MessageType} Message sent at {DateTime.Now.ToShortTimeString()}";
                statusLabel.Content = ViewModel.Status;
            }
            catch (Exception exception)
            {
                ViewModel.Status = exception.Message;
                statusLabel.Content = ViewModel.Status;
            }
        }

        private void BeautifyMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.Message))
            {
                ViewModel.Message = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(ViewModel.Message), Formatting.Indented);
                MessageTexBox.Text = ViewModel.Message;
            }
        }

        private SNSMessagePublisher GetPublisher()
        {
            var region = RegionEndpoint.GetBySystemName(ViewModel.Region);

            if (ViewModel.UseDefaultCredentials)
            {
                return new SNSMessagePublisher(region);
            }

            if (!string.IsNullOrEmpty(ViewModel.SessionToken))
            {
                return new SNSMessagePublisher(ViewModel.AWSAccessKey, ViewModel.AWSSecretKey, ViewModel.SessionToken, region);
            }

            return new SNSMessagePublisher(ViewModel.AWSAccessKey, ViewModel.AWSSecretKey, region);
        }
    }
}
