namespace AI
{
    public class StateMachine
    {
        private State _currentState;

        public bool Enabled { get; set; }

        public void Initialize(State initialState)
        {
            Enabled = true;
            TransitionTo(initialState);
        }

        public void TransitionTo(State nextState)
        {
            _currentState?.Exit();

            _currentState = nextState;
            _currentState.Enter();
        }

        public void Update()
        {
            if (!Enabled)
            {
                return;
            }

            _currentState?.Update();
        }
    }
}