using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "risk_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PerTradeRiskPct = table.Column<decimal>(type: "numeric", nullable: false),
                    DailyLossCapR = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxConcurrent = table.Column<int>(type: "integer", nullable: false),
                    SlippageBps = table.Column<int>(type: "integer", nullable: false),
                    NoEntryAfter = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "signals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Underlying = table.Column<string>(type: "text", nullable: false),
                    Side = table.Column<int>(type: "integer", nullable: false),
                    Strike = table.Column<int>(type: "integer", nullable: false),
                    Expiry = table.Column<DateOnly>(type: "date", nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric", nullable: false),
                    OptionSymbol = table.Column<string>(type: "text", nullable: false),
                    ReasonJson = table.Column<string>(type: "jsonb", nullable: false),
                    StrategyId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "strategies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    AiThreshold = table.Column<decimal>(type: "numeric", nullable: false),
                    ConfigJson = table.Column<string>(type: "jsonb", nullable: false),
                    RiskProfileId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_strategies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "risk_profiles");

            migrationBuilder.DropTable(
                name: "signals");

            migrationBuilder.DropTable(
                name: "strategies");
        }
    }
}
