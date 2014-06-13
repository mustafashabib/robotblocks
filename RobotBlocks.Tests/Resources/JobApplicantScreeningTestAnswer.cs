
using System;
namespace Handshake.Domain.Entities
{
  /**
   * This entity joins a job applicant's answer to 
   * a screening test question.
   * It acts as a link between the housing parent screening test 
   * entity, the job application, and the questions belonging to the screening
   * test. 
   * This denormalization exists so that we can quickly query from the ScreeningTest
   * side both the number and status of people taking this test as well as so we 
   * can avoid some extra joins from applicant's tests to their housing test/job combo.
   * If we wanted to avoid that, we can remove the relationship from JobApplicationScreeningTests
   * back to ScreeningTests and go through the job applicant's test answer to the question to 
   * the parent screening test. Seems worth breaking normalization to avoid that though.
   * I also considered avoiding the JobApplicationScreeningTest entity entirely, but 
   * felt that it added enough value by allowing us to attach statuses to a job applicant's
   * test as well as things like a due date or whatever else we may need.
   * 
   * In the diagram below, the left number in the relationship always refers to the entity to 
   * the left on horizontal lines, or above in vertical lines.  
   * 
   * Jobs---(1:0..1)---ScreeningTests---(1:m)---ScreeningTestQuestions-----------|
   *   |                |                         |                              |
   *   |                |                         |                              |
   * (1:m)              |                      (1:m)                             |
   *   |                |                         |                              |
   *   |                |                         |                              |
   * JobApplications  (1:m)                   Questions                        (1:m)
   *   |                |                                                        |
   *   |                |                                                        |
   * (1:0..1)           |                                                        |
   *   |                |                                                        |
   *   |                |                                                        |
   * JobApplicationScreeningTests---(1:m)---JobApplicationScreeningTestAnswers---|
   *   |
   *   |
   * (m:1)
   *   | 
   *   |
   * JobApplicationScreeningTestStatuses
   *
   **/
  public partial class JobApplicantScreeningTestAnswer : Entity
  {
    public long Id { get; set; }
    public long RelatedJobApplicantScreeningTestId { get; set; }
    public long RelatedScreeningTestQuestionId { get; set; }

    public long TimeToAnswerInMilliseconds { get; set; }
    public string FinalAnswerContent { get; set; }
    public DateTime DateStarted { get; set; }
    public string PlaybackEvents { get; set; }

    public JobApplicantScreeningTest RelatedJobApplicantScreeningTest { get; set; }
    public ScreeningTestQuestion RelatedScreeningTestQuestion { get; set; }
  }
}
