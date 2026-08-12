import { useState } from "react";

function DepartmanFormu({ departman, onKaydet, onVazgec, kaydediliyor }) {
  const [departmanAdi, setDepartmanAdi] = useState(departman?.departmanAdi ?? "");
  const [hata, setHata] = useState("");

  function formGonder(olay) {
    olay.preventDefault();
    setHata("");

    if (departmanAdi.trim() === "") {
      setHata("Departman adı boş bırakılamaz.");
      return;
    }

    onKaydet({ departmanAdi: departmanAdi.trim() });
  }

  return (
    <form className="form-karti" onSubmit={formGonder}>
      <h2>{departman ? "Departman Güncelle" : "Yeni Departman Ekle"}</h2>

      <div className="form-alan">
        <label htmlFor="departmanAdi">Departman Adı</label>
        <input
          id="departmanAdi"
          type="text"
          maxLength={100}
          value={departmanAdi}
          onChange={(olay) => setDepartmanAdi(olay.target.value)}
        />
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

export default DepartmanFormu;
