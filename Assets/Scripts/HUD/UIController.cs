using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController player;
    public static int currentFaceSet = 0;
    public Sprite[] facesFullHP;
    public Sprite[] facesHighHP;
    public Sprite[] facesLowHP;
    public Sprite[] facesLowestHP;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        changeFace();
    }

    public void changeFace(){
        if (player.health > 75){
            GameObject.Find("Face").GetComponent<Image>().sprite = facesFullHP[currentFaceSet];
        }
        else if (player.health > 50){
            GameObject.Find("Face").GetComponent<Image>().sprite = facesHighHP[currentFaceSet];
        } 
        else if (player.health > 25){
            GameObject.Find("Face").GetComponent<Image>().sprite = facesLowHP[currentFaceSet];
        } 
        else if (player.health > 0){
            GameObject.Find("Face").GetComponent<Image>().sprite = facesLowestHP[currentFaceSet];
        } 
    }
}
