namespace Core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Duration { get; set; }
        public string Address { get; set; }
        public int PatientAge { get; set; }
        public string Gender { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; } 

        public int NurseId { get; set; }
        public string PatientId { get; set; }

        public Nurse Nurse { get; set; }
        public Patient Patient { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        InProgress,
        Completed,
        Cancelled,
        Approved // <--- Add this line
    }
}
