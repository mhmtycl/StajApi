namespace StajApi.Features.Calisanlar;

public class CreateCalisanCommand
{
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public decimal Maas { get; set; }
}
