using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace Model
{
    public class MotorInfo
    {
        public double Speed { get; set; }
        public double Position { get; set; }
    }

    class MQTT
    {
        private static Motor m_m;
        public MQTT(Motor m)
        {
            m_m = m;
            _ = Main();
        }
        static async Task Main()
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                //.WithClientId("MyClient")
                .WithTcpServer("192.168.18.108", 1883) // Replace "localhost" with your broker address
                .WithCleanSession()
                .Build();

            try
            {
                await mqttClient.ConnectAsync(options);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while connecting: " + e.Message);
                return;
            }

            while (true)
            {
                var motorInfo = new MotorInfo
                {
                    Speed = m_m.VelocityAverage,
                    Position = m_m.PositionAverage
                };

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("MotorInfo")
                    .WithPayload(JsonConvert.SerializeObject(motorInfo))
                    .WithRetainFlag()
                    .Build();

                try
                {
                    await mqttClient.PublishAsync(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while publishing: " + e.Message);
                    break;
                }

                // Publish every second
                await Task.Delay(2000);
            }
        }
    }
}
