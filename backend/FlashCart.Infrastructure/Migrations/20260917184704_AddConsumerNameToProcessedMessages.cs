using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsumerNameToProcessedMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessedMessages_EventId",
                table: "ProcessedMessages");

            migrationBuilder.AddColumn<string>(
                name: "ConsumerName",
                table: "ProcessedMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_EventId_ConsumerName",
                table: "ProcessedMessages",
                columns: new[] { "EventId", "ConsumerName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessedMessages_EventId_ConsumerName",
                table: "ProcessedMessages");

            migrationBuilder.DropColumn(
                name: "ConsumerName",
                table: "ProcessedMessages");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_EventId",
                table: "ProcessedMessages",
                column: "EventId",
                unique: true);
        }
    }
}
