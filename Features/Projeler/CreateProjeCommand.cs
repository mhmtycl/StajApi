namespace StajApi.Features.Projeler;

public class CreateProjeCommand
{
    public string ProjeAdi { get; set; } = "";
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public decimal? Butce { get; set; }
}
