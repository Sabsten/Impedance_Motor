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

                if (!m.IsLocked && !m.IsUnderthreshold(m.Constantes.EGG_TORQUE_SENSITIVITY))
                {
                    m.StopWait();
                    m.Wait(1000);
                }
                
            }
            m.Terminate();
        }

    }
}