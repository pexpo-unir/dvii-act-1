namespace AI
{
    public abstract class State
    {
        protected readonly StateMachine StateMachine;

        protected readonly StateContext Context;

        protected State(StateMachine stateMachine, StateContext context)
        {
            StateMachine = stateMachine;
            Context = context;
        }

        public abstract void Enter();

        public abstract void Update();

        public abstract void Exit();
    }
}