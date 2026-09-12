using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilitaryDraftSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenAndOfficerLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRetired",
                table: "RecruitmentOfficers",
                newName: "Status");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Death_OccurredAt",
                table: "RecruitmentOfficers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Death_Reason",
                table: "RecruitmentOfficers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndReason",
                table: "RecruitmentOfficers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerId",
                table: "RecruitmentOfficers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StatusChangedAt",
                table: "RecruitmentOfficers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Citizens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NaturalLifespanYears",
                table: "Citizens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CemeteryRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubjectType = table.Column<int>(type: "INTEGER", nullable: false),
                    SubjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<int>(type: "INTEGER", nullable: false),
                    DiedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    OriginalRecordDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CemeteryRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeathStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubjectType = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeathStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkerLifecycleStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Reason = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerLifecycleStatistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CemeteryRecords");

            migrationBuilder.DropTable(
                name: "DeathStatistics");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "WorkerLifecycleStatistics");

            migrationBuilder.DropColumn(
                name: "Death_OccurredAt",
                table: "RecruitmentOfficers");

            migrationBuilder.DropColumn(
                name: "Death_Reason",
                table: "RecruitmentOfficers");

            migrationBuilder.DropColumn(
                name: "EndReason",
                table: "RecruitmentOfficers");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "RecruitmentOfficers");

            migrationBuilder.DropColumn(
                name: "StatusChangedAt",
                table: "RecruitmentOfficers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Citizens");

            migrationBuilder.DropColumn(
                name: "NaturalLifespanYears",
                table: "Citizens");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "RecruitmentOfficers",
                newName: "IsRetired");
        }
    }
}
