using System.Collections.Generic;

namespace Handshake.Domain.Entities
{
  public partial class Question : Entity
  {
    public long Id { get; set; }
    public string QuestionText { get; set; }
    public long RelatedAccountId { get; set; }

    public Account RelatedAccount { get; set; }
    /* m:m relationship through QuestionKeywords */
    public IEnumerable<QuestionKeyword> RelatedQuestionKeywords { get; set; }
  }
}
