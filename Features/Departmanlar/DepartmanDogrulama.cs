namespace StajApi.Features.Departmanlar;

public static class DepartmanDogrulama
{
    public static string? Kontrol(string departmanAdi)
    {
        if (string.IsNullOrWhiteSpace(departmanAdi))
        {
            return "Departman adı boş bırakılamaz.";
        }

        if (departmanAdi.Length > 100)
        {
            return "Departman adı en fazla 100 karakter olabilir.";
        }

        return null;
    }
}
