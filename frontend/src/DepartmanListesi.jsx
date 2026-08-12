import { useEffect, useState } from "react";
import {
  departmanEkle,
  departmanGuncelle,
  departmanSil,
  departmanlariGetir
} from "./api.js";
import DepartmanFormu from "./DepartmanFormu.jsx";

function DepartmanListesi({ tokenlar, onTokenYenilendi }) {
  const [departmanlar, setDepartmanlar] = useState([]);
  const [hata, setHata] = useState("");
  const [bilgi, setBilgi] = useState("");
  const [yukleniyor, setYukleniyor] = useState(true);
  const [formAcik, setFormAcik] = useState(false);
  const [duzenlenen, setDuzenlenen] = useState(null);
  const [kaydediliyor, setKaydediliyor] = useState(false);

  useEffect(() => {
    let gecerli = true;

    async function baslangicVerisiniYukle() {
      try {
        const gelen = await departmanlariGetir(tokenlar, onTokenYenilendi);

        if (gecerli) {
          setDepartmanlar(gelen);
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

    baslangicVerisiniYukle();

    return () => {
      gecerli = false;
    };
  }, []);

  async function listeyiYenile() {
    const gelen = await departmanlariGetir(tokenlar, onTokenYenilendi);
    setDepartmanlar(gelen);
  }

  function yeniEkle() {
    setDuzenlenen(null);
    setFormAcik(true);
    setHata("");
    setBilgi("");
  }

  function duzenle(departman) {
    setDuzenlenen(departman);
    setFormAcik(true);
    setHata("");
    setBilgi("");
  }

  function formuKapat() {
    setFormAcik(false);
    setDuzenlenen(null);
  }

  async function kaydet(veri) {
    setKaydediliyor(true);
    setHata("");
    setBilgi("");

    try {
      const sonuc = duzenlenen
        ? await departmanGuncelle(duzenlenen.departmanId, veri, tokenlar, onTokenYenilendi)
        : await departmanEkle(veri, tokenlar, onTokenYenilendi);

      await listeyiYenile();

      setFormAcik(false);
      setDuzenlenen(null);
      setBilgi(sonuc?.message ?? "Kayıt tamamlandı.");
    } catch (sorun) {
      setHata(sorun.message);
    } finally {
      setKaydediliyor(false);
    }
  }

  async function sil(departman) {
    const onay = window.confirm(
      `${departman.departmanAdi} departmanı silinecek. Emin misiniz?`
    );

    if (!onay) {
      return;
    }

    setHata("");
    setBilgi("");

    try {
      const sonuc = await departmanSil(departman.departmanId, tokenlar, onTokenYenilendi);

      await listeyiYenile();

      setBilgi(sonuc?.message ?? "Departman silindi.");
    } catch (sorun) {
      setHata(sorun.message);
    }
  }

  return (
    <div>
      <div className="bolum-basligi">
        <h2>Departmanlar</h2>
        <button type="button" className="islem-butonu" onClick={yeniEkle}>
          Yeni Departman
        </button>
      </div>

      {hata !== "" && <div className="hata">{hata}</div>}
      {bilgi !== "" && <div className="basarili">{bilgi}</div>}

      {formAcik && (
        <DepartmanFormu
          key={duzenlenen?.departmanId ?? "yeni"}
          departman={duzenlenen}
          onKaydet={kaydet}
          onVazgec={formuKapat}
          kaydediliyor={kaydediliyor}
        />
      )}

      {yukleniyor && <p className="bilgi">Liste yükleniyor...</p>}

      {!yukleniyor && hata === "" && departmanlar.length === 0 && (
        <p className="bilgi">Kayıtlı departman bulunamadı.</p>
      )}

      {!yukleniyor && departmanlar.length > 0 && (
        <div className="tablo-kutusu">
          <table className="dar-tablo">
            <thead>
              <tr>
                <th>No</th>
                <th>Departman Adı</th>
                <th className="sag">İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {departmanlar.map((departman) => (
                <tr key={departman.departmanId}>
                  <td>{departman.departmanId}</td>
                  <td>{departman.departmanAdi}</td>
                  <td className="sag islem-hucresi">
                    <button
                      type="button"
                      className="satir-butonu"
                      onClick={() => duzenle(departman)}
                    >
                      Düzenle
                    </button>
                    <button
                      type="button"
                      className="satir-butonu sil-butonu"
                      onClick={() => sil(departman)}
                    >
                      Sil
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default DepartmanListesi;
