namespace StajApi.Features.Calisanlar;

public class UpdateCalisanCommand
{
    public int CalisanId { get; set; }
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public decimal Maas { get; set; }
}
