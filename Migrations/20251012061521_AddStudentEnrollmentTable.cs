using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorLiveMentor10.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentEnrollmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_AssignedSubjects_AssignedSubjectId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_AssignedSubjectId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_Department_Year",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_AssignedSubjects_SubjectId_Department_Year",
                table: "AssignedSubjects");

            migrationBuilder.DropColumn(
                name: "AssignedSubjectId",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "AssignedSubjects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "StudentEnrollments",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AssignedSubjectId = table.Column<int>(type: "int", nullable: false),
                    StudentEnrollmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEnrollments", x => new { x.StudentId, x.AssignedSubjectId });
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_AssignedSubjects_AssignedSubjectId",
                        column: x => x.AssignedSubjectId,
                        principalTable: "AssignedSubjects",
                        principalColumn: "AssignedSubjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSubjects_SubjectId",
                table: "AssignedSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AssignedSubjectId",
                table: "StudentEnrollments",
                column: "AssignedSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_AssignedSubjects_SubjectId",
                table: "AssignedSubjects");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AssignedSubjectId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "AssignedSubjects",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AssignedSubjectId",
                table: "Students",
                column: "AssignedSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Department_Year",
                table: "Students",
                columns: new[] { "Department", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSubjects_SubjectId_Department_Year",
                table: "AssignedSubjects",
                columns: new[] { "SubjectId", "Department", "Year" });

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AssignedSubjects_AssignedSubjectId",
                table: "Students",
                column: "AssignedSubjectId",
                principalTable: "AssignedSubjects",
                principalColumn: "AssignedSubjectId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
