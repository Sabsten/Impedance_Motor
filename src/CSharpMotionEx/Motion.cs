using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using sFndCLIWrapper;

namespace CSharpMotionEx
{
    class Motion
    {
        //*********************************************************************************
        //This program will load configuration files onto each node connected to the port, then executes
        //sequential repeated moves on each axis.
        //*********************************************************************************

        const int ACC_LIM_RPM_PER_SEC = 500;
        const int VEL_LIM_RPM = 25;
        const int MOVE_DISTANCE_CNTS = 50000;
        const int NUM_MOVES = 1;
        const int TIME_TILL_TIMEOUT = 10000;//The timeout used for homing(ms)

        const double ACCELERATION_MODEL_VMAX = 80;
        const double ACCELERATION_MODEL_SLOPE = 0.01;
        const double ACCELERATION_MODEL_TORQUE_SENSITIVITY = 3;
        const double ACCELERATION_MODEL_TOQUE_FLEXION_POINT = 200;
        /*const double ACCELERATION_MODEL_VMAX = 80;
        const double ACCELERATION_MODEL_SLOPE = 0.002;
        const double ACCELERATION_MODEL_TORQUE_SENSITIVITY = 30;
        const double ACCELERATION_MODEL_TOQUE_FLEXION_POINT = 1500;*/

        /*const double DECELERATION_MODEL_VMAX = 40;
        const double DECELERATION_MODEL_WIDTH = 75;
        const double DECELERATION_MODEL_SHIFT = 45;
        const double DECELERATION_MODEL_SLOPE = 2;
        const double DECELERATION_MODEL_DAMPING_COEFFICIENT = 2.5;*/
        const double DECELERATION_MODEL_VMAX = 40;
        const double DECELERATION_MODEL_WIDTH = 100;
        const double DECELERATION_MODEL_SHIFT = 45;
        const double DECELERATION_MODEL_SLOPE = 2;
        const double DECELERATION_MODEL_DAMPING_COEFFICIENT = 2;


        static void ExitProgram(int errCode)
        {
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
            Environment.Exit(errCode);
        }

        //Write the data collected into an excel file
        static void WriteFile(ref List<double> torques, ref List<double> velocities, ref List<double> positions)
        {
            string basePath = "C:/Users/djila/OneDrive/Documents/test/";
            string fileName = "dataMotor_"+(int)Math.Round(velocities[0]);
            string ext = ".csv";
            int counter = 0;

            string filePath = basePath + fileName + ext;
            Console.WriteLine(torques.Count);
            Console.WriteLine(velocities.Count);
            Console.WriteLine(positions.Count);

            while (File.Exists(filePath))
            {
                counter++;
                filePath = basePath + fileName + "(" + counter.ToString() + ")" + ext;
            }

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("numValeur;torque;velocity;position");
                for (int i = 0; i < torques.Count; i++)
                {
                    sw.WriteLine("{0};{1};{2};{3}", i, torques[i], velocities[i], positions[i]);
                }
            }
            Console.WriteLine("Les valeurs ont été sauvegardées dans le fichier random.csv");
            ClearInfo(ref torques, ref velocities, ref positions);
        }

        static void RefreshInfo(ref cliValueDouble torqueValue, ref cliValueDouble velocityValue, ref cliValueDouble positionValue, ref List<double> torques, ref List<double> velocities, ref List<double> positions)
        {
            //Refresh the data
            torqueValue.Refresh();
            velocityValue.Refresh();
            positionValue.Refresh();
            //Add to data list
            torques.Add(torqueValue.Value());
            velocities.Add(velocityValue.Value());
            positions.Add(positionValue.Value());
        }

        static void ClearInfo(ref List<double> torques, ref List<double> velocities, ref List<double> positions)
        {
            torques.Clear();
            velocities.Clear();
            positions.Clear();
        }

        static double AccelerationModel(double torqueMoy, double velocityMoy)
        {
            double velocityOutput = ACCELERATION_MODEL_VMAX / (1 + Math.Exp(-ACCELERATION_MODEL_SLOPE * (ACCELERATION_MODEL_TORQUE_SENSITIVITY*torqueMoy * velocityMoy - ACCELERATION_MODEL_TOQUE_FLEXION_POINT)));
            Console.WriteLine($"{velocityOutput}");
            return velocityOutput;
        }

