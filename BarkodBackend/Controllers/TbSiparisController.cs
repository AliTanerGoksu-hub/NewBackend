using BarkodBackend.Context;
using BarkodBackend.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BarkodBackend.Controllers
{
    [Route("api/[controller]")]
    public class TbSiparisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public TbSiparisController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DB")!;
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetProductsAsync(string lSiparisNo, string sSiparisiVeren, string sFirmaAciklama, string beginDate, string endDate, string sSaticiRumuzu, string opt = ">", double lKalan = 0)
        {
            try
            {

                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT  tbSiparis.lSiparisNo, tbSiparis.dteSiparisTarihi, SUM(tbSiparis.lMiktari) AS lMiktari, ISNULL(SUM(tbSevkiyat.lSevkMiktari),0) AS lSevkiyat, SUM(tbSiparis.lMiktari) - ISNULL(SUM(tbSevkiyat.lSevkMiktari),0) AS lKalan, ISNULL(SUM(tbYukleme.lYukleme),0) as lYukleme,SUM(tbSiparis.lTutari) AS lTutari, SUM(tbSiparis.lTutari * (100 + tbSiparis.nKdvOrani) / 100) AS 
lNetTutar, SUM(tbSiparis.lIskontosuzTutari - tbSiparis.lTutari) AS lIskontoTutari, SUM(tbSiparis.lIskontosuzTutari - tbSiparis.lTutari) / SUM(tbSiparis.lIskontosuzTutari) AS nIsk, ISNULL(tbSiparisAciklamasi.sAciklama1, '') 
AS sAciklama1, ISNULL(tbSiparisAciklamasi.sAciklama2, '') AS sAciklama2, ISNULL(tbSiparisAciklamasi.sAciklama3, '') AS sAciklama3, ISNULL(tbSiparisAciklamasi.sAciklama4, '') AS sAciklama4, 
ISNULL(tbSiparisAciklamasi.sAciklama5, '') AS sAciklama5, ISNULL(tbSiparisAciklamasi.bKilitli, 0) AS bKilitli, tbFirma.nFirmaID, tbFirma.sKodu AS sFirmaKodu, tbFirma.sAciklama AS sFirmaAciklama, SUM(ROUND(tbSiparis.lTutari 
* (tbSiparis.nKdvOrani / 100), 2)) AS lKdvTutari, SUBSTRING(tbSiparis.sSiparisiAlan, 1, 30) AS sSiparisiAlan, SUBSTRING(tbSiparis.sSiparisiAlan, 31, 30) AS sSiparisiVeren FROM tbStok 
INNER JOIN tbSiparis INNER JOIN tbFirma ON tbSiparis.nFirmaID = tbFirma.nFirmaID ON tbStok.nStokID = tbSiparis.nStokID 
INNER JOIN tbKdv ON tbStok.sKdvTipi = tbKdv.sKdvTipi 
LEFT OUTER JOIN (SELECT ABS(SUM(lGirisMiktar1) - SUM(lCikisMiktar1)) AS lSevkMiktari , nSiparisID , nFirmaID FROM tbStokFisiDetayi WHERE (nSiparisID IS NOT NULL) GROUP BY nSiparisID , nFirmaID) tbSevkiyat ON tbSiparis.nFirmaID = tbSevkiyat.nFirmaID AND tbSiparis.nSiparisID = tbSevkiyat.nSiparisID
LEFT OUTER JOIN tbSiparisAciklamasi ON tbSiparis.nSiparisTipi = tbSiparisAciklamasi.nSiparisTipi AND tbSiparis.nFirmaID = tbSiparisAciklamasi.nFirmaID AND tbSiparis.lSiparisNo = tbSiparisAciklamasi.lSiparisNo 
LEFT OUTER JOIN (SELECT     TARIH AS dteSiparisTarihi, IZAHAT AS sFisTipi, FISNO AS lFisNo, FIRMANO,STOKNO,SUM(MIKTAR) AS lYukleme  FROM ASTOKPAKETDETAY  
GROUP BY TARIH, IZAHAT, FISNO,FIRMANO,STOKNO) tbYukleme ON tbSiparis.nFirmaID = tbYukleme.FIRMANO AND tbSiparis.dteSiparisTarihi = tbYukleme.dteSiparisTarihi and tbYukleme.lFisNo = tbSiparis.lSiparisNo and
tbYukleme.sFisTipi ='AS' and tbYukleme.STOKNO = tbSiparis.nStokID 
WHERE 
(tbSiparis.lSiparisNo BETWEEN 1 AND 999999999) AND 
(tbSiparis.dteSiparisTarihi BETWEEN '{beginDate}' AND  DATEADD(DAY,1,'{endDate}'))   AND 
(SUBSTRING(tbSiparis.sSiparisiAlan, 1, 30) BETWEEN '' AND 'zzzzzzzzzzzzzzzzzzzz') AND 
(SUBSTRING(tbSiparis.sSiparisiAlan, 31, 30) BETWEEN '' AND 'zzzzzzzzzzzzzzzzzzzz') AND 
(tbSiparis.nSiparisTipi = 1) AND
(tbSiparis.lSiparisNo  LIKE '{lSiparisNo}%' or '{lSiparisNo}'='' ) and
(tbSiparis.sSaticiRumuzu  = '{sSaticiRumuzu}') and
(tbFirma.sAciklama  LIKE '{sFirmaAciklama}%' or '{sFirmaAciklama}'='' ) 
GROUP BY tbSiparis.dteSiparisTarihi, tbSiparis.lSiparisNo, tbFirma.nFirmaID, tbFirma.sKodu, tbFirma.sAciklama, tbSiparisAciklamasi.sAciklama1, tbSiparisAciklamasi.sAciklama2, tbSiparisAciklamasi.sAciklama3, 
tbSiparisAciklamasi.sAciklama4, tbSiparisAciklamasi.sAciklama5, tbSiparisAciklamasi.bKilitli, tbSiparis.sSiparisiAlan  HAVING SUM(tbSiparis.lMiktari) - ISNULL(SUM(tbSevkiyat.lSevkMiktari),0) > 0 
ORDER BY tbSiparis.dteSiparisTarihi desc, tbSiparis.lSiparisNo, tbFirma.sKodu";
                    var stocks = await connection.QueryAsync<TbSiparis>(query);
                    return Ok(stocks);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails(string lSiparisNo, string sSaticiRumuzu = "")
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                SET DATEFORMAT DMY
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
                SELECT 
                    tbSiparis.nSiparisID, 
                    tbSiparis.lSiparisNo, 
                    tbSiparis.dteSiparisTarihi,
                    tbSiparis.sBirimCinsi, 

                    -- lMiktari: birim çevrimine göre üst birime dönüştür (örn. Adet → Koli)
                    SUM(
                        CAST(tbSiparis.lMiktari AS decimal(18,6)) /
                        CAST(CASE WHEN sbc.lOran IS NULL OR sbc.lOran = 0 THEN 1.0 ELSE sbc.lOran END AS decimal(18,6))
                    ) AS lMiktari,

                    ISNULL(SUM(tbSevkiyat.lSevkMiktari), 0) AS lSevkiyat, 

                    -- lKalan aynı birimde: (çevirilmiş lMiktari) - lSevkiyat
                    SUM(
                        CAST(tbSiparis.lMiktari AS decimal(18,6)) /
                        CAST(CASE WHEN sbc.lOran IS NULL OR sbc.lOran = 0 THEN 1.0 ELSE sbc.lOran END AS decimal(18,6))
                    ) - ISNULL(SUM(tbSevkiyat.lSevkMiktari), 0) AS lKalan, 

                    ISNULL(SUM(tbYukleme.lYukleme), 0) AS lYukleme,
                    SUM(tbSiparis.lTutari) AS lTutari, 
                    SUM(tbSiparis.lTutari * (100 + tbSiparis.nKdvOrani) / 100) AS lNetTutar, 
                    SUM(tbSiparis.lIskontosuzTutari - tbSiparis.lTutari) AS lIskontoTutari, 
                    tbSiparis.nIskontoYuzdesi, 
                    SUM(tbSiparis.lIskontosuzTutari - tbSiparis.lTutari) / NULLIF(SUM(tbSiparis.lIskontosuzTutari), 0) AS nIsk, 
                    ISNULL(tbSiparisAciklamasi.sAciklama1, '') AS sAciklama1, 
                    ISNULL(tbSiparisAciklamasi.sAciklama2, '') AS sAciklama2, 
                    ISNULL(tbSiparisAciklamasi.sAciklama3, '') AS sAciklama3, 
                    ISNULL(tbSiparisAciklamasi.sAciklama4, '') AS sAciklama4, 
                    ISNULL(tbSiparisAciklamasi.sAciklama5, '') AS sAciklama5, 
                    ISNULL(tbSiparisAciklamasi.bKilitli, 0) AS bKilitli, 
                    tbFirma.nFirmaID, 
                    tbFirma.sKodu AS sFirmaKodu, 
                    tbFirma.sAciklama AS sFirmaAciklama, 
                    SUM(ROUND(tbSiparis.lTutari * (tbSiparis.nKdvOrani / 100), 2)) AS lKdvTutari, 
                    SUBSTRING(tbSiparis.sSiparisiVeren, 1, 30) AS sSiparisiVeren,
                    tbStok.sKodu 
                FROM tbStok 
                INNER JOIN tbSiparis ON tbStok.nStokID = tbSiparis.nStokID 
                INNER JOIN tbFirma ON tbSiparis.nFirmaID = tbFirma.nFirmaID 
                INNER JOIN tbKdv ON tbStok.sKdvTipi = tbKdv.sKdvTipi 

                -- Birim çevirim tablosu
                LEFT JOIN tbStokBirimCevrimi sbc
                    ON sbc.nStokID = tbStok.nStokID
                    -- Eğer tabloda birim alanı varsa aç:
                    -- AND sbc.sBirim = tbSiparis.sBirimCinsi

                LEFT OUTER JOIN (
                    SELECT 
                        ABS(SUM(lGirisMiktar1) - SUM(lCikisMiktar1)) AS lSevkMiktari, 
                        nSiparisID, 
                        nFirmaID 
                    FROM tbStokFisiDetayi 
                    WHERE nSiparisID IS NOT NULL 
                    GROUP BY nSiparisID, nFirmaID
                ) tbSevkiyat ON tbSiparis.nFirmaID = tbSevkiyat.nFirmaID 
                    AND tbSiparis.nSiparisID = tbSevkiyat.nSiparisID 

                LEFT OUTER JOIN tbSiparisAciklamasi 
                    ON tbSiparis.nSiparisTipi = tbSiparisAciklamasi.nSiparisTipi 
                    AND tbSiparis.nFirmaID = tbSiparisAciklamasi.nFirmaID 
                    AND tbSiparis.lSiparisNo = tbSiparisAciklamasi.lSiparisNo 

                LEFT OUTER JOIN (
                    SELECT 
                        TARIH AS dteSiparisTarihi, 
                        IZAHAT AS sFisTipi, 
                        FISNO AS lFisNo, 
                        FIRMANO,
                        STOKNO,
                        SUM(MIKTAR) AS lYukleme  
                    FROM ASTOKPAKETDETAY 
                    GROUP BY TARIH, IZAHAT, FISNO, FIRMANO, STOKNO
                ) tbYukleme 
                    ON tbSiparis.nFirmaID = tbYukleme.FIRMANO 
                    AND tbSiparis.dteSiparisTarihi = tbYukleme.dteSiparisTarihi 
                    AND tbYukleme.lFisNo = tbSiparis.lSiparisNo 
                    AND tbYukleme.sFisTipi = 'AS' 
                    AND tbYukleme.STOKNO = tbSiparis.nStokID 

                WHERE 
                    tbSiparis.lSiparisNo = @lSiparisNo
                    AND tbSiparis.sSaticiRumuzu = @sSaticiRumuzu
                    AND tbSiparis.nSiparisTipi = 1

                GROUP BY 
                    tbSiparis.nSiparisID, 
                    tbStok.sKodu, 
                    tbSiparis.sBirimCinsi, 
                    tbSiparis.nIskontoYuzdesi, 
                    tbSiparis.dteSiparisTarihi, 
                    tbSiparis.lSiparisNo, 
                    tbFirma.nFirmaID, 
                    tbFirma.sKodu, 
                    tbFirma.sAciklama, 
                    tbSiparisAciklamasi.sAciklama1, 
                    tbSiparisAciklamasi.sAciklama2, 
                    tbSiparisAciklamasi.sAciklama3, 
                    tbSiparisAciklamasi.sAciklama4, 
                    tbSiparisAciklamasi.sAciklama5, 
                    tbSiparisAciklamasi.bKilitli, 
                    tbSiparis.sSiparisiVeren

                ORDER BY 
                    tbSiparis.dteSiparisTarihi, 
                    tbSiparis.lSiparisNo, 
                    tbFirma.sKodu";

                    var stocks = await connection.QueryAsync<TbSiparis>(query, new { lSiparisNo, sSaticiRumuzu });
                    return Ok(stocks);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("GetOrderBp")]
        public async Task<IActionResult> GetOrderBp(string variable, string sSaticiRumuzu, string sDepo)
        {
            try
            {
                string today = DateTime.Now.ToString("dd/MM/yyyy");
                using (var connection = new SqlConnection(_connectionString))
                {

        //            string query = $@"SELECT top (50) * FROM tbFirma 
        //                        WHERE
        //                        ((tbFirma.sKodu like '%{variable}%' or '{variable}'='') OR (tbFirma.sAciklama like '%{variable}%' or '{variable}'='')) AND
								//tbFirma.bAktif = 1 and 
								//tbFirma.bSipariseKapali = 0  And 
								//(tbFirma.sKodu  like '120%') And (tbFirma.sSaticiRumuzu  = @sSaticiRumuzu)  ORDER BY 
								//tbFirma.sKodu";
                string    query = $@"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET DATEFORMAT DMY
SELECT TOP (50)
    f.nFirmaID,
    f.sKodu,
    f.sAciklama,
    f.sAdres1,
    f.sAdres2,
    f.sSemt,
	f.sIl,
	f.sUlke,
	f.sPostaKodu,
	f.sVergiDairesi,
	f.sVergiNo,
    f.bAktif,
    f.bSipariseKapali,
    f.sSaticiRumuzu,
    f.lKrediLimiti,
    fa.sAciklama1 + ' ' + fa.sAciklama2 + ' ' + fa.sAciklama3 + ' ' + fa.sAciklama4 + ' ' + fa.sAciklama5 AS ISTIHBARAT,
    
    ISNULL((
        SELECT SUM(lBorcTutar - lAlacakTutar)
        FROM tbFirmaHareketi 
        WHERE nFirmaID = f.nFirmaID AND dteIslemTarihi <= '{today}'
    ), 0) AS Bakiye,

    ISNULL((
        SELECT SUM(lTutar)
        FROM tbCekSenet
        WHERE sCekSenetTipi = 'as'
          AND nSonCekSenetIslem IN (1, 3, 4, 5, 6)
          AND nVerenID = f.nFirmaID
    ), 0) AS SenetRisk,

    ISNULL((
        SELECT SUM(lTutar)
        FROM tbCekSenet
        WHERE sCekSenetTipi = 'ac'
          AND nSonCekSenetIslem IN (1, 3, 4, 5, 6)
          AND nVerenID = f.nFirmaID
    ), 0) AS CekRisk,

    ISNULL((
        SELECT SUM(lBorcTutar - lAlacakTutar)
        FROM tbFirmaHareketi 
        WHERE nFirmaID = f.nFirmaID AND dteIslemTarihi <= '{today}'
    ), 0)
    + ISNULL((
        SELECT SUM(lTutar)
        FROM tbCekSenet
        WHERE sCekSenetTipi = 'as'
          AND nSonCekSenetIslem IN (1, 3, 4, 5, 6)
          AND nVerenID = f.nFirmaID
    ), 0)
    + ISNULL((
        SELECT SUM(lTutar)
        FROM tbCekSenet
        WHERE sCekSenetTipi = 'ac'
          AND nSonCekSenetIslem IN (1, 3, 4, 5, 6)
          AND nVerenID = f.nFirmaID
    ), 0) AS ToplamRisk,

    f.lKrediLimiti - ABS(
        ISNULL((
            SELECT SUM(lBorcTutar - lAlacakTutar)
            FROM tbFirmaHareketi 
            WHERE nFirmaID = f.nFirmaID AND dteIslemTarihi <= '{today}'
        ), 0)
    ) AS lKalanKredi

FROM tbFirma f
INNER JOIN tbFirmaAciklamasi fa ON f.nFirmaID = fa.nFirmaID
INNER JOIN tbFirmaSinifi fs ON f.nFirmaID = fs.nFirmaID
WHERE
    (
        (f.sKodu LIKE '%' + @variable + '%' OR @variable = '')
        OR (f.sAciklama LIKE '%' + @variable + '%' OR @variable = '')
    )
    AND f.bAktif = 1
    AND f.bSipariseKapali = 0
    AND f.sKodu LIKE '120%'
    AND f.sSaticiRumuzu = @sSaticiRumuzu
    AND f.sDepo IN ('', '{sDepo}')
ORDER BY f.sKodu;
";
                    var result = await connection.QueryAsync<TbFirma>(query, new { variable = variable ?? "", sSaticiRumuzu = sSaticiRumuzu ?? "" });
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("GetOrderStocks")]
        public async Task<IActionResult> GetOrderStocks(string variable = "", string sFiyatTipi = "1", string sDepo = "")
        {
            try
            {
                string today = DateTime.Now.ToString("dd/MM/yyyy");
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"SET DATEFORMAT dmy;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT TOP 50
    CAST(tbStok.nStokID AS VARCHAR(20)) AS nStokID,
    tbStok.sKodu,
    tbStok.sAciklama,
    tbStok.sKisaAdi,
    tbStok.nStokTipi,
    tbStok.sBedenTipi,
    tbStok.sKavalaTipi,
    tbStok.sRenk,
    tbStok.sBeden,
    tbStok.sKavala,
    tbStok.sBirimCinsi1,
    tbStok.sBirimCinsi2,
    tbStok.nIskontoYuzdesi,
    tbStok.sKdvTipi,
    tbStok.nTeminSuresi,
    tbStok.lAsgariMiktar,
    tbStok.lAzamiMiktar,
    tbStok.sOzelNot,
    tbStok.nFiyatlandirma,
    tbStok.sModel,
    tbStok.sKullaniciAdi,
    tbStok.dteKayitTarihi,
    tbStok.bEksiyeDusulebilirmi,
    tbStok.sDefaultAsortiTipi,
    tbStok.bEksideUyarsinmi,
    tbStok.bOTVVar,
    tbStok.sOTVTipi,
    tbStok.nIskontoYuzdesiAV,
    tbStok.bEk1,
    tbStok.nEk2,
    tbStok.nPrim,
    tbStok.nEn,
    tbStok.nBoy,
    tbStok.nYukseklik,
    tbStok.nHacim,
    tbStok.nAgirlik,
    tbStok.sDovizCinsi,
    tbStok.sAlisKdvTipi,
    tbStok.nWebIskontoYuzdesi,
    tbStok.bWebGoruntule,
    tbStok.bWebTavsiye,
    tbStok.nOIV,
    tbStok.bOIVVar,
    tbStok.dteGuncelZaman,
    ISNULL(BirimCevrim.nBirimCarpan, 1) AS nBirimCarpan,
    CAST(ISNULL(StokMiktar.lMevcut, 0) / ISNULL(BirimCevrim.nBirimCarpan, 1) AS MONEY) AS lMevcut2,
    Barkod.sBarkod,
    Barkod.nMuadilBarkod,
    ISNULL(StokMiktar.lMevcut, 0) AS lMevcut,
    ISNULL(Fiyat.lFiyat1, 0) AS lFiyat1,
    Renk.sRenkAdi,
    FiyatDegisti.bFiyatDegisti,
    CASE WHEN CONVERT(CHAR(10), tbStok.dteKayitTarihi, 103) = @today THEN 1 ELSE 0 END AS bYeni,
    tbDepoDetayi.sDepo1,
    ResimKolonlari.pResim1,
    ResimKolonlari.pResim2,
    ResimKolonlari.pResim3,
    ResimKolonlari.pResim4,
    ResimKolonlari.pResim5,
    ResimKolonlari.pResim6,
    ResimKolonlari.yol1,
    ResimKolonlari.yol2,
    ResimKolonlari.yol3,
    ResimKolonlari.yol4,
    ResimKolonlari.yol5,
    ResimKolonlari.yol6
FROM tbStok
OUTER APPLY (
    SELECT
        MAX(CASE WHEN rn = 1 THEN CONVERT(NVARCHAR(MAX), pResim) END) AS pResim1,
        MAX(CASE WHEN rn = 1 THEN yol END) AS yol1,
        MAX(CASE WHEN rn = 2 THEN CONVERT(NVARCHAR(MAX), pResim) END) AS pResim2,
        MAX(CASE WHEN rn = 2 THEN yol END) AS yol2,
        MAX(CASE WHEN rn = 3 THEN CONVERT(NVARCHAR(MAX), pResim) END) AS pResim3,
        MAX(CASE WHEN rn = 3 THEN yol END) AS yol3,
        MAX(CASE WHEN rn = 4 THEN CONVERT(NVARCHAR(MAX), pResim) END) AS pResim4,
        MAX(CASE WHEN rn = 4 THEN yol END) AS yol4,
        MAX(CASE WHEN rn = 5 THEN CONVERT(NVARCHAR(MAX), pResim) END) AS pResim5,
        MAX(CASE WHEN rn = 5 THEN yol END) AS yol5,
        MAX(CASE WHEN rn = 6 THEN CONVERT(NVARCHAR(MAX), pResim) END) AS pResim6,
        MAX(CASE WHEN rn = 6 THEN yol END) AS yol6
    FROM (
        SELECT pResim, yol, ROW_NUMBER() OVER (ORDER BY nSira) AS rn
        FROM tbStokResmi
        WHERE sModel = tbStok.sModel
    ) AS ResimAlt
) AS ResimKolonlari
OUTER APPLY (
    SELECT
        MAX(CASE WHEN nKisim = 0 THEN sBarkod END) AS sBarkod,
        MAX(CASE WHEN nKisim = 1 THEN sBarkod END) AS nMuadilBarkod
    FROM tbStokBarkodu
    WHERE nStokID = tbStok.nStokID
) Barkod
OUTER APPLY (
    SELECT SUM(lGirisMiktar1 - lCikisMiktar1) AS lMevcut
    FROM tbStokFisiDetayi
    WHERE nStokID = tbStok.nStokID
) StokMiktar
OUTER APPLY (
    SELECT ISNULL(lOran, 1) AS nBirimCarpan
    FROM tbStokBirimCevrimi
    WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2
) BirimCevrim
OUTER APPLY (
    SELECT lFiyat AS lFiyat1
    FROM tbStokFiyati
    WHERE nStokID = tbStok.nStokID AND sFiyatTipi = @sFiyatTipi
) Fiyat
OUTER APPLY (
    SELECT sRenkAdi
    FROM tbRenk
    WHERE sRenk = tbStok.sRenk
) Renk
OUTER APPLY (
    SELECT COUNT(DISTINCT nStokID) AS bFiyatDegisti
    FROM tbStokFiyati
    WHERE dteFiyatTespitTarihi = @today AND sFiyatTipi = @sFiyatTipi AND nStokID = tbStok.nStokID
) FiyatDegisti
LEFT JOIN (
    SELECT nStokID, SUM(CASE WHEN RTRIM(UPPER(sDepo)) = @sDepo THEN lGirisMiktar1 - lCikisMiktar1 ELSE 0 END) AS sDepo1
    FROM tbStokFisiDetayi
    GROUP BY nStokID
) tbDepoDetayi ON tbStok.nStokID = tbDepoDetayi.nStokID
WHERE
    tbStok.sKodu like 'TS%'
    AND tbStok.nStokTipi < 5
    AND (
        tbStok.sKodu LIKE 'TS%' + @variable + '%'
        OR tbStok.sAciklama LIKE '%' + @variable + '%'
        OR tbStok.nStokID IN (
            SELECT nStokID FROM tbStokBarkodu WHERE sBarkod LIKE '%' + @variable + '%'
        )
        OR @variable = ''
    )
ORDER BY tbStok.sKodu";
                    var stocks = await connection.QueryAsync<TbStok>(query, new { variable, sFiyatTipi, sDepo, today });
                    return Ok(stocks);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("GetStock")]
        public async Task<IActionResult> GetStock(int currentPage, int pageSize, string variable, string sFiyatTipi, string sDepo)
        {
            try
            {
                string today = DateTime.Now.ToString("dd/MM/yyyy");
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@" SET DATEFORMAT DMY
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT 
    CAST(tbStok.nStokID AS nvarchar) AS nStokID,
    tbStok.sKodu,
    tbStok.sAciklama,
    tbStok.sKisaAdi,
    tbStok.nStokTipi,
    tbStok.sBedenTipi,
    tbStok.sKavalaTipi,
    tbStok.sRenk,
    tbStok.sBeden,
    tbStok.sKavala,
    tbStok.sBirimCinsi1,
    tbStok.sBirimCinsi2,
    tbStok.nIskontoYuzdesi,
    tbStok.sKdvTipi,
    tbStok.nTeminSuresi,
    tbStok.lAsgariMiktar,
    tbStok.lAzamiMiktar,
    tbStok.sOzelNot,
    tbStok.nFiyatlandirma,
    tbStok.sModel,
    tbStok.sKullaniciAdi,
    tbStok.dteKayitTarihi,
    tbStok.bEksiyeDusulebilirmi,
    tbStok.sDefaultAsortiTipi,
    tbStok.bEksideUyarsinmi,
    tbStok.bOTVVar,
    tbStok.sOTVTipi,
    tbStok.nIskontoYuzdesiAV,
    tbStok.bEk1,
    tbStok.nEk2,
    tbStok.nPrim,
    tbStok.nEn,
    tbStok.nBoy,
    tbStok.nYukseklik,
    tbStok.nHacim,
    tbStok.nAgirlik,
    tbStok.sDovizCinsi,
    tbStok.sAlisKdvTipi,
    tbStok.nWebIskontoYuzdesi,
    tbStok.bWebGoruntule,
    tbStok.bWebTavsiye,
    tbStok.nOIV,
    tbStok.bOIVVar,
    tbStok.dteGuncelZaman,

    ISNULL(BirimCevrim.nBirimCarpan, 1) AS nBirimCarpan,
    CAST(ISNULL(StokMiktar.lMevcut, 0) / ISNULL(BirimCevrim.nBirimCarpan, 1) AS MONEY) AS lMevcut2,

    Barkod.sBarkod,
    Barkod.nMuadilBarkod,
    ISNULL(StokMiktar.lMevcut, 0) AS lMevcut,

    ISNULL(Fiyat.lFiyat1, 0) AS lFiyat1,
        Renk.sRenkAdi,
    FiyatDegisti.bFiyatDegisti,

    bYeni = CASE WHEN CONVERT(CHAR(10), tbStok.dteKayitTarihi, 103) = '{today}' THEN 1 ELSE 0 END,
    tbDepoDetayi.sDepo1,
 ResimKolonlari.pResim1,
 ResimKolonlari.pResim2,
 ResimKolonlari.pResim3,
 ResimKolonlari.pResim4,
 ResimKolonlari.pResim5,
 ResimKolonlari.pResim6

FROM tbStok
OUTER APPLY (
    SELECT 
        MAX(CASE WHEN rn = 1 THEN pResim END) AS pResim1,
        MAX(CASE WHEN rn = 2 THEN pResim END) AS pResim2,
        MAX(CASE WHEN rn = 3 THEN pResim END) AS pResim3,
        MAX(CASE WHEN rn = 4 THEN pResim END) AS pResim4,
        MAX(CASE WHEN rn = 5 THEN pResim END) AS pResim5,
        MAX(CASE WHEN rn = 6 THEN pResim END) AS pResim6
    FROM (
        SELECT 
            pResim,
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM tbStokResmi
        WHERE sModel = tbStok.sModel
    ) AS ResimAlt
) AS ResimKolonlari
-- Barkodları tekilleştir
OUTER APPLY (
    SELECT 
        MAX(CASE WHEN nKisim = 0 THEN sBarkod END) AS sBarkod,
        MAX(CASE WHEN nKisim = 1 THEN sBarkod END) AS nMuadilBarkod
    FROM tbStokBarkodu
    WHERE nStokID = tbStok.nStokID
) Barkod

-- Mevcut miktarı tek satır olarak getir
OUTER APPLY (
    SELECT SUM(lGirisMiktar1 - lCikisMiktar1) AS lMevcut
    FROM tbStokFisiDetayi
    WHERE nStokID = tbStok.nStokID
) StokMiktar

-- Birim çevrim
OUTER APPLY (
    SELECT ISNULL(lOran, 1) AS nBirimCarpan
    FROM tbStokBirimCevrimi
    WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2
) BirimCevrim

-- Fiyat
OUTER APPLY (
    SELECT lFiyat AS lFiyat1
    FROM tbStokFiyati
    WHERE nStokID = tbStok.nStokID AND sFiyatTipi = '{sFiyatTipi}'
) Fiyat

-- KDV oranı
OUTER APPLY (
    SELECT nKdvOrani
    FROM tbKdv
    WHERE sKdvTipi = tbStok.sKdvTipi
) Kdv

-- Renk adı
OUTER APPLY (
    SELECT sRenkAdi
    FROM tbRenk
    WHERE sRenk = tbStok.sRenk
) Renk

-- Fiyat değişti mi?
OUTER APPLY (
    SELECT COUNT(DISTINCT nStokID) AS bFiyatDegisti
    FROM tbStokFiyati
    WHERE dteFiyatTespitTarihi BETWEEN '{today}' and '{today}'
      AND sFiyatTipi IN ('{sFiyatTipi}') AND nStokID = tbStok.nStokID
) FiyatDegisti

-- Depo detayı
LEFT JOIN (
    SELECT nStokID, SUM(CASE WHEN RTRIM(UPPER(sDepo)) = '{sDepo}' THEN lGirisMiktar1 - lCikisMiktar1 ELSE 0 END) AS sDepo1
    FROM tbStokFisiDetayi
    GROUP BY nStokID
) tbDepoDetayi ON tbStok.nStokID = tbDepoDetayi.nStokID

 WHERE
   -- tbStok.sKodu LIKE 'TS%' AND
    tbStok.nStokTipi < 5 AND
    tbStok.sKodu='{variable}'
Order By tbStok.sKodu
";





                    var result = await connection.QueryAsync(query, new { variable, sFiyatTipi, sDepo, today });
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        public class SiparisModel
        {
            public int nFirmaId { get; set; }
            public string sFiyatTipi { get; set; }
            public string sSaticiRumuzu { get; set; }
            public string PERSONELADI { get; set; }
            public string sDepo { get; set; }
            public List<UrunSecimi> yeniSiparis { get; set; }
            public TbFirmaAdresi? adres { get; set; }
            public TbSiparisAciklamasi? siparisAciklamasi { get; set; }

        }

        [HttpPost("AddNewOrder")]
        public async Task<IActionResult> AddNewOrder([FromBody] SiparisModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.sSaticiRumuzu) || string.IsNullOrEmpty(model.sDepo) || string.IsNullOrEmpty(model.PERSONELADI))
                {
                    return BadRequest(new { message = "Satıcı rumuzu veya depo bilgisi eksik." });
                }

                string siparisNo = await LastOrderId();
               // TbFirma firmaInfo = await GetTbFirmaInfo(model.nFirmaId);
                int iSiprisNo = Convert.ToInt32(siparisNo) + 1;
                using (var connection = new SqlConnection(_connectionString))
                  
                {
                    foreach (var siparis in model.yeniSiparis)
                    {
                        var birimCevirimi = await StokBirimCevir(siparis);
                        double cevrim = birimCevirimi.birimCevirimi;
                        double lOran = birimCevirimi.lOran;
                        string sBirimCinsi = birimCevirimi.sBirimCinsi;
                        string dteSiparisTarihi = DateTime.Now.ToString("yyyy-MM-dd");
                        double iskontosuzTutar = siparis.Urun.lFiyat1 * siparis.Miktar * lOran;
                        double iskontoYuzdesi = siparis.Urun.nIskontoYuzdesi;
                        double tutar = (iskontoYuzdesi == 0) ? iskontosuzTutar : iskontosuzTutar - iskontosuzTutar * iskontoYuzdesi / 100;

                        string query = @"
                            INSERT INTO tbSiparis 
    (nSiparisTipi, lSiparisNo, nFirmaID, nStokID, dteSiparisTarihi, dteTeslimTarihi, sSiparisIslem, nReceteNo, 
     sSiparisiAlan, sSiparisiVeren, sFiyatTipi, lMiktari, lFiyati, lTutari, sAsortiTipi, sAsortiMiktari, 
     nKdvOrani, nIskontoYuzdesi, lIskontosuzTutari, sKullaniciAdi, dteKayitTarihi, bKapandimi, 
     lReserveMiktari, sHangiUygulama, nPartiID, sSatirAciklama, nValorGun, sDovizCinsi, lDovizFiyati, 
     lDovizKuru, sSaticiRumuzu, sDepo, sBirimCinsi, lBirimMiktar, sAmbalaj,bOnay)
VALUES 
    (1, @lSiparisNo, @nFirmaID, @nStokID, @dteSiparisTarihi, @dteSiparisTarihi, '', 0, @PERSONELADI, 
     @sSiparisiVeren, @sFiyatTipi, @lMiktari, @lFiyati, @lTutari, '', 0, @nKdvOrani, 
     @nIskontoYuzdesi, @lIskontosuzTutari, @PERSONELADI, @dteSiparisTarihi, 0, 0, '', 0, 
     'Isk1:' + CAST(CAST(@nIskontoYuzdesi AS DECIMAL(5,2)) AS VARCHAR) + ' Isk2:00.00 Isk3:00.00 Isk4:00.00', 
     0, '', 0, 0, @sSaticiRumuzu, @sDepo, @sBirimCinsi2, @lOran, @sBirimCinsi1,0)
     UPDATE tbParamAlSiparis SET lSonSiparisNo = @lSiparisNo 

";

                        var parameters = new
                        {
                            lSiparisNo = iSiprisNo,
                            nFirmaID = model.nFirmaId,
                            sSaticiRumuzu = model.sSaticiRumuzu,
                            sFiyatTipi = model.sFiyatTipi,
                            sDepo = model.sDepo,
                            lOran = lOran,
                            sBirimCinsi1 = sBirimCinsi,
                            sBirimCinsi2 = siparis.SelectedBirimCinsi,
                            nKdvOrani = siparis.Urun.nKdvOrani,
                            nIskontoYuzdesi = iskontoYuzdesi,
                            lIskontosuzTutari = iskontosuzTutar,
                            nStokID = siparis.Urun.nStokID,
                            dteSiparisTarihi = dteSiparisTarihi,
                            sSiparisiVeren = siparis.Urun.sAciklama,
                            lMiktari = cevrim,
                            lFiyati = siparis.Urun.lFiyat1,
                            lTutari = tutar,
                            PERSONELADI = model.PERSONELADI
                        };

                        await connection.ExecuteAsync(query, parameters);
                    }

                    await AddtbSiparisPesinAdres(iSiprisNo, model.nFirmaId,  model.adres);
                    await AddtbSiparisAciklamasi(iSiprisNo, model.nFirmaId,  model.siparisAciklamasi);
                    return Ok(iSiprisNo);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<TbStokBirimCinsi> StokBirimCevir(UrunSecimi siparis)
        {
            try
            {
                string secilenBirim = siparis.SelectedBirimCinsi;
                double miktar = siparis.Miktar;
                string nStokID = siparis.Urun.nStokID;
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"SELECT 
    bc.nBirimCarpan as 'lOran',
    tbStok.sBirimCinsi2 as 'sBirimCinsi',
    bc.nBirimCarpan * {miktar} AS 'birimCevirimi'
FROM tbStok
CROSS APPLY (
    SELECT ISNULL((
        SELECT ISNULL(lOran, 0)
        FROM tbStokBirimCevrimi
        INNER JOIN tbStok AS t2 ON tbStokBirimCevrimi.nStokID = t2.nStokID 
            AND tbStokBirimCevrimi.sBirimCinsi = t2.sBirimCinsi2
        WHERE t2.nStokID = tbStok.nStokID
          AND tbStokBirimCevrimi.sBirimCinsi = '{secilenBirim}'
    ), 1) AS nBirimCarpan
) AS bc
WHERE tbStok.nStokID = '{nStokID}'";

                    var result = await connection.QueryFirstOrDefaultAsync<TbStokBirimCinsi>(query, new { nStokID, secilenBirim, miktar });
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return null;
            }
        }

        public async Task<IActionResult> AddtbSiparisPesinAdres(int siparisNo, int nFirmaID,  TbFirmaAdresi? adres)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SET DATEFORMAT DMY
                        SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
                        INSERT INTO tbSiparisPesinAdres 
                            (nSiparisTipi, lSiparisNo, sAciklama, sAdres1, sAdres2, sSemt, sIl, sUlke, sPostaKodu, sVergiDairesi, sVergiNo, sSubeMagaza)
                        VALUES 
                            (1, @siparisNo, @sAciklama, @sAdres1, @sAdres2, @sSemt, @sIl, @sUlke, @sPostaKodu, @sVergiDairesi, @sVergiNo, '')";

                    await connection.ExecuteAsync(query, new
                    {
                        siparisNo,
                        sAciklama = (String.IsNullOrEmpty(adres.sAciklama)) ? "" : adres.sAciklama,
                        sAdres1 = (String.IsNullOrEmpty(adres.sAdres1)) ? "" : adres.sAdres1,
                        sAdres2 = (String.IsNullOrEmpty(adres.sAdres2)) ? "" : adres.sAdres2,
                        sSemt = (String.IsNullOrEmpty(adres.sSemt)) ? "" : adres.sSemt,
                        sIl = (String.IsNullOrEmpty(adres.sIl)) ? "" : adres.sIl,
                        sUlke = (String.IsNullOrEmpty(adres.sUlke)) ? "" : adres.sUlke,
                        sPostaKodu = (String.IsNullOrEmpty(adres.sPostaKodu)) ? "" : adres.sPostaKodu,
                        sVergiDairesi = (String.IsNullOrEmpty(adres.sVergiDairesi)) ? "" : adres.sVergiDairesi,
                        sVergiNo = (String.IsNullOrEmpty(adres.sVergiNo)) ? "" : adres.sVergiNo
                    });

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        public async Task<IActionResult> AddtbSiparisAciklamasi(int siparisNo, int nFirmaID,  TbSiparisAciklamasi? siparisAciklamasi)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SET DATEFORMAT DMY
                        SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
                        INSERT INTO tbSiparisAciklamasi 
                            (nSiparisTipi, lSiparisNo,nFirmaID, sAciklama1,sAciklama2,sAciklama3,sAciklama4,sAciklama5)
                        VALUES 
                            (1, @siparisNo, @nFirmaID,@sAciklama1,@sAciklama2,@sAciklama3,@sAciklama4,@sAciklama5 )";

                    await connection.ExecuteAsync(query, new
                    {
                        siparisNo,
                        nFirmaID=nFirmaID,
                        sAciklama1 = (String.IsNullOrEmpty(siparisAciklamasi.sAciklama1)) ? "" : siparisAciklamasi.sAciklama1,
                        sAciklama2 = (String.IsNullOrEmpty(siparisAciklamasi.sAciklama2)) ? "" : siparisAciklamasi.sAciklama2,
                        sAciklama3 = (String.IsNullOrEmpty(siparisAciklamasi.sAciklama3)) ? "" : siparisAciklamasi.sAciklama3,
                        sAciklama4 = (String.IsNullOrEmpty(siparisAciklamasi.sAciklama4)) ? "" : siparisAciklamasi.sAciklama4,
                        sAciklama5 = (String.IsNullOrEmpty(siparisAciklamasi.sAciklama5)) ? "" : siparisAciklamasi.sAciklama5,
                      
                    });

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(int lMiktar, int nSiparisID, double nIskontoYuzdesi)
        {
            try
            {
                if (nIskontoYuzdesi == 0)
                    nIskontoYuzdesi = 1;

                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
        SET DATEFORMAT DMY
        SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

        UPDATE s
        SET 
            -- lMiktari: birim çevrimi (örn. Adet → Koli için bölme)
            s.lMiktari =
                CAST(@lMiktar AS decimal(18,6)) *
                CAST(CASE WHEN ISNULL(cv.lOran,0)=0 THEN 1.0 ELSE cv.lOran END AS decimal(18,6)),

            -- Tutar hesapları (mevcut mantık korunuyor)
            s.lTutari = (@lMiktar * s.lFiyati) - (@lMiktar * s.lFiyati) * @nIskontoYuzdesi / 100.0,
            s.lIskontosuzTutari = (@lMiktar * s.lFiyati)
        FROM tbSiparis s
        LEFT JOIN tbStok st
            ON st.nStokID = s.nStokID
        OUTER APPLY (
            SELECT TOP 1 sbc.lOran
            FROM tbStokBirimCevrimi sbc
            WHERE sbc.nStokID = st.nStokID
            -- Eğer sütun varsa, sipariş birimine öncelik vermek için açın:
            -- AND sbc.sBirim = s.sBirimCinsi
            ORDER BY sbc.lOran DESC
        ) cv
        WHERE s.nSiparisID = @nSiparisID";

                    await connection.ExecuteAsync(query, new { lMiktar, nSiparisID, nIskontoYuzdesi });
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpPost("UpdateDiscount")]
        public async Task<IActionResult> UpdateDiscount(int lMiktar, int nSiparisID, double nIskontoYuzdesi)
        {
            try
            {
                double tempIskonto = (nIskontoYuzdesi == 0) ? 1 : nIskontoYuzdesi;
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                SET DATEFORMAT DMY
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
                UPDATE tbSiparis
                SET nIskontoYuzdesi = @nIskontoYuzdesi,
                    lTutari = @lMiktar * lFiyati - @lMiktar * lFiyati * @tempIskonto / 100,
                    lIskontosuzTutari = @lMiktar * lFiyati
                WHERE nSiparisID = @nSiparisID;
                SELECT * FROM tbSiparis WHERE nSiparisID = @nSiparisID";
                    var siparis = await connection.QueryFirstOrDefaultAsync<TbSiparis>(query, new { lMiktar, nSiparisID, nIskontoYuzdesi, tempIskonto });
                    if (siparis == null) return BadRequest(new { message = "Sipariş bulunamadı." });
                    return Ok(siparis);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpPost("DeleteOrderRow")]
        public async Task<IActionResult> DeleteOrderRow( int nSiparisID)
        {
            try
            {
              
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
SET DATEFORMAT  DMY
Delete from tbSiparis WHERE nSiparisID = @nSiparisID";

                    await connection.ExecuteAsync(query, new {  nSiparisID });
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder( string lSiparisNo)
        {
            try
            {
              
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"

SET DATEFORMAT  DMY
DELETE tbStokFisiDetayi
FROM tbStokFisiDetayi
INNER JOIN tbSiparis ON tbSiparis.nSiparisID = tbStokFisiDetayi.nSiparisID
WHERE tbSiparis.lSiparisNo = @lSiparisNo;

DELETE FROM tbSiparis
WHERE lSiparisNo =  @lSiparisNo;";

                    await connection.ExecuteAsync(query, new {  lSiparisNo });
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<string> LastOrderId()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT lSonSiparisNo FROM tbParamAlSiparis ";
                    var lastId = await connection.QueryFirstOrDefaultAsync<string>(query);
                    return lastId ?? "0";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return "Bir hata oluştu: " + ex.Message;
            }
        }

        [HttpGet]
        public async Task<TbFirma> GetTbFirmaInfo(int nFirmaID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT sAciklama, sAdres1, sAdres2, sSemt, sIl, sUlke, sPostaKodu, sVergiDairesi, sVergiNo 
                        FROM tbFirma 
                        WHERE nFirmaID = @nFirmaID";

                    return await connection.QueryFirstOrDefaultAsync<TbFirma>(query, new { nFirmaID });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return null;
            }
        }
    }
}