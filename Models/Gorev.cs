namespace StajApi.Models;

public class Gorev
{
    public int GorevId { get; set; }
    public int CalisanId { get; set; }
    public string CalisanAdi { get; set; } = "";
    public int ProjeId { get; set; }
    public string ProjeAdi { get; set; } = "";
    public string? GorevAdi { get; set; }
    public string? Durum { get; set; }
    public DateTime? TeslimTarihi { get; set; }
}
