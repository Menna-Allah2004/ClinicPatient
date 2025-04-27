using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicPatient.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAllDoctorPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Doctors",
                newName: "WorkingHours");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "AspNetUsers",
                newName: "Location");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId1",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisit",
                table: "Patients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorsId",
                table: "MedicalReports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "MedicalReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLink",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BloodType", "DateOfBirth", "Location", "WorkingHours" },
                values: new object[] { "O+", null, "غزة - السرايا", null });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BloodType", "DateOfBirth", "Location", "WorkingHours" },
                values: new object[] { "O+", null, "غزة - الشفا", null });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BloodType", "DateOfBirth", "Location", "WorkingHours" },
                values: new object[] { "O+", null, "خانيونس - البلد", null });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DoctorId1",
                table: "Patients",
                column: "DoctorId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalReports_DoctorsId",
                table: "MedicalReports",
                column: "DoctorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalReports_AspNetUsers_DoctorsId",
                table: "MedicalReports",
                column: "DoctorsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_DoctorId1",
                table: "Patients",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalReports_AspNetUsers_DoctorsId",
                table: "MedicalReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_DoctorId1",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_DoctorId1",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_MedicalReports_DoctorsId",
                table: "MedicalReports");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "LastVisit",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DoctorsId",
                table: "MedicalReports");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "MedicalReports");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "WorkingHours",
                table: "Doctors",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "AspNetUsers",
                newName: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLink",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Address",
                value: "غزة - السرايا");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "Address",
                value: "غزة - الشفا");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "Address",
                value: "خانيونس - البلد");
        }
    }
}
