using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS.Customers.API.Migrations
{
    /// <inheritdoc />
    public partial class Add_RegistrationNumber_Index_Customers_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_RegistrationNumber",
                table: "Customers",
                column: "RegistrationNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_RegistrationNumber",
                table: "Customers");
        }
    }
}
