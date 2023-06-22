using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyIdleState : FSMState<EnemyController>
    {
        public EnemyIdleState(EnemyController controller) : base(controller)
        {

        }

        public override void OnEnter()
        {
            //throw new System.NotImplementedException();
        }

        public override void OnExit()
        {
            //throw new System.NotImplementedException();
        }

        public override void OnUpdate(float deltaTime)
        {
            //throw new System.NotImplementedException();
        }
    }
}
