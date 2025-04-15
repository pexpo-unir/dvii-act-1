namespace AI
{
    public class StateContext
    {
        public BearEnemy Owner { get; set; }

        public PlayerCharacter Player { get; set; }

        public float AttackDistance { get; set; }

        public float AttackCooldownTime { get; set; }
    }
}