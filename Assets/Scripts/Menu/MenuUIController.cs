using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    private GameManager gameManager;

    private Animator mAnimator;
    private AudioSource mAudioSource;

    [SerializeField] private TextMeshProUGUI txtCantEnemigos;

    [SerializeField] private AudioClip clipDisparo;
    [SerializeField] private AudioClip clipClick;
    [SerializeField] private AudioClip clipCharger;


    //----------------------------------------------------

    void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        //Desbloqueamos el Cursor
        Cursor.lockState = CursorLockMode.None;
    }

    //----------------------------------------------------
    private void LateUpdate()
    {
        //Mostramos siempre la cantidad de enemigos de GameManager
        txtCantEnemigos.text = gameManager.cantEnemigosIniciales.ToString();
    }

    //------------------------------------------------------------------

    public void IniciarFadeIn()
    {
        //Iniciamos la Animacion de FadeIn
        mAnimator.SetBool("FadeIn", true);
    }

    //--------------------------------------------------------

    public void StartGame()
    {
        //Cargamos el nivel Principal
        SceneManager.LoadScene("MainScene");
    }

    //-----------------------------------------------------------

    public void ReducirEnemigos()
    {
        gameManager.DisminuirCantEnemigos();
    }

    public void AumentarEnemigos()
    {
        gameManager.IncrementarCantEnemigos();
    }

    //-----------------------------------------------------------
    public void ReproducirDisparo()
    {
        mAudioSource.PlayOneShot(clipDisparo, 0.70f);
    }

    public void ReproducirClick()
    {
        mAudioSource.PlayOneShot(clipClick, 0.70f);
    }

    public void ReproducirCharger()
    {
        mAudioSource.PlayOneShot(clipCharger, 0.70f);
    }
}
