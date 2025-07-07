using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructrure.Migrations
{
    /// <inheritdoc />
    public partial class initv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SlideHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalSlideId = table.Column<int>(type: "int", nullable: false),
                    Ask = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerCorrect = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlideHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlideHistoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionHistories_SlideHistories_SlideHistoryId",
                        column: x => x.SlideHistoryId,
                        principalTable: "SlideHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCreate = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalSlideId = table.Column<int>(type: "int", nullable: false),
                    SlideHistoryId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeElapsed = table.Column<TimeSpan>(type: "time", nullable: true),
                    PresentationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionHistories_SlideHistories_SlideHistoryId",
                        column: x => x.SlideHistoryId,
                        principalTable: "SlideHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionHistories_UserHistories_UserHistoryId",
                        column: x => x.UserHistoryId,
                        principalTable: "UserHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionHistories_SlideHistoryId",
                table: "OptionHistories",
                column: "SlideHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionHistories_SlideHistoryId",
                table: "SessionHistories",
                column: "SlideHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionHistories_UserHistoryId",
                table: "SessionHistories",
                column: "UserHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionHistories");

            migrationBuilder.DropTable(
                name: "SessionHistories");

            migrationBuilder.DropTable(
                name: "SlideHistories");

            migrationBuilder.DropTable(
                name: "UserHistories");
        }
    }
}
