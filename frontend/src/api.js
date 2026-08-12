const API_URL = "http://localhost:5089";

export async function girisYap(kullaniciAdi, sifre) {
  const cevap = await fetch(`${API_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username: kullaniciAdi, password: sifre })
  });

  const veri = await cevap.json().catch(() => null);

  if (!cevap.ok) {
    throw new Error(veri?.message ?? "Giriş yapılamadı.");
  }

  return {
    accessToken: veri.accessToken,
    refreshToken: veri.refreshToken
  };
}

export async function tokenYenile(refreshToken) {
  const cevap = await fetch(`${API_URL}/api/auth/refresh`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken })
  });

  const veri = await cevap.json().catch(() => null);

  if (!cevap.ok) {
    throw new Error(veri?.message ?? "Oturum süresi doldu, tekrar giriş yapın.");
  }

  return {
    accessToken: veri.accessToken,
    refreshToken: veri.refreshToken
  };
}

async function yetkiliIstek(yol, secenekler, tokenlar, tokenlariGuncelle) {
  const istekOlustur = (accessToken) =>
    fetch(`${API_URL}${yol}`, {
      ...secenekler,
      headers: {
        "Content-Type": "application/json",
        ...secenekler.headers,
        Authorization: `Bearer ${accessToken}`
      }
    });

  let cevap = await istekOlustur(tokenlar.accessToken);

  if (cevap.status === 401) {
    const yeniTokenlar = await tokenYenile(tokenlar.refreshToken);
    tokenlariGuncelle(yeniTokenlar);

    cevap = await istekOlustur(yeniTokenlar.accessToken);

    if (cevap.status === 401) {
      throw new Error("Oturum süresi doldu, tekrar giriş yapın.");
    }
  }

  return cevap;
}

export async function calisanlariGetir(tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    "/api/calisanlar",
    {},
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Çalışan listesi alınamadı.");
  }

  return cevap.json();
}

export async function calisanEkle(calisan, tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    "/api/calisanlar",
    { method: "POST", body: JSON.stringify(calisan) },
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Çalışan eklenemedi.");
  }
}

export async function calisanGuncelle(calisan, tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    `/api/calisanlar/${calisan.calisanId}`,
    { method: "PUT", body: JSON.stringify(calisan) },
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Çalışan güncellenemedi.");
  }
}

export async function calisanSil(calisanId, tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    `/api/calisanlar/${calisanId}`,
    { method: "DELETE" },
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Çalışan silinemedi.");
  }
}

export async function yoneticileriGetir(tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    "/api/yoneticiler",
    {},
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Yönetici listesi alınamadı.");
  }

  return cevap.json();
}

export async function yoneticiEkle(yonetici, tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    "/api/yoneticiler",
    { method: "POST", body: JSON.stringify(yonetici) },
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Yönetici eklenemedi.");
  }
}

export async function yoneticiGuncelle(yonetici, tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    `/api/yoneticiler/${yonetici.yoneticiId}`,
    { method: "PUT", body: JSON.stringify(yonetici) },
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Yönetici güncellenemedi.");
  }
}

export async function yoneticiSil(yoneticiId, tokenlar, tokenlariGuncelle) {
  const cevap = await yetkiliIstek(
    `/api/yoneticiler/${yoneticiId}`,
    { method: "DELETE" },
    tokenlar,
    tokenlariGuncelle
  );

  if (!cevap.ok) {
    throw new Error("Yönetici silinemedi.");
  }
}
