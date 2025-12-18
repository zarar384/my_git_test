public record CombatStats(int Damage, int CritChance, int DodgeChance);

public static class OnePieceFunc
{
    public static CombatStats GetBaseCombatStats(OnePieceCharacterDto c)
    {
        int CalculateDamage(OnePieceCharacterDto ch)
        {
            int baseDamage = ch.Damage;
            int rarityMultiplier = ch.Rarity switch
            {
                "Common" => 1,
                "Uncommon" => 2,
                "Rare" => 3,
                "Epic" => 5,
                "Legendary" => 8,
                "Mythic" => 13,
                _ => 1
            };
            int specialMoveBonus = string.IsNullOrEmpty(ch.SpecialMove) ? 0 : 50;
            double bountyScale = Math.Log10(ch.Bounty + 1);
            return (int)(baseDamage * rarityMultiplier + specialMoveBonus + bountyScale * 10);
        }

        int CalculateCritChance(OnePieceCharacterDto ch)
        {
            int baseCrit = ch.CritChance;
            int dfBonus = !string.IsNullOrEmpty(ch.DevilFruit) ? (ch.Rarity switch
            {
                "Common" => 2,
                "Uncommon" => 5,
                "Rare" => 8,
                "Epic" => 12,
                "Legendary" => 18,
                "Mythic" => 25,
                _ => 0
            }) : 0;
            int specialCritBonus = string.IsNullOrEmpty(ch.SpecialMove) ? 0 : 7;
            return Math.Min(100, baseCrit + dfBonus + specialCritBonus);
        }

        int CalculateDodgeChance(OnePieceCharacterDto ch)
        {
            int baseDodge = ch.DodgeChance;
            int rarityDodgeBonus = ch.Rarity switch
            {
                "Rare" => 5,
                "Epic" => 10,
                "Legendary" => 15,
                "Mythic" => 20,
                _ => 0
            };
            int specialDodgeBonus = string.IsNullOrEmpty(ch.SpecialMove) ? 0 : 8;
            int bountyPenalty = ch.Bounty switch
            {
                >= 1_000_000_000 => -10,
                >= 500_000_000 => -5,
                _ => 0
            };
            return Math.Clamp(baseDodge + rarityDodgeBonus + specialDodgeBonus + bountyPenalty, 0, 100);
        }

        return new CombatStats(CalculateDamage(c), CalculateCritChance(c), CalculateDodgeChance(c));
    }

    public static Func<bool, string, string, string, IEnumerable<OnePieceCharacterDto>>
       ParseOnePieceCharacters =>
           (skipHeader, lineBreak, delimiter, fileName) =>
           {
               var filePath = Path.Combine("Resources", fileName);
               var data = File.ReadAllText(filePath);

               return data
                   .Split(lineBreak)
                   .Skip(skipHeader ? 1 : 0)
                   .Where(line => !string.IsNullOrWhiteSpace(line))
                   .Select(x => x.Split(delimiter))
                   .Select(x => new OnePieceCharacterDto
                   {
                       Name = x[0],
                       Role = x[1],
                       DevilFruit = x[2],
                       CrewName = x[3],
                       Bounty = long.Parse(x[4]),
                       Damage = int.Parse(x[5]),
                       CritChance = int.Parse(x[6]),
                       DodgeChance = int.Parse(x[7]),
                       Rarity = x[8],
                       SpecialMove = x[9]
                   });
           };
}
