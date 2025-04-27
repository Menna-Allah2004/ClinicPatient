using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicPatient.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAllDoctorsPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_DoctorId1",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_DoctorId1",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "Patients");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "Patients",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DoctorId",
                table: "Patients",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_DoctorId",
                table: "Patients",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_DoctorId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_DoctorId",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId1",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DoctorId1",
                table: "Patients",
                column: "DoctorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_DoctorId1",
                table: "Patients",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
