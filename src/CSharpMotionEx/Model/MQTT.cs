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
        private Motor m_m;
        private IMqttClient mqttClient;
        public MQTT(Motor m)
        {
            _ = Main(m);
        }
        static async Task Main(Motor m)
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                //.WithClientId("MyClient")
                .WithTcpServer("192.168.18.40", 1883) 
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
                    
                    Speed = m.VelocityAverage,
                    Position = m.PositionAverage
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

                await Task.Delay(500);
            }
        }
    }
}
