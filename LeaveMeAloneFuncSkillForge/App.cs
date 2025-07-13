
namespace LeaveMeAloneFuncSkillForge
{
    public static class App
    {
        public static void RunApp()
        {
            var csvParser = new OnePieceCharactersCsvParser();
            var onePieceCharacters = csvParser.GetDataFromCsv();

            onePieceCharacters.ForEach(character =>
            {
                var combatPower = OnePieceFunc.EvaluateCharacterCombatPower(character);

                Console.WriteLine(combatPower);
            });
        }
    }
}
