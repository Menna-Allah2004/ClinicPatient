using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicPatient.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorAvailabilities_Doctors_DoctorId1",
                table: "DoctorAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_DoctorAvailabilities_DoctorId1",
                table: "DoctorAvailabilities");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "DoctorAvailabilities");

            migrationBuilder.RenameColumn(
                name: "Specialization",
                table: "Doctors",
                newName: "Specialty");

            migrationBuilder.RenameColumn(
                name: "ExperienceYears",
                table: "Doctors",
                newName: "Experience");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Education",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "License",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Workplace",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "License",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Workplace",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "Specialty",
                table: "Doctors",
                newName: "Specialization");

            migrationBuilder.RenameColumn(
                name: "Experience",
                table: "Doctors",
                newName: "ExperienceYears");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Education",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoctorId1",
                table: "DoctorAvailabilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilities_DoctorId1",
                table: "DoctorAvailabilities",
                column: "DoctorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorAvailabilities_Doctors_DoctorId1",
                table: "DoctorAvailabilities",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id");
        }
    }
}
