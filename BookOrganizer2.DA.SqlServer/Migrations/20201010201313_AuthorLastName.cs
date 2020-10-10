using Microsoft.EntityFrameworkCore.Migrations;

namespace BookOrganizer2.DA.SqlServer.Migrations
{
    public partial class AuthorLastName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Authors",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Authors");
        }
    }
}
