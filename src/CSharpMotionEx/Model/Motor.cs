using CSharpMotionEx.Class;
using sFndCLIWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public cliValueDouble ActualPosition { get { return m_positionValue; } }

        private int m_NodePort;
        public int NodePort { get { return m_NodePort; } }
        private bool m_IsLocked;
        public bool IsLocked { get { return m_IsLocked; } }

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
        }
        public void SetNodePort(int portNumber)
        {
            m_NodePort = portNumber;
        }

        public void Wait(int delay)
        {
            m_myMgr.Delay(10);
        }

        public void WaitUntilMoveDone(uint delayRefresh)
        {
            while (!m_Node.NodeObject.Motion.MoveIsDone())
            {
                m_myMgr.Delay(delayRefresh);
            }
        }

        public void SetVelocity(double velocityNumber)
        {
            m_Node.NodeObject.Motion.MoveVelStart(velocityNumber);
        }

        public void Lock(uint delay, cliNodeStopCodes stopType = cliNodeStopCodes.STOP_TYPE_ABRUPT)
        {
            m_Node.NodeObject.Motion.NodeStop(stopType);
            m_Node.NodeObject.EnableReq(false);
            m_myMgr.Delay(delay);
            m_Node.NodeObject.EnableReq(true);
            m_IsLocked = true;
        }

        public void Unlock()
        {
            m_Node.NodeObject.EnableReq(false);
            m_IsLocked = false;
        }

        public event Action MotorPositionRefreshed;
        public void RefreshInfo(int itteration)
        {
            m_torquesList.Clear();
            m_velocitiesList.Clear();
            m_positionsList.Clear();

            for (int j = 0; j < itteration; j++)
            {
                m_torqueValue.Refresh();
                m_velocityValue.Refresh();
                m_positionValue.Refresh();
                m_torquesList.Add(m_torqueValue.Value());
                m_velocitiesList.Add(m_velocityValue.Value());
                m_positionsList.Add(m_positionValue.Value());
            }

            // Trigger the event after the motor position is refreshed.
            MotorPositionRefreshed?.Invoke();

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
        public double AccelerationModel()
        {
            m_IsAccelerate = true;
            m_IsDecelerate = false;
            return m_constantes.ACCERATION_MODEL_VMAX / (1 + Math.Exp(m_constantes.ACCELERATION_MODEL_SLOPE * (m_constantes.ACCELERATION_MODEL_TORQUE_SENSITIVITY * TorqueAverage * VelocityAverage - m_constantes.ACCELERATION_MODEL_TOQUE_FLEXION_POINT)));
        }
        public void SetPositon(int target, bool absolute)
        {
            m_Node.NodeObject.Motion.MovePosnStart(target, absolute, false);
        }
        public double DecelerationModel()
        {
            m_IsDecelerate = true;
            m_IsAccelerate = false;
            double decel = -m_constantes.DECELERATION_MODEL_SLOPE * ((Math.Pow(VelocityAverage - m_constantes.DECELERATION_MODEL_SHIFT, 2)) / (m_constantes.DECELERATION_MODEL_WIDTH)) + m_constantes.DECELERATION_MODEL_VMAX - TorqueAverage * m_constantes.DECELERATION_MODEL_DAMPING_COEFFICIENT;
            return decel < 0 ? 0 : (decel > 80 ? 80 : decel);
        }
    }
}
