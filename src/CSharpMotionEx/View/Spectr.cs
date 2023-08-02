using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
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
        // New function below that wait for the motors to be defined
        // public List<Motor> motors { get { return m_motors; } }
        private volatile bool m_updateNeeded = true;
        private bool m_inSubMenu = false;

        public Interface()
        {
            ModMenu();
        }

        public void ModMenu()
        {
            m_ModeListObjectt = ParseJson(GetModsDirectory());
            int lastWindowWidth = -1;

            DisplayMenu(0);

            // Créer une nouvelle tâche pour lire les entrées de l'utilisateur
            Task.Factory.StartNew(() =>
            {
                m_inSubMenu = false;
                ModeSelection();
                m_inSubMenu = true;
                m_updateNeeded = true;
                Thread.Sleep(500);
                MotorSelection();
            });

            while (true)
            {
                int windowWidth = Console.WindowWidth;

                // Si la fenêtre est redimensionnée ou qu'une sélection a été faite, mettez à jour l'affichage
                if (windowWidth != lastWindowWidth || m_updateNeeded)
                {
                    DisplayMenu(windowWidth);
                    lastWindowWidth = windowWidth;
                    m_updateNeeded = false;
                }
                System.Threading.Thread.Sleep(500);
            }
        }

        // Les mêmes méthodes que votre code original...

    private void DisplayMenu(int windowWidth)
        {
            AnsiConsole.Clear();

            if (m_inSubMenu) return;

            AnsiConsole.Write(
                new FigletText("Impedance-Motor")
                    .Color(Color.Red));

            int tableWidth = (int)(windowWidth * 0.8);

            var table = new Table()
                .Width(tableWidth)
                .BorderColor(Color.Red)
                .AddColumn(new TableColumn("[u]Num[/]").Centered())
                .AddColumn(new TableColumn("[u]Name[/]").Centered())
                .AddColumn(new TableColumn("[u]Explaination[/]").Centered());

            m_ModeList = new List<string>();

            foreach (var mod in m_ModeListObjectt)
            {
                table.AddRow(new Text(mod.Number.ToString()).Centered(),
                             new Text(mod.Name).Centered(),
                             new Text(mod.Explaination).Centered());
                m_ModeList.Add(mod.Number + " - " + mod.Name);
                table.AddRow("", "", "");
            }

            AnsiConsole.Write(
                    new Panel(table)
                        .Expand()
                        .BorderColor(Color.Red)
                        .Header("[[ [bold underline blue on white]List of Mods[/] ]]"));
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
                    list.Add("Port " + i + " : Not found");
                }

                var fruits = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("What [green]motor[/] do you want to use ?")
                        .NotRequired()
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to select motor(s))[/]")
                        .InstructionsText(
                            "[grey](Press [blue]<space>[/] to toggle a motor, " +
                            "[green]<enter>[/] to pick it)[/]")
                        .AddChoices(list));
            }
            m_motors = MManager.MotorList;
        }

        public List<Motor> getMotors()
        {
            while (m_motors == null)
            {
                Thread.Sleep(500);
            }
            return m_motors;
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
                    break;
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
    }
}
