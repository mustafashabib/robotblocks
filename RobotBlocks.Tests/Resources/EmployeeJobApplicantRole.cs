
namespace Handshake.Domain.Entities
{
  /// <summary>
  /// The role an employee has for a specific job applicant's hiring workflow
  /// </summary>
  public partial class EmployeeJobApplicantRole : Entity
  {
    public short Id { get; set; }
    public string Name { get; set; }
  }
}
