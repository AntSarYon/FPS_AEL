using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Crawler
{
    public class CrawlerMovingState : FSMState<CrawlerController>
    {
        public CrawlerMovingState(CrawlerController controller) : base(controller)
        {
            Transitions.Add(new FSMTransition<CrawlerController>(
                isValid: () => {
                    //Si el Enemigo no está en la zona de deteccion
                    return mController.PlayerCollider == null;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDDLE
                    return new CrawlerIdleState(mController);
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
            //Activamos los HitBoxes
            mController.EnableHitbox();

            //Activamose l Flag de Animación para Caminar
            mController.MAnimator.SetBool("IsWalking", true);

            mController.MAudioSource.PlayOneShot(mController.clipJugadorCerca, 0.30f);
        }

        public override void OnExit()
        {
            //Desactivamos los HitBoxes
            mController.DisableHitbox();
            //Activamose l Flag de Animación para Caminar
            mController.MAnimator.SetBool("IsWalking", false);
        }

        public override void OnUpdate(float deltaTime)
        {

            //Indicamos que el NavMeshAgent está en movimiento
            mController.NavMeshAgent.isStopped = false;

            //Asignamos que el destino del NavMeshAgent sea la pisición del Jugador
            mController.NavMeshAgent.SetDestination(mController.PlayerCollider.transform.position);

        }
    }
}