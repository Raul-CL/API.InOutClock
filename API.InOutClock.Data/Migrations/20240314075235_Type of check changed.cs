using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.InOutClock.Data.Migrations
{
    /// <inheritdoc />
    public partial class Typeofcheckchanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "RecordEvaluation",
                table: "Checks",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RecordEvaluation",
                table: "Checks",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
