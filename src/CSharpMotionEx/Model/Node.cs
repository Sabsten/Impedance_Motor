﻿using sFndCLIWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
namespace CSharpMotionEx.Class
{
    class Node
    {
        public string m_Binding;
        public string Binding { get { return m_Binding; } }
        public string m_NodeType;
        public string NodeType { get { return m_NodeType; } }

        public string m_UserID;
        public string UserID { get { return m_UserID; } }
        public string m_FirmwareVersion;
        public string FirmwareVersion { get { return m_FirmwareVersion; } }
        public string m_SerialNumber;
        public string SerialNumber { get { return m_SerialNumber; } }
        public string m_Model;
        public string Model { get { return m_Model; } }
        private cliINode m_NodeObject;
        public cliINode NodeObject { get { return m_NodeObject; } }
        private cliValueDouble m_torqueValue;
        public cliValueDouble TorqueValue { get { return m_torqueValue; } }
        private cliValueDouble m_velocityValue;
        public cliValueDouble VelocityValue { get { return m_velocityValue; } }
        private cliValueDouble m_positionValue;
        public cliValueDouble PositionValue { get { return m_positionValue; } }

        public void LearnNode(cliINode NodeObject, int n)
        {
            if (NodeObject != null)
            {
                m_NodeType = NodeObject.Info.NodeType().ToString();
                m_UserID = NodeObject.Info.UserID.Value();
                m_Model = NodeObject.Info.Model.Value();
                m_SerialNumber = NodeObject.Info.SerialNumber.Value().ToString();
                m_torqueValue = NodeObject.Motion.TrqMeasured;
                m_velocityValue = NodeObject.Motion.VelMeasured;
                m_positionValue = NodeObject.Motion.PosnMeasured;
                Console.WriteLine(m_positionValue.Value());
                Console.ReadKey();
                m_NodeObject = NodeObject;
            }
        }

        Dictionary<string, string> NodeToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "Bind", m_Binding },
                { "NodeType", m_NodeType },
                { "UserID", m_UserID },
                { "FirmwareVersion", m_FirmwareVersion },
                { "SerialNumber", m_SerialNumber },
                { "Model", m_Model },
            };
        }
    }
}
