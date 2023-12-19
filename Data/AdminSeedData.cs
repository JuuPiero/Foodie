using Microsoft.EntityFrameworkCore.Migrations;
namespace AppCore.Data;
public static class AdminSeedData {
    public static void SeedData(MigrationBuilder migrationBuilder) {
        migrationBuilder.InsertData(
            table: "Admin",
            columns: new[] { "FullName", "Email", "Password", "CreatedAt", "UpdatedAt" /* ... other columns */ },
            values: new object[] { "Admin", "admin@localhost.com", "123456789", DateTime.Now, DateTime.Now }
        );
    }
}