using System;
using System.Collections.Generic;
using System.IO;
using sFndCLIWrapper;

namespace Model
{
    class EggMode
    {
        public EggMode(Motor m)
        {
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(10);
                Console.WriteLine("Average Velocity : " + m.VelocityAverage);
                Console.WriteLine("Average Torque : " + m.TorqueAverage);

                if (m.IsLocked == false)
                {
                    if (m.VelocityAverage == 0){m.SetVelocity(m.Constantes.EGG_VELOCITY); }
                    if (Math.Abs(m.Constantes.EGG_TORQUE_SENSITIVITY) < Math.Abs(m.TorqueAverage)){m.Lock(10);}
                }
                else
                {
                    if (Math.Abs(m.Constantes.EGG_TORQUE_SENSITIVITY) >= Math.Abs(m.TorqueAverage)){m.Unlock();}
                }
                m.Wait(10);
            }

            m.Lock(1000, cliNodeStopCodes.STOP_TYPE_DISABLE_RAMP);
            m.WaitUntilMoveDone(500);
            Console.ReadKey(true);
            ErrorAndQuit("Fin program");
        }

        public static void ErrorAndQuit(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}