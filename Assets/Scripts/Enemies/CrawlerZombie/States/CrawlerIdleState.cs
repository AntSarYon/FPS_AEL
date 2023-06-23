using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Crawler 
{
    public class CrawlerIdleState : FSMState<CrawlerController>
    {
        public CrawlerIdleState(CrawlerController controller) : base(controller)
        {
            Transitions.Add(new FSMTransition<CrawlerController>(
                isValid: () => {
                    //Si el Enemigo entró a la zona de Deteccion
                    return mController.PlayerCollider != null;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Moving
                    return new CrawlerMovingState(mController);
                }));

            Transitions.Add(new FSMTransition<CrawlerController>(
                isValid: () => {
                    //Si el Enemigo entró a la zona de Deteccion
                    return mController.Health <= 0;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Moving
                    return new CrawlerDyingState(mController);
                }));
        }

        public override void OnEnter()
        {
            //Desactivamos los Hitbox
            mController.DisableHitbox();
            //Desactivamos el Flag de animacion para el movimiento
            mController.MAnimator.SetBool("IsWalking", false);
        }

        public override void OnExit()
        {
            //throw new System.NotImplementedException();
        }

        public override void OnUpdate(float deltaTime)
        {
            // NOS DETENEMOS

            //Asignamos velocidad a 0
            mController.MRb.velocity = Vector3.zero;

            //Detenemos al NavMeshAgent
            mController.NavMeshAgent.isStopped = true;

            //Reiniciamos la Ruta del NavMesh
            mController.NavMeshAgent.ResetPath();
        }
    }
}

