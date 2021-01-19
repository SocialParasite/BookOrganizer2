using Microsoft.EntityFrameworkCore.Migrations;

namespace BookOrganizer2.DA.SqlServer.Migrations
{
	public partial class GetMonthlyReadsReport : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
				CREATE PROC GetMonthlyReads
				@year	AS INT,
				@month	AS INT
				AS
				SET NOCOUNT ON;
				
				BEGIN
					SELECT		b.Title, br.ReadDate
					FROM		Books b
					JOIN		BookReadDate br ON BookId = b.Id
					WHERE		YEAR(br.ReadDate) = @year
					AND			MONTH(br.ReadDate) = @month
				END"
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP PROC IF EXISTS GetMonthlyReads");
		}
	}
}
