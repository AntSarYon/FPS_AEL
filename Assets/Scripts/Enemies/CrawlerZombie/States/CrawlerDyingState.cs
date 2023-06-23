using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crawler
{
    public class CrawlerDyingState : FSMState<CrawlerController>
    {
        private float tiempoMuerte;

        public CrawlerDyingState(CrawlerController controller) : base(controller)
        {

        }

        public override void OnEnter()
        {
            //Desactivamos el Flag de animacion para el movimiento
            mController.MAnimator.SetBool("IsWalking", false);

            //Asignamos velocidad a 0
            mController.MRb.velocity = Vector3.zero;

            //Detenemos al NavMeshAgent
            mController.NavMeshAgent.isStopped = true;

            //Reiniciamos la Ruta del NavMesh
            mController.NavMeshAgent.ResetPath();

            //Inicializamos el Tiempo de muerte en 0
            tiempoMuerte = 0;

            //Activamos el Flag de animacion para la Muerte (en base al Aleatorio)
            mController.MAnimator.SetTrigger("Dying");
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float deltaTime)
        {
            //Si el tiempo de animacion de muerte ya apso los 5 segundos...
            if (tiempoMuerte >= 5F)
            {
                //Destruimos el Zombie
                GameObject.Destroy(mController.gameObject);
            }

            //Sino
            else
            {
                //Incrementamos el Tiempo de Muerte
                tiempoMuerte += deltaTime;
            }
        }
    }
}