        static double DecelerationModel(double torqueMoy, double velocityMoy)
        {
            //double velocityOutput = -DECELERATION_MODEL_SLOPE*(Math.Pow(velocityMoy-DECELERATION_MODEL_SHIFT,2))/(DECELERATION_MODEL_WIDTH+Math.Pow(torqueMoy,2))+DECELERATION_MODEL_VMAX-torqueMoy* DECELERATION_MODEL_DAMPING_COEFFICIENT;
            double velocityOutput = -DECELERATION_MODEL_SLOPE * ((Math.Pow(velocityMoy - DECELERATION_MODEL_SHIFT, 2)) / (DECELERATION_MODEL_WIDTH)) + DECELERATION_MODEL_VMAX - torqueMoy * DECELERATION_MODEL_DAMPING_COEFFICIENT;
            Console.WriteLine($"{velocityOutput}");
            return velocityOutput;
        }


        static double PositionDependingOnTorque(double torqueMoy)
        {
            double position = 200 / (1 + Math.Exp(-0.1 * (torqueMoy * 4 - 30)));
            return (position);
        }


        static void Main(string[] args)
        {

            Console.WriteLine("Motion Example starting. Press Enter to continue.");
            Console.ReadLine();

            // Create the SysManager object. This object will coordinate actions among various ports
            // and within nodes. In this example we use this object to setup and open our port.
            cliSysMgr myMgr = new cliSysMgr();
            List<String> comHubPorts = new List<String>();

            // This will try to open the port. If there is an error/exception during the port opening,
            // the code will jump to the catch loop where detailed information regarding the error will be displayed;
            // otherwise the catch loop is skipped over
            myMgr.FindComHubPorts(comHubPorts);
            int portCount = comHubPorts.Count;

            Console.WriteLine("Found {0} SC Hubs.", comHubPorts.Count);
           
            for (int i = 0; i < portCount && portCount < 3; i++)
            {
                myMgr.ComPortHub((uint)i, comHubPorts[i], cliSysMgr._netRates.MN_BAUD_12X);
            }

            if (portCount < 1)
            {
                Console.WriteLine("Unable to locate SC hub port.");
                ExitProgram(-1);
            }

            myMgr.PortsOpen(portCount);
            for (int i = 0; i < portCount; i++)
            {
                // Create a reference to our current port and an array of references to the port's nodes
                cliIPort myPort = myMgr.Ports(i);
                cliINode[] myNodes = new cliINode[myPort.NodeCount()];
                Console.WriteLine("Port {0}: state={1}, nodes={2}", myPort.NetNumber(), myPort.OpenState(), myPort.NodeCount());

                // Once the code gets past this point, it can be assumed that the Port has been opened without issue
                // Now we can get a reference to our port object which we will use to access the node objects
                for (int n = 0; n < myPort.NodeCount(); n++)
                {
                    // Populate the node array
                    myNodes[n] = myPort.Nodes(n);
                    myNodes[n].EnableReq(false);       // Ensure Node is disabled
                    myMgr.Delay(200);

                    Console.WriteLine("   Node[{0}]: type={1}", n, myNodes[n].Info.NodeType());
                    Console.WriteLine("            userID: {0}", myNodes[n].Info.UserID);
                    Console.WriteLine("        FW version: {0}", myNodes[n].Info.FirmwareVersion.Value());
                    Console.WriteLine("          Serial #: {0}", myNodes[n].Info.SerialNumber.Value());
                    Console.WriteLine("             Model: {0}", myNodes[n].Info.Model.Value());

                    // The following statements will attempt to enable the node.  First,
                    // any shutdowns or NodeStops are cleared, finally the node is enabled
                    myNodes[n].Status.AlertsClear();
                    myNodes[n].Motion.NodeStopClear();
                    myNodes[n].EnableReq(true);
                    Console.WriteLine("Node {0} enabled.", n);
                    double timeout = myMgr.TimeStampMsec() + TIME_TILL_TIMEOUT;     // Define a timeout in case the node is unable to enable
                                                                                    // This will loop checking on the Real time values of the node's Ready status

                    while (!myNodes[n].Motion.IsReady())
                    {
                        if (myMgr.TimeStampMsec() > timeout)
                        {
                            Console.WriteLine("Error: Timed out waiting for Node {0} to enable.", n);
                            Console.ReadLine();
                            Environment.Exit(1);
                        }
                    }
                    

                    // At this point the Node is enabled, and we will now check to see if the Node has been homed
                    // Check the Node to see if it has already been homed

                    /*if (myNodes[n].Motion.Homing.HomingValid())
                    {
                        if (myNodes[n].Motion.Homing.WasHomed())
                        {
                            Console.WriteLine("Node {0} has already been homed, current position is: {1} ", n, myNodes[n].Motion.PosnMeasured.Value());
                            Console.WriteLine("Rehoming Node... ");
                        }
                        else
                        {
                            Console.WriteLine("Node [{0}] has not been homed.  Homing Node now...", n);
                        }
                        // Now we will home the Node
                        myNodes[n].Motion.Homing.Initiate();

                        timeout = myMgr.TimeStampMsec() + TIME_TILL_TIMEOUT;    // Define a timeout in case the node is unable to enable
                                                                                // Basic mode - Poll until disabled
                        while (!myNodes[n].Motion.Homing.WasHomed())
                        {
                            if (myMgr.TimeStampMsec() > timeout)
                            {
                                Console.WriteLine("fds [{0}]", myNodes[n].Status) ;
                                Console.WriteLine("Node did not complete homing:  \n\t -Ensure Homing settings have been defined through ClearView. \n\t -Check for alerts/Shutdowns \n\t -Ensure timeout is longer than the longest possible homing move.\n");
                                ExitProgram(-1);
                            }
                        }
                        Console.WriteLine("Node completed homing");
                    }
                    else
                    {
                        Console.WriteLine("Node[{0}] has not had homing setup through ClearView.  The node will not be homed.", n);
                    }*/
                }
                
                //////////////////////////////////////////////////////////////////////////////////////
                //At this point we will execute the code we want
                //////////////////////////////////////////////////////////////////////////////////////
                for (int j = 0; j < NUM_MOVES; j++)
                {
                    for (int n = 0; n < myPort.NodeCount(); n++)
                    {
                        myNodes[n].AccUnit(cliINode._accUnits.RPM_PER_SEC);         // Set the units for Acceleration to RPM/SEC
                        myNodes[n].VelUnit(cliINode._velUnits.RPM);                 // Set the units for Velocity to RPM
                        myNodes[n].TrqUnit(cliINode._trqUnits.AMPS);                // Set the units of torque to Amps
                        myNodes[n].Motion.AccLimit.Value(ACC_LIM_RPM_PER_SEC);      // Set Acceleration Limit (RPM/Sec)
                        myNodes[n].Motion.VelLimit.Value(VEL_LIM_RPM);              // Set Velocity Limit (RPM)
                        Console.WriteLine("Moving Node {0}", n);

                        //Initiate variables used to get motor infos

                        cliValueDouble torqueValue = myNodes[n].Motion.TrqMeasured;
                        cliValueDouble velocityValue = myNodes[n].Motion.VelMeasured;
                        cliValueDouble positionValue = myNodes[n].Motion.PosnMeasured;

                        List<double> torqueList = new List<double>();
                        List<double> velocityList = new List<double>();
                        List<double> positionList = new List<double>();



                        //Make a mode choice
                        Console.WriteLine("1- Mode 1");
                        Console.WriteLine("2- Mode 2");
                        Console.WriteLine("3- Mode 3");
                        switch (Console.ReadLine())
                        {
                            case "1":
                                Console.WriteLine("");
                                ExitProgram(1);
                                break;
                            case "2":
                                Console.WriteLine("");
                                break;
                            default:
                                Console.WriteLine("");
                                break;
                        }

                        //Programme propotionnel à la torque
                        Console.WriteLine("Programme proportionnel");
                        while (!Console.KeyAvailable)
                        {
                            for (int loop = 0; loop < 10; loop++)
                            {
                                RefreshInfo(ref torqueValue, ref velocityValue, ref positionValue, ref torqueList, ref velocityList, ref positionList);
                            }
                            double torqueMoy = torqueList.Average();
                            double velocityMoy = velocityList.Average();
                            double positionMoy = positionList.Average();
                            //Vérifier pour une torque dans le sens de rotation
                            //Dans le cas ou le moteur ne bouge pas
                            Console.WriteLine("Velocity moy:" + velocityMoy);
                            Console.WriteLine("Torque moy:" + torqueMoy);
                            if (velocityMoy < 10 && velocityMoy > -10 && myNodes[n].Motion.MoveIsDone())
                            {
                                myNodes[n].EnableReq(true);
                                //Démarrage avec assistance
                                /*double estimatedVelocity = AccelerationModel(Math.Abs(torqueMoy), Math.Abs(velocityMoy));
                                if (torqueMoy < -0.5)
                                {
                                    myNodes[n].Motion.MoveVelStart(estimatedVelocity);
                                }
                                else if (torqueMoy > 0.5)
                                {
                                    myNodes[n].Motion.MoveVelStart(-estimatedVelocity);
                                }*/

                                //myNodes[n].Motion.MovePosnStart(5000, false, false);
                                while (!myNodes[n].Motion.MoveIsDone())
                                {
                                    myMgr.Delay(500);
                                    Console.WriteLine("Attente fini");
                                }
                                double posDiff = 0;
                                double posObj = 0;
                                double lastPosDiff = 0;
                                while (true)
                                {
                                    lastPosDiff = posDiff;
                                    ClearInfo(ref torqueList, ref velocityList, ref positionList);
                                    for (int loop = 0; loop < 10; loop++)
                                    {
                                        RefreshInfo(ref torqueValue, ref velocityValue, ref positionValue, ref torqueList, ref velocityList, ref positionList);
                                    }
                                    torqueMoy = torqueList.Average();
                                    velocityMoy = velocityList.Average();
                                    positionMoy = positionList.Average();
                                    //myNodes[n].Motion.MovePosnStart(0, false, false);
                                    posObj = PositionDependingOnTorque(Math.Abs(torqueMoy));
                                    Console.WriteLine("Position à atteindre"+posObj);
                                    Console.WriteLine("Force moyenne"+torqueMoy);


                                    if (torqueMoy < 0)
                                    {
                                        posDiff = Math.Abs(positionMoy - posObj);
                                    }
                                    else
                                    {
                                        posDiff = -Math.Abs(positionMoy - posObj);
                                    }
                                    Console.WriteLine(posDiff);
                                    if (Math.Abs(Math.Abs(posDiff)-Math.Abs(lastPosDiff)) > 20)
                                    {
                                        myNodes[n].Motion.MovePosnStart((int)posDiff, false, false);
                                        while (!myNodes[n].Motion.MoveIsDone())
                                        {
                                            myMgr.Delay(500);
                                            Console.WriteLine("Attente fini");
                                        }
                                    }

                                    Console.WriteLine("Diff" + Math.Abs(Math.Abs(posDiff) - Math.Abs(lastPosDiff)));
                                    Console.WriteLine("Diff" + posDiff);
                                    myMgr.Delay(1000);

                                }
                                myNodes[n].EnableReq(false);



                                //Démarrage sans assistance
                                double lastvelmoy = velocityMoy;
                                double lasttorquemoy = torqueMoy;
                                double maxvalue = 0;
                                bool launch = false;
                                //Tant que on a pas dépassé la valeur maximal ou que on a indiqué une même direction pendant 30 tours
                                while (maxvalue < 80 && launch == false)
                                {
                                    //Enreigstre les derniers relevés
                                    lastvelmoy = velocityMoy;
                                    lasttorquemoy = torqueMoy;
                                    //rafraichis les valeurs
                                    ClearInfo(ref torqueList, ref velocityList, ref positionList);
                                    for (int loop = 0; loop < 10; loop++)
                                    {
                                        RefreshInfo(ref torqueValue, ref velocityValue, ref positionValue, ref torqueList, ref velocityList, ref positionList);
                                    }
                                    torqueMoy = torqueList.Average();
                                    velocityMoy = velocityList.Average();

                                    //Dans le sens opposé
                                        if (((torqueMoy > 0.2 && lasttorquemoy < 0) || (torqueMoy < -0.2 && lasttorquemoy > 0) || (velocityMoy >0 && lastvelmoy <0) || (velocityMoy < 0 && lastvelmoy > 0)) && Math.Abs(maxvalue) > 10)
                                    {
                                        launch = true;
                                    }
                                    //Si on a un faible couple (ralentissement ou relachement)
                                    else if (Math.Abs(torqueMoy) < 0.5 && Math.Abs(maxvalue) > 10)
                                    {

                                        Console.WriteLine("Torque moy:" + torqueMoy);
                                        Console.WriteLine("Vel moy:" + velocityMoy);

                                        Console.WriteLine("Vel moy:" + maxvalue);
                                        //Vérifie si changement de sens ou si on a laché le bras, si on l'a laché le moteur prends le relai
                                        //Dans le même sens et on a déjà dépassé les 30 (pour pas valider juste au démarrage)
                                        if (((torqueMoy > 0 && lasttorquemoy > 0) || (torqueMoy < 0 && lasttorquemoy < 0)) && Math.Abs(maxvalue) > 5)
                                        {
                                            launch = true;
                                        }
                                        
                                    }

                                    //On prends la valeur la plus élevée
                                    if (Math.Abs(maxvalue) < Math.Abs(velocityMoy))
                                    {
                                        if (Math.Abs(velocityMoy) > 80)
                                        {
                                            maxvalue = 80;
                                        }
                                        else
                                        {
                                            maxvalue = Math.Abs(velocityMoy);
                                        }
                                    }

                                    
                                }
                                if(lastvelmoy < 0)
                                {
                                    torqueMoy = -maxvalue;
                                }
                                else
                                {
                                    torqueMoy = maxvalue;
                                }
                                myNodes[n].EnableReq(true);
                                myNodes[n].Motion.MoveVelStart(torqueMoy);
                            }
                            //Libérer le moteur pour des torque plus élevés
                            /*else if (Math.Abs(torqueMoy) > 10)
                            {
                                myNodes[n].EnableReq(false);
                                while (Math.Abs(torqueMoy) > 10 || Math.Abs(velocityMoy) > 80)
                                {
                                    ClearInfo(ref torqueList, ref velocityList, ref positionList);
                                    for (int loop = 0; loop < 10; loop++)
                                    {
                                        RefreshInfo(ref torqueValue, ref velocityValue, ref positionValue, ref torqueList, ref velocityList, ref positionList);
                                    }
                                    torqueMoy = torqueList.Average();
                                    velocityMoy = velocityList.Average();
                                    Console.WriteLine("Torque moy:" + torqueMoy);
                                    myMgr.Delay(100);
                                }
                                myNodes[n].EnableReq(true);
                                myNodes[n].Motion.MoveVelStart(velocityMoy);
                            }*/

                            //Le moteur est en fonctionnement et on exerce une force dans le sens de rotation
                            else if (torqueMoy > 0 && velocityMoy < 0 || torqueMoy < 0 && velocityMoy > 0)
                            {
                                if(Math.Abs(torqueMoy) > 20)
                                {
                                    myNodes[n].EnableReq(false);
                                    while (Math.Abs(velocityMoy)>10)
                                    {
                                        ClearInfo(ref torqueList, ref velocityList, ref positionList);
                                        for (int loop = 0; loop < 10; loop++)
                                        {
                                            RefreshInfo(ref torqueValue, ref velocityValue, ref positionValue, ref torqueList, ref velocityList, ref positionList);
                                        }
                                        torqueMoy = torqueList.Average();
                                        velocityMoy = velocityList.Average();
                                    }
                                    myNodes[n].EnableReq(true);
                                }
                                else if (Math.Abs(torqueMoy) > 1.3)
                                {
                                    double estimatedVelocity = AccelerationModel(Math.Abs(torqueMoy), Math.Abs(velocityMoy));
                                    Console.WriteLine("Extimated volocity:" + estimatedVelocity);
                                    if (Math.Abs(velocityMoy - estimatedVelocity) > 2 && Math.Abs(estimatedVelocity) > Math.Abs(velocityMoy))
                                    {
                                        if (torqueMoy < -1.3)
                                        {
                                            myNodes[n].Motion.MoveVelStart(estimatedVelocity);
                                        }
                                        else if (torqueMoy > 1.3)
                                        {
                                            myNodes[n].Motion.MoveVelStart(-estimatedVelocity);
                                        }
                                    }
                                }
                            }
                            //Le moteur est en fonctionnement et on exerce une force dans le sens contraire de rotation
                            else if (torqueMoy > 0 && velocityMoy > 0 || torqueMoy < 0 && velocityMoy < 0)
                            {
                                if (Math.Abs(torqueMoy) > 1.3)
                                {
                                    //Arrêt focé si torque trop élevé
                                    if (Math.Abs(torqueMoy) > 7)
                                    {
                                        myNodes[n].Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
                                        myNodes[n].EnableReq(false);
                                        myNodes[n].EnableReq(true);
                                    }

                                        //Arrêt si vitesse faible
                                        else if (velocityMoy < 10 || torqueMoy > 6)
                                    {
                                        myNodes[n].Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
                                        myMgr.Delay(0);
                                    }
                                    //Fonction uniquement à exécuter si la vitesse n'est pas proche de 0, estimation ralentissement
                                    else
                                    {
                                        double estimatedVelocity = DecelerationModel(Math.Abs(torqueMoy), Math.Abs(velocityMoy));
                                        Console.WriteLine("Freinage!!!");
                                        Console.WriteLine("Extimated volocity:" + estimatedVelocity);
                                        if (torqueMoy < -1.3)
                                        {
                                            myNodes[n].Motion.MoveVelStart(-estimatedVelocity);
                                        }
                                        else if (torqueMoy > 1.3)
                                        {
                                            myNodes[n].Motion.MoveVelStart(estimatedVelocity);
                                        }
                                    }
                                }
                            }
                            myMgr.Delay(100);
                            ClearInfo(ref torqueList, ref velocityList, ref positionList);
                        }
                        myNodes[n].Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_RAMP);
                        while (!myNodes[n].Motion.MoveIsDone())
                        {
                            myMgr.Delay(5000);
                        }
                        myNodes[n].EnableReq(false);
                        Console.ReadKey(true);
                        ExitProgram(1);





                        //Allows to run the motor in one direction and get the data in excel
                        Console.WriteLine("Programme évaluation");
                        myNodes[n].Motion.MoveVelStart(15);
                        myMgr.Delay(500);
                        while (!Console.KeyAvailable)
                        {
                            RefreshInfo(ref torqueValue,ref velocityValue,ref positionValue,ref torqueList,ref velocityList,ref positionList);
                            Console.WriteLine("Couple mesuré: {0}", torqueValue.Value());
                            velocityList.Add(velocityValue.Value());
                            torqueList.Add(torqueValue.Value());
                            positionList.Add(positionValue.Value());

                        }
                        Console.ReadKey(true);
                        WriteFile(ref torqueList, ref velocityList, ref positionList);
                        myNodes[n].EnableReq(false);
                        ClearInfo(ref torqueList, ref velocityList, ref positionList);
                       




                            //Allows to run the motor in one direction, and change direction when the tork is high
                            Console.WriteLine("Programme inversion sens rotation");
                        while (!Console.KeyAvailable)
                        {
                            torqueValue.Refresh();
                            Console.WriteLine("Couple mesuré: {0}", torqueValue.Value());
                            velocityValue.Refresh();
                            
                            if (Math.Abs(torqueValue.Value()) > 1.5)
                            {
                                myNodes[n].Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_RAMP);
                                myNodes[n].EnableReq(false);
                                myMgr.Delay(5000);
                                myNodes[n].EnableReq(true);
                                if (torqueValue.Value() < 0)
                                {
                                    myNodes[n].Motion.MoveVelStart(VEL_LIM_RPM-25);
                                }
                                else
                                {
                                    myNodes[n].Motion.MoveVelStart(-VEL_LIM_RPM+25);
                                }
                            }
                            myMgr.Delay(500);
                        }
                        Console.ReadKey(true);


                        //Allows to run the motor in one direction, stop and start again depending of the torque applied
                        Console.WriteLine("Programme démarrage manuel");
                        while (!Console.KeyAvailable)
                        {
                            torqueValue.Refresh();
                            Console.WriteLine("Couple mesuré: {0}", torqueValue.Value());
                            velocityValue.Refresh();
                            Console.WriteLine("Vitesse mesurée: {0}", velocityValue.Value());
                            positionValue.Refresh();

                            Console.WriteLine("Move is done: {0}", myNodes[n].Motion.MoveIsDone());
                            //Cas de marche
                            if (Math.Abs(velocityValue.Value()) > 40)
                            {
                                if (Math.Abs(torqueValue.Value()) > 1.5)
                                {
                                    myNodes[n].Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_RAMP);
                                    while (myNodes[n].Motion.MoveIsDone() == false)
                                    {
                                        myMgr.Delay(500);
                                    }
                                    myNodes[n].EnableReq(false);
                                }
                            }
                            //Cas d'arrêt
                            else
                            {
                                if (myNodes[n].Motion.MoveIsDone() && Math.Abs(velocityValue.Value()) > 20 )
                                {
                                    if (velocityValue.Value() < 0)
                                    {
                                        Console.WriteLine("!!!!!!!!!!!!");
                                        myNodes[n].EnableReq(true);
                                        myNodes[n].Motion.MoveVelStart(VEL_LIM_RPM);
                                    }
                                    else
                                    {
                                        Console.WriteLine("!!!!!!!!!!!!");
                                        myNodes[n].EnableReq(true);
                                        myNodes[n].Motion.MoveVelStart(-VEL_LIM_RPM);
                                    }
                                }
                            }

                            //Met à jour les variables
                            torqueList.Add(torqueValue.Value());
                            velocityList.Add(velocityValue.Value());
                            positionList.Add(torqueValue.Value());
                            positionList.Add(positionValue.Value());
                            myMgr.Delay(250);
                        }
                        Console.ReadKey(true);

                        WriteFile(ref torqueList, ref velocityList, ref positionList);
                        myNodes[n].EnableReq(true);
                        Console.WriteLine("Fin programme");




                        //Se déplacer vers la position 0 (intervalle 40)
                        Console.WriteLine("Début programme position");
                        while (!Console.KeyAvailable)
                        {
                            positionValue.Refresh();
                            if (positionValue.Value() > 20)
                            {
                                myNodes[n].Motion.MoveVelStart(-VEL_LIM_RPM);
                            }
                            else if (positionValue.Value() < 20)
                            {
                                myNodes[n].Motion.MoveVelStart(VEL_LIM_RPM);
                            }
                            Console.WriteLine(positionValue.Value());
                            myMgr.Delay(10);
                        }
                        Console.ReadKey(true);
                        Console.WriteLine("Fin programme");



                        //myNodes[n].Motion.MoveWentDone();                           // Clear the rising edge Move done register
                        //myNodes[n].Motion.MovePosnStart(50000, false, false);
                        //myNodes[n].Motion.MoveVelStart(0);

                        //Même stop que dans le logiciel
                        //myNodes[n].Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_RAMP);
                        //Stop d'urgence
                        //myNodes[n].Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_ABRUPT);

                        Console.WriteLine("Node {0} Move Done", n);
                    }
                }

                //////////////////////////////////////////////////////////////////////////////////////////////
                //After moves have completed Disable node, and close ports
                //////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine("Disabling nodes, and closing port");
                for (int n = 0; n < myPort.NodeCount(); n++)
                {
                    myNodes[n].EnableReq(false);
                    // This will dispose of the reference to the node. This frees up memory (similar to C++'s delete)
                    // NOTE: All Teknic CLI classes implement the IDisposable pattern and should be properly disposed of when no longer in use.
                    myNodes[n].Dispose();
                }
                myPort.Dispose();                   // Dispose of the port reference because we're done with it now
                myMgr.PortsClose();
            }
            myMgr.Dispose();
            ExitProgram(1);
        }
    }
}
