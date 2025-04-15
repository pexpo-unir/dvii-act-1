namespace AI
{
    public class StateContext
    {
        public BearEnemy Owner { get; set; }

        public CharacterBase Player { get; set; }

        public float AttackDistance { get; set; }

        public float AttackCooldownTime { get; set; }
    }
}