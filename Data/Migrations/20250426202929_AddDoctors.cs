using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicPatient.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "Address", "Bio", "City", "ConsultationFee", "CreatedAt", "Education", "Experience", "FullName", "ImageUrl", "License", "Rating", "RatingCount", "Specialty", "UpdatedAt", "UserId", "Workplace" },
                values: new object[,]
                {
                    { 1, "غزة - السرايا", "استشاري أمراض القلب والأوعية الدموية بخبرة أكثر من 15 عاماً في علاج أمراض القلب والشرايين", "غزة", null, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 15, "أحمد محمد", null, null, 4.5m, 0, "قلب وأوعية دموية", new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "83670e60-4703-4748-8f56-3433db72cead", "عيادة أحمد" },
                    { 2, "غزة - الشفا", "استشارية الأمراض الجلدية والتجميل، متخصصة في علاج مشاكل البشرة والجلد والليزر التجميلي", "غزة", null, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 13, "سارة علي", null, null, 5m, 0, "جلدية", new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "4c3734ae-0661-45b2-985c-4142bf3fd57e", "عيادة سارة" },
                    { 3, "خانيونس - البلد", "استشاري جراحة العظام والمفاصل، متخصص في جراحات استبدال المفاصل وإصابات الملاعب", "خانيونس", null, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 20, "خالد العمري", null, null, 4m, 0, "جراحة عظام", new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "43e7b2b8-91ab-4b37-823a-0668332d073a", "عيادة خالد" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
