namespace Games4TradeAPI.Dtos
{
    public class SystemDto
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }

    public class SystemCreateOrUpdateDto
    {
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }
}
