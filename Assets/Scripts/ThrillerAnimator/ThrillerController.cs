using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrillerController : MonoBehaviour
{
    private Animator mAnimator;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
    }

    public void DispararParteII()
    {
        mAnimator.SetTrigger("StartPart2");
    }
    public void DispararParteIII()
    {
        mAnimator.SetTrigger("StartPart3");
    }
    public void DispararParteIV()
    {
        mAnimator.SetTrigger("StartPart4");
    }
}
