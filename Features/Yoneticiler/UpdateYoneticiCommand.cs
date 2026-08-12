namespace StajApi.Features.Yoneticiler;

public class UpdateYoneticiCommand
{
    public int YoneticiId { get; set; }
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public string Departman { get; set; } = "";
}
