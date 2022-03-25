namespace LawyerHelper.Models
{
    public class Process_Relation
    {
        public int Id { get; set; }
        public int ProcessId { get; set; }
        public int? CustomersId { get; set; }
        public int? LawyerId { get; set; }
    }
}