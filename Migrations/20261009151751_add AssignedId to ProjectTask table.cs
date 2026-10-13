using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kanban_api.Migrations
{
    /// <inheritdoc />
    public partial class addAssignedIdtoProjectTasktable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "ProjectTasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "ProjectTasks");
        }
    }
}
