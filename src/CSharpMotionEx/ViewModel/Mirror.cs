using System;
using sFndCLIWrapper;
using Model;

namespace ViewModel
{
    class Miror
    {
        public Miror(Motor m)
        {
            m.Initialize();
            double VelocityIndex = 50;
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo();
                if (!m.IsLocked)
                {
                    if (m.VelocityAverage == 0)
                    {
                        m.SetVelocity(VelocityIndex);
                    }
                    long now2 = DateTimeOffset.Now.ToUnixTimeSeconds();
                    if (Math.Abs(m.Constantes.EGG_TORQUE_SENSITIVITY) < Math.Abs(m.TorqueAverage) && Math.Abs(now - now2) > 1/10000)
                    {
                        now = DateTimeOffset.Now.ToUnixTimeSeconds();
                        m.TempStop();
                        VelocityIndex = -VelocityIndex;
                        m.SetVelocity(VelocityIndex);
                    }
                }
            }
            m.Terminate();
        }
    }
}
