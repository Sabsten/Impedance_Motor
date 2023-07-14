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
                Console.Clear();
                MenuPrincipal.MotorSelection();
                Console.Clear();
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
                            _ = new Acceleration(MotorList[0]);
                            break;
                        case "4":
                            Console.WriteLine("");
                            _ = new RecordPosition(MotorList[0]);
                            break;
                        case "5":
                            Console.WriteLine("");
                            _ = new PositionReturn(MotorList[0]);
                            break;
                        case "6":
                            Console.WriteLine("");
                            _ = new FlipFlap(MotorList[0], MotorList[0]);
                            break;
                        case "7":
                            Console.WriteLine("");
                            _ = new Crips(MotorList[0]);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("No motor available..");
                    Console.ReadKey();
                }
            };
        }
    }
}