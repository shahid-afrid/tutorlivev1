namespace TutorLiveMentor.Models
{
    public class StudentEnrollment
    {
        public int StudentEnrollmentId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int AssignedSubjectId { get; set; }
        public AssignedSubject AssignedSubject { get; set; }
    }
}