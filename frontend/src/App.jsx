import { useState } from "react";
import Login from "./Login.jsx";
import CalisanListesi from "./CalisanListesi.jsx";
import YoneticiListesi from "./YoneticiListesi.jsx";

function App() {
  const [tokenlar, setTokenlar] = useState(() => {
    const kayitli = localStorage.getItem("tokenlar");
    return kayitli ? JSON.parse(kayitli) : null;
  });
  const [aktifSayfa, setAktifSayfa] = useState("calisanlar");

  function tokenlariKaydet(yeniTokenlar) {
    localStorage.setItem("tokenlar", JSON.stringify(yeniTokenlar));
    setTokenlar(yeniTokenlar);
  }

  function cikisYap() {
    localStorage.removeItem("tokenlar");
    setTokenlar(null);
  }

  if (!tokenlar) {
    return <Login onGirisBasarili={tokenlariKaydet} />;
  }

  return (
    <div className="liste-sayfa">
      <div className="ust-bar">
        <h1>Personel Yönetim Paneli</h1>
        <button type="button" className="cikis-butonu" onClick={cikisYap}>
          Çıkış Yap
        </button>
      </div>

      <div className="sekmeler">
        <button
          type="button"
          className={aktifSayfa === "calisanlar" ? "sekme aktif" : "sekme"}
          onClick={() => setAktifSayfa("calisanlar")}
        >
          Çalışanlar
        </button>
        <button
          type="button"
          className={aktifSayfa === "yoneticiler" ? "sekme aktif" : "sekme"}
          onClick={() => setAktifSayfa("yoneticiler")}
        >
          Yöneticiler
        </button>
      </div>

      {aktifSayfa === "calisanlar" && (
        <CalisanListesi
          tokenlar={tokenlar}
          onTokenYenilendi={tokenlariKaydet}
        />
      )}

      {aktifSayfa === "yoneticiler" && (
        <YoneticiListesi
          tokenlar={tokenlar}
          onTokenYenilendi={tokenlariKaydet}
        />
      )}
    </div>
  );
}

export default App;
