using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToWalletEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Wallets",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Wallets");
        }
    }
}
