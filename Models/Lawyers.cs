using System.ComponentModel.DataAnnotations.Schema;

namespace LawyerHelper.Models
{
    public class Lawyers
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Expertise { get; set; } = "";
        [ForeignKey("ProcessId")]
        public List<Process>? Process { get; set; }
    }
}