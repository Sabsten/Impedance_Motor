﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Newtonsoft.Json;
using ScottPlot.Renderable;

namespace Model
{
    public class MotorInfo
    {
        public double Speed { get; set; }
        public double Position { get; set; }

    }

    class MQTT
    {
        private static IMqttClient mqttClient;
        public MQTT(Motor m)
        {
            _ = Main();
        }
        static async Task Main()
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

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
        }

        public async Task Publish(double Velocity, double Position)
        {

            var motorInfo = new MotorInfo
            {
                Speed = Velocity * 6,
                Position = Position
            };

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("MotorInfo")
                .WithPayload(JsonConvert.SerializeObject(motorInfo))
                .WithRetainFlag()
                .Build();
            try
            {
                mqttClient.PublishAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while publishing: " + e.Message);
            }
        }
    }
}
