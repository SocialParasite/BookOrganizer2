using Microsoft.EntityFrameworkCore.Migrations;

namespace BookOrganizer2.DA.SqlServer.Migrations
{
	public partial class GetBooksWithoutDescriptionSP : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
				CREATE PROC GetBooksWithoutDescription
				AS
				SET NOCOUNT ON;
				
				BEGIN
					SELECT	Title
					FROM	Books
					WHERE	datalength(Description) = 0
					OR		Description is null
				END"
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP PROC IF EXISTS GetBooksWithoutDescription");
		}
	}
}
