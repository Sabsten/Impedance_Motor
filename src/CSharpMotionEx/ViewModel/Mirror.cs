using System;
using System.Collections.Generic;
using System.IO;
using sFndCLIWrapper;
using Model;

namespace ViewModel
{
    class Miror
    {
        public Miror(Motor m)
        {
            double VelocityIndex = 50;
            double TorqueLimit = 0.15;
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(10);
                Console.WriteLine("Average Velocity : " + m.VelocityAverage);
                Console.WriteLine("Average Torque : " + m.TorqueAverage);

                if (m.IsLocked == false)
                {
                    if (m.VelocityAverage == 0)
                    {
                        m.SetVelocity(VelocityIndex);
                    }
                    long now2 = DateTimeOffset.Now.ToUnixTimeSeconds();

                    if (Math.Abs(TorqueLimit) < Math.Abs(m.TorqueAverage) && (Math.Abs(now - now2) > 0.5))
                    {
                        now = DateTimeOffset.Now.ToUnixTimeSeconds();
                        m.Lock(1, cliNodeStopCodes.STOP_TYPE_ABRUPT);
                        m.Unlock();
                        VelocityIndex = -VelocityIndex;
                        m.SetVelocity(VelocityIndex);
                    }
                }
                m.Wait(1);
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
