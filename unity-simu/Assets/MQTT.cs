using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using UnityEngine;

namespace Model
{
    public class MotorInfo
    {
        public float Speed { get; set; }
        public float Position { get; set; }
    }
    public class MQTT : MonoBehaviour
    {
        private static string receivedMessage;
        private static HingeJoint hinge;

        private async void Start()
        {
            hinge = gameObject.GetComponent<HingeJoint>();
            await Handle_Received_Application_Message();
        }

        public static async Task Handle_Received_Application_Message()
        {
            /*
             * This sample subscribes to a topic and processes the received message.
             */

            var mqttFactory = new MqttFactory();
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("192.168.18.40", 1883).Build();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    if (e.ApplicationMessage.Topic == "MotorInfo")
                    {
                        receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    }

                    return Task.CompletedTask;
                };

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("MotorInfo"))
                    .Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                Debug.Log("MQTT client subscribed to topic.");

                while (true)
                {
                    // Process the received message (if any)
                    if (!string.IsNullOrEmpty(receivedMessage))
                    {
                        MotorInfo motorData = JsonConvert.DeserializeObject<MotorInfo>(receivedMessage);

                        if (motorData.Speed != 0)
                        {
                            JointMotor motor = hinge.motor;
                            motor.targetVelocity = motorData.Speed;
                            hinge.motor = motor;  // Assignez le nouveau moteur à votre HingeJoint

                            Debug.Log(motorData.Speed);
                        }

                        // Reset the receivedMessage variable after processing
                        receivedMessage = "";
                    }

                    await Task.Delay(500);
                }
            }
        }
    }
}
