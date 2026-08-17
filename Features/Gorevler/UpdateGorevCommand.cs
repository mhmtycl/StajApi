namespace StajApi.Features.Gorevler;

public class UpdateGorevCommand
{
    public int GorevId { get; set; }
    public string? GorevAdi { get; set; }
    public int CalisanId { get; set; }
    public int ProjeId { get; set; }
    public string? Durum { get; set; }
    public DateTime? TeslimTarihi { get; set; }
}
