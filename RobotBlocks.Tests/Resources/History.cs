using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handshake.Domain.Entities
{
  public partial class History : Entity
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public short RelatedHistoryTypeId { get; set; }
  }
}
