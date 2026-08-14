namespace StajApi.Models;

public class Proje
{
    public int ProjeId { get; set; }
    public string ProjeAdi { get; set; } = "";
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public decimal? Butce { get; set; }
}
