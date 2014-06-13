using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handshake.Domain.Entities
{
  /** 
   * The associating entity between screening tests and questions.
   * They contain meta data about this screening test question
   **/
  public class ScreeningTestQuestion : Entity
  {
    public long Id { get; set; }
    public long RelatedQuestionId { get; set; }
    public long RelatedScreeningTestId { get; set; }
    public short MaximumTimeAllowedInMinutes { get; set; }
    public short SortOrder { get; set; }

    public Question RelatedQuestion { get; set; }
    public ScreeningTest RelatedScreeningTest { get; set; }

  }
}
