namespace Games4TradeAPI.Dtos
{
    public class GenreDto
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class GenreCreateOrUpdateDto
    {
        public string Value { get; set; } 
    }
}
