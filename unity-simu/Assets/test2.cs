using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sFndCLIWrapper;

public class test2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Transform virtualMotor;

    // Variables pour stocker les donn�es du moteur r�el
    private float realMotorVelocity;
    private float realMotorPosition;

    void Update()
    {
        // Mettez ici votre code pour obtenir la position et la vitesse du moteur r�el

        // Utilisez la position du moteur r�el pour d�finir la rotation du moteur virtuel
        virtualMotor.rotation = Quaternion.Euler(0, realMotorPosition, 0);
        // Assurez-vous d'utiliser le bon axe

        // Si n�cessaire, utilisez �galement la vitesse pour mettre � jour d'autres param�tres
    }
}

