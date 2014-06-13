using System.Collections.Generic;

namespace Handshake.Domain.Entities
{
  public partial class User : Entity
  {
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string EncryptedPasswordSalt { get; set; }
    public string EncryptedPasswordHash { get; set; }
    public int LogInAttempts { get; set; }
    public short RelatedUserStatusId { get; set; }
    public short RelatedRoleId { get; set; }
    public long RelatedAccountId { get; set; }
    public Account RelatedAccount { get; set; }
    public IEnumerable<JobApplicant> RelatedApplicants { get; set; }
    public IEnumerable<Employee> RelatedEmployees { get; set; }
  }
}
