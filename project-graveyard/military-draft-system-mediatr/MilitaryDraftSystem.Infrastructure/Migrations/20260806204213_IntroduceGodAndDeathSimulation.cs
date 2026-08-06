using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilitaryDraftSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceGodAndDeathSimulation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PopulationGenerators");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Death_OccurredAt",
                table: "Citizens",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Death_Reason",
                table: "Citizens",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Gods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    GenerationInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    LastGenerationAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    MinCitizensPerGeneration = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCitizensPerGeneration = table.Column<int>(type: "INTEGER", nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentChance = table.Column<int>(type: "INTEGER", nullable: false),
                    CriminalRecordChance = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gods", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gods");

            migrationBuilder.DropColumn(
                name: "Death_OccurredAt",
                table: "Citizens");

            migrationBuilder.DropColumn(
                name: "Death_Reason",
                table: "Citizens");

            migrationBuilder.CreateTable(
                name: "PopulationGenerators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CriminalRecordChance = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    GenerationInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    LastGenerationAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCitizensPerGeneration = table.Column<int>(type: "INTEGER", nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MinCitizensPerGeneration = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentChance = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopulationGenerators", x => x.Id);
                });
        }
    }
}
