using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS.Customers.API.Migrations
{
    /// <inheritdoc />
    public partial class Add_Normalzied_Name_Columns_Customers_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedLegalName",
                table: "Customers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTradeName",
                table: "Customers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedLegalName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "NormalizedTradeName",
                table: "Customers");
        }
    }
}
