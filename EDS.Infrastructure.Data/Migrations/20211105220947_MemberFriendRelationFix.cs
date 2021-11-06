using Microsoft.EntityFrameworkCore.Migrations;

namespace EDS.Infrastructure.Data.Migrations
{
    public partial class MemberFriendRelationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Members_MemberId",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_MemberId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Friends");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Friends",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friends_MemberId",
                table: "Friends",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Members_MemberId",
                table: "Friends",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
