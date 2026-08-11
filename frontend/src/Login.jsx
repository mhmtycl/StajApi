import { useState } from "react";
import { girisYap } from "./api.js";

function Login({ onGirisBasarili }) {
  const [kullaniciAdi, setKullaniciAdi] = useState("");
  const [sifre, setSifre] = useState("");
  const [hata, setHata] = useState("");
  const [yukleniyor, setYukleniyor] = useState(false);

  async function formGonder(olay) {
    olay.preventDefault();
    setHata("");

    if (kullaniciAdi.trim() === "" || sifre === "") {
      setHata("Kullanıcı adı ve şifre boş bırakılamaz.");
      return;
    }

    setYukleniyor(true);

    try {
      const tokenlar = await girisYap(kullaniciAdi, sifre);
      onGirisBasarili(tokenlar);
    } catch (sorun) {
      setHata(sorun.message);
    } finally {
      setYukleniyor(false);
    }
  }

  return (
    <div className="login-sayfa">
      <form className="kart" onSubmit={formGonder}>
        <h1>Personel Yönetim Paneli</h1>
        <p className="alt-baslik">Devam etmek için giriş yapın.</p>

        <label htmlFor="kullaniciAdi">Kullanıcı Adı</label>
        <input
          id="kullaniciAdi"
          type="text"
          autoComplete="username"
          value={kullaniciAdi}
          onChange={(olay) => setKullaniciAdi(olay.target.value)}
        />

        <label htmlFor="sifre">Şifre</label>
        <input
          id="sifre"
          type="password"
          autoComplete="current-password"
          value={sifre}
          onChange={(olay) => setSifre(olay.target.value)}
        />

        {hata !== "" && <div className="hata">{hata}</div>}

        <button type="submit" disabled={yukleniyor}>
          {yukleniyor ? "Giriş yapılıyor..." : "Giriş Yap"}
        </button>
      </form>
    </div>
  );
}

export default Login;
