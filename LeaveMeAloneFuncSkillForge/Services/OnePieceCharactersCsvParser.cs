namespace LeaveMeAloneFuncSkillForge.Services
{
    public class OnePieceCharactersCsvParser
    {
        private readonly string _filePath;

        public OnePieceCharactersCsvParser(string? filePath = null)
        {
            if (filePath == null)
            {
                filePath = Path.Combine("Resources", "OnePieceCharacters.csv");
            }

            _filePath = filePath;
        }

        public List<OnePieceCharacterDto> GetDataFromCsv()
        {
            var result = File.ReadAllText(_filePath)
                    .Split(Environment.NewLine)
                    .Skip(1)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(x => x.Split(",").ToArray())
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
                    })
                    .ToList();

            return result;
        }

        public void CalculateReport()
        {
            //var text = File.ReadAllText(_filePath);

            //var splitLines = text.Split(Environment.NewLine);
            //var splitLinesAndFields = splitLines.Select(x=>x.Split(",").ToArray());

            // Name,Role,DevilFruit,CrewName,Bounty,Damage,CritChance,DodgeChance,Rarity,SpecialMove
            //var parseData = splitLinesAndFields.Select(x => new OnePieceCharacterDto
            //{
            //    Name = x[0],
            //    Role = x[1],
            //    DevilFruit = x[2],
            //    CrewName = x[3],
            //    Bounty = long.Parse(x[4]),
            //    Damage = int.Parse(x[5]),
            //    CritChance = int.Parse(x[6]),
            //    DodgeChance = int.Parse(x[7]),
            //    Rarity = x[8],
            //    SpecialMove = x[9]
            //});

            //var groupedByCrewName = parseData.GroupBy(x => x.CrewName);

            //var aggregatedCrewStats = groupedByCrewName.Select(g =>
            //    g.Aggregate(
            //        (Crew: g.Key, Count: 0, TotalBounty: 0L, TotalDamage: 0, TotalCrit: 0),
            //        (acc, dto) => (
            //            acc.Crew,
            //            acc.Count + 1, 
            //            acc.TotalBounty + dto.Bounty,
            //            acc.TotalDamage + dto.Damage,
            //            acc.TotalCrit + dto.CritChance
            //        )
            //    )
            //);

            //var crewReports = aggregatedCrewStats.Select(x => new CrewReport
            //{
            //    CrewName = x.Crew,
            //    CharacterCount = x.Count,
            //    TotalBounty = x.TotalBounty,
            //    AverageDamage = x.Count > 0 ? (double)x.TotalDamage / x.Count : 0,
            //    AverageCritChance = x.Count > 0 ? (double)x.TotalCrit / x.Count : 0,
            //});

            //var reportTextLines = crewReports.Select(x =>
            //    $"{x.CrewName}\t {x.CharacterCount}\t {x.TotalBounty}\t {x.AverageDamage}\t {x.AverageCritChance}");

            // Read CSV file, skip header and empty lines, split by comma, group by crew name,
            // aggregate count, total bounty, total damage, and total crit chance,
            // then calculate and format average damage and crit chance per crew.
            var reportTextLines = File.ReadAllText(_filePath)
                    .Split(Environment.NewLine)
                    .Skip(1)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(x => x.Split(",").ToArray())
                    .GroupBy(x => x[3]) // byCrew
                    .Select(x =>
                x.Aggregate((Crew: x.Key, Count: 0, TotalBounty: 0L, TotalDamage: 0, TotalCrit: 0),
                (acc, val) => (
                        acc.Crew,
                        acc.Count + 1,
                        acc.TotalBounty + long.Parse(val[4]),
                        acc.TotalDamage + int.Parse(val[5]),
                        acc.TotalCrit + int.Parse(val[6])
                    )
                )
              ).Select(x => $"{x.Crew}\t {x.Count}\t {x.TotalBounty}\t {(x.Count > 0 ? (double)x.TotalDamage / x.Count : 0)}\t {(x.Count > 0 ? (double)x.TotalCrit / x.Count : 0)}");

            var reportBody = string.Join(Environment.NewLine, reportTextLines);
            var reportHeader = "Crew Name\t Character Count\t Total Bounty\t Average Damage\t Average Crit Chance";

            var finalCrewReport = $"{reportHeader}{Environment.NewLine}{reportBody}";

            Console.WriteLine(finalCrewReport);
        }

        public void RunTest()
        {
            var csvParser = new OnePieceCharactersCsvParser();
            var onePieceCharacters = csvParser.GetDataFromCsv();

            onePieceCharacters.ForEach(character =>
            {
                var combatPower = OnePieceFunc.GetBaseCombatStats(character);

                Console.WriteLine(combatPower);
            });
        }
    }

    internal class CrewReport
    {
        public string CrewName { get; set; }
        public int CharacterCount { get; set; }
        public long TotalBounty { get; set; }
        public double AverageDamage { get; set; }
        public double AverageCritChance { get; set; }
    }
}
