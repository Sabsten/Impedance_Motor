using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CSharpMotionEx.Class;
using sFndCLIWrapper;
using ViewModel;

namespace Model
{
    class MotorManager
    {
        private cliIPort NodePort;
        private int SelectedPort;
        private List< Node> m_NodeList;
        public List<Node> NodeList { get { return m_NodeList; } }
        private cliINode[] myNodes;
        private cliSysMgr myMgr;
        private int m_NodeCount;
        private string m_ProgramName;
        public string ProgramName { get { return m_ProgramName; } }
        private List<Motor> m_motorList;
        public List<Motor> MotorList { get { return m_motorList; } }

        private List<Motor> m_PortList;
        public List<Motor> PortList { get { return m_PortList; } }
        private int Counter = 0;
        public void SetProgramName(string ProgramName)
        {
            m_ProgramName = ProgramName;
        }
        public MotorManager()
        {
            m_NodeCount = 4;
            OpenPorts();
            m_motorList = new List<Motor>();
            m_NodeList = new List<Node>();
            Constantes c = new Constantes();
            myMgr.PortsOpen(m_NodeCount);
            for (int i = 0; i < Counter; i++)
            {
                cliIPort myPort = myMgr.Ports(i);
                myNodes = new cliINode[myPort.NodeCount()];
                //Console.WriteLine("Port {" + myPort.NetNumber() + "}: state={" + myPort.OpenState() + "}, nodes={" + myPort.NodeCount() + "}");
                for (int n = 0; n < myNodes.Count(); n++)
                {
                    NodeStarndartTraiement(myPort.Nodes(n), c);
                    Node NodePack = new Node();
                    NodePack.LearnNode(myPort.Nodes(n), n);
                    m_NodeList.Add(NodePack);
                    Motor m = new Motor(NodePack.TorqueValue, NodePack.VelocityValue, NodePack.PositionValue, myMgr);
                    m.AddNode(NodePack);
                    m_motorList.Add(m);
                    while (!myPort.Nodes(n).Motion.IsReady())
                    {
                        if (myMgr.TimeStampMsec() > myMgr.TimeStampMsec() + c.TIME_TILL_TIMEOUT)
                        {
                               
                        }
                    }
                }
            }
        }

        void NodeStarndartTraiement(cliINode Node, Constantes c)
        {
            Node.EnableReq(false);
            Node.AccUnit(cliINode._accUnits.RPM_PER_SEC);         // Set the units for Acceleration to RPM/SEC
            Node.VelUnit(cliINode._velUnits.RPM);                 // Set the units for Velocity to RPM
            Node.TrqUnit(cliINode._trqUnits.AMPS);                // Set the units of torque to Amps
            Node.Motion.AccLimit.Value(c.ACC_LIM_RPM_PER_SEC);      // Set Acceleration Limit (RPM/Sec)
            Node.Motion.VelLimit.Value(c.VEL_LIM_RPM);              // Set Velocity Limit (RPM)
            Node.Status.AlertsClear();
            Node.Motion.NodeStopClear();
            Node.EnableReq(true);
        }

        void ErrorAndQuit(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(1);
        }

        private void OpenPorts()
        {
            List<String> comHubPorts = new List<String>();
            // Create the SysManager object. This object will coordinate actions among various ports
            // and within nodes. In this example we use this object to setup and open our port.
            myMgr = new cliSysMgr();
            // This will try to open the port. If there is an error/exception during the port opening,
            // the code will jump to the catch loop where detailed information regarding the error will be displayed;
            // otherwise the catch loop is skipped over
            myMgr.FindComHubPorts(comHubPorts);
            for (int i = 0; i < m_NodeCount; i++)
            {
                try
                {
                    myMgr.ComPortHub((uint)i, comHubPorts[i], cliSysMgr._netRates.MN_BAUD_12X);
                    Counter++;
                }
                catch (Exception e)
                {
                }
            }
        }

        public void ClosePorts()
        {   
            for (int n = 0; n < m_NodeCount; n++)
            {
                cliIPort myPort = myMgr.Ports(n);
                // This will dispose of the reference to the node. This frees up memory (similar to C++'s delete)
                // NOTE: All Teknic CLI classes implement the IDisposable pattern and should be properly disposed of when no longer in use.
                myNodes[n].EnableReq(false);
                myNodes[n].Dispose();
                myPort.Dispose();
            }
            // Dispose of the port reference because we're done with it now
            myMgr.PortsClose();
            myMgr.Dispose();
            Environment.Exit(0);
        }
    }
}