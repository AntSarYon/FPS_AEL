using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            GetComponent<PlayerController>().EnPared = true;
            print("Pegado a Pared");
        }
    }

    //-------------------------------------------------------------------------------

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            GetComponent<PlayerController>().EnPared = false;
            print("Se separ� de la Pared");
        }
    }
}
