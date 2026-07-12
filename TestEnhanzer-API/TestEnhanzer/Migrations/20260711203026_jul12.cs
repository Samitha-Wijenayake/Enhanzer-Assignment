using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestEnhanzer.Migrations
{
    /// <inheritdoc />
    public partial class jul12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserDetailsId = table.Column<int>(type: "int", nullable: false),
                    Location_Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_Details_UserDetails_UserDetailsId",
                        column: x => x.UserDetailsId,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_Details_UserDetailsId_Location_Code",
                table: "Location_Details",
                columns: new[] { "UserDetailsId", "Location_Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_Username",
                table: "UserDetails",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Location_Details");

            migrationBuilder.DropTable(
                name: "UserDetails");
        }
    }
}
