namespace Games4Trade.Dtos
{
    public class SystemGetDto
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
    }

    public class SystemCreateOrUpdateDto
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
    }
}
