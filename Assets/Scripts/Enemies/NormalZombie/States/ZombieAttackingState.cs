using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Zombie
{
    public class ZombieAttackingState : FSMState<ZombieController>
    {
        public ZombieAttackingState(ZombieController controller) : base(controller)
        {
            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo no esta en la zona de Deteccion, ni en la de ataque, y no estamos atacando
                    return mController.PlayerDetectionCollider == null && mController.PlayerAttackCollider == null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new ZombieIdleState(mController);
                }));

            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo no esta en la zona de Deteccion, ni en la de ataque, y no estamos atacando
                    return mController.PlayerAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new ZombieAttackingState(mController);
                }));

            Transitions.Add(new FSMTransition<ZombieController>(
                isValid: () => {
                    //Si el Enemigo no esta en la zona de Deteccion, ni en la de ataque, y no estamos atacando
                    return mController.PlayerDetectionCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new ZombieRunningState(mController);
                }));

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
            //Volteamos en direccion al Player
            mController.transform.LookAt(mController.PlayerAttackCollider.transform);

            //Desactivamos el Flag de Animacion para Caminar
            mController.MAnimator.SetBool("IsWalking", false);

            //Disparamos el Trigger de Ataque
            mController.MAnimator.SetTrigger("Attack");

            mController.MAudioSource.PlayOneShot(mController.clipAtacando, 0.30f);
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdate(float deltaTime)
        {
            //Nos detenemos asignando la velocididad en 0
            mController.MRb.velocity = new Vector3(
                0f,
                0f,
                0f
            );

            //Detenemos al NavMeshAgent
            mController.NavMeshAgent.isStopped = true;
        }
    }
}
