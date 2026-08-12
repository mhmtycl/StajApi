import { useEffect, useState } from "react";
import {
  yoneticileriGetir,
  yoneticiEkle,
  yoneticiGuncelle,
  yoneticiSil
} from "./api.js";

const bosForm = { ad: "", soyad: "", departman: "" };

function YoneticiListesi({ tokenlar, onTokenYenilendi }) {
  const [yoneticiler, setYoneticiler] = useState([]);
  const [hata, setHata] = useState("");
  const [yukleniyor, setYukleniyor] = useState(true);
  const [form, setForm] = useState(bosForm);
  const [duzenlenenId, setDuzenlenenId] = useState(null);
  const [kaydediliyor, setKaydediliyor] = useState(false);

  async function listeyiYukle() {
    try {
      const liste = await yoneticileriGetir(tokenlar, onTokenYenilendi);
      setYoneticiler(liste);
      setHata("");
    } catch (sorun) {
      setHata(sorun.message);
    } finally {
      setYukleniyor(false);
    }
  }

  useEffect(() => {
    listeyiYukle();
  }, []);

  function alanDegisti(olay) {
    setForm({ ...form, [olay.target.name]: olay.target.value });
  }

  function duzenlemeyeBasla(yonetici) {
    setDuzenlenenId(yonetici.yoneticiId);
    setForm({
      ad: yonetici.ad,
      soyad: yonetici.soyad,
      departman: yonetici.departman
    });
  }

  function formuTemizle() {
    setDuzenlenenId(null);
    setForm(bosForm);
  }

  async function formGonder(olay) {
    olay.preventDefault();
    setHata("");

    if (
      form.ad.trim() === "" ||
      form.soyad.trim() === "" ||
      form.departman.trim() === ""
    ) {
      setHata("Ad, soyad ve departman boş bırakılamaz.");
      return;
    }

    setKaydediliyor(true);

    try {
      const yonetici = {
        ad: form.ad.trim(),
        soyad: form.soyad.trim(),
        departman: form.departman.trim()
      };

      if (duzenlenenId === null) {
        await yoneticiEkle(yonetici, tokenlar, onTokenYenilendi);
      } else {
        await yoneticiGuncelle(
          { ...yonetici, yoneticiId: duzenlenenId },
          tokenlar,
          onTokenYenilendi
        );
      }

      formuTemizle();
      await listeyiYukle();
    } catch (sorun) {
      setHata(sorun.message);
    } finally {
      setKaydediliyor(false);
    }
  }

  async function kayitSil(yonetici) {
    const onay = window.confirm(
      `${yonetici.ad} ${yonetici.soyad} silinsin mi?`
    );

    if (!onay) {
      return;
    }

    setHata("");

    try {
      await yoneticiSil(yonetici.yoneticiId, tokenlar, onTokenYenilendi);

      if (duzenlenenId === yonetici.yoneticiId) {
        formuTemizle();
      }

      await listeyiYukle();
    } catch (sorun) {
      setHata(sorun.message);
    }
  }

  return (
    <div>
      <form className="kayit-formu" onSubmit={formGonder}>
        <h2>
          {duzenlenenId === null ? "Yeni Yönetici Ekle" : "Yönetici Düzenle"}
        </h2>

        <div className="form-satiri">
          <div className="form-alani">
            <label htmlFor="yonetici-ad">Ad</label>
            <input
              id="yonetici-ad"
              name="ad"
              value={form.ad}
              onChange={alanDegisti}
            />
          </div>

          <div className="form-alani">
            <label htmlFor="yonetici-soyad">Soyad</label>
            <input
              id="yonetici-soyad"
              name="soyad"
              value={form.soyad}
              onChange={alanDegisti}
            />
          </div>

          <div className="form-alani">
            <label htmlFor="yonetici-departman">Departman</label>
            <input
              id="yonetici-departman"
              name="departman"
              value={form.departman}
              onChange={alanDegisti}
            />
          </div>
        </div>

        <div className="form-butonlari">
          <button type="submit" disabled={kaydediliyor}>
            {kaydediliyor
              ? "Kaydediliyor..."
              : duzenlenenId === null
                ? "Ekle"
                : "Güncelle"}
          </button>

          {duzenlenenId !== null && (
            <button type="button" className="ikincil" onClick={formuTemizle}>
              İptal
            </button>
          )}
        </div>
      </form>

      {hata !== "" && <div className="hata">{hata}</div>}

      {yukleniyor && <p className="bilgi">Liste yükleniyor...</p>}

      {!yukleniyor && hata === "" && yoneticiler.length === 0 && (
        <p className="bilgi">Kayıtlı yönetici bulunamadı.</p>
      )}

      {!yukleniyor && yoneticiler.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>No</th>
              <th>Ad</th>
              <th>Soyad</th>
              <th>Departman</th>
              <th className="sag">İşlemler</th>
            </tr>
          </thead>
          <tbody>
            {yoneticiler.map((yonetici) => (
              <tr key={yonetici.yoneticiId}>
                <td>{yonetici.yoneticiId}</td>
                <td>{yonetici.ad}</td>
                <td>{yonetici.soyad}</td>
                <td>{yonetici.departman}</td>
                <td className="sag">
                  <button
                    type="button"
                    className="tablo-butonu"
                    onClick={() => duzenlemeyeBasla(yonetici)}
                  >
                    Düzenle
                  </button>
                  <button
                    type="button"
                    className="tablo-butonu sil"
                    onClick={() => kayitSil(yonetici)}
                  >
                    Sil
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default YoneticiListesi;
