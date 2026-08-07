using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAnalysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDiseaseFieldForLEthalityIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "LethalityIndex",
                table: "diseases",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LethalityIndex",
                table: "diseases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
