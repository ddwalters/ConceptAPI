using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConceptAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameToFirebaseUid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "Users",
                newName: "FirebaseUid");

            migrationBuilder.RenameIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                newName: "IX_Users_FirebaseUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirebaseUid",
                table: "Users",
                newName: "GoogleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_FirebaseUid",
                table: "Users",
                newName: "IX_Users_GoogleId");
        }
    }
}
