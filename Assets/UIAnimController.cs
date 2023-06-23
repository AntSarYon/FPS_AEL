using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIAnimController : MonoBehaviour
{
    private Animator mAnimator;

    public Animator MAnimator { get => mAnimator; set => mAnimator = value; }

    private void Start()
    {
        mAnimator = GetComponent<Animator>();

        //Esta sera la UIController que el GameManager gestionara
        GameManager.Instance.UiController = this;
    }

    //-----------------------------------------------------------

    public void PosicionarTransicionDetras()
    {
        transform.Find("Transition").SetAsFirstSibling();
    }

    public void PosicionarTransicionDelante()
    {
        transform.Find("Transition").SetAsLastSibling();
    }

    //--------------------------------------------------------------

    public void IrACreditosFinales()
    {
        SceneManager.LoadScene("ThrillerEnding");
    }

    public void IrAMainMenu()
    {
        SceneManager.LoadScene("IntroMenu");
    }
}
