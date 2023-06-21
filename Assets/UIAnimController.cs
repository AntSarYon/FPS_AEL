using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimController : MonoBehaviour
{
    public void PosicionarTransicionDetras()
    {
        transform.Find("Transition").SetAsFirstSibling();
    }
}
