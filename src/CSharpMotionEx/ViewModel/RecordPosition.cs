using System;
using System.Collections.Generic;
using System.IO;
using sFndCLIWrapper;
using Model;
using Spectre.Console;
using System.Windows.Documents;
using System.Diagnostics;

namespace ViewModel
{
    class RecordPosition
    {
        private List<double> m_VelocityList;
        private List<double> m_PositionList;
        private Motor m;
        private int precision = 60;

        public RecordPosition(Motor motor)
        {
            List<string> Info = new List<string>();
            Info.Add("1 - Play");
            Info.Add("2 - Record based on velocity");
            Info.Add("3 - Record based on position");
            m = motor;
            m.Disable();
            while (true)
                {
                    string choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select [red]record[/] or [green]Play[/] a motion.")
                            .PageSize(10)
                            .MoreChoicesText("[grey](Move up and down to reveal more frameworks)[/]")
                            .AddChoices(Info));
                    Console.Clear();
                    switch (choice.Split(" - ".ToCharArray())[0])
                    {
                        case "1":
                            if(!(m_VelocityList == null))
                        {
                            PlayVelocities();
                        }
                                else if (!(m_PositionList == null))
                        {
                                PlayPositions();
                            }
                            break;
                        case "2":
                            RecordVelocities();
                            break;
                        case "3":
                            RecordPositions();
                            break;
                    }
                }
            


        }
        public void PlayVelocities()
        {
            m.Enable();
            //m.SetPositon(m_initial_position, true);
            foreach (var item in m_VelocityList)
            {
                if (item == 0)
                {
                    //m.LockReccord(1, cliNodeStopCodes.STOP_TYPE_RAMP);
                    m.Stop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
                }
                else
                {
                    m.SetVelocity(item);

                }
                m.Wait(30);
            }
            m.Disable();
        }

        public void PlayPositions()
        {
            m.ResetPositionToHome();
            m.Enable();
            foreach (var item in m_PositionList)
            {
                m.RefreshInfo(1);
                Console.WriteLine((int)(item - m.PositionAverage));
                m.SetPosition((int)(item - m.PositionAverage));
                while (m.MoveIsDone() == false)
                {
                    m.Wait(5);
                }
            }
            m.Disable();
        }


        public void RecordVelocities()

        {
            Console.WriteLine("Please set the initial position of the motor. Then press any key to record.");
            Console.ReadKey();
            m_PositionList = null;
            m_VelocityList = new List<double>();
            Console.WriteLine("Recording motion... Press any key to stop recording.");
            while (!Console.KeyAvailable)
            {
                m.Wait(30);
                m.RefreshInfo(1);
                m_VelocityList.Add(m.VelocityAverage);
            }
        }

        public void RecordPositions()

        {
            Console.WriteLine("Please set the initial position of the motor. Then press any key to record.");
            Console.ReadKey();
            m.ResetPositionToHome();
            m_VelocityList = null;
            m_PositionList = new List<double>();
            Console.WriteLine("Recording motion... Press any key to stop recording.");
            while (!Console.KeyAvailable)
            {
                m.Wait(500);
                m.RefreshInfo(1);
                Console.WriteLine(m.PositionAverage);
                m_PositionList.Add(m.PositionAverage);
            }
        }
    }
}