using System;
using System.Collections.Generic;
using System.IO;
using sFndCLIWrapper;
using Model;
using Spectre.Console;
using System.Windows.Documents;

namespace ViewModel
{
    class RecordPosition
    {
        private List<double> m_PositionList;
        private Motor m;
        private cliValueDouble m_initial_position;
        public RecordPosition(Motor motor)
        {
            List<string> Info = new List<string>();
            Info.Add("1 - Play");
            Info.Add("1 - Record");
            m = motor;
            while (true)
            {
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [red]record[/] or [green]Play[/] a motion.")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more frameworks)[/]")
                        .AddChoices(Info));
                switch (choice.Split(" - ".ToCharArray())[1])
                { 
                    case "Record":
                        RecordPositions();
                        break;
                    case "Play":
                        Play();
                        break;
                }
            }

        }
        public void Play()
        {
            //m.SetPositon(m_initial_position, true);
            foreach (var item in m_PositionList)
            {
                m.SetVelocity(item);
            }
        }

        public void RecordPositions()
        {
            Console.WriteLine("Please set the initial position of the motor. Then press any key to record.");
            m.Unlock();
            while (!Console.KeyAvailable)
            {
            }
            m_initial_position = m.ActualPosition;
            m_PositionList = new List<double>();
            Console.WriteLine("Recording motion... Press any key to stop recording.");
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(10);
                m_PositionList.Add(m.VelocityAverage);
                //Console.WriteLine("Average Velocity : " + m.VelocityAverage);
                //Console.WriteLine("Average Torque : " + m.TorqueAverage);
            }
        }
    }
}