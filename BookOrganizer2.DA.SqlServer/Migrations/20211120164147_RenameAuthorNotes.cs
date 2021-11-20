using Microsoft.EntityFrameworkCore.Migrations;

namespace BookOrganizer2.DA.SqlServer.Migrations
{
    public partial class RenameAuthorNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Authors",
                newName: "NotesOld");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotesOld",
                table: "Authors",
                newName: "Notes");
        }
    }
}
