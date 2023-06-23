using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController player;
    public static int currentFace = 0;
    public Sprite[] facesFullHP;
    public Sprite[] facesHighHP;
    public Sprite[] facesLowHP;
    public Sprite[] facesLowestHP;
    
    void Start()
    {
        player = GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeFace(){
        if (player.health == 100){
            GameObject.Find("Face").GetComponent<Image>().sprite = facesFullHP[0];
        }
        else if (player.health > 75){
            GameObject.Find("Face").GetComponent<Image>().sprite = facesHighHP[0];
        } 
    }
}
