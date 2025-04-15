using UnityEngine;

namespace AI
{
    public class ChaseState : State
    {
        public ChaseState(StateMachine stateMachine, StateContext context) : base(stateMachine, context)
        {
        }

        public override void Enter()
        {
        }

        public override void Update()
        {
            Context.Owner.MoveTo(Context.Player.transform.position);

            if (Vector3.Distance(Context.Player.transform.position, Context.Owner.transform.position) <
                Context.AttackDistance)
            {
                StateMachine.TransitionTo(new AttackState(StateMachine, Context));
            }
        }

        public override void Exit()
        {
            Context.Owner.StopMovement();
        }
    }
}