namespace LawyerHelper.Models
{
    public class Info
    {
        public int Id { get; set; }

        public int? CustomersId { get; set; }
        public int? ProcessId { get; set; }
        public string Description { get; set; } = "";
    }
}