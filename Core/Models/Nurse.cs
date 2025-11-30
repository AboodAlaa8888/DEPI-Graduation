namespace Core.Models
{
    public class Nurse
    {
        public int Id { get; set; }
        //public string Name { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }
        public int Age { get; set; }
        public int Experience_years { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public string? PictureUrl { get; set; }

        public List<Order>? Orders { get; set; }

    }
}
