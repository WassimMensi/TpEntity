using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdvancedModeling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Patients",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Patients",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "Patients",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Patients",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_City",
                table: "Departments",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_Country",
                table: "Departments",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_PostalCode",
                table: "Departments",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_Street",
                table: "Departments",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentDepartmentId",
                table: "Departments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    HireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StaffType = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Function = table.Column<string>(type: "TEXT", nullable: true),
                    Specialty = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LicenseNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Service = table.Column<string>(type: "TEXT", nullable: true),
                    Grade = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staff_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_DepartmentId",
                table: "Staff",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_LicenseNumber",
                table: "Staff",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ContactAddress_City",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ContactAddress_Country",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ContactAddress_PostalCode",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ContactAddress_Street",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ParentDepartmentId",
                table: "Departments");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
