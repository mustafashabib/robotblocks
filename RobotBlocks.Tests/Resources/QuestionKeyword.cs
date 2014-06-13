
namespace Handshake.Domain.Entities
{
  public partial class QuestionKeyword : Entity
  {
    public long Id { get; set; }
    public long RelatedKeywordId { get; set; }
    public long RelatedQuestionId { get; set; }

    public Keyword RelatedKeyword { get; set; }
    public Question RelatedQuestion { get; set; }
  }
}
