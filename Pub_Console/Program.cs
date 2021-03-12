using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Pub_Console
{
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Publisher....");
            try
            {
                // Create a new MQTT client.
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();
                string pclientId = Guid.NewGuid().ToString();
                //configure options
                _options = new MqttClientOptionsBuilder()
                    .WithClientId(pclientId) //"PublisherId"
                    .WithTcpServer("localhost", 1884)
                    .WithCredentials("suhel", "%suhel@786%")
                    .WithCleanSession()
                    .Build();




                //handlers
                _client.UseConnectedHandler(e =>
                {
                    Console.WriteLine("Connected successfully with MQTT Brokers.");
                });
                _client.UseDisconnectedHandler(e =>
                {
                    Console.WriteLine("Disconnected from MQTT Brokers.");
                });
                _client.UseApplicationMessageReceivedHandler(e =>
                {
                    try
                    {
                        string topic = e.ApplicationMessage.Topic;
                        if (string.IsNullOrWhiteSpace(topic) == false)
                        {
                            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                            Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, ex);
                    }
                });


                //connect
                // _client.ConnectAsync(_options).Wait();
                await _client.ConnectAsync(_options);


                Console.WriteLine("Press key to publish message.");
                Console.ReadLine();
                //simulating publish
                SimulatePublish();


                Console.WriteLine("Simulation ended! press any key to exit.");
                Console.ReadLine();


                _client.DisconnectAsync().Wait();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //This method send messages to topic "test"
        static void SimulatePublish()
        {
            //string send_msg = @"{ 'Name': 'Suhel', 'Contact':'123456' }";
            string jsonData = string.Empty;
            jsonData = File.ReadAllText(@"C:\\Users\\INSUAHM\\Desktop\\Testing.json");
            //var counter = 0;
            //while (counter < 10)
            //{
            //counter++;
            var testMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("test")
                    .WithPayload($"Payload: {jsonData}")
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();


                if (_client.IsConnected)
                {
                    Console.WriteLine($"publishing at {DateTime.UtcNow}");
                    _client.PublishAsync(testMessage);
                }
                Thread.Sleep(2000);
            //}
        }
    }
}
