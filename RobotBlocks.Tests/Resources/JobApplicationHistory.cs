
namespace Handshake.Domain.Entities
{
  public partial class JobApplicationHistory : Entity
  {
    public long Id { get; set; }
    public long RelatedJobApplicationId { get; set; }
    public long RelatedHistoryId { get; set; }
  }
}
