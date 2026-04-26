using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionTargetTypeRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "BarberProfiles");

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessAccounting",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessAnalytics",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessInventory",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPostProducts",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanReceiveBookings",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanUseBanners",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TargetType",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanId",
                table: "BarberProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BarberProfiles_SubscriptionPlanId",
                table: "BarberProfiles",
                column: "SubscriptionPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarberProfiles_SubscriptionPlans_SubscriptionPlanId",
                table: "BarberProfiles",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarberProfiles_SubscriptionPlans_SubscriptionPlanId",
                table: "BarberProfiles");

            migrationBuilder.DropIndex(
                name: "IX_BarberProfiles_SubscriptionPlanId",
                table: "BarberProfiles");

            migrationBuilder.DropColumn(
                name: "CanAccessAccounting",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CanAccessAnalytics",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CanAccessInventory",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CanPostProducts",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CanReceiveBookings",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CanUseBanners",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "BarberProfiles");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlan",
                table: "BarberProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
