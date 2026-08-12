namespace StajApi.Features.Departmanlar;

public class DepartmanIslemSonucu
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public bool BulunamadiMi { get; set; }
    public int DepartmanId { get; set; }
}
