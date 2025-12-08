using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCenterManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerAvailabilityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TrainerAvailabilities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TrainerAvailabilities");
        }
    }
}
