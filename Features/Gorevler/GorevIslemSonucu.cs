namespace StajApi.Features.Gorevler;

public class GorevIslemSonucu
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public bool BulunamadiMi { get; set; }
    public int GorevId { get; set; }
}
