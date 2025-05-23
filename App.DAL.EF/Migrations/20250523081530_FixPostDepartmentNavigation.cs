using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixPostDepartmentNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostDepartments_Departments_DepartmentId1",
                table: "PostDepartments");

            migrationBuilder.DropIndex(
                name: "IX_PostDepartments_DepartmentId1",
                table: "PostDepartments");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "PostDepartments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId1",
                table: "PostDepartments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostDepartments_DepartmentId1",
                table: "PostDepartments",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PostDepartments_Departments_DepartmentId1",
                table: "PostDepartments",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
