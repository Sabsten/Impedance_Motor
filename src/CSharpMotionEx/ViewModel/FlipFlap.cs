using System;
using sFndCLIWrapper;
using Model;

namespace ViewModel
{
    class FlipFlap
    {
        public FlipFlap(Motor m, Motor m2)
        {
            double previousTorque = 0;
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(30);
                Console.WriteLine("Average Velocity : " + m.VelocityAverage);
                Console.WriteLine("Average Torque : " + m.TorqueAverage);

                if (m.TorqueAverage != 0 && m.VelocityAverage != 0)
                {
                    if (previousTorque - m.TorqueAverage >= 0.13)
                    {
                        m.SetVelocity(m.AccelerationModel());
                    }
                    else
                    {
                        m.SetVelocity(m.DecelerationModel());
                    }
                    previousTorque = m.TorqueAverage;
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
        }
    }
}
