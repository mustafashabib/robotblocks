using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handshake.Domain.Entities
{
  /// <summary>
  /// A screening test can be created for a Job 
  /// It is a 1:1 relationship from here to a Job
  /// to keep things simple.
  /// ScreeningTests group a Set of ScreeningTestQuestions
  /// ScreeningTestQuestions are a selection of questions
  /// and some metadata about how it's displayed for the particular
  /// screening test they're associated with (order, time limit per 
  /// question, etc). The Question that the ScreeningTestQuestion
  /// associates can be used across many ScreeningTests.
  /// </summary>
  public class ScreeningTest : Entity
  {
    public long Id { get; set; }
    public long RelatedJobId { get; set; }

    /* one screening test is made up of several questions */
    public IEnumerable<ScreeningTestQuestion> RelatedScreeningTestQuestions { get; set; }
    public Job RelatedJob { get; set; }
  }
}

