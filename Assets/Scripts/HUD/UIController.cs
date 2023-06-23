using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    
    private PlayerController player;
    private int currentFaceSet;

    private Image imgFace;

    public Sprite[] facesFullHP;
    public Sprite[] facesHighHP;
    public Sprite[] facesLowHP;
    public Sprite[] facesLowestHP;
    
    //---------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos referencia al player
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        imgFace = GetComponent<Image>();
        currentFaceSet = 4;
    }

    //------------------------------------------------------------------------------

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
            imgFace.sprite = facesFullHP[currentFaceSet];
        }
        else if (player.health > 50){
            imgFace.sprite = facesHighHP[currentFaceSet];
        } 
        else if (player.health > 25){
            imgFace.sprite = facesLowHP[currentFaceSet];
        } 
        else if (player.health > 0){
            imgFace.sprite = facesLowestHP[currentFaceSet];
        } 
    }
}
