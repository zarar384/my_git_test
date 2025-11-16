using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Models;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class Left4BonesGame
    {
        private readonly UserInteraction _ui;
        private Player _player = new Player();
        private static Random _rnd = new Random();

        public Left4BonesGame(UserInteraction ui)
        {
            _ui = ui;
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Left4Bones!");

            while (_player.IsAlive)
            {
                Console.WriteLine("\nA new skeleton appears!");
                var skeleton = new Skeleton();
                Fight(skeleton);

                if (!_player.IsAlive)
                {
                    Console.WriteLine("You died. Game over!");
                    break;
                }

                Console.WriteLine($"You have {_player.Gold} gold.");
                Shop();
            }
        }

        private void Fight(Skeleton skeleton)
        {
            while (_player.IsAlive && skeleton.Health > 0)
            {
                Console.WriteLine($"Skeleton HP: {skeleton.Health}, Your HP: {_player.Health}");
                var input = _ui.GetUserInput("Press ENTER to attack...");
                
                skeleton.Health -= _player.Damage;
                Console.WriteLine($"You hit the skeleton for {_player.Damage} damage. Skeleton health: {skeleton.Health}");
                if (skeleton.Health <= 0)
                {
                    var goldEarned = _rnd.Next(5, 20);
                    _player.Gold += goldEarned;
                    Console.WriteLine($"You defeated the skeleton and earned {goldEarned} gold!");
                    break;
                }
                _player.Health -= skeleton.Damage;
                Console.WriteLine($"The skeleton hits you for {skeleton.Damage} damage. Your health: {_player.Health}");
            }
        }

        private void Shop()
        {
            Console.WriteLine("Do you want to buy upgrades? 1-Damage (+5) 50 gold, 2-Health (+20) 30 gold, 0-Nothing");
            var input = _ui.GetUserInput("Your choice:");

            if (input is IntegerInput choice)
            {
                switch (choice.Input)
                {
                    case 1 when _player.Gold >= 50:
                        _player.Damage += 5;
                        _player.Gold -= 50;
                        Console.WriteLine("Damage upgraded!");
                        break;
                    case 2 when _player.Gold >= 30:
                        _player.Health += 20;
                        _player.Gold -= 30;
                        Console.WriteLine("Health upgraded!");
                        break;
                    default:
                        Console.WriteLine("Not enough gold or no upgrade chosen.");
                        break;
                }
            }
        }
    }

    public static class L4BGameRun
    {
        public static void Run()
        {
            var console = new ConsoleShim();
            var ui = new UserInteraction(console);
            var game = new Left4BonesGame(ui);
            game.Run();
        }
    }
}
