namespace StajApi.Features.Gorevler;

public class CreateGorevCommand
{
    public string? GorevAdi { get; set; }
    public int CalisanId { get; set; }
    public int ProjeId { get; set; }
    public string? Durum { get; set; }
    public DateTime? TeslimTarihi { get; set; }
}
