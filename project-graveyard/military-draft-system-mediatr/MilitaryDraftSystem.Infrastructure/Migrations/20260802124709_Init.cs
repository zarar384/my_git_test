using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilitaryDraftSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutomaticRecruitmentAgents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExecutionInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    LastExecutionAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomaticRecruitmentAgents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Citizens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    MedicalCategory = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    HasCriminalRecord = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsStudent = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citizens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PopulationGenerators",
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
                    table.PrimaryKey("PK_PopulationGenerators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentOfficers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentOfficers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Summonses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CitizenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    RecruitmentOfficerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AutomaticRecruitmentAgentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IssuedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summonses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutomaticRecruitmentAgents");

            migrationBuilder.DropTable(
                name: "Citizens");

            migrationBuilder.DropTable(
                name: "PopulationGenerators");

            migrationBuilder.DropTable(
                name: "RecruitmentOfficers");

            migrationBuilder.DropTable(
                name: "Summonses");
        }
    }
}
