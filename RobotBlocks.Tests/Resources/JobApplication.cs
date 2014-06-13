using System;

namespace Handshake.Domain.Entities
{
  public class JobApplication : Entity
  {
    public long Id { get; set; }
    public long RelatedJobId { get; set; }
    public short RelatedJobApplicationStatusId { get; set; }

    // TODO: Figure out the data structure for the actual info of the job application
    public string ApplicationInfo { get; set; }

    public Job RelatedJob { get; set; }
    public JobApplicationStatus RelatedJobApplicationStatus { get; set; }

    public JobApplicantScreeningTest RelatedJobApplicationScreeningTest { get; set; }
    public JobApplicant RelatedJobApplicant { get; set; }
    public DateTime AppliedOn { get; set; }
  }
}
