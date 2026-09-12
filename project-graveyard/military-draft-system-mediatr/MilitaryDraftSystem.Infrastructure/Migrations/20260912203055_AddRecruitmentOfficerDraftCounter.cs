using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilitaryDraftSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruitmentOfficerDraftCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DraftedCitizensCount",
                table: "RecruitmentOfficers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DraftedCitizensCount",
                table: "RecruitmentOfficers");
        }
    }
}
