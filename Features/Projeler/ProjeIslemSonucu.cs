namespace StajApi.Features.Projeler;

public class ProjeIslemSonucu
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public bool BulunamadiMi { get; set; }
    public int ProjeId { get; set; }
}
