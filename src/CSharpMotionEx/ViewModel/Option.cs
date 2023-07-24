using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using Model;
using Spectre.Console;

namespace Model
{
    class Option
    {
        public Option(Motor m)
        {
            List<string> optionsList = new List<string>();
            optionsList.Add("Motor");
            optionsList.Add("All variable");
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Select [green]the option[/] you want to change")
                .PageSize(20)
                .MoreChoicesText("[grey](Move up and down to reveal more frameworks)[/]")
                .AddChoices(optionsList));

            switch (choice)
            {
                case "Motor":
                    Console.WriteLine("");
                    SetMotorConstant(m);
                    break;
                case "All":
                    Console.WriteLine("");
                    ChangeConstant();
                    break;
            }

            void ChangeConstant()
            {
                Constantes c = new Constantes();
                List<string> options = new List<string>();
                foreach (var item in c.GetConstantsList)
                {
                    options.Add(item.Key + " = " + item.Value);
                }
                string framework = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Select [green]variable[/] you want to change")
                    .PageSize(20)
                    .MoreChoicesText("[grey](Move up and down to reveal more frameworks)[/]")
                    .AddChoices(options));

                Console.Write("You want to change " + framework.Split("=".ToCharArray())[0] + ".");
                Console.WriteLine("Enter the new value of this variable.");
                var valeurInput = Console.ReadLine();

                ;
            }
            void SetMotorConstant(Motor motor)
            {
                Constantes c = new Constantes();
                motor.SetVelocity(100);
                List<double> a = new List<double>();
                for (int j = 0; j < 10; j++)
                {
                    m.RefreshInfo(100);
                    a.Add(motor.TorqueAverage);
                }

                c.WriteConstant("MOTOR_SEUIL", " " + a.Max());
            }
        }
    }
}
