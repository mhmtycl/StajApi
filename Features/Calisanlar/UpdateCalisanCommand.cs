namespace StajApi.Features.Calisanlar;

public class UpdateCalisanCommand
{
    public int CalisanId { get; set; }
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public string? Email { get; set; }
    public decimal? Maas { get; set; }
    public int? DepartmanId { get; set; }
}
