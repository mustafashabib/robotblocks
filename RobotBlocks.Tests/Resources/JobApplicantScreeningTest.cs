using System;
namespace Handshake.Domain.Entities
{
  public partial class JobApplicantScreeningTest : Entity
  {
    public long Id { get; set; }
    public long RelatedScreeningTestId { get; set; }
    public long RelatedJobApplicationId { get; set; }
    public long RelatedJobApplicantScreeningTestStatusId { get; set; }

    public string UniqueUrl { get; set; }
    public DateTime? DueDate { get; set; }

    public ScreeningTest RelatedScreeningTest { get; set; }
    public JobApplicantScreeningTestStatus RelatedJobApplicantScreeningTestStatus { get; set; }
    public JobApplicant RelatedJobApplication { get; set; }

  }
}
