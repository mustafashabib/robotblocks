using System.Collections.Generic;

namespace Handshake.Domain.Entities
{
  public partial class Account : Entity
  {
    public long Id { get; set; }
    public string CompanyName { get; set; }
    public string Vanity { get; set; }
    public short RelatedAccountStatusId { get; set; }

    public IEnumerable<User> RelatedUsers { get; set; }
    public IEnumerable<Job> RelatedJobs { get; set; }
  }
}
