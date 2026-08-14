import { useState } from "react";

function tarihAlaniIcin(deger) {
  if (!deger) {
    return "";
  }

  return deger.slice(0, 10);
}

function ProjeFormu({ proje, onKaydet, onVazgec, kaydediliyor }) {
  const [projeAdi, setProjeAdi] = useState(proje?.projeAdi ?? "");
  const [baslangicTarihi, setBaslangicTarihi] = useState(
    tarihAlaniIcin(proje?.baslangicTarihi)
  );
  const [bitisTarihi, setBitisTarihi] = useState(
    tarihAlaniIcin(proje?.bitisTarihi)
  );
  const [butce, setButce] = useState(
    proje?.butce === null || proje?.butce === undefined ? "" : String(proje.butce)
  );
  const [hata, setHata] = useState("");

  function formGonder(olay) {
    olay.preventDefault();
    setHata("");

    if (projeAdi.trim() === "") {
      setHata("Proje adı boş bırakılamaz.");
      return;
    }

    if (butce !== "" && (Number.isNaN(Number(butce)) || Number(butce) < 0)) {
      setHata("Bütçe geçerli bir sayı olmalı.");
      return;
    }

    if (baslangicTarihi !== "" && bitisTarihi !== "" && bitisTarihi < baslangicTarihi) {
      setHata("Bitiş tarihi başlangıç tarihinden önce olamaz.");
      return;
    }

    onKaydet({
      projeAdi: projeAdi.trim(),
      baslangicTarihi: baslangicTarihi === "" ? null : baslangicTarihi,
      bitisTarihi: bitisTarihi === "" ? null : bitisTarihi,
      butce: butce === "" ? null : Number(butce)
    });
  }

  return (
    <form className="form-karti" onSubmit={formGonder}>
      <h2>{proje ? "Proje Güncelle" : "Yeni Proje Ekle"}</h2>

      <div className="form-satir">
        <div className="form-alan">
          <label htmlFor="projeAdi">Proje Adı</label>
          <input
            id="projeAdi"
            type="text"
            maxLength={100}
            value={projeAdi}
            onChange={(olay) => setProjeAdi(olay.target.value)}
          />
        </div>

        <div className="form-alan">
          <label htmlFor="butce">Bütçe</label>
          <input
            id="butce"
            type="number"
            min="0"
            step="1"
            value={butce}
            onChange={(olay) => setButce(olay.target.value)}
          />
        </div>
      </div>

      <div className="form-satir">
        <div className="form-alan">
          <label htmlFor="baslangicTarihi">Başlangıç Tarihi</label>
          <input
            id="baslangicTarihi"
            type="date"
            value={baslangicTarihi}
            onChange={(olay) => setBaslangicTarihi(olay.target.value)}
          />
        </div>

        <div className="form-alan">
          <label htmlFor="bitisTarihi">Bitiş Tarihi</label>
          <input
            id="bitisTarihi"
            type="date"
            value={bitisTarihi}
            onChange={(olay) => setBitisTarihi(olay.target.value)}
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

export default ProjeFormu;
