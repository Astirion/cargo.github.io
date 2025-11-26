using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoGo.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTravelersAndSenders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Senders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    From = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    To = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Travelers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    From = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    To = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    Reward = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travelers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Senders");

            migrationBuilder.DropTable(
                name: "Travelers");
        }
    }
}
