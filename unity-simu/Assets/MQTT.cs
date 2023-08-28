using System;
using System.Collections.Generic;
using System.Drawing;
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
        private static bool isInitialPositionSet = false;
        private bool isFirstMessage = true; // Ajout pour gérer le premier message

        private async void Start()
        {
            hinge = gameObject.GetComponent<HingeJoint>();
            await Handle_Received_Application_Message();
        }

        public async Task Handle_Received_Application_Message()
        {
<<<<<<< Updated upstream


=======
>>>>>>> Stashed changes
            var mqttFactory = new MqttFactory();
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("192.168.18.40", 1883).Build();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    // Bloc de code ajouté pour gérer le premier message reçu
                    if (isFirstMessage)
                    {
                        if (e.ApplicationMessage.Retain)
                        {
                            // Si le message est retenu et que c'est le premier message, ignorez-le
                            return Task.CompletedTask;
                        }
                        isFirstMessage = false;
                    }

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
                            // Get current Euler rotation
                            Vector3 currentEulerRotation = transform.rotation.eulerAngles;

                            // Create the desired rotation (only updating the X axis)
                            Quaternion desiredRotation = Quaternion.Euler(motorData.Position, currentEulerRotation.y, currentEulerRotation.z);

                            // Lerp from the current rotation to the desired rotation
                            float lerpValue = 0.1f;  // Adjust for smoother or quicker transitions.
                            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, lerpValue);

                            Debug.Log("Set position to: " + motorData.Position);
                        }

                        // Set the motor velocity
                        JointMotor motor = hinge.motor;
                        motor.targetVelocity = motorData.Speed;
                        hinge.motor = motor;

                        Debug.Log("Set speed to: " + motorData.Speed);

                        // Reset the receivedMessage variable after processing
                        receivedMessage = "";
                    }

                    await Task.Delay(10);
                }
            }
        }


    }
}
