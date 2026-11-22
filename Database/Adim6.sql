-- Adım 6: pgAdmin'de TasinmazYonetimiDB üzerinde çalıştırınız.

-- 1) "Yeni Kayit" kaydını kodla uyumlu hale getir
UPDATE "IslemTipleri" SET "IslemTipiAdi" = 'Yeni Kayıt' WHERE "Id" = 3;

-- 2) Çıkış yapılan (iptal edilen) token'lar
CREATE TABLE IF NOT EXISTS "IptalEdilenTokenlar" (
    "Id" integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "TokenId" varchar(100) NOT NULL UNIQUE,
    "BitisZamani" timestamptz NOT NULL
);
