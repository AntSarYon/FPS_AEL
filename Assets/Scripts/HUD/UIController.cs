using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController player;
    private int currentFaceSet;
    public Sprite[] facesFullHP;
    public Sprite[] facesHighHP;
    public Sprite[] facesLowHP;
    public Sprite[] facesLowestHP;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        currentFaceSet = 4;
    }

    // Update is called once per frame
    void Update()
    {
        changeFace();
        if (player.IsGrappling() == true){
            currentFaceSet = 0;
        }
        else if (player.IsWallrunning() == true){
            currentFaceSet = 1;
        }
        else if (player.IsOnFloor() == true){
            currentFaceSet = 4;
        }
        else if (player.IsInAir() == true){
            currentFaceSet = 2;
        }
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
