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

async function tokenliIstek(yol, secenekler, accessToken) {
  const cevap = await fetch(`${API_URL}${yol}`, {
    ...secenekler,
    headers: {
      ...(secenekler.headers ?? {}),
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (cevap.status === 401) {
    return null;
  }

  return cevap;
}

async function istekAt(yol, secenekler, tokenlar, tokenlariGuncelle) {
  let cevap = await tokenliIstek(yol, secenekler, tokenlar.accessToken);

  if (cevap === null) {
    const yeniTokenlar = await tokenYenile(tokenlar.refreshToken);
    tokenlariGuncelle(yeniTokenlar);

    cevap = await tokenliIstek(yol, secenekler, yeniTokenlar.accessToken);

    if (cevap === null) {
      throw new Error("Oturum süresi doldu, tekrar giriş yapın.");
    }
  }

  return cevap;
}

async function sonucuCoz(cevap, varsayilanHata) {
  const veri = await cevap.json().catch(() => null);

  if (!cevap.ok) {
    throw new Error(veri?.message ?? varsayilanHata);
  }

  return veri;
}

export async function calisanlariGetir(tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt("/api/calisanlar", {}, tokenlar, tokenlariGuncelle);

  return sonucuCoz(cevap, "Çalışan listesi alınamadı.");
}

export async function departmanlariGetir(tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt("/api/departmanlar", {}, tokenlar, tokenlariGuncelle);

  return sonucuCoz(cevap, "Departman listesi alınamadı.");
}

export async function calisanEkle(calisan, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    "/api/calisanlar",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(calisan)
    },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Çalışan eklenemedi.");
}

export async function calisanGuncelle(id, calisan, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    `/api/calisanlar/${id}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(calisan)
    },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Çalışan güncellenemedi.");
}

export async function departmanEkle(departman, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    "/api/departmanlar",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(departman)
    },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Departman eklenemedi.");
}

export async function departmanGuncelle(id, departman, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    `/api/departmanlar/${id}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(departman)
    },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Departman güncellenemedi.");
}

export async function departmanSil(id, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    `/api/departmanlar/${id}`,
    { method: "DELETE" },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Departman silinemedi.");
}

export async function projeleriGetir(tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt("/api/projeler", {}, tokenlar, tokenlariGuncelle);

  return sonucuCoz(cevap, "Proje listesi alınamadı.");
}

export async function projeEkle(proje, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    "/api/projeler",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(proje)
    },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Proje eklenemedi.");
}

export async function projeGuncelle(id, proje, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    `/api/projeler/${id}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(proje)
    },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Proje güncellenemedi.");
}

export async function projeSil(id, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    `/api/projeler/${id}`,
    { method: "DELETE" },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Proje silinemedi.");
}

export async function calisanSil(id, tokenlar, tokenlariGuncelle) {
  const cevap = await istekAt(
    `/api/calisanlar/${id}`,
    { method: "DELETE" },
    tokenlar,
    tokenlariGuncelle
  );

  return sonucuCoz(cevap, "Çalışan silinemedi.");
}
