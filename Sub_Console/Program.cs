using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Sub_Console
{
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;

        [Obsolete]
        static async Task  Main(string[] args)
        {
            //Console.WriteLine("Hello World3!");
            try
            {
                Console.WriteLine("Starting Subsriber....");

                //create subscriber client
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();
                string sclientId = Guid.NewGuid().ToString();
                //configure options
                _options = new MqttClientOptionsBuilder()
                    .WithClientId(sclientId)
                    .WithTcpServer("localhost", 1884)
                    .WithCredentials("suhel", "%suhel@786%")
                    .WithCleanSession()
                    .Build();

                //Handlers
                _client.UseConnectedHandler(async e =>
                {
                    Console.WriteLine("Connected successfully with MQTT Brokers.");

                    //Subscribe to topic
                    //_client.SubscribeAsync(new TopicFilterBuilder().WithTopic("test").Build()).Wait();

                    await _client.SubscribeAsync(new TopicFilterBuilder().WithTopic("test").Build());
                });
                _client.UseDisconnectedHandler(e =>
                {
                    Console.WriteLine("Disconnected from MQTT Brokers.");
                });
                _client.UseApplicationMessageReceivedHandler(e =>
                {
                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();

                    //  Task.Run(() => _client.PublishAsync("hello/world"));
                });

                //actually connect
                //_client.ConnectAsync(_options).Wait();

                await _client.ConnectAsync(_options);

                Console.WriteLine("Press key to exit");
                Console.ReadLine();


                _client.DisconnectAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
