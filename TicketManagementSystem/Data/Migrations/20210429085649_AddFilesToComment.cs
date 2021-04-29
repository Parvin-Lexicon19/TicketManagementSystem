using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketManagementSystem.Data.Migrations
{
    public partial class AddFilesToComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CommentId",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CommentId",
                table: "Documents",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Comments_CommentId",
                table: "Documents",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Comments_CommentId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CommentId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Documents");
        }
    }
}
