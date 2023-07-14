using System;
using System.Collections.Generic;
using MaterialDesignThemes.Wpf;
using Model;
using Spectre.Console;

namespace ViewModel
{
    class MainProgram
    {
        static void Main()
        {
            while (true)
            {
                Interface MenuPrincipal = new Interface();
                MenuPrincipal.ModMenu();
                MenuPrincipal.ModeSelection();
                MenuPrincipal.MotorSelection();
                List<Motor> MotorList = MenuPrincipal.motors;
                if (MotorList.Count != 0)
                {
                    switch (MenuPrincipal.NumProg)
                    {
                        case "1":
                            Console.WriteLine("");
                            _ = new EggMode(MotorList[0]);
                            break;
                        case "2":
                            Console.WriteLine("");
                            _ = new Miror(MotorList[0]);
                            break;
                        case "3":
                            Console.WriteLine("");
                            _ = new PositionReturn(MotorList[0]);
                            break;
                        case "4":
                            Console.WriteLine("");
                            _ = new PositionReturn(MotorList[0]);
                            break;
                        case "5":
                            Console.WriteLine("");
                            _ = new PositionReturn(MotorList[0]);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("No motor");
                    while (!Console.KeyAvailable)
                    {
                    }
                }
            };
        }
    }
}