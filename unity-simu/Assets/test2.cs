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

    // Variables pour stocker les données du moteur réel
    private float realMotorVelocity;
    private float realMotorPosition;

    void Update()
    {
        // Mettez ici votre code pour obtenir la position et la vitesse du moteur réel

        // Utilisez la position du moteur réel pour définir la rotation du moteur virtuel
        virtualMotor.rotation = Quaternion.Euler(0, realMotorPosition, 0);
        // Assurez-vous d'utiliser le bon axe

        // Si nécessaire, utilisez également la vitesse pour mettre à jour d'autres paramètres
    }
}

