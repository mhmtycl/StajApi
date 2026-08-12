namespace StajApi.Features.Calisanlar;

public class CalisanIslemSonucu
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public bool BulunamadiMi { get; set; }
    public int CalisanId { get; set; }
}
