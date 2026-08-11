import { useEffect, useState } from "react";
import { calisanlariGetir } from "./api.js";

function CalisanListesi({ tokenlar, onTokenYenilendi, onCikis }) {
  const [calisanlar, setCalisanlar] = useState([]);
  const [hata, setHata] = useState("");
  const [yukleniyor, setYukleniyor] = useState(true);

  useEffect(() => {
    let gecerli = true;

    async function listeyiYukle() {
      try {
        const liste = await calisanlariGetir(tokenlar, onTokenYenilendi);

        if (gecerli) {
          setCalisanlar(liste);
        }
      } catch (sorun) {
        if (gecerli) {
          setHata(sorun.message);
        }
      } finally {
        if (gecerli) {
          setYukleniyor(false);
        }
      }
    }

    listeyiYukle();

    return () => {
      gecerli = false;
    };
  }, []);

  function maasYaz(maas) {
    return maas.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) + " TL";
  }

  return (
    <div className="liste-sayfa">
      <div className="ust-bar">
        <h1>Çalışan Listesi</h1>
        <button type="button" className="cikis-butonu" onClick={onCikis}>
          Çıkış Yap
        </button>
      </div>

      {hata !== "" && <div className="hata">{hata}</div>}

      {yukleniyor && <p className="bilgi">Liste yükleniyor...</p>}

      {!yukleniyor && hata === "" && calisanlar.length === 0 && (
        <p className="bilgi">Kayıtlı çalışan bulunamadı.</p>
      )}

      {!yukleniyor && calisanlar.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>No</th>
              <th>Ad</th>
              <th>Soyad</th>
              <th className="sag">Maaş</th>
            </tr>
          </thead>
          <tbody>
            {calisanlar.map((calisan) => (
              <tr key={calisan.calisanId}>
                <td>{calisan.calisanId}</td>
                <td>{calisan.ad}</td>
                <td>{calisan.soyad}</td>
                <td className="sag">{maasYaz(calisan.maas)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default CalisanListesi;
