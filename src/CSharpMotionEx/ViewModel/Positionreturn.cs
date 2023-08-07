using System;
using sFndCLIWrapper;
using Model;

namespace ViewModel
{
    class PositionReturn
    {
        public PositionReturn(Motor m)
        {
            m.Disable();
            Console.WriteLine("Please set the initial position of the motor. Then press any key to begin.");
            Console.ReadKey();
            m.ResetPositionToHome();
            m.Enable();

            double torqueMoy = 0;
            double velocityMoy = 0;
            double positionMoy = 0;
            double posObj = 0;
            double posDiff = 0;
            while (true)
            {
                m.RefreshInfo(10);
                torqueMoy = m.TorqueAverage;
                velocityMoy = m.VelocityAverage;
                positionMoy = m.PositionAverage;
                //myNodes[n].Motion.MovePosnStart(0, false, false);
                posObj = m.PositionDependingOnTorque(Math.Abs(torqueMoy));



                double estimation_vitesse_back = m.VelocityDependingOnPositionBackward(Math.Abs(positionMoy));


                double estimation_vitesse_forth = m.VelocityDependingOnPositionForward(Math.Abs(positionMoy));

                Console.WriteLine(torqueMoy);

                //Pour les cas ou on est proche de 0 et on applique pas de force
                if ((positionMoy * m.TorqueAverage > 0 && Math.Abs(positionMoy) > 500) || Math.Abs(m.TorqueAverage) < 1)
                {
                    posDiff = 0;
                    m.RefreshInfo(5);

                    if (Math.Abs(m.TorqueAverage) > 1)
                    {
                        posDiff = 0;
                    }
                    else if (Math.Abs(positionMoy) < 300){
                        m.Stop();
                    }
                    else if (positionMoy > 300)
                        {
                            posDiff = -estimation_vitesse_back;
                        }
                        else if (positionMoy < -300)
                        {
                            posDiff = estimation_vitesse_back;
                        }
                        else
                        {
                            posDiff = 0;
                    }
                    

                }
                //Va position positive
                else if (posObj > 60 && torqueMoy < -1)
                {
                    //Pour avancer
                    //Stop si trop loin
                    if (Math.Abs(positionMoy) > 1700)
                    {
                        posDiff = 0;
                    }
                    else
                    {
                        posDiff = estimation_vitesse_forth;
                    }
                }
                //Va position négative
                else if (posObj > 60 && torqueMoy > 1)
                {
                    //Pour avancer
                    //Stop si trop loin
                    if (Math.Abs(positionMoy) > 1700)
                    {
                        posDiff = 0;
                    }
                    else
                    {
                        posDiff = -estimation_vitesse_forth;
                    }
                    Console.WriteLine("Vitesse estimée 2:" + positionMoy);
                }
                else
                {
                    posDiff = 0;
                }

                Console.WriteLine("Déplacement" + torqueMoy);

                m.SetVelocity(posDiff);
                m.Wait(50);
            }
        }

        public static void ErrorAndQuit(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
