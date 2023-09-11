using System;
using Model;

namespace ViewModel
{
    class FlipFlap
    {
        public FlipFlap(Motor m1,Motor m2)
        {
            m1.Initialize();
            m2.Initialize();

            while (!Console.KeyAvailable)
            {
                m1.RefreshInfo();
                m2.RefreshInfo();

                if (m1.IsLocked && !m1.IsUnderthreshold(m1.Constantes.EGG_TORQUE_SENSITIVITY))
                {
                    m1.StopWait();
                    m2.SetVelocity(50);
                }

                if (m2.IsLocked && !m2.IsUnderthreshold(m2.Constantes.EGG_TORQUE_SENSITIVITY))
                {
                    m2.StopWait();
                    m1.SetVelocity(50);
                }
                m1.Wait(100);
                m2.Wait(100);
            }
            m1.Terminate();
            m2.Terminate();
        }
    }
}
