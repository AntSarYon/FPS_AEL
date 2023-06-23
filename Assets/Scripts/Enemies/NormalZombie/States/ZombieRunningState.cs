using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Zombie
{
    public class ZombieRunningState : FSMState<ZombieController>
    {
        private float tiempoTrotando;

        public ZombieRunningState(ZombieController controller) : base(controller)
        {

            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo entró a la zona de Ataque, y no estamos atacando
                    return mController.PlayerAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Attacking
                    return new ZombieAttackingState(mController);
                }));

            //- - - - - -- - - - - - - - - - - - - - - - -- - - - -  -

            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo no esta en la zona de Deteccion, ni en la de ataque, y no estamos atacando
                    return mController.PlayerDetectionCollider == null && mController.PlayerAttackCollider == null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new ZombieIdleState(mController);
                }));

            //- - - - - -- - - - - - - - - - - - - - - - -- - - - -  -

            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo no esta en la zona de Deteccion, ni en la de ataque, y no estamos atacando
                    return mController.Health <= 0;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Dying
                    return new ZombieDyingState(mController);
                }));
        }

        public override void OnEnter()
        {
            //Activamose el Flag de Animación para Caminar
            //mController.MAnimator.SetBool("IsWalking", true);

            //Inicializamos el Tiempo trotando
            tiempoTrotando = 0;

            //La velocidad inicia del NavMeshAgent sera de 2
            mController.NavMeshAgent.speed = 2;
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float deltaTime)
        {
            //Activamose el Flag de Animación para Caminar
            mController.MAnimator.SetBool("IsWalking", true);

            //Indicamos que el NavMeshAgent está en movimiento
            mController.NavMeshAgent.isStopped = false;

            //Asignamos que el destino del NavMeshAgent sea la pisición del Jugador
            mController.NavMeshAgent.SetDestination(mController.PlayerDetectionCollider.transform.position);

            

            //Si llevan más de 3 segundos trotando
            if (tiempoTrotando >= 3)
            {
                //Activamos el Trigger para empezar la animacion de correr
                mController.MAnimator.SetTrigger("StartRunning");

                //Aumentamos la velocidad a 3
                mController.NavMeshAgent.speed = 3;
            }
            else
            {
                //Incrementamos el tiempo Trotando
                tiempoTrotando += deltaTime;
            }


        }
    }
}

