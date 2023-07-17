using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Model;
using Newtonsoft.Json;
using Spectre.Console;
namespace ViewModel
{
    class Interface
    {
        private List<string> m_ModeList;
        private List<Mod> m_ModeListObjectt;
        private string m_NumProg;
        public string NumProg { get { return m_NumProg; } }
        private string m_NumMotorRequired;
        public string NumMotorRequired { get { return m_NumMotorRequired; } }
        private List<Motor> m_motors;
        public List<Motor> motors { get { return m_motors; } }
        public void ModMenu()
        {
            AnsiConsole.Write(
                new FigletText("Impedance-Motor")
                    .Color(Color.Red));

            m_ModeListObjectt = ParseJson(GetModsDirectory());

            // Crée un tableau
            var table = new Table()
                .BorderColor(Color.Red)
                .AddColumn(new TableColumn("[u]Num[/]").Centered())
                .AddColumn(new TableColumn("[u]Name[/]").Centered())
                .AddColumn(new TableColumn("[u]Explaination[/]").Centered());
            m_ModeList = new List<string>();
            // Ajoute une ligne au tableau
            foreach (var mod in m_ModeListObjectt)
            {
                table.AddRow(new Text(mod.Number.ToString()).Centered(),
                             new Text(mod.Name).Centered(),
                             new Text(mod.Explaination).Centered());
                m_ModeList.Add(mod.Number + " - " + mod.Name);
                table.AddRow("","","");
            }

            AnsiConsole.Write(
                    new Panel(table)
                        .Expand()
                        .BorderColor(Color.Red)
                        .Header("[[ [bold underline blue on white]List of Mods[/] ]]"));
            AnsiConsole.Write("");
        }
        public void ModeSelection()
        {
            string framework = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Mode[/] you want to execute")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more frameworks)[/]")
                    .AddChoices(m_ModeList));
            m_NumMotorRequired = m_ModeListObjectt[int.Parse(framework.Split(" - ".ToCharArray())[0]) - 1].Motor_required;
            m_NumProg = framework.Split(" - ".ToCharArray())[0];

        }

        public void MotorSelection()
        {
            MotorManager MManager = new MotorManager();
            if (int.Parse(m_NumMotorRequired) >= 2)
            {
                List<string> list = new List<string>();
                int nodeLimit = 4;
                int Counter = 0;
                foreach (var item in MManager.NodeList)
                {
                    list.Add("Port " + Counter + " : " + item.NodeObject.Port.ToString() + " - " + item.SerialNumber);
                    Counter++;
                }
                for (int i = Counter; i < nodeLimit; i++)
                {
                    // Code to be executed in each iteration
                    list.Add("Port " + i + " : Not found");
                }

                // Ask for the user's favorite fruits
                var fruits = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("What [green]motor[/] do you want to use ?")
                        .NotRequired() // Not required to have a favorite fruit
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to select motor(s))[/]")
                        .InstructionsText(
                            "[grey](Press [blue]<space>[/] to toggle a motor, " +
                            "[green]<enter>[/] to pick it)[/]")
                        .AddChoices(list));
            }
            m_motors = MManager.MotorList;


            // Subscribe to the MotorPositionRefreshed event for each motor.
            foreach (Motor motor in m_motors)
            {
                motor.MotorPositionRefreshed += () => DisplayMotorPosition(motor);
            }
        }
        string GetModsDirectory(string cfgFileName = "statham.json")
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory);

            for (int i = 0; i < 4; i++)
            {
                if (directoryInfo.Parent != null)
                {
                    directoryInfo = directoryInfo.Parent;
                }
                else
                {
                    break; // Si le répertoire parent n'existe pas, on sort de la boucle
                }
            }

            return directoryInfo.FullName + "\\" + cfgFileName;
        }

        public static List<Mod> ParseJson(string json)
        {
            ModsCollection modsCollection = JsonConvert.DeserializeObject<ModsCollection>(File.ReadAllText(json));
            return modsCollection.Mods;
        }
        public class Mod
        {
            public int Number { get; set; }
            public string Name { get; set; }
            public string Explaination { get; set; }
            public string Motor_required { get; set; }
        }
        public class ModsCollection
        {
            public List<Mod> Mods { get; set; }
        }

        private void DisplayMotorPosition(Motor motor)
        {
            int motorPosition = (int)Math.Round(motor.Position); // Get the position of the motor and round it to the nearest integer
            string motorArm = new string('-', motorPosition) + "*" + new string('-', (50 - motorPosition)); // Create the ASCII representation of the motor arm

            AnsiConsole.Clear(); // Clear the console
            AnsiConsole.Write(
                new Panel(motorArm) // Display the motor arm
                    .Expand()
                    .BorderColor(Color.Red)
                    .Header("[[ [bold underline blue on white]Motor position[/] ]]"));
        }

    }
}