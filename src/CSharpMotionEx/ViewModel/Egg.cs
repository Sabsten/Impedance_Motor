using sFndCLIWrapper;
using System;

namespace Model
{
    class EggMode
    {
        public EggMode(Motor m)
        {
            while (!Console.KeyAvailable)
            {
                m.RefreshInfo(10);

                if (m.IsLocked == false)
                {
                    if (m.VelocityAverage == 0) { m.SetVelocity(m.Constantes.EGG_VELOCITY); }
                    if (Math.Abs(m.Constantes.EGG_TORQUE_SENSITIVITY) < Math.Abs(m.TorqueAverage)) { m.Lock(10); }
                }
                else
                {
                    if (Math.Abs(m.Constantes.EGG_TORQUE_SENSITIVITY) >= Math.Abs(m.TorqueAverage)) { m.Unlock(); }
                }
                RefreshDrawing(m);
                m.Wait(10);
            }

            m.Lock(1000, cliNodeStopCodes.STOP_TYPE_DISABLE_RAMP);
            m.WaitUntilMoveDone(500);
            Console.ReadKey(true);
            ErrorAndQuit("Fin program");
        }

        public static void ErrorAndQuit(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(1);
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
            int normalizedPosition = (int)(motor.PositionAverage * (consoleWidth - 1)); // Soustraire 1 pour rester dans la plage valide

            // Vérifier si normalizedPosition dépasse les limites de la console
            if (normalizedPosition < 0)
            {
                normalizedPosition = 0; // Réinitialiser à 0 si inférieur à 0
            }
            else if (normalizedPosition >= consoleWidth)
            {
                normalizedPosition = consoleWidth - 1; // Réduire à la dernière position valide si supérieur ou égal à la largeur de la console
            }

            // Dessiner le bras du moteur
            Console.SetCursorPosition(normalizedPosition - armLength, Console.CursorTop - 4);
            Console.WriteLine(new string('-', armLength));
        }

        // Ajoutez ici la définition de la classe Motor
    }
}