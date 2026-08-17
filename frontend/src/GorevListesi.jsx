import { useEffect, useState } from "react";
import {
  calisanlariGetir,
  gorevEkle,
  gorevGuncelle,
  gorevSil,
  gorevleriGetir,
  projeleriGetir
} from "./api.js";
import GorevFormu from "./GorevFormu.jsx";

function GorevListesi({ tokenlar, onTokenYenilendi }) {
  const [gorevler, setGorevler] = useState([]);
  const [calisanlar, setCalisanlar] = useState([]);
  const [projeler, setProjeler] = useState([]);
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
        const gelenGorevler = await gorevleriGetir(tokenlar, onTokenYenilendi);
        const gelenCalisanlar = await calisanlariGetir(tokenlar, onTokenYenilendi);
        const gelenProjeler = await projeleriGetir(tokenlar, onTokenYenilendi);

        if (gecerli) {
          setGorevler(gelenGorevler);
          setCalisanlar(gelenCalisanlar);
          setProjeler(gelenProjeler);
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
    const gelen = await gorevleriGetir(tokenlar, onTokenYenilendi);
    setGorevler(gelen);
  }

  function yeniEkle() {
    setDuzenlenen(null);
    setFormAcik(true);
    setHata("");
    setBilgi("");
  }

  function duzenle(gorev) {
    setDuzenlenen(gorev);
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
        ? await gorevGuncelle(duzenlenen.gorevId, veri, tokenlar, onTokenYenilendi)
        : await gorevEkle(veri, tokenlar, onTokenYenilendi);

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

  async function sil(gorev) {
    const onay = window.confirm(
      `${gorev.gorevAdi} görevi silinecek. Emin misiniz?`
    );

    if (!onay) {
      return;
    }

    setHata("");
    setBilgi("");

    try {
      const sonuc = await gorevSil(gorev.gorevId, tokenlar, onTokenYenilendi);

      await listeyiYenile();

      setBilgi(sonuc?.message ?? "Görev silindi.");
    } catch (sorun) {
      setHata(sorun.message);
    }
  }

  function tarihYaz(deger) {
    if (!deger) {
      return "-";
    }

    return new Date(deger).toLocaleDateString("tr-TR");
  }

  return (
    <div>
      <div className="bolum-basligi">
        <h2>Görevler</h2>
        <button type="button" className="islem-butonu" onClick={yeniEkle}>
          Yeni Görev
        </button>
      </div>

      {hata !== "" && <div className="hata">{hata}</div>}
      {bilgi !== "" && <div className="basarili">{bilgi}</div>}

      {formAcik && (
        <GorevFormu
          key={duzenlenen?.gorevId ?? "yeni"}
          gorev={duzenlenen}
          calisanlar={calisanlar}
          projeler={projeler}
          onKaydet={kaydet}
          onVazgec={formuKapat}
          kaydediliyor={kaydediliyor}
        />
      )}

      {yukleniyor && <p className="bilgi">Liste yükleniyor...</p>}

      {!yukleniyor && hata === "" && gorevler.length === 0 && (
        <p className="bilgi">Kayıtlı görev bulunamadı.</p>
      )}

      {!yukleniyor && gorevler.length > 0 && (
        <div className="tablo-kutusu">
          <table>
            <thead>
              <tr>
                <th>No</th>
                <th>Görev Adı</th>
                <th>Çalışan</th>
                <th>Proje</th>
                <th>Durum</th>
                <th>Teslim</th>
                <th className="sag">İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {gorevler.map((gorev) => (
                <tr key={gorev.gorevId}>
                  <td>{gorev.gorevId}</td>
                  <td>{gorev.gorevAdi ?? "-"}</td>
                  <td>{gorev.calisanAdi}</td>
                  <td>{gorev.projeAdi}</td>
                  <td>{gorev.durum ?? "-"}</td>
                  <td>{tarihYaz(gorev.teslimTarihi)}</td>
                  <td className="sag islem-hucresi">
                    <button
                      type="button"
                      className="satir-butonu"
                      onClick={() => duzenle(gorev)}
                    >
                      Düzenle
                    </button>
                    <button
                      type="button"
                      className="satir-butonu sil-butonu"
                      onClick={() => sil(gorev)}
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

export default GorevListesi;
