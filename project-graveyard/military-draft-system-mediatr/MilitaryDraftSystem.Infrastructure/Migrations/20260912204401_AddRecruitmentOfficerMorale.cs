using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilitaryDraftSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruitmentOfficerMorale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuiltIncidentsCount",
                table: "RecruitmentOfficers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoralePercent",
                table: "RecruitmentOfficers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuiltIncidentsCount",
                table: "RecruitmentOfficers");

            migrationBuilder.DropColumn(
                name: "MoralePercent",
                table: "RecruitmentOfficers");
        }
    }
}
