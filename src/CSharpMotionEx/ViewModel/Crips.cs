using sFndCLIWrapper;
using System;

namespace Model
{
    class Crips
    {
        public Crips(Motor m)
        {
            m.Initialize();
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(50);
                if (m.MoveIsDone() == false && m.TorqueAverage > 1)
                {
                    m.Unlock();
                }
                else
                {
                    m.Lock();
                }

                m.Wait(100);
            }
            m.Terminate();
        }
    }
}