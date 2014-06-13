using System.Collections.Generic;

namespace Handshake.Domain.Entities
{
  public partial class Job : Entity
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public long RelatedAccountId { get; set; }
    public bool IsPublic { get; set; }

    /*TODO: replace with a related defined type status
     * instead of the IsActive boolean ? */
    public bool IsActive { get; set; }

    public Account RelatedAccount { get; set; }
    public IEnumerable<JobHistory> RelatedJobHistories { get; set; }
    public IEnumerable<JobApplication> RelatedJobApplications { get; set; }

    /** 
     * The Job's screening test, if any.
     * The ScreeningTest references the Job via 
     * an FK into this table.
    **/
    public ScreeningTest RelatedScreeningTest { get; set; }

  }
}
