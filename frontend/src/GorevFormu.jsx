import { useState } from "react";

const DURUMLAR = ["Beklemede", "Devam Ediyor", "Tamamlandi"];

function tarihAlaniIcin(deger) {
  if (!deger) {
    return "";
  }

  return deger.slice(0, 10);
}

function GorevFormu({ gorev, calisanlar, projeler, onKaydet, onVazgec, kaydediliyor }) {
  const [gorevAdi, setGorevAdi] = useState(gorev?.gorevAdi ?? "");
  const [calisanId, setCalisanId] = useState(
    gorev?.calisanId === null || gorev?.calisanId === undefined
      ? ""
      : String(gorev.calisanId)
  );
  const [projeId, setProjeId] = useState(
    gorev?.projeId === null || gorev?.projeId === undefined ? "" : String(gorev.projeId)
  );
  const [durum, setDurum] = useState(gorev?.durum ?? "");
  const [teslimTarihi, setTeslimTarihi] = useState(tarihAlaniIcin(gorev?.teslimTarihi));
  const [hata, setHata] = useState("");

  function formGonder(olay) {
    olay.preventDefault();
    setHata("");

    if (gorevAdi.trim() === "") {
      setHata("Görev adı boş bırakılamaz.");
      return;
    }

    if (calisanId === "") {
      setHata("Çalışan seçilmelidir.");
      return;
    }

    if (projeId === "") {
      setHata("Proje seçilmelidir.");
      return;
    }

    onKaydet({
      gorevAdi: gorevAdi.trim(),
      calisanId: Number(calisanId),
      projeId: Number(projeId),
      durum: durum === "" ? null : durum,
      teslimTarihi: teslimTarihi === "" ? null : teslimTarihi
    });
  }

  return (
    <form className="form-karti" onSubmit={formGonder}>
      <h2>{gorev ? "Görev Güncelle" : "Yeni Görev Ekle"}</h2>

      <div className="form-alan">
        <label htmlFor="gorevAdi">Görev Adı</label>
        <input
          id="gorevAdi"
          type="text"
          maxLength={150}
          value={gorevAdi}
          onChange={(olay) => setGorevAdi(olay.target.value)}
        />
      </div>

      <div className="form-satir">
        <div className="form-alan">
          <label htmlFor="calisan">Çalışan</label>
          <select
            id="calisan"
            value={calisanId}
            onChange={(olay) => setCalisanId(olay.target.value)}
          >
            <option value="">Seçiniz</option>
            {calisanlar.map((calisan) => (
              <option key={calisan.calisanId} value={calisan.calisanId}>
                {calisan.ad} {calisan.soyad}
              </option>
            ))}
          </select>
        </div>

        <div className="form-alan">
          <label htmlFor="proje">Proje</label>
          <select
            id="proje"
            value={projeId}
            onChange={(olay) => setProjeId(olay.target.value)}
          >
            <option value="">Seçiniz</option>
            {projeler.map((proje) => (
              <option key={proje.projeId} value={proje.projeId}>
                {proje.projeAdi}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="form-satir">
        <div className="form-alan">
          <label htmlFor="durum">Durum</label>
          <select
            id="durum"
            value={durum}
            onChange={(olay) => setDurum(olay.target.value)}
          >
            <option value="">Seçilmedi</option>
            {DURUMLAR.map((secenek) => (
              <option key={secenek} value={secenek}>
                {secenek}
              </option>
            ))}
          </select>
        </div>

        <div className="form-alan">
          <label htmlFor="teslimTarihi">Teslim Tarihi</label>
          <input
            id="teslimTarihi"
            type="date"
            value={teslimTarihi}
            onChange={(olay) => setTeslimTarihi(olay.target.value)}
          />
        </div>
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

export default GorevFormu;
