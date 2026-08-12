import { useState } from "react";
import Login from "./Login.jsx";
import CalisanListesi from "./CalisanListesi.jsx";
import DepartmanListesi from "./DepartmanListesi.jsx";

function App() {
  const [tokenlar, setTokenlar] = useState(() => {
    const kayitli = localStorage.getItem("tokenlar");
    return kayitli ? JSON.parse(kayitli) : null;
  });
  const [sayfa, setSayfa] = useState("calisanlar");

  function tokenlariKaydet(yeniTokenlar) {
    localStorage.setItem("tokenlar", JSON.stringify(yeniTokenlar));
    setTokenlar(yeniTokenlar);
  }

  function cikisYap() {
    localStorage.removeItem("tokenlar");
    setTokenlar(null);
    setSayfa("calisanlar");
  }

  if (!tokenlar) {
    return <Login onGirisBasarili={tokenlariKaydet} />;
  }

  return (
    <div className="liste-sayfa">
      <div className="ust-bar">
        <h1>Personel Yönetim Paneli</h1>
        <button type="button" className="ikincil-buton tekil-buton" onClick={cikisYap}>
          Çıkış Yap
        </button>
      </div>

      <div className="sekmeler">
        <button
          type="button"
          className={sayfa === "calisanlar" ? "sekme sekme-aktif" : "sekme"}
          onClick={() => setSayfa("calisanlar")}
        >
          Çalışanlar
        </button>
        <button
          type="button"
          className={sayfa === "departmanlar" ? "sekme sekme-aktif" : "sekme"}
          onClick={() => setSayfa("departmanlar")}
        >
          Departmanlar
        </button>
      </div>

      {sayfa === "calisanlar" ? (
        <CalisanListesi tokenlar={tokenlar} onTokenYenilendi={tokenlariKaydet} />
      ) : (
        <DepartmanListesi tokenlar={tokenlar} onTokenYenilendi={tokenlariKaydet} />
      )}
    </div>
  );
}

export default App;
