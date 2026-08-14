namespace StajApi.Features.Projeler;

public static class ProjeDogrulama
{
    public static string? Kontrol(
        string projeAdi,
        DateTime? baslangicTarihi,
        DateTime? bitisTarihi,
        decimal? butce)
    {
        if (string.IsNullOrWhiteSpace(projeAdi))
        {
            return "Proje adı boş bırakılamaz.";
        }

        if (projeAdi.Length > 100)
        {
            return "Proje adı en fazla 100 karakter olabilir.";
        }

        if (baslangicTarihi.HasValue
            && bitisTarihi.HasValue
            && bitisTarihi.Value < baslangicTarihi.Value)
        {
            return "Bitiş tarihi başlangıç tarihinden önce olamaz.";
        }

        if (butce.HasValue && butce.Value < 0)
        {
            return "Bütçe negatif olamaz.";
        }

        return null;
    }
}
