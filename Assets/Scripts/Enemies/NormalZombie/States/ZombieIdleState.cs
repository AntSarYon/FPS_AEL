using Crawler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Zombie 
{
    public class ZombieIdleState : FSMState<ZombieController>
    {
        public ZombieIdleState(ZombieController controller) : base(controller)
        {
            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo entró a la zona de Ataque, y no estamos atacando
                    return mController.PlayerAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Running
                    return new ZombieAttackingState(mController);
                }));

            //- - - - - -- - - - - - - - - - - - - - - - -- - - - -  -

            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo entró a la zona de Deteccion, y no estamos atacando
                    return mController.PlayerDetectionCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Running
                    return new ZombieRunningState(mController);
                }));

            //- - - - - -- - - - - - - - - - - - - - - - -- - - - -  -

            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo no esta en la zona de Deteccion, ni en la de ataque, y no estamos atacando
                    return mController.Health <= 0;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new ZombieDyingState(mController);
                }));
        }

        public override void OnEnter()
        {
            //Desactivamos el Flag de animacion para el movimiento
            //mController.MAnimator.SetBool("IsWalking", false);
        }

        public override void OnExit()
        {
            //throw new System.NotImplementedException();
        }

        public override void OnUpdate(float deltaTime)
        {
            //Desactivamos el Flag de animacion para el movimiento
            mController.MAnimator.SetBool("IsWalking", false);

            //Asignamos velocidad a 0
            mController.MRb.velocity = Vector3.zero;

            //Detenemos al NavMeshAgent
            mController.NavMeshAgent.isStopped = true;

            //Reiniciamos la Ruta del NavMesh
            mController.NavMeshAgent.ResetPath();
        }
    }
}
