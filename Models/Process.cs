using System.ComponentModel.DataAnnotations.Schema;

namespace LawyerHelper.Models
{
    public class Process
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public DateTime Deadline { get; set; }
        public int State { get; set; }

        [NotMapped]
        public List<int>? LawyersIds { get; set; }
        [NotMapped]
        public List<int>? CustomersIds { get; set; }

        public List<History>? History { get; set; }
        public List<Info>? Info { get; set; }
        public List<Lawyers>? Lawyers { get; set; }
        public List<Customers>? Customers { get; set; }
    }
}