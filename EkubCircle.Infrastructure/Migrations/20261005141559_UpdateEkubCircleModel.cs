using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkubCircle.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEkubCircleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CircleMembers_Circles_CircleId",
                table: "CircleMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CircleMembers_CircleMemberId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Rounds_RoundId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_CircleMembers_ReceiverMemberId",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Circles_CircleId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_ReceiverMemberId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CircleMemberId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RoundId_CircleMemberId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaidOutAmount",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "ReceiverMemberId",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "CircleMemberId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "Contribution",
                table: "Circles",
                newName: "ContributionAmount");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "CircleMembers",
                newName: "Position");

            migrationBuilder.RenameIndex(
                name: "IX_CircleMembers_CircleId_OrderNumber",
                table: "CircleMembers",
                newName: "IX_CircleMembers_CircleId_Position");

            migrationBuilder.AddColumn<decimal>(
                name: "ContributionAmount",
                table: "Rounds",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PotAmount",
                table: "Rounds",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Rounds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLabel",
                table: "Circles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "CurrentReceiverPosition",
                table: "Circles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentRoundNumber",
                table: "Circles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MemberCount",
                table: "Circles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_ReceiverId",
                table: "Rounds",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MemberId",
                table: "Payments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RoundId_MemberId",
                table: "Payments",
                columns: new[] { "RoundId", "MemberId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CircleMembers_AspNetUsers_UserId",
                table: "CircleMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CircleMembers_Circles_CircleId",
                table: "CircleMembers",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Circles_AspNetUsers_OrganizerId",
                table: "Circles",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_MemberId",
                table: "Payments",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Rounds_RoundId",
                table: "Payments",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_AspNetUsers_ReceiverId",
                table: "Rounds",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Circles_CircleId",
                table: "Rounds",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CircleMembers_AspNetUsers_UserId",
                table: "CircleMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CircleMembers_Circles_CircleId",
                table: "CircleMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Circles_AspNetUsers_OrganizerId",
                table: "Circles");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_MemberId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Rounds_RoundId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_AspNetUsers_ReceiverId",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Circles_CircleId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_ReceiverId",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MemberId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RoundId_MemberId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ContributionAmount",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "PotAmount",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CurrentReceiverPosition",
                table: "Circles");

            migrationBuilder.DropColumn(
                name: "CurrentRoundNumber",
                table: "Circles");

            migrationBuilder.DropColumn(
                name: "MemberCount",
                table: "Circles");

            migrationBuilder.RenameColumn(
                name: "ContributionAmount",
                table: "Circles",
                newName: "Contribution");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "CircleMembers",
                newName: "OrderNumber");

            migrationBuilder.RenameIndex(
                name: "IX_CircleMembers_CircleId_Position",
                table: "CircleMembers",
                newName: "IX_CircleMembers_CircleId_OrderNumber");

            migrationBuilder.AddColumn<decimal>(
                name: "PaidOutAmount",
                table: "Rounds",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiverMemberId",
                table: "Rounds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CircleMemberId",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLabel",
                table: "Circles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_ReceiverMemberId",
                table: "Rounds",
                column: "ReceiverMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CircleMemberId",
                table: "Payments",
                column: "CircleMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RoundId_CircleMemberId",
                table: "Payments",
                columns: new[] { "RoundId", "CircleMemberId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CircleMembers_Circles_CircleId",
                table: "CircleMembers",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CircleMembers_CircleMemberId",
                table: "Payments",
                column: "CircleMemberId",
                principalTable: "CircleMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Rounds_RoundId",
                table: "Payments",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_CircleMembers_ReceiverMemberId",
                table: "Rounds",
                column: "ReceiverMemberId",
                principalTable: "CircleMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Circles_CircleId",
                table: "Rounds",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
