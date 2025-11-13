namespace TutorLiveMentor.Models
{
    public class StudentEnrollment
    {
        // Composite primary key: (StudentId, AssignedSubjectId) - configured in AppDbContext
        public string StudentId { get; set; }
        public Student Student { get; set; }
        
        public int AssignedSubjectId { get; set; }
        public AssignedSubject AssignedSubject { get; set; }
        
        // ?? PRECISE TIMESTAMP: First-come-first-served with millisecond precision
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}