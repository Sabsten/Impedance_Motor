using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using Model;

namespace Model
{
    class Option
    {
        public Option()
        {
            Console.Write("Entrez la constantes que vous souhaitez modifier");
            string userInput = Console.ReadLine();
            Console.WriteLine("entrez la valeur que vous souhaitez attribuer à la constante");
            var valeurInput = Console.ReadLine();

            Constantes constantes = new Constantes();
            constantes.WriteConstant(userInput, valeurInput);
        }


    }
}
