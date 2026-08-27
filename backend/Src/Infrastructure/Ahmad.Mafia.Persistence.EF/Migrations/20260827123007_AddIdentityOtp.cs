using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ahmad.Mafia.Persistence.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scenario",
                table: "Rooms",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                table: "Rooms",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "NightGuardTargetPlayerId",
                table: "GameSessions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NightGuardianPlayerId",
                table: "GameSessions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NightInvestigateTargetPlayerId",
                table: "GameSessions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NightInvestigatorPlayerId",
                table: "GameSessions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NightSaveTargetPlayerId",
                table: "GameSessions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NightSerialKillerTargetPlayerId",
                table: "GameSessions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scenario",
                table: "GameSessions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsMafiaLeader",
                table: "GamePlayers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LastInvestigationIsMafia",
                table: "GamePlayers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastInvestigationTargetId",
                table: "GamePlayers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OtpChallenges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModificationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModificationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAccounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_Mobile_CreatedAtUtc",
                table: "OtpChallenges",
                columns: new[] { "Mobile", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAccounts_Mobile",
                table: "PlayerAccounts",
                column: "Mobile",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpChallenges");

            migrationBuilder.DropTable(
                name: "PlayerAccounts");

            migrationBuilder.DropColumn(
                name: "Scenario",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "NightGuardTargetPlayerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "NightGuardianPlayerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "NightInvestigateTargetPlayerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "NightInvestigatorPlayerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "NightSaveTargetPlayerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "NightSerialKillerTargetPlayerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "Scenario",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "IsMafiaLeader",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "LastInvestigationIsMafia",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "LastInvestigationTargetId",
                table: "GamePlayers");
        }
    }
}
