using System.Collections.Generic;

namespace WeddingPlanner.Models
{
  public class Dashboard
  {
    public List<Wedding> Weddings { get; set; }
    public Users User { get; set; }
  }
}