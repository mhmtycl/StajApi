import { useEffect, useState } from "react";
import {
  projeEkle,
  projeGuncelle,
  projeSil,
  projeleriGetir
} from "./api.js";
import ProjeFormu from "./ProjeFormu.jsx";

function ProjeListesi({ tokenlar, onTokenYenilendi }) {
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
        const gelen = await projeleriGetir(tokenlar, onTokenYenilendi);

        if (gecerli) {
          setProjeler(gelen);
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
    const gelen = await projeleriGetir(tokenlar, onTokenYenilendi);
    setProjeler(gelen);
  }

  function yeniEkle() {
    setDuzenlenen(null);
    setFormAcik(true);
    setHata("");
    setBilgi("");
  }

  function duzenle(proje) {
    setDuzenlenen(proje);
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
        ? await projeGuncelle(duzenlenen.projeId, veri, tokenlar, onTokenYenilendi)
        : await projeEkle(veri, tokenlar, onTokenYenilendi);

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

  async function sil(proje) {
    const onay = window.confirm(
      `${proje.projeAdi} projesi silinecek. Emin misiniz?`
    );

    if (!onay) {
      return;
    }

    setHata("");
    setBilgi("");

    try {
      const sonuc = await projeSil(proje.projeId, tokenlar, onTokenYenilendi);

      await listeyiYenile();

      setBilgi(sonuc?.message ?? "Proje silindi.");
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

  function butceYaz(butce) {
    if (butce === null || butce === undefined) {
      return "-";
    }

    return butce.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) + " TL";
  }

  return (
    <div>
      <div className="bolum-basligi">
        <h2>Projeler</h2>
        <button type="button" className="islem-butonu" onClick={yeniEkle}>
          Yeni Proje
        </button>
      </div>

      {hata !== "" && <div className="hata">{hata}</div>}
      {bilgi !== "" && <div className="basarili">{bilgi}</div>}

      {formAcik && (
        <ProjeFormu
          key={duzenlenen?.projeId ?? "yeni"}
          proje={duzenlenen}
          onKaydet={kaydet}
          onVazgec={formuKapat}
          kaydediliyor={kaydediliyor}
        />
      )}

      {yukleniyor && <p className="bilgi">Liste yükleniyor...</p>}

      {!yukleniyor && hata === "" && projeler.length === 0 && (
        <p className="bilgi">Kayıtlı proje bulunamadı.</p>
      )}

      {!yukleniyor && projeler.length > 0 && (
        <div className="tablo-kutusu">
          <table>
            <thead>
              <tr>
                <th>No</th>
                <th>Proje Adı</th>
                <th>Başlangıç</th>
                <th>Bitiş</th>
                <th className="sag">Bütçe</th>
                <th className="sag">İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {projeler.map((proje) => (
                <tr key={proje.projeId}>
                  <td>{proje.projeId}</td>
                  <td>{proje.projeAdi}</td>
                  <td>{tarihYaz(proje.baslangicTarihi)}</td>
                  <td>{tarihYaz(proje.bitisTarihi)}</td>
                  <td className="sag">{butceYaz(proje.butce)}</td>
                  <td className="sag islem-hucresi">
                    <button
                      type="button"
                      className="satir-butonu"
                      onClick={() => duzenle(proje)}
                    >
                      Düzenle
                    </button>
                    <button
                      type="button"
                      className="satir-butonu sil-butonu"
                      onClick={() => sil(proje)}
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

export default ProjeListesi;
