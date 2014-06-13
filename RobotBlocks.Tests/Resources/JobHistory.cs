
namespace Handshake.Domain.Entities
{
  public partial class JobHistory : Entity
  {
    public long Id { get; set; }
    public long RelatedJobId { get; set; }
    public long RelatedHistoryId { get; set; }

    public History RelatedHistory { get; set; }
    public Job RelatedJob { get; set; }
  }
}
