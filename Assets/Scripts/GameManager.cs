using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private PlayerController player;
    private UIAnimController uiController;

    public int cantEnemigosIniciales;

    private int maxEnemigos = 50;
    private int minEnemigos = 10;

    public PlayerController Player { get => player; set => player = value; }
    public UIAnimController UiController { get => uiController; set => uiController = value; }

    //----------------------------------------------------
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        cantEnemigosIniciales = 20;
    }

    private void Update()
    {
        if (player != null && uiController != null)
        {
            if (player.health <= 0)
            {
                uiController.MAnimator.SetTrigger("FadeCredits");
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                uiController.MAnimator.SetTrigger("FadeMenu");
            }
        }

        
    }

    //------------------------------------------------------

    public void DisminuirCantEnemigos()
    {
        if (cantEnemigosIniciales > minEnemigos)
        {
            cantEnemigosIniciales -= 5;
        }
    }

    public void IncrementarCantEnemigos()
    {
        if (cantEnemigosIniciales < maxEnemigos)
        {
            cantEnemigosIniciales += 5;
        }
    }

}
