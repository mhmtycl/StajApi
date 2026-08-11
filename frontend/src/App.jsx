import { useState } from "react";
import Login from "./Login.jsx";
import CalisanListesi from "./CalisanListesi.jsx";

function App() {
  const [tokenlar, setTokenlar] = useState(() => {
    const kayitli = localStorage.getItem("tokenlar");
    return kayitli ? JSON.parse(kayitli) : null;
  });

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
    <CalisanListesi
      tokenlar={tokenlar}
      onTokenYenilendi={tokenlariKaydet}
      onCikis={cikisYap}
    />
  );
}

export default App;
