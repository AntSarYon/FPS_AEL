using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthUI : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    private float vida;
    [SerializeField]
    private Text SaludText;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();

        SaludText = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        //Actualizar salud
        vida = player.health;
        Debug.Log(vida);
        SaludText.text = "hola";      
    }
}
