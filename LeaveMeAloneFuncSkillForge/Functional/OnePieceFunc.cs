namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class OnePieceFunc
    {
        public static string EvaluateCharacterCombatPower(OnePieceCharacterDto character)
        {
            // create nuanced combat evaluation
            return character switch
            {
                // Legendary Devil Fruit users with ultra high bounty and rare special move get ultimate rank
                {
                    DevilFruit: { Length: > 0 }, // has a devil fruit 
                    Bounty: >= 1_000_000_000,   // bounty >= 1 billion
                    Rarity: "Legendary" or "Mythic", // rarity is Legendary or Mythic
                    SpecialMove: not null and not ""  // has a special move
                } =>
                    $"🔥 {character.Name} is a top-tier Devil Fruit user! " +
                    $"With a bounty of {character.Bounty:N0} and the devastating move '{character.SpecialMove}', " +
                    $"their damage is calculated at {CalculateDamage(character)} with critical chance at {CalculateCritChance(character)}%. " +
                    $"Dodge ability stands at {CalculateDodgeChance(character)}%.",

                // High bounty fighters without devil fruit but rare rarity
                {
                    DevilFruit: null or "",
                    Bounty: >= 500_000_000,
                    Rarity: "Epic" or "Rare"
                } =>
                    $"⚔️ {character.Name} is a formidable fighter with a bounty of {character.Bounty:N0} and no Devil Fruit. " +
                    $"Damage potential: {CalculateDamage(character)}, Crit Chance: {CalculateCritChance(character)}%, Dodge Chance: {CalculateDodgeChance(character)}%.",

                // Characters with SpecialMove but low bounty, evaluated as tactical fighters
                {
                    SpecialMove: not null and not "",
                    Bounty: < 500_000_000,
                    Rarity: "Uncommon" or "Common"
                } =>
                    $"🛡️ Tactical fighter {character.Name} uses '{character.SpecialMove}' skillfully. " +
                    $"Damage: {CalculateDamage(character)}, Crit: {CalculateCritChance(character)}%, Dodge: {CalculateDodgeChance(character)}%.",

                // Devil Fruit users with medium bounty and uncommon rarity but without special move
                {
                    DevilFruit: { Length: > 0 },
                    Bounty: >= 100_000_000 and < 1_000_000_000,
                    Rarity: "Uncommon" or "Rare",
                    SpecialMove: null or ""
                } =>
                    $"🍎 {character.Name} harnesses the Devil Fruit power effectively. " +
                    $"Damage output: {CalculateDamage(character)}, Crit Chance: {CalculateCritChance(character)}%, Dodge Chance: {CalculateDodgeChance(character)}%.",

                // no special powers or low bounty
                _ =>
                    $"{character.Name} is an average fighter with basic abilities. " +
                    $"Damage: {CalculateDamage(character)}, Crit Chance: {CalculateCritChance(character)}%, Dodge Chance: {CalculateDodgeChance(character)}%."
            };


            // сompute damage based on bounty, rarity and special move presence
            int CalculateDamage(OnePieceCharacterDto c)
            {
                int baseDamage = c.Damage;

                // Increase damage for rarity tiers
                int rarityMultiplier = c.Rarity switch
                {
                    "Common" => 1,
                    "Uncommon" => 2,
                    "Rare" => 3,
                    "Epic" => 5,
                    "Legendary" => 8,
                    "Mythic" => 13,
                    _ => 1
                };

                // bonus damage if special move exists
                int specialMoveBonus = string.IsNullOrEmpty(c.SpecialMove) ? 0 : 50;

                // Bounty scaling factor (logarithmic)
                double bountyScale = Math.Log10(c.Bounty + 1);

                // final damage calculation
                return (int)(baseDamage * rarityMultiplier + specialMoveBonus + bountyScale * 10);
            }

            // compute critical chance
            int CalculateCritChance(OnePieceCharacterDto c)
            {
                int baseCrit = c.CritChance;

                // devil Fruit users gain crit chance bonus depending on rarity
                int dfBonus = !string.IsNullOrEmpty(c.DevilFruit) ? (c.Rarity switch
                {
                    "Common" => 2,
                    "Uncommon" => 5,
                    "Rare" => 8,
                    "Epic" => 12,
                    "Legendary" => 18,
                    "Mythic" => 25,
                    _ => 0
                }) : 0;

                // special move adds critical precision
                int specialCritBonus = string.IsNullOrEmpty(c.SpecialMove) ? 0 : 7;

                return Math.Min(100, baseCrit + dfBonus + specialCritBonus);
            }

            // compute dodge chance
            int CalculateDodgeChance(OnePieceCharacterDto c)
            {
                int baseDodge = c.DodgeChance;

                // rare and above get dodge bonus
                int rarityDodgeBonus = c.Rarity switch
                {
                    "Rare" => 5,
                    "Epic" => 10,
                    "Legendary" => 15,
                    "Mythic" => 20,
                    _ => 0
                };

                // devil fruit users gain dodge from their special moves evasiveness
                int specialDodgeBonus = string.IsNullOrEmpty(c.SpecialMove) ? 0 : 8;

                // lower bounty means more evasive and tricky fighters
                int bountyPenalty = c.Bounty switch
                {
                    >= 1_000_000_000 => -10,
                    >= 500_000_000 => -5,
                    _ => 0
                };

                int finalDodge = baseDodge + rarityDodgeBonus + specialDodgeBonus + bountyPenalty;

                return Math.Clamp(finalDodge, 0, 100);
            }
        }
    }
}
