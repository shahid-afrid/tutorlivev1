using System.Collections.Generic;

namespace TutorLiveMentor.Models
{
    public class AssignedSubject
    {
        public int AssignedSubjectId { get; set; }
        public int FacultyId { get; set; }
        public int SubjectId { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }
        public int SelectedCount { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual Subject Subject { get; set; }

        // Update this to use the new enrollment entity
        public virtual ICollection<StudentEnrollment> Enrollments { get; set; }
    }
}
