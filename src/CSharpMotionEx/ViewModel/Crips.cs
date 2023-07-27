using sFndCLIWrapper;
using System;
using System.Drawing.Text;
using System.Windows.Input;

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
                double lasttorquemoy = m.TorqueAverage;
                

                if (m.MoveIsDone() == false && lasttorquemoy>1)
                {
                    m.Unlock();
                    
                }
                else if (m.MoveIsDone() == true)
                {
                    m.Lock();
                }

                m.Wait(100);
            }

            m.Terminate();
        }

        string YourBestFunction()
        {
            return "Seb is the best";
        }
    }
}