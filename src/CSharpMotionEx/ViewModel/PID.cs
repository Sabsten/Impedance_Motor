using System;
using Model;

namespace ViewModel
{
    class PID
    {
        private double Kp, Ki, Kd;
        private double erreurCoupleIntegral;
        private double erreurCouplePrecedente;


        public PID(Motor m)
        {
            m.Initialize();
            double coupleCible = 0;

            //A régler jusqu'à ce que le système oscille
            //this.Kp = 15 * 0.6;
            //this.Ki = 2* this.Kp / 4;
            //this.Kd = this.Kp * 5 / 8;
            this.Kp = 5;
            this.Ki = 3;
            this.Kd = 2;
            this.erreurCoupleIntegral = 0;
            this.erreurCouplePrecedente = 0;

            // facteur de lissage pour la vitesse
            double alpha = 0.1;
            double vitessePrecedente = 0;
            double commandeVitessePrecedente = 0;

            while (true) // Boucle de contrôle
            {
                m.RefreshInfo(15);
                // Mesurer le couple actuel (à adapter à votre fonction)
                double coupleActuel = m.TorqueAverage;

                // Calculer l'erreur de couple
                double erreurCouple = coupleCible - coupleActuel;

                // Accumuler l'erreur pour la composante intégrale
                erreurCoupleIntegral += erreurCouple;

                // Calculer la dérivée de l'erreur de couple
                double erreurCoupleDerivee = erreurCouple - erreurCouplePrecedente;
                double commandeVitesse = Kp * erreurCouple + Ki * erreurCoupleIntegral + Kd * erreurCoupleDerivee;


                if (Math.Abs(m.TorqueAverage) < 1 && Math.Abs(m.VelocityAverage) < 10)
                {
                    Console.WriteLine("fdsfdsqfd");
                    // Calculer la commande de vitesse en utilisant le régulateur PID
                    commandeVitesse = alpha * commandeVitesse + (1 - alpha) * commandeVitessePrecedente;
                    commandeVitessePrecedente = commandeVitesse;
                }


                // Envoyer la commande de vitesse au moteur (à adapter à votre fonction)
                if (Math.Abs(commandeVitesse) > 10)
                {
                    //Vitesse de sécurité si trop rapide
                    if(Math.Abs(commandeVitesse) > 80)
                    {
                        if (commandeVitesse > 0)
                        {
                            m.SetVelocity(80);
                        }
                        else
                        {
                            m.SetVelocity(-80);
                        }
                    }
                    else
                    {
                        m.SetVelocity(commandeVitesse);
                    }
                }
                else { 
                    m.SetVelocity(0);
                }

                // Mise à jour de l'erreur précédente
                erreurCouplePrecedente = erreurCouple;

                // À adapter selon la fréquence de mise à jour souhaitée
                System.Threading.Thread.Sleep(10);
                m.Wait(30);

            }

        }

    }
}