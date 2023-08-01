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


                double diff = 0;
                if (positionMoy < 0)
                {
                    diff = posObj - positionMoy;
                }
                else if (positionMoy > 0)
                {
                    diff = positionMoy + posObj;
                }
                if (diff < 0)
                {
                    diff = 0;
                }

                double estimation_vitesse_back = m.VelocityDependingOnPositionBackward(diff);

                double estimation_vitesse_forth = m.VelocityDependingOnPositionForward(Math.Abs(positionMoy));

                Console.WriteLine(torqueMoy);

                //Pour les cas ou on est proche de 0 et on applique pas de force
                if (-60 < posObj && posObj < 60 && Math.Abs(torqueMoy)<1)
                {
                    if (positionMoy > 30)
                    {
                        posDiff = -estimation_vitesse_back;
                    }
                    else if (positionMoy < -30)
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
                    if (Math.Abs(positionMoy) > 3500)
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
                    if (Math.Abs(positionMoy) > 3500)
                    {
                        posDiff = 0;
                    }
                    else
                    {
                        posDiff = -estimation_vitesse_forth;
                    }
                    Console.WriteLine("Vitesse estimée 2:" + estimation_vitesse_forth);
                }
                else
                {
                    posDiff = 0;
                }


                Console.WriteLine("Position à atteindre: " + posObj);
                Console.WriteLine("Position moy: " + positionMoy);
                Console.WriteLine("Force moyenne" + torqueMoy);
                Console.WriteLine("Déplacement" + posDiff);

                Console.WriteLine("///////////");

                m.SetVelocity(posDiff);
                m.Wait(100);
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
