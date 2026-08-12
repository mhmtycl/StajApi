import { useState } from "react";

function CalisanFormu({ calisan, departmanlar, onKaydet, onVazgec, kaydediliyor }) {
  const [ad, setAd] = useState(calisan?.ad ?? "");
  const [soyad, setSoyad] = useState(calisan?.soyad ?? "");
  const [email, setEmail] = useState(calisan?.email ?? "");
  const [maas, setMaas] = useState(
    calisan?.maas === null || calisan?.maas === undefined ? "" : String(calisan.maas)
  );
  const [departmanId, setDepartmanId] = useState(
    calisan?.departmanId === null || calisan?.departmanId === undefined
      ? ""
      : String(calisan.departmanId)
  );
  const [hata, setHata] = useState("");

  function formGonder(olay) {
    olay.preventDefault();
    setHata("");

    if (ad.trim() === "" || soyad.trim() === "") {
      setHata("Ad ve soyad boş bırakılamaz.");
      return;
    }

    if (maas !== "" && (Number.isNaN(Number(maas)) || Number(maas) < 0)) {
      setHata("Maaş geçerli bir sayı olmalı.");
      return;
    }

    onKaydet({
      ad: ad.trim(),
      soyad: soyad.trim(),
      email: email.trim() === "" ? null : email.trim(),
      maas: maas === "" ? null : Number(maas),
      departmanId: departmanId === "" ? null : Number(departmanId)
    });
  }

  return (
    <form className="form-karti" onSubmit={formGonder}>
      <h2>{calisan ? "Çalışan Güncelle" : "Yeni Çalışan Ekle"}</h2>

      <div className="form-satir">
        <div className="form-alan">
          <label htmlFor="ad">Ad</label>
          <input
            id="ad"
            type="text"
            maxLength={50}
            value={ad}
            onChange={(olay) => setAd(olay.target.value)}
          />
        </div>

        <div className="form-alan">
          <label htmlFor="soyad">Soyad</label>
          <input
            id="soyad"
            type="text"
            maxLength={50}
            value={soyad}
            onChange={(olay) => setSoyad(olay.target.value)}
          />
        </div>
      </div>

      <div className="form-satir">
        <div className="form-alan">
          <label htmlFor="email">E-posta</label>
          <input
            id="email"
            type="text"
            maxLength={150}
            value={email}
            onChange={(olay) => setEmail(olay.target.value)}
          />
        </div>

        <div className="form-alan">
          <label htmlFor="maas">Maaş</label>
          <input
            id="maas"
            type="number"
            min="0"
            step="1"
            value={maas}
            onChange={(olay) => setMaas(olay.target.value)}
          />
        </div>
      </div>

      <div className="form-alan">
        <label htmlFor="departman">Departman</label>
        <select
          id="departman"
          value={departmanId}
          onChange={(olay) => setDepartmanId(olay.target.value)}
        >
          <option value="">Seçilmedi</option>
          {departmanlar.map((departman) => (
            <option key={departman.departmanId} value={departman.departmanId}>
              {departman.departmanAdi}
            </option>
          ))}
        </select>
      </div>

      {hata !== "" && <div className="hata">{hata}</div>}

      <div className="buton-grubu">
        <button type="submit" disabled={kaydediliyor}>
          {kaydediliyor ? "Kaydediliyor..." : "Kaydet"}
        </button>
        <button
          type="button"
          className="ikincil-buton"
          onClick={onVazgec}
          disabled={kaydediliyor}
        >
          Vazgeç
        </button>
      </div>
    </form>
  );
}

export default CalisanFormu;
