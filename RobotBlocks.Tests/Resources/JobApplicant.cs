using System.Collections.Generic;

namespace Handshake.Domain.Entities
{
  public partial class JobApplicant : Entity
  {
    public long Id { get; set; }
    public long RelatedUserId { get; set; }

    public User RelatedUser { get; set; }
    public IEnumerable<JobApplication> JobApplications { get; set; }
  }
}
