
namespace Handshake.Domain.Entities
{
  public partial class Keyword : Entity
  {
    public long Id { get; set; }
    public string KeywordText { get; set; }
    public long RelatedAccountId { get; set; }

    public Account RelatedAccount { get; set; }
  }
}
