namespace StajApi.Features.Calisanlar;

public class CreateCalisanCommand
{
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public string? Email { get; set; }
    public decimal? Maas { get; set; }
    public int? DepartmanId { get; set; }
}
