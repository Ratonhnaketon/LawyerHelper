using System.ComponentModel.DataAnnotations.Schema;

namespace LawyerHelper.Models
{
    public class Customers
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<Info>? Info { get; set; }
        
        [ForeignKey("ProcessId")]
        public List<Process>? Process { get; set; }
    }
}