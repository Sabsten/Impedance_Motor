using sFndCLIWrapper;
using System;

namespace Model
{
    class ZZ_Class_Example
    {
        public ZZ_Class_Example(Motor m)
        {
            m.Initialize();

            while (!Console.KeyAvailable)
            {
                m.RefreshInfo();

                //Some code...

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