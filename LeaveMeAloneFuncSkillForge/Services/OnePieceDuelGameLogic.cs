using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class State
    {
        public OnePieceCharacterDto Player { get; set; }
        public OnePieceCharacterDto Enemy { get; set; }
        public List<string> Log { get; set; } = new();
    }

    public class OnePieceDuelGameLogic
    {
        private static readonly Random random = new();

        public static Func<OnePieceCharacterDto, int> CalculateDamage = character =>
        {
            var stats = OnePieceFunc.GetBaseCombatStats(character);
            var critMultiplier = random.Next(0, 100) < stats.CritChance ? 1.5 : 1.0;
            return (int)(stats.Damage * critMultiplier);
        };

        public static Func<OnePieceCharacterDto, bool> IsDodged = character =>
            random.Next(0, 100) < OnePieceFunc.GetBaseCombatStats(character).DodgeChance;

        // Player turn
        public static Func<State, State> PlayerAttack = state =>
        {
            var dmg = IsDodged(state.Enemy) ? 0 : CalculateDamage(state.Player);
            state.Enemy.HP -= dmg;
            state.Log.Add($"{state.Player.Name} attacks {state.Enemy.Name} and deals {dmg} damage!");
            return state;
        };

        // Enemy move
        public static Func<State, State> EnemyAttack = state =>
        {
            var dmg = IsDodged(state.Player) ? 0 : CalculateDamage(state.Enemy);
            state.Player.HP -= dmg;
            state.Log.Add($"{state.Enemy.Name} attacks {state.Player.Name} and deals {dmg} damage!");
            return state;
        };

        // Full turn
        public static Func<State, State> BattleTurn = state =>
            PlayerAttack(EnemyAttack(state));
    }
}
