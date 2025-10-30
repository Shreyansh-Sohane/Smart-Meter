using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMeterBackend.Migrations
{
    /// <inheritdoc />
    public partial class addedConsumerPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "passwordhash",
                table: "consumer",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passwordhash",
                table: "consumer");
        }
    }
}
