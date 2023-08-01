using CSharpMotionEx.Class;
using MQTTnet.Client;
using MQTTnet.Server;
using sFndCLIWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ViewModel;
namespace Model
{
    class Motor
    {
        private List<double> m_torquesList;
        private List<double> m_velocitiesList;
        private List<double> m_positionsList;
        private Constantes m_constantes;
        public Constantes Constantes { get { return m_constantes; ; } }
        private cliValueDouble m_torqueValue;
        private cliValueDouble m_velocityValue;
        private cliValueDouble m_positionValue;

        private cliSysMgr m_myMgr;
        public double TorqueAverage { get { return m_torquesList.Average(); ; } }
        public double VelocityAverage { get { return m_velocitiesList.Average(); ; } }
        public double PositionAverage { get { return m_positionsList.Average(); } }

        private Node m_Node;
        private MQTT m_MQTT = null;

        private long m_now;
        private long m_now2;
        public cliValueDouble ActualPosition { get { return m_positionValue; } }

        private int m_NodePort;
        public int NodePort { get { return m_NodePort; } }
        public bool IsLocked { get { return m_Node.NodeObject.EnableReq(); } }

        private bool m_IsAccelerate;
        public bool IsAccelerate { get { return m_IsAccelerate; } }

        private bool m_IsDecelerate;
        public bool IsDecelerate { get { return m_IsDecelerate; } }

        public Motor(cliValueDouble torqueValue, cliValueDouble velocityValue, cliValueDouble positionValue, cliSysMgr MyMgr)
        {
            m_torquesList = new List<double>();
            m_velocitiesList = new List<double>();
            m_positionsList = new List<double>();
            m_torqueValue = torqueValue;
            m_velocityValue = velocityValue;
            m_positionValue = positionValue;
            m_constantes = new Constantes();
            m_myMgr = MyMgr;
            m_MQTT = new MQTT(this);
        }
        public void SetNodePort(int portNumber)
        {
            m_NodePort = portNumber;
        }

        public void Wait(int delay)
        {
            m_myMgr.Delay((uint)delay);
        }

        public void WaitUntilMoveDone(uint delayRefresh)
        {
            while (!m_Node.NodeObject.Motion.MoveIsDone())
            {
                m_myMgr.Delay(delayRefresh);
            }
        }

        public void Terminate()
        {
            Lock(cliNodeStopCodes.STOP_TYPE_DISABLE_RAMP);
            WaitUntilMoveDone(500);
            Console.ReadKey(true);
            ErrorAndQuit("End program");
        }

        public void ErrorAndQuit(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(1);
        }

        public bool MoveIsDone()
        {
            return m_Node.NodeObject.Motion.MoveIsDone();
        }

        public void ResetPositionToHome()
        {
            RefreshInfo(1);
            int gap = (int)PositionAverage;
            m_Node.NodeObject.Motion.AddToPosition(-gap);

        }

        public void SetVelocity(double velocityNumber)
        {
            m_Node.NodeObject.Motion.MoveVelStart(velocityNumber);
            Publish(velocityNumber, VelocityAverage);
        }

        public void SetPosition(int positionNumber)
        {
            m_Node.NodeObject.Motion.MovePosnStart(positionNumber,false,false) ;
        }

        public void Lock(cliNodeStopCodes stopType = cliNodeStopCodes.STOP_TYPE_ABRUPT)
        {
            m_Node.NodeObject.Motion.NodeStop(stopType);
            Disable();
        }
        public void TempStop(cliNodeStopCodes stopType = cliNodeStopCodes.STOP_TYPE_ABRUPT)
        {
            m_Node.NodeObject.Motion.NodeStop(stopType);
        }

        public void Unlock()
        {
            Enable();
        }

        public bool IsUnderthreshold(double threshold)
        {
            return threshold > Math.Abs(TorqueAverage);
        }

        public void Disable()
        {
            m_Node.NodeObject.EnableReq(false);
        }


        public void Enable()
        {
            m_Node.NodeObject.EnableReq(true);
        }

        public void Initialize()
        {
            Enable();
            ResetPositionToHome();

        }

        public void StopWait(cliNodeStopCodes stopType = cliNodeStopCodes.STOP_TYPE_ABRUPT)
        {
            m_Node.NodeObject.Motion.NodeStop(stopType);
            Publish(0, PositionAverage);
            Disable();
            Enable();
            
        }

        public void Stop( cliNodeStopCodes stopType = cliNodeStopCodes.STOP_TYPE_ABRUPT)
        {
            m_Node.NodeObject.Motion.NodeStop(stopType);
            Publish(0, PositionAverage);

        }

        public void RefreshInfo(int itteration = 10)
        {
            m_torquesList.Clear();
            m_velocitiesList.Clear();
            m_positionsList.Clear();
            m_now2 = DateTimeOffset.Now.ToUnixTimeSeconds();    
            Console.WriteLine(m_Node.NodeObject.Info.PositioningResolution.Value());
            for (int j = 0; j < itteration; j++)
            {
                m_torqueValue.Refresh();
                m_velocityValue.Refresh();
                m_positionValue.Refresh();
                m_torquesList.Add(m_torqueValue.Value());
                m_velocitiesList.Add(m_velocityValue.Value());
                m_positionsList.Add(m_positionValue.Value());
            }
        }

        public void Publish(double velocity, double position)
        {
            _ = m_MQTT.Publish(velocity, position);
        }

        public double Position
        {
            get { return m_positionValue.Value(); }
        }

        public void GoHome()
        {
            m_positionValue = m_Node.PositionValue;
        }

        public void ClearInfo()
        {
            m_torquesList.Clear();
            m_velocitiesList.Clear();
            m_positionsList.Clear();
        }

        public void AddNode(Node Node)
        {
            m_Node = Node;
        }


        public void SetPositon(int target, bool absolute)
        {
            m_Node.NodeObject.Motion.MovePosnStart(target, absolute, false);
        }

        //Modeles des mouvements

        public double AccelerationModel()
        {
            m_IsAccelerate = true;
            m_IsDecelerate = false;
            return m_constantes.ACCERATION_MODEL_VMAX / (1 + Math.Exp(m_constantes.ACCELERATION_MODEL_SLOPE * (m_constantes.ACCELERATION_MODEL_TORQUE_SENSITIVITY * TorqueAverage * VelocityAverage - m_constantes.ACCELERATION_MODEL_TOQUE_FLEXION_POINT)));
        }

        public double DecelerationModel()
        {
            m_IsDecelerate = true;
            m_IsAccelerate = false;
            double decel = -m_constantes.DECELERATION_MODEL_SLOPE * ((Math.Pow(VelocityAverage - m_constantes.DECELERATION_MODEL_SHIFT, 2)) / (m_constantes.DECELERATION_MODEL_WIDTH)) + m_constantes.DECELERATION_MODEL_VMAX - TorqueAverage * m_constantes.DECELERATION_MODEL_DAMPING_COEFFICIENT;
            return decel < 0 ? 0 : (decel > 80 ? 80 : decel);
        }


        public double PositionDependingOnTorque(double torqueMoy)
        {
            double position = 1000 / (1 + Math.Exp(-0.1 * (torqueMoy * 2 - 30)));
            return (position);
        }

        public double VelocityDependingOnPositionForward(double diff)
        {
            double velocity = 50 - (50 / (1 + Math.Exp(-0.03 * (diff * 0.08 - 200))));
            return (velocity);
        }
        public double VelocityDependingOnPositionBackward(double diff)
        {
            double velocity = (50 / (1 + Math.Exp(-0.1 * (diff * 0.03 - 30))));
            return (velocity);
        }



    }
}
