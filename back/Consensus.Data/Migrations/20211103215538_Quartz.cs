using Microsoft.EntityFrameworkCore.Migrations;

namespace Consensus.Data.Migrations
{
    public partial class Quartz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var resourcePath = "Consensus.Data.Migrations.20211103215538_Quartz.sql";
            using var stream = GetType().Assembly.GetManifestResourceStream(resourcePath);
            using var streamReader = new StreamReader(stream);
            migrationBuilder.Sql(streamReader.ReadToEnd());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
