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
        private List<double> m_PositionList2;
        private Motor m;
        private cliValueDouble m_initial_position;
        public RecordPosition(Motor motor)
        {
            motor.RecordFalse();
            List<string> Info = new List<string>();
            Info.Add("1 - Play");
            Info.Add("2 - Record");
            while (true)
            {
                m = motor;
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
                            Play();

                            break;
                        case "2":
                            RecordPositions();
                            break;
                    }
                }
            }
            m.RecordTrue();


        }
        public void Play()
        {
            m.Lock(1);

            //m.SetPositon(m_initial_position, true);
            foreach (var item in m_PositionList)
            {
                if (item == 0)
                {
                    m.Lock(1, cliNodeStopCodes.STOP_TYPE_ABRUPT);
                    m.Unlock();

                }
                else
                {
                    m.Wait(0);
                    m.SetVelocity(item);

                }
                m.Wait(1);

            }

            m.Lock(1, cliNodeStopCodes.STOP_TYPE_ABRUPT);
        }


        public void RecordPositions()

        {

            m.Unlock();
            Console.WriteLine("Please set the initial position of the motor. Then press any key to record.");
            Console.ReadKey();
            m_initial_position = m.ActualPosition;
            m_PositionList = new List<double>();
            Console.WriteLine("Recording motion... Press any key to stop recording.");
            while (!Console.KeyAvailable)
            {
                m.Unlock();
                m.Wait(500);
                m.RefreshInfo(1);
                m_PositionList.Add(m.VelocityAverage);
                //m_PositionList2.Add(m.PositionAverage);
                //Console.WriteLine("Average Velocity : " + m.VelocityAverage);
                //Console.WriteLine("Average Torque : " + m.TorqueAverage);
            }
            //m.Lock(10, cliNodeStopCodes.STOP_TYPE_ABRUPT);
        }
    }
}