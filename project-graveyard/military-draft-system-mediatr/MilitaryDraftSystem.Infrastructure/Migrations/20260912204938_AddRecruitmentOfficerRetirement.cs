using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilitaryDraftSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruitmentOfficerRetirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRetired",
                table: "RecruitmentOfficers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRetired",
                table: "RecruitmentOfficers");
        }
    }
}
