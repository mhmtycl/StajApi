namespace StajApi.Features.Gorevler;

public static class GorevDogrulama
{
    public static string? Kontrol(
        string? gorevAdi,
        int calisanId,
        int projeId,
        string? durum)
    {
        if (string.IsNullOrWhiteSpace(gorevAdi))
        {
            return "Görev adı boş bırakılamaz.";
        }

        if (gorevAdi.Length > 150)
        {
            return "Görev adı en fazla 150 karakter olabilir.";
        }

        if (calisanId <= 0)
        {
            return "Çalışan seçilmelidir.";
        }

        if (projeId <= 0)
        {
            return "Proje seçilmelidir.";
        }

        if (!string.IsNullOrWhiteSpace(durum) && durum.Length > 30)
        {
            return "Durum en fazla 30 karakter olabilir.";
        }

        return null;
    }
}
