using System;
using System.Collections.Generic;
using System.IO;
using sFndCLIWrapper;
using Model;
using Spectre.Console;
using System.Windows.Documents;
using System.Diagnostics;
using System.Linq;

namespace ViewModel
{
    class RecordPosition
    {
        private List<double> m_VelocityList;
        private List<double> m_PositionList;
        private List<double> m_PositionList2;
        private Motor m;

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
            Console.WriteLine("Please set the initial position of the motor. Then press any key to play.");
            Console.ReadKey();
            m.ResetPositionToHome();
            m.Enable();
            double lastVelocity = 0;
            int i = 0;

            foreach (var velocity in m_VelocityList)
            {
                m.RefreshInfo(1);
                m.Wait(5);

                if (velocity == 0)
                {
                    m.Stop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
                }
                else if (velocity * lastVelocity < 0)
                {
                    m.Stop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
                }

                m.SetVelocity(velocity);
                double targetPosition = m_PositionList2.ElementAt(i);
                double currentPosition = m.PositionAverage;

                while ((velocity > 0 && currentPosition < targetPosition) ||
                       (velocity < 0 && currentPosition > targetPosition))
                {
                    m.RefreshInfo(1);
                    Console.WriteLine("Current" + currentPosition);
                    Console.WriteLine("Target" + targetPosition);
                    m.Wait(5);
                    currentPosition = m.PositionAverage;
                }

                i++;
                lastVelocity = velocity;
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
            m_PositionList2 = new List<double>();
            m_VelocityList = new List<double>();
            Console.WriteLine("Recording motion... Press any key to stop recording.");
            while (!Console.KeyAvailable)
            {
                m.Wait(30);
                m.RefreshInfo(1);
                m_VelocityList.Add(m.VelocityAverage);
                m_PositionList2.Add(m.PositionAverage);
            }
        }

        public void RecordPositions()

        {
            //A chaque fois changement de signe = changement de sens
            bool pos = true;
            double delta = 0;
            double lastDelta = 0;
            double lastPosition = 0;
            Console.WriteLine("Please set the initial position of the motor. Then press any key to record.");
            Console.ReadKey();
            m.ResetPositionToHome();
            m_VelocityList = null;
            m_PositionList = new List<double>();
            Console.WriteLine("Recording motion... Press any key to stop recording.");
            m_PositionList.Add(m.PositionAverage);
            while (!Console.KeyAvailable)
            {
                m.Wait(500);
                m.RefreshInfo(1);
                Console.WriteLine(m.PositionAverage);
                delta = m.PositionAverage - lastPosition;

                if ((delta * lastDelta > 0) || m_PositionList.Last() == 0 || lastPosition == 0)
                {
                    m_PositionList.Add(m.PositionAverage);
                }
                lastDelta = delta;
                lastPosition = m.PositionAverage;
            }
            //Supprimer les positions superflues:
            // Parcourir la liste à l'envers
            for (int i = m_PositionList.Count - 2; i >= 0; i--)
            {
                double difference = Math.Abs(m_PositionList[i + 1] - m_PositionList[i]); // Utiliser la valeur absolue

                if (difference < 300)
                {
                    m_PositionList.RemoveAt(i + 1); // Supprimer l'élément suivant
                }
            }
        }
    }
}