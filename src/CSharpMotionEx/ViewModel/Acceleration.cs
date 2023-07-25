using System;
using sFndCLIWrapper;
using Model;
using System.Threading;

namespace ViewModel
{
    class Acceleration
    {
        public Acceleration(Motor m)
        {

            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(15);
                Console.WriteLine("Velocity moy:" + m.VelocityAverage);
                Console.WriteLine("Torque moy:" + m.TorqueAverage);
                //Vérifier pour une torque dans le sens de rotation
                //Dans le cas ou le moteur ne bouge pas
                if (m.VelocityAverage < 10 && m.VelocityAverage > -10 && m.MoveIsDone())
                {
                    m.Disabled();
                    //Démarrage sans assistance
                    double lastvelmoy = m.VelocityAverage;
                    double lasttorquemoy = m.TorqueAverage;
                    double maxvalue = 0;
                    bool launch = false;
                    //Tant que on a pas dépassé la valeur maximal ou que on a indiqué une même direction pendant 30 tours



                    while (maxvalue < 80 && launch == false)
                    {
                        //Enreigstre les derniers relevés
                        lastvelmoy = m.VelocityAverage;
                        lasttorquemoy = m.TorqueAverage;
                        //rafraichis les valeurs
                        m.RefreshInfo(15);

                        //Dans le sens opposé
                        if (((m.TorqueAverage > 0.2 && lasttorquemoy < 0) || (m.TorqueAverage < -0.2 && lasttorquemoy > 0) || (m.VelocityAverage > 0 && lastvelmoy < 0) || (m.VelocityAverage < 0 && lastvelmoy > 0)) && Math.Abs(maxvalue) > 10)
                        {
                            launch = true;
                        }
                        //Si on a un faible couple (ralentissement ou relachement)
                        else if (Math.Abs(m.TorqueAverage) < 0.5 && Math.Abs(maxvalue) > 10)
                        {

                            Console.WriteLine("Torque moy:" + m.TorqueAverage);
                            Console.WriteLine("Vel moy:" + m.VelocityAverage);

                            Console.WriteLine("Vel moy:" + maxvalue);
                            //Vérifie si changement de sens ou si on a laché le bras, si on l'a laché le moteur prends le relai
                            //Dans le même sens et on a déjà dépassé les 30 (pour pas valider juste au démarrage)
                            if (((m.TorqueAverage > 0 && lasttorquemoy > 0) || (m.TorqueAverage < 0 && lasttorquemoy < 0)) && Math.Abs(maxvalue) > 5)
                            {
                                launch = true;
                            }

                        }

                        //On prends la valeur la plus élevée
                        if (Math.Abs(maxvalue) < Math.Abs(m.VelocityAverage))
                        {
                            if (Math.Abs(m.VelocityAverage) > 80)
                            {
                                maxvalue = 80;
                            }
                            else
                            {
                                maxvalue = Math.Abs(m.VelocityAverage);
                            }
                        }


                    }

                    double velocityValue = 0;
                    if (lastvelmoy < 0)
                    {
                        velocityValue = -maxvalue;
                    }
                    else
                    {
                        velocityValue = maxvalue;
                    }
                    m.Enabled();
                    m.SetVelocity(velocityValue);
                }
                //Dans le cas ou le moteur bouge et qu'il y a une torque dans le sens de rotation
                else if (m.TorqueAverage > 0 && m.VelocityAverage < 0 || m.TorqueAverage < 0 && m.VelocityAverage > 0)
                {
                    if (Math.Abs(m.TorqueAverage) > 20)
                    {
                        m.Disabled();
                        while (Math.Abs(m.VelocityAverage) > 10)
                        {
                            m.RefreshInfo(15);
                        }
                        m.Enabled();
                    }
                    else if (Math.Abs(m.TorqueAverage) > 0.5)
                    {
                        double estimatedVelocity = m.AccelerationModel();
                        Console.WriteLine("Extimated volocity:" + estimatedVelocity);
                        if (Math.Abs(m.VelocityAverage - estimatedVelocity) > 2 && Math.Abs(estimatedVelocity) > Math.Abs(m.VelocityAverage))
                        {
                            if (m.TorqueAverage < -0.5)
                            {
                                m.SetVelocity(estimatedVelocity);
                            }
                            else if (m.TorqueAverage > 0.5)
                            {
                                m.SetVelocity(-estimatedVelocity);
                            }
                        }
                    }
                }
                //Le moteur est en fonctionnement et on exerce une force dans le sens contraire de rotation
                else if (m.TorqueAverage > 0 && m.VelocityAverage > 0 || m.TorqueAverage < 0 && m.VelocityAverage < 0)
                {
                    if (Math.Abs(m.TorqueAverage) > 1.3)
                    {
                        //Arrêt focé si torque trop élevé
                        if (Math.Abs(m.TorqueAverage) > 7)
                        {
                            m.Stop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
                            m.Disabled();
                            m.Enabled();
                        }


                        //Arrêt si vitesse faible
                        else if (m.VelocityAverage < 10 || m.TorqueAverage > 6)
                        {
                            m.Stop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
                            m.Wait(0);
                        }
                        //Fonction uniquement à exécuter si la vitesse n'est pas proche de 0, estimation ralentissement
                        else
                        {
                            double estimatedVelocity = m.DecelerationModel();
                            Console.WriteLine("Freinage!!!");
                            Console.WriteLine("Extimated volocity:" + estimatedVelocity);
                            if (m.TorqueAverage < -1.3)
                            {
                                m.SetVelocity(-estimatedVelocity);
                            }
                            else if (m.TorqueAverage > 1.3)
                            {
                                m.SetVelocity(estimatedVelocity);
                            }
                        }
                    }
                }
                m.Wait(100);
                m.ClearInfo();
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
