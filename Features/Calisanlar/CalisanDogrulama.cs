namespace StajApi.Features.Calisanlar;

public static class CalisanDogrulama
{
    public static string? Kontrol(string ad, string soyad, string? email, decimal? maas)
    {
        if (string.IsNullOrWhiteSpace(ad))
        {
            return "Ad alanı boş bırakılamaz.";
        }

        if (ad.Length > 50)
        {
            return "Ad en fazla 50 karakter olabilir.";
        }

        if (string.IsNullOrWhiteSpace(soyad))
        {
            return "Soyad alanı boş bırakılamaz.";
        }

        if (soyad.Length > 50)
        {
            return "Soyad en fazla 50 karakter olabilir.";
        }

        if (!string.IsNullOrWhiteSpace(email) && email.Length > 150)
        {
            return "E-posta en fazla 150 karakter olabilir.";
        }

        if (maas.HasValue && maas.Value < 0)
        {
            return "Maaş negatif olamaz.";
        }

        return null;
    }
}
