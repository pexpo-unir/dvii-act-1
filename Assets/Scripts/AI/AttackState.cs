using UnityEngine;

namespace AI
{
    // TODO: the attack is executed too quickly when Exit Attack -> Enter Chase -> Exit Chase -> Enter Attack.
    // Check Enter() cooldown
    public class AttackState : State
    {
        private float _attackCooldown;

        public AttackState(StateMachine stateMachine, StateContext context) : base(stateMachine, context)
        {
        }

        public override void Enter()
        {
            _attackCooldown = Context.AttackCooldownTime;
        }

        public override void Update()
        {
            var direction = (Context.Player.transform.position - Context.Owner.transform.position).normalized;

            if (_attackCooldown >= Context.AttackCooldownTime)
            {
                Context.Owner.Attack(direction);

                _attackCooldown = 0f;
            }
            else
            {
                _attackCooldown += Time.deltaTime;
            }

            if (Vector3.Distance(Context.Player.transform.position, Context.Owner.transform.position) >=
                Context.AttackDistance)
            {
                StateMachine.TransitionTo(new ChaseState(StateMachine, Context));
            }
        }

        public override void Exit()
        {
        }
    }
}