
namespace Handshake.Domain.Entities
{
  /// <summary>
  /// Describes the role an employee may have with a job applicant's hiring process
  /// </summary>
  public partial class EmployeeJobApplicantMembership : Entity
  {
    public long Id { get; set; }
    public short RelatedEmployeeJobApplicantRoleId { get; set; }
    public long RelatedEmployeeId { get; set; }
    public long RelatedJobApplicantId { get; set; }
  }
}
