
namespace Handshake.Domain.Entities
{
  public partial class Employee : Entity
  {
    public long Id { get; set; }
    public long RelatedUserId { get; set; }

    public User RelatedUser { get; set; }
  }
}
