﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave_Manager_Console.Migrations
{
    /// <inheritdoc />
    public partial class LeaveLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveLimit_Employees_EmployeeId",
                table: "LeaveLimit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveLimit",
                table: "LeaveLimit");

            migrationBuilder.RenameTable(
                name: "LeaveLimit",
                newName: "LeaveLimits");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveLimit_EmployeeId",
                table: "LeaveLimits",
                newName: "IX_LeaveLimits_EmployeeId");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                table: "LeaveLimits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveLimits",
                table: "LeaveLimits",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLimits_EmployeeId1",
                table: "LeaveLimits",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveLimits_Employees_EmployeeId",
                table: "LeaveLimits",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveLimits_Employees_EmployeeId1",
                table: "LeaveLimits",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveLimits_Employees_EmployeeId",
                table: "LeaveLimits");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveLimits_Employees_EmployeeId1",
                table: "LeaveLimits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveLimits",
                table: "LeaveLimits");

            migrationBuilder.DropIndex(
                name: "IX_LeaveLimits_EmployeeId1",
                table: "LeaveLimits");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "LeaveLimits");

            migrationBuilder.RenameTable(
                name: "LeaveLimits",
                newName: "LeaveLimit");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveLimits_EmployeeId",
                table: "LeaveLimit",
                newName: "IX_LeaveLimit_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveLimit",
                table: "LeaveLimit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveLimit_Employees_EmployeeId",
                table: "LeaveLimit",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
