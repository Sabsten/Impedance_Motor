using sFndCLIWrapper;
using System;

namespace Model
{
    class EggMode
    {
        public EggMode(Motor m)
        {
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(10);

                if (m.IsLocked == false)
                {
                    if (m.VelocityAverage == 0)
                    {
                         m.SetVelocity(m.Constantes.EGG_VELOCITY); 
                    }
                    if (Math.Abs(m.Constantes.EGG_TORQUE_SENSITIVITY) < Math.Abs(m.TorqueAverage))
                    {
                        m.Lock(1000);
                    }
                }
                else
                {
                    if (Math.Abs(m.Constantes.EGG_TORQUE_SENSITIVITY) >= Math.Abs(m.TorqueAverage))
                    {
                        if (m.TorqueAverage < m.Constantes.EGG_TORQUE_SENSITIVITY)
                        {
                            m.Unlock();
                            m.Wait(10000);
                        }
                    }
                }
                m.Wait(100);
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