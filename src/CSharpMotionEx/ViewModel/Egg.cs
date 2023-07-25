using sFndCLIWrapper;
using System;

namespace Model
{
    class EggMode
    {
        public EggMode(Motor m)
        {
            m.Initialize();

            while (!Console.KeyAvailable)
            {
                m.RefreshInfo();

                if (!m.IsLocked && m.VelocityAverage == 0)
                {
                    m.SetVelocity(m.Constantes.EGG_VELOCITY);
                }

                if (IsUnderthreshold(m))
                {
                    //m.Lock(1000);
                    m.Lock();
                }
                else if (m.IsLocked && !IsUnderthreshold(m))
                {
                    m.Unlock();
                    //m.Wait(10000);
                }

                m.Wait(100);
            }
            m.Terminate();
        }

        bool IsUnderthreshold(Motor m)
        {
            return m.Constantes.EGG_TORQUE_SENSITIVITY < Math.Abs(m.TorqueAverage);
        }
    }
}