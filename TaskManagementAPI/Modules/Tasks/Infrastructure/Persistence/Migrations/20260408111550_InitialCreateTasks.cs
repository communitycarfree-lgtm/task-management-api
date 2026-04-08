using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDependencies_Tasks_BlockedByTaskId",
                table: "TaskDependencies");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDependencies_Tasks_BlockedByTaskId",
                table: "TaskDependencies",
                column: "BlockedByTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDependencies_Tasks_BlockedByTaskId",
                table: "TaskDependencies");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDependencies_Tasks_BlockedByTaskId",
                table: "TaskDependencies",
                column: "BlockedByTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
