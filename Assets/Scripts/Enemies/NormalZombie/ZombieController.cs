using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    //Tendremos una Maquina de Estados Finita (FSM)
    private FSM<ZombieController> mFSM;

    //------------------------------------------------------------------

    void Start()
    {

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        //Creamos un FSM indicando este Script como principal componente
        mFSM = new FSM<ZombieController>(

            //El Estado inicial ser�...
            new Zombie.ZombieIdleState(this)
            );

        // Activamos la m�quina de estados
        mFSM.Begin();
    }

    //---------------------------------------------------------------------------

    void Update()
    {
        
    }
}
