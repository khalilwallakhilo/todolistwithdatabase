using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todolistwithdatabase.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.InsertData(
                table: "Lists",
                columns: new[] { "Id", "Description", "DueDate", "IsCompleted", "Title" },
                values: new object[] { 90, "hah", "2024-10-5", false, "Lorem Epsum" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "PasswordHash", "Role" },
                values: new object[] { "Test", "test", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DeleteData(
                table: "Lists",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
