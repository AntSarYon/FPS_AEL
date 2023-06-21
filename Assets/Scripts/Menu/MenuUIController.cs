using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    private Animator mAnimator;
    private AudioSource mAudioSource;

    [SerializeField] private AudioClip clipDisparo;
    [SerializeField] private AudioClip clipCharger;


    //----------------------------------------------------

    void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();
    }//

    //-----------------------------------------------------

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

    public void ReproducirDisparo()
    {
        mAudioSource.PlayOneShot(clipDisparo, 0.70f);
    }

    public void ReproducirCharger()
    {
        mAudioSource.PlayOneShot(clipCharger, 0.70f);
    }
}
