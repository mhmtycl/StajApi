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

async function calisanlariIste(accessToken) {
  const cevap = await fetch(`${API_URL}/api/calisanlar`, {
    headers: { Authorization: `Bearer ${accessToken}` }
  });

  if (cevap.status === 401) {
    return null;
  }

  if (!cevap.ok) {
    throw new Error("Çalışan listesi alınamadı.");
  }

  return cevap.json();
}

export async function calisanlariGetir(tokenlar, tokenlariGuncelle) {
  let liste = await calisanlariIste(tokenlar.accessToken);

  if (liste === null) {
    const yeniTokenlar = await tokenYenile(tokenlar.refreshToken);
    tokenlariGuncelle(yeniTokenlar);

    liste = await calisanlariIste(yeniTokenlar.accessToken);

    if (liste === null) {
      throw new Error("Oturum süresi doldu, tekrar giriş yapın.");
    }
  }

  return liste;
}
