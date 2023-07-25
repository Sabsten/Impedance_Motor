﻿using sFndCLIWrapper;
using System;

namespace Model
{
    class Crisp
    {
        public Crisp(Motor m)
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