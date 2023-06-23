using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthUI : MonoBehaviour
{
    private PlayerController player;
    private TMP_Text  SaludText;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        SaludText = GetComponent<TMP_Text >();

    }

    // Update is called once per frame
    void Update()
    {
        //Actualizar salud
        SaludText.text = player.health.ToString();
    }
}
