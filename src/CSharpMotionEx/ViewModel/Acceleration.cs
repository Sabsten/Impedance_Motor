using System;
using sFndCLIWrapper;
using Model;
using System.Threading;

namespace ViewModel
{
    class Acceleration
    {
        public Acceleration(Motor m)
        {
            int consoleWidth = Console.WindowWidth;
            int barWidth = consoleWidth - 20;
            double previousTorque = 0;
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(30);
                Console.WriteLine("Average Velocity : " + m.VelocityAverage);
                Console.WriteLine("Average Torque : " + m.TorqueAverage);

                if (m.TorqueAverage != 0 && m.VelocityAverage != 0)
                {
                    if (previousTorque - m.TorqueAverage >= 0.13)
                    {
                        m.SetVelocity(m.AccelerationModel());
                    }
                    else
                    {
                        m.SetVelocity(m.DecelerationModel());
                    }
                    previousTorque = m.TorqueAverage;
                }

                m.Wait(1);
            }

            m.Lock(1000, cliNodeStopCodes.STOP_TYPE_DISABLE_RAMP);
            m.WaitUntilMoveDone(500);
            Console.ReadKey(true);
            ErrorAndQuit("Fin program");
        }
        static void DrawBar(double value, int barWidth)
        {
            int normalizedValue = (int)(value * barWidth);
            string bar = new string('#', normalizedValue);
            string emptySpace = new string(' ', barWidth - normalizedValue);

            Console.Write("|");
            Console.Write(bar);
            Console.Write(emptySpace);
            Console.Write("|");
        }

        public static void ErrorAndQuit(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(1);
        }
        static void RefreshDrawing(Motor motor)
        {
            int consoleWidth = Console.WindowWidth;

            // Enregistrer la position actuelle du curseur
            int initialCursorTop = Console.CursorTop;

            // Déplacer le curseur à la position initiale
            Console.SetCursorPosition(0, initialCursorTop);

            // Dessiner le moteur
            Console.WriteLine("     _______");
            Console.WriteLine("   _/       \\_");
            Console.WriteLine("  / |       | \\");
            Console.WriteLine(" /  |       |  \\");
            Console.WriteLine("|   |       |   |");
            Console.WriteLine("|___|_______|___|");
            Console.WriteLine();

            // Positionner le bras du moteur en fonction de PositionAverage
            int armLength = 10; // Longueur du bras du moteur
            int normalizedPosition = (int)(motor.PositionAverage * consoleWidth);

            // Dessiner le bras du moteur
            Console.SetCursorPosition(normalizedPosition - armLength, Console.CursorTop - 4);
            Console.WriteLine(new string('-', armLength));
        }
    }
}
