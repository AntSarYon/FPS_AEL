using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class HealthUI : MonoBehaviour
{
    private PlayerController player;
    private TMP_Text  SaludText;

    //---------------------------------------------------------------------------------

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        SaludText = GetComponent<TMP_Text >();

    }

    //-----------------------------------------------------------------------------------

    void Update()
    {
        //Actualizar salud
        SaludText.text = player.health.ToString();
    }
}