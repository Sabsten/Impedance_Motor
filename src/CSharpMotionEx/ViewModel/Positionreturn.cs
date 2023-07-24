using System;
using sFndCLIWrapper;
using Model;

namespace ViewModel
{
    class PositionReturn
    {
        public PositionReturn(Motor m)
        {
            double previousTorque = 0;
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(30);
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
