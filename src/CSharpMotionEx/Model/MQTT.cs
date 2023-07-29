using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
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
        private static IMqttClient mqttClient;
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
                .WithTcpServer("192.168.0.140", 1883) 
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
        }

        public async Task Publish()
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
            }
        }
    }
}
