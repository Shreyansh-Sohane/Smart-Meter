﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMeterBackend.Migrations
{
    /// <inheritdoc />
    public partial class imageURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imageurl",
                table: "User",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageurl",
                table: "User");
        }
    }
}
