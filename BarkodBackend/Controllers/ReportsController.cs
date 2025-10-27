using BarkodBackend.Context;
using BarkodBackend.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BarkodBackend.Controllers
{
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public ReportsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DB")!;
        }
        [HttpGet("PayableCheque")]
        public async Task<IActionResult> GetPayableCheque(string startDate, string endDate)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"set dateformat dmy SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT tbCekSenet.nCekSenetID, tbCekSenet.lCekSenetNo, tbCekSenet.sCekSenetTipi,tbCekSenet.lTutar, DATEDIFF(day, '09/03/2022', tbCekSenet.dteVadeTarihi) AS 
nGecikme,DATEDIFF(day, tbCekSenetBordro.dteBordroTarihi, tbCekSenet.dteVadeTarihi) AS nOpsiyon,tbCekSenet.dteVadeTarihi, tbCekSenet.dteDuzenlemeTarihi,tbCekSenet.sBankaKodu, tbCekSenet.sBankaHesapNo,tbCekSenet.sIl,
tbCekSenet.sSemt, tbCekSenet.sBorclusu, tbCekSenet.sBorcluVergiDairesi, tbCekSenet.sBorcluVergiNumarasi, tbCekSenet.sNot, tbCekSenet.nVerenID,tbCekSenet.sCekSenetKodu1, tbCekSenet.sCekSenetKodu2,tbBanka.sAciklama AS sBankaAciklama,
tbCekSenet.sOrjinalCekSenetNo, VEREN.sKodu AS sVerenFirmaKodu, VEREN.sAciklama AS sVerenFirmaAciklama, tbCekSenet.nSonCekSenetIslem, tbCekSenetIslem.sAciklama AS sIslem, tbFirma_1.sKodu AS sAlanFirmaKodu, tbFirma_1.sAciklama AS sAlanFirmaAciklama,
tbCekSenet.sHareketTipi, tbCekSenet.sDovizCinsi1, tbCekSenet.lDovizMiktari1, tbCekSenet.lDovizKuru1,tbCekSenet.sDovizCinsi2, tbCekSenet.lDovizMiktari2, tbCekSenet.lDovizKuru2,tbCekSenetBordro.nBordroSatirID, tbCekSenetBordro.dteBordroTarihi,
(SELECT SUM(lProtestoMasrafi) FROM tbCekSenetBordro WHERE nCekSenetID = tbCekSenet.nCekSenetID) AS lTahsilat, tbCekSenet.lTutar - (SELECT SUM(lProtestoMasrafi) FROM tbCekSenetBordro WHERE nCekSenetID = tbCekSenet.nCekSenetID) AS lKalan,
tbCekSenet.sKullaniciAdi,tbCekSenet.dteKayitTarihi FROM tbFirma VEREN INNER JOIN tbCekSenet ON VEREN.nFirmaID = tbCekSenet.nVerenID INNER JOIN tbBanka ON tbCekSenet.sBankaKodu = tbBanka.sBankaKodu INNER JOIN tbHareketTipi
ON tbCekSenet.sHareketTipi = tbHareketTipi.sHareketTipi INNER JOIN tbFirma tbFirma_1 INNER JOIN tbCekSenetBordro ON tbFirma_1.nFirmaID = tbCekSenetBordro.nFirmaID ON tbCekSenet.nSonBordroSatirID = tbCekSenetBordro.nBordroSatirID INNER JOIN
tbCekSenetIslem ON tbCekSenet.nSonCekSenetIslem = tbCekSenetIslem.nCekSenetIslem WHERE (tbCekSenet.sCekSenetTipi = 'BC') AND
tbCekSenet.dteVadeTarihi between '{startDate}' and '{endDate}' and 
tbCekSenet.lCekSenetNo between '1' and '999999999' and 
tbCekSenet.sOrjinalCekSenetNo between '' and 'zzzzzzzzz' and tbCekSenetBordro.nCekSenetIslem IN ('20',6)  ORDER BY tbCekSenet.dteVadeTarihi";

                    var result = await connection.QueryAsync<TbPayableCheque>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("SalesAnalysis")]
        public async Task<IActionResult> GetSalesAnalysis(string startDate, string endDate)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"set dateformat dmy SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT distinct TOP 100  * FROM (SELECT CAST(tbStokFisiDetayi.nStokFisiID AS NVARCHAR(20)) AS nAlisVerisID, tbStokFisiDetayi.nFirmaID, tbStokFisiDetayi.nMusteriID, 
tbFirma.sKodu as lKodu,tbFirma.sAciklama AS sMusteriAdi, tbStokFisiDetayi.dteFisTarihi AS dteTarih,  tbStokFisiMaster.sFisTipi AS fisTipi, tbStokFisiMaster.lFisNo AS lNo, tbStokFisiDetayi.sKasiyerRumuzu, 
tbStokFisiDetayi.nGirisCikis,(Select sAdi + ' ' +sSoyadi FROM tbSatici where sSaticiRumuzu=tbStokFisiDetayi.sSaticiRumuzu)  as satici,  tbStokFisiDetayi.sDepo AS sMagaza  FROM tbFirma INNER JOIN tbStokFisiMaster INNER JOIN tbStokFisiDetayi ON tbStokFisiMaster.nStokFisiID = tbStokFisiDetayi.nStokFisiID
ON tbFirma.nFirmaID = tbStokFisiDetayi.nFirmaID LEFT OUTER JOIN tbStok ON tbStokFisiDetayi.nStokID = tbStok.nStokID 
WHERE (tbStokFisiDetayi.sFisTipi IN ('FS')) 
AND  tbStokFisiDetayi.dteFisTarihi BETWEEN '{startDate}' AND '{endDate}'
) t order by dteTarih  desc, lNo";

                    var result = await connection.QueryAsync<TbSalesAnalysis>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("SalesAnalysisDetail")]
        public async Task<IActionResult> SalesAnalysisDetail(string startDate, string endDate, string nAlisVerisID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"set dateformat dmy SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT * FROM (SELECT CAST(tbStokFisiDetayi.nStokFisiID AS NVARCHAR(20)) AS nAlisVerisID, tbStokFisiDetayi.nFirmaID, tbStokFisiDetayi.nMusteriID, 
tbFirma.sKodu as lKodu,tbFirma.sAciklama AS sMusteriAdi, tbStokFisiDetayi.dteFisTarihi AS dteTarih, tbStokFisiMaster.dteKayitTarihi, tbStokFisiMaster.sFisTipi AS fisTipi, tbStokFisiMaster.lFisNo AS lNo, tbStok.nStokID, tbStok.sKodu AS sKodu, 
tbStok.sModel, tbStok.sBeden, tbStok.sAciklama AS sStokAciklama, ISNULL(tbStokFisiDetayi.lCikisMiktar1, 0) AS Miktar, ISNULL(tbStokFisiDetayi.lCikisMiktar1, 0) * (SELECT isnull(lfiyat , 0) FROM tbstokfiyati WHERE nStokId = tbstok.NstokId AND sFiyatTipi = 'M') 
AS MALIYET, ISNULL(tbStokFisiDetayi.lBrutFiyat* (100 + tbStokFisiDetayi.nKdvOrani) / 100, 0) AS Fiyat, ISNULL(tbStokFisiDetayi.lIskontoTutari* (100 + tbStokFisiDetayi.nKdvOrani) / 100, 0) AS Iskonto, tbStokFisiDetayi.lPrimTutari as Prim,
tbStokFisiDetayi.lCikisTutar * (100 + tbStokFisiDetayi.nKdvOrani) / 100 AS lNetTutar, tbStokFisiDetayi.lBrutTutar, tbStokFisiDetayi.sOdemeKodu, tbStokFisiDetayi.sFiyatTipi,tbStokFisiDetayi.sSaticiRumuzu,(Select sAdi + ' ' +sSoyadi FROM tbSatici where sSaticiRumuzu=tbStokFisiDetayi.sSaticiRumuzu)  as satici, 
tbStokFisiDetayi.sKasiyerRumuzu, 
tbStokFisiDetayi.nGirisCikis, tbStokFisiDetayi.sDepo AS sMagaza, tbStok.sRenk, (SELECT 
sRenkAdi FROM tbRenk WHERE sRenk = tbStok.sRenk) AS sRenkAdi, tbStokFisiMaster.sYaziIle, (SELECT TOP 1 sBarkod FROM tbStokBarkodu WHERE nStokID = tbStok.nStokID) AS sBarkod 
FROM tbFirma INNER JOIN tbStokFisiMaster INNER JOIN tbStokFisiDetayi ON tbStokFisiMaster.nStokFisiID = tbStokFisiDetayi.nStokFisiID ON tbFirma.nFirmaID = tbStokFisiDetayi.nFirmaID LEFT OUTER JOIN tbStok ON tbStokFisiDetayi.nStokID = tbStok.nStokID 
WHERE tbStokFisiDetayi.nStokFisiID = '{nAlisVerisID}' and (tbStokFisiDetayi.sFisTipi IN ('FS')) AND  tbStokFisiDetayi.dteFisTarihi BETWEEN '{startDate}' AND '{endDate}') t ";

                    var result = await connection.QueryAsync<SalesAnalysisDetail>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("SalesRemainingReport")]
        public async Task<IActionResult> GetSalesRemainingReport(string startDate, string endDate, string magaza)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Magaza filtresi - sadece boş değilse ekle
                    string magazaFilter1 = string.IsNullOrEmpty(magaza) ? "" : $"And sDepo IN ('{magaza}')";
                    string magazaFilter2 = string.IsNullOrEmpty(magaza) ? "" : $"AND tbAlisVeris.sMagaza IN ('{magaza}')";
                    string magazaFilter3 = string.IsNullOrEmpty(magaza) ? "" : $"AND tbStokFisiDetayi.sDepo IN ('{magaza}')";
                    
                    string query = $@"set dateformat dmy SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT top 100 *, Mevcut - ISNULL(Bekleyen, 0) AS NetMevcut FROM (SELECT nStokID , sKodu , sStokAciklama , sBarkod ,sBeden,sRenkAdi, sRenk ,
SUM(Miktar) AS miktar , sSaticiRumuzu,satici,(SELECT TOP 1 tbStokFisiDetayi.dteFisTarihi FROM tbStokFisiDetayi INNER JOIN tbFisTipi ON tbStokFisiDetayi.sFisTipi = tbFisTipi.sFisTipi WHERE (tbStokFisiDetayi.sFisTipi <> 'T')
AND (tbStokFisiDetayi.sFisTipi = 'FA') AND (tbFisTipi.bSatismi = 0) AND tbStokFisiDetayi.nGirisCikis = 1 AND tbStokFisiDetayi.nStokID = Satislar.nStokID ORDER BY tbStokFisiDetayi.dteFisTarihi DESC) AS SonAlisTarihi ,
(SELECT TOP 1 ISNULL(tbStokFisiDetayi.lGirisMiktar1 , 0) AS lGirisMiktar1 FROM tbStokFisiDetayi INNER JOIN tbFisTipi ON tbStokFisiDetayi.sFisTipi = tbFisTipi.sFisTipi WHERE (tbStokFisiDetayi.sFisTipi <> 'T') 
AND (tbStokFisiDetayi.sFisTipi = 'FA') AND (tbFisTipi.bSatismi = 0) AND tbStokFisiDetayi.nGirisCikis = 1 AND tbStokFisiDetayi.nStokID = Satislar.nStokID ORDER BY tbStokFisiDetayi.dteFisTarihi DESC) AS SonAlisMiktari , (SELECT isnull(SUM(tbStokFisiDetayi.lGirisMiktar1 - tbStokFisiDetayi.lCikisMiktar1) , 0) FROM tbStokFisiDetayi WHERE Satislar.nStokID = tbStokFisiDetayi.nStokID  {magazaFilter1}  ) AS mevcut ,
(SELECT isnull(SUM(tbStokFisiDetayi.lGirisMiktar1 - tbStokFisiDetayi.lCikisMiktar1) , 0) FROM tbStokFisiDetayi WHERE Satislar.nStokID = tbStokFisiDetayi.nStokID ) AS envanter,(SELECT SUM(lKalanMiktar) AS Siparis 
FROM (SELECT tbStok.nStokID , tbStok.sKodu , tbSiparis.lMiktari - SUM(ISNULL(tbStokFisiDetayi.lSevkMiktari , 0)) + SUM(ISNULL(tbStokFisiDetayi.lSevkIadeMiktari , 0)) AS lKalanMiktar FROM (SELECT nSiparisID , 
isnull(abs(SUM(lGirisMiktar1 * (nGirisCikis - 2))) , 0) lSevkMiktari , isnull(abs(SUM(lGirisTutar * (nGirisCikis - 2))) , 0) lSevkTutari , isnull(abs(SUM(lGirisMiktar1 * (nGirisCikis - 1))) , 0) lSevkIadeMiktari , 
isnull(abs(SUM(lGirisTutar * (nGirisCikis - 1))) , 0) lSevkIadeTutari FROM tbStokFisiDetayi , tbStok , tbFirma WHERE tbStok.nStokID = tbStokFisiDetayi.nStokID AND tbFirma.nFirmaID = tbStokFisiDetayi.nFirmaID AND 
dteIslemTarihi BETWEEN '{startDate}' AND '{endDate}' AND 
nSiparisID IS NOT NULL GROUP BY nSiparisID) tbStokFisiDetayi RIGHT OUTER JOIN tbStok INNER JOIN tbSiparis ON tbStok.nStokID = tbSiparis.nStokID ON 
tbStokFisiDetayi.nSiparisID = tbSiparis.nSiparisID WHERE (tbSiparis.bKapandimi = 0) AND (tbSiparis.nSiparisTipi = 2) GROUP BY tbStok.nStokID , tbStok.sKodu , tbStok.sAciklama , tbSiparis.lSiparisNo , 
tbSiparis.nSiparisID , tbSiparis.dteSiparisTarihi , tbSiparis.dteTeslimTarihi , tbSiparis.sSiparisIslem , tbSiparis.lMiktari , tbSiparis.lReserveMiktari , tbSiparis.nValorGun , CAST(tbSiparis.bKapandimi AS int)
HAVING (tbSiparis.lMiktari - SUM(ISNULL(tbStokFisiDetayi.lSevkMiktari , 0)) + SUM(ISNULL(tbStokFisiDetayi.lSevkIadeMiktari , 0)) > 0)) tSiparis WHERE (tSiparis.nStokID=Satislar.nStokID)) as siparis, (SELECT 
ISNULL(SUM(tbAlisverisSiparis.lGCMiktar - tbAlisverisSiparis.bEkAlan1) , 0) AS BEKLEYEN FROM tbAlisverisSiparis INNER JOIN tbAlisVeris ON tbAlisverisSiparis.nAlisverisID = tbAlisVeris.nAlisverisID INNER JOIN tbStok 
stok ON tbAlisverisSiparis.nStokID = stok.nStokID WHERE (tbAlisverisSiparis.nGirisCikis = 3) AND (tbAlisverisSiparis.lGCMiktar <> tbAlisverisSiparis.bEkAlan1) 
AND (tbAlisVeris.sFisTipi = 'SP' OR tbAlisVeris.sFisTipi = 'SK') 
AND (tbAlisverisSiparis.dteTeslimEdilecek BETWEEN '{startDate}' AND '{endDate}') 
AND (stok.nStokID = Satislar.nStokID) 
AND (tbAlisVeris.dteFaturaTarihi BETWEEN '{startDate}' AND '{endDate}')  
{magazaFilter2} GROUP BY stok.sKodu) AS Bekleyen
FROM (SELECT CAST(tbStokFisiDetayi.nStokFisiID AS NVARCHAR(20)) AS nAlisVerisID, tbStokFisiDetayi.nMusteriID, tbFirma.sAciklama AS sMusteriAdi, tbStokFisiDetayi.dteFisTarihi AS dteTarih, tbStokFisiMaster.dteKayitTarihi,
tbStokFisiMaster.sFisTipi AS fisTipi, tbStokFisiMaster.lFisNo AS lNo, tbStok.nStokID, tbStok.sKodu AS sKodu, tbStok.sAciklama AS sStokAciklama, ISNULL(tbStokFisiDetayi.lCikisMiktar1, 0) AS Miktar, 
ISNULL(tbStokFisiDetayi.lCikisMiktar1, 0) * (SELECT isnull(lfiyat , 0) FROM tbstokfiyati WHERE nStokId = tbstok.NstokId AND sFiyatTipi = 'M') AS MALIYET, ISNULL(tbStokFisiDetayi.lBrutFiyat, 0) AS Fiyat, 
ISNULL(tbStokFisiDetayi.lIskontoTutari, 0) AS Iskonto, tbStokFisiDetayi.lCikisTutar AS lNetTutar, tbStokFisiDetayi.sOdemeKodu, tbStokFisiDetayi.sFiyatTipi,tbStokFisiDetayi.sSaticiRumuzu ,(SELECT sAdi+' '+sSoyAdi FROM tbSatici WHERE sSaticiRumuzu=tbStokFisiDetayi.sSaticiRumuzu)as satici,
tbStokFisiDetayi.sKasiyerRumuzu, tbStokFisiDetayi.nGirisCikis, tbStokFisiDetayi.sDepo AS sMagaza, tbStok.sRenk, tbStok.sBeden,tbStokFisiMaster.sYaziIle, (SELECT TOP 1 sBarkod FROM tbStokBarkodu
WHERE nStokID = tbStok.nStokID) AS sBarkod,(SELECT sRenkAdi FROM tbRenk WHERE sRenk = tbStok.sRenk) AS sRenkAdi FROM tbFirma INNER JOIN tbStokFisiMaster 
INNER JOIN tbStokFisiDetayi ON tbStokFisiMaster.nStokFisiID = tbStokFisiDetayi.nStokFisiID ON tbFirma.nFirmaID = tbStokFisiDetayi.nFirmaID LEFT OUTER JOIN tbStok ON tbStokFisiDetayi.nStokID = tbStok.nStokID 
WHERE (tbStokFisiDetayi.sFisTipi <> 'PF')
AND (tbStokFisiDetayi.dteFisTarihi BETWEEN '{startDate}' AND '{endDate}'   {magazaFilter3})
) Satislar  
GROUP BY nStokID , sKodu , sStokAciklama , sBarkod , sRenk,sBeden,sRenkAdi,sSaticiRumuzu ,satici HAVING (SUM(Miktar) <> 0)) NetSatisKalan  Order by sKodu";

                    var result = await connection.QueryAsync<TbSalesRemainingReport>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("SalesTurnover")]
        public async Task<IActionResult> GetSalesTurnover(string startDate, string endDate, string sDepo)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"set dateformat dmy SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT
DAY(dteFaturaTarihi) AS periyod, SUM(lMalBedeli) AS tutar, SUM(lMalIskontoTutari) AS iskonto
, SUM(lKdv1) + SUM(lKdv2) + SUM(lKdv3) + SUM(lKdv4) + SUM(lKdv5) AS toplamKdv, SUM(lToplamMiktar) AS miktar,
SUM(lNetTutar) AS netCiro, SUM(lMalBedeli) - (SUM(lKdv1) + SUM(lKdv2) + SUM(lKdv3) + SUM(lKdv4) + SUM(lKdv5)) AS araToplam,
COUNT(DISTINCT nStokFisiID) AS musteriSayisi FROM (SELECT nStokFisiID,dteFisTarihi AS dteFaturaTarihi ,
lMalBedeli , lMalIskontoTutari , lKdv1 , lKdv2 , lKdv3 , lKdv4 , lKdv5 , lToplamMiktar , lNetTutar 
, nFirmaID AS nMusteriID FROM tbStokFisiMaster  WHERE 
dteFisTarihi Between '{startDate}' and '{endDate}'  and sDepo='{sDepo}' and
sFisTipi IN ('FS')  ) t GROUP BY DAY(dteFaturaTarihi) ORDER BY DAY(dteFaturaTarihi)";

                    var result = await connection.QueryAsync<TbSalesTurnover>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("SalesTurnoverClassifications")]
        public async Task<IActionResult> GetSalesTurnoverClassifications(string startDate, string endDate, string sinif)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"set dateformat dmy SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT  '' AS PERIYOD, sSinifKodu, sAciklama, SUM(Miktar) AS miktar, SUM(Iskonto) AS iskonto, SUM(lNetTutar) AS tutar,
SUM(Miktar * ISNULL(Maliyet, 0)) AS maliyet, SUM(lNetTutar) - SUM(Miktar * ISNULL(Maliyet, 0)) AS kar,yuzde = CASE WHEN SUM(Miktar * ISNULL(Maliyet, 0)) <> 0 AND SUM(lNetTutar) - SUM(Miktar * ISNULL(Maliyet, 0)) <> 0
THEN (SUM(lNetTutar) - SUM(Miktar * ISNULL(Maliyet, 0))) / SUM(Miktar*ISNULL(Maliyet, 0.01))  ELSE 0 END,COUNT(DISTINCT nMusteriID) AS MUSTERISAYISI,COUNT(DISTINCT nAlisVerisID) AS FISSAYISI,
SUM(lNetTutar)/COUNT(DISTINCT nAlisverisID) FISORTALAMA   FROM (SELECT tbStokFisiDetayi.dteFisTarihi AS dteFaturaTarihi, tbStokFisiDetayi.sFisTipi, tbStokFisiDetayi.sDepo AS sMagaza,
tbStokFisiDetayi.nFirmaID as nMusteriID, CAST(tbStokFisiDetayi.nStokFisiID AS CHAR(20)) as nAlisVerisID, ISNULL(tbStokFisiDetayi.lCikisMiktar1, 0) AS Miktar, 
ISNULL(tbStokFisiDetayi.lIskontoTutari * (100 + tbStokFisiDetayi.nKdvOrani) / 100, 0) AS Iskonto, (tbStokFisiDetayi.lBrutTutar - tbStokFisiDetayi.lIskontoTutari) * (100 + tbStokFisiDetayi.nKdvOrani) / 100 AS lNetTutar,
tbSSinif{sinif}.sSinifKodu, tbSSinif{sinif}.sAciklama, (SELECT isnull(lfiyat , 0) FROM tbstokfiyati WHERE nStokId = tbStokFisiDetayi.NstokId AND sFiyatTipi = 'M') AS Maliyet 
FROM tbSSinif{sinif} INNER JOIN tbStokSinifi ON tbSSinif{sinif}.sSinifKodu = tbStokSinifi.sSinifKodu{sinif} INNER JOIN tbStokFisiDetayi ON tbStokSinifi.nStokID = tbStokFisiDetayi.nStokID 
WHERE (tbStokFisiDetayi.dteFisTarihi BETWEEN '{startDate}' AND '{endDate}')  ) Satislar  WHERE dteFaturaTarihi Between '{startDate}' and '{endDate}'  
GROUP BY sSinifKodu, sAciklama ORDER BY  sSinifKodu, sAciklama ";

                    var result = await connection.QueryAsync<TbSalesTurnoverClassifications>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("SalesTurnoverVendors")]
        public async Task<IActionResult> GetSalesTurnoverVendors(string startDate, string endDate, string sSaticiRumuzu)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"set dateformat dmy SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT '' AS periyod, sKat,sSaticiRumuzu, Satici AS satici, SUM(Miktar) AS miktar, SUM(Iskonto) AS iskonto,SUM(Prim) AS prim, SUM(lNetTutar) AS tutar,
COUNT(DISTINCT nMusteriID) AS musteriSayisi,COUNT(DISTINCT nAlisVerisID) AS FISSAYISI,SUM(lNetTutar)/COUNT(DISTINCT nAlisverisID) FISORTALAMA  ,SUM(lMaliyetTutar) as lMaliyetTutar,SUM(lKar) as lKar,nOran = CASE WHEN SUM(lMaliyetTutar) <> 0 AND SUM(lKar) <> 0 
THEN (SUM(lKar) / SUM(lMaliyetTutar)) * 100 ELSE 100 END FROM (SELECT tbAlisVeris.dteFaturaTarihi , tbAlisVeris.sFisTipi , tbAlisveris.sMagaza , tbAlisVeris.nMusteriID , 
tbAlisVeris.nAlisVerisID , ISNULL(tbStokFisiDetayi.lCikisMiktar1 , 0) AS Miktar , ISNULL(tbStokFisiDetayi.lIskontoTutari , 0) AS Iskonto ,ISNULL(tbStokFisiDetayi.lPrimTutari , 0) AS prim , tbStokFisiDetayi.lBrutTutar - 
tbStokFisiDetayi.lIskontoTutari AS lNetTutar , tbStokFisiDetayi.lCikisMiktar1 * ISNULL ((SELECT isnull(lfiyat, 0) FROM tbstokfiyati WHERE nStokId = tbStokFisiDetayi.NstokId AND sFiyatTipi = 'M'), 0) AS lMaliyetTutar, 
ISNULL(tbStokFisiDetayi.lCikisTutar * (100 + tbStokFisiDetayi.nKdvOrani) / 100, 0) - (tbStokFisiDetayi.lCikisMiktar1 * ISNULL ((SELECT isnull(lfiyat, 0) FROM tbstokfiyati WHERE nStokId = tbStokFisiDetayi.NstokId AND 
sFiyatTipi = 'M'), 0)) AS lKar,tbStokFisiDetayi.sSaticiRumuzu , tbSaticiKatlari.sAdi + ' ' + tbSaticiKatlari.sSoyadi AS Satici,tbSaticiKatlari.sKat FROM tbAlisVeris LEFT OUTER JOIN tbSaticiKatlari INNER JOIN 
tbStokFisiDetayi ON tbSaticiKatlari.sSaticiRumuzu = tbStokFisiDetayi.sSaticiRumuzu ON tbAlisVeris.nAlisverisID = tbStokFisiDetayi.nAlisverisID WHERE
(tbAlisVeris.dteFaturaTarihi BETWEEN '{startDate}' AND '{endDate}') AND 
(tbAlisVeris.sFisTipi = 'P' OR tbAlisVeris.sFisTipi = 'K' OR tbAlisVeris.sFisTipi = 'Ks' OR tbAlisVeris.sFisTipi = 'PD' OR tbAlisVeris.sFisTipi = 'PTX') 
AND (tbAlisVeris.dteFaturaTarihi BETWEEN '{startDate}' AND '{endDate}') 
UNION ALL SELECT tbAlisVeris.dteFaturaTarihi , tbAlisVeris.sFisTipi , tbAlisveris.sMagaza , tbAlisVeris.nMusteriID , tbAlisVeris.nAlisVerisID , tbAlisverisSiparis.lGCMiktar AS Miktar , 
tbAlisverisSiparis.lIskontoTutari AS Iskonto ,tbAlisverisSiparis.lPrimTutari AS Prim , tbAlisverisSiparis.lBrutTutar - tbAlisverisSiparis.lIskontoTutari AS lNetTutar , tbAlisVerisSiparis.lGCMiktar * ISNULL ((SELECT 
isnull(lfiyat, 0) FROM tbstokfiyati WHERE nStokId = tbAlisVerisSiparis.nStokID AND sFiyatTipi = 'M'), 0) AS lMaliyetTutar, tbAlisVerisSiparis.lTutar - (tbAlisVerisSiparis.lGCMiktar * ISNULL ((SELECT isnull(lfiyat, 0) 
FROM tbstokfiyati WHERE nStokId = tbAlisVerisSiparis.NstokId AND sFiyatTipi = 'M'), 0)) AS lKar,tbAlisverisSiparis.sSaticiRumuzu , tbSaticiKatlari.sAdi + ' ' + tbSaticiKatlari.sSoyadi AS Satici,tbSaticiKatlari.sKat FROM tbAlisverisSiparis INNER JOIN tbAlisVeris
ON tbAlisverisSiparis.nAlisverisID = tbAlisVeris.nAlisverisID INNER JOIN tbSaticiKatlari ON tbAlisverisSiparis.sSaticiRumuzu = tbSaticiKatlari.sSaticiRumuzu 
WHERE(tbAlisVeris.dteFaturaTarihi BETWEEN '{startDate}' AND '{endDate}')
UNION ALL SELECT tbStokFisiDetayi.dteFisTarihi AS dteFaturaTarihi, tbStokFisiDetayi.sFisTipi, tbStokFisiDetayi.sDepo AS sMagaza, tbStokFisiDetayi.nFirmaID as nMusteriID, Cast(tbStokFisiDetayi.nStokFisiID as NVARCHAR(20)) nAlisVerisID,
ISNULL(tbStokFisiDetayi.lCikisMiktar1, 0) AS Miktar, ISNULL(tbStokFisiDetayi.lIskontoTutari* (100 + tbStokFisiDetayi.nKdvOrani) / 100, 0) AS Iskonto, tbStokFisiDetayi.lPrimTutari AS Prim,
(tbStokFisiDetayi.lBrutTutar - tbStokFisiDetayi.lIskontoTutari) * (100 + tbStokFisiDetayi.nKdvOrani) / 100 AS lNetTutar, tbStokFisiDetayi.lCikisMiktar1 * ISNULL ((SELECT isnull(lfiyat, 0) FROM tbstokfiyati WHERE nStokId = tbStokFisiDetayi.NstokId AND sFiyatTipi = 'M'), 0) AS lMaliyetTutar,
ISNULL(tbStokFisiDetayi.lCikisTutar * (100 + tbStokFisiDetayi.nKdvOrani) / 100, 0) - (tbStokFisiDetayi.lCikisMiktar1 * ISNULL ((SELECT isnull(lfiyat, 0) FROM tbstokfiyati WHERE nStokId = tbStokFisiDetayi.NstokId AND sFiyatTipi = 'M'), 0)) AS lKar,
tbStokFisiDetayi.sSaticiRumuzu, tbSaticiKatlari.sAdi + ' ' + tbSaticiKatlari.sSoyadi AS Satici, tbSaticiKatlari.sKat FROM tbSaticiKatlari INNER JOIN tbStokFisiDetayi
ON tbSaticiKatlari.sSaticiRumuzu = tbStokFisiDetayi.sSaticiRumuzu 
WHERE  (tbStokFisiDetayi.dteFisTarihi BETWEEN '{startDate}' AND '{endDate}')
) Satislar
WHERE dteFaturaTarihi Between '{startDate}' and '{endDate}'  AND Satislar.sSaticiRumuzu='{sSaticiRumuzu}'
GROUP BY sKat,sSaticiRumuzu, Satici ORDER BY SUM(lNetTutar),sSaticiRumuzu, Satici";

                    var result = await connection.QueryAsync<TbSalesTurnoverVendors>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("GetVendors")]
        public async Task<IActionResult> GetVendors()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"select sSaticiRumuzu,sAdi,sSoyadi,bAktif,sDepo from tbSatici";
                    var vendors = await connection.QueryAsync<TbSatici>(query);
                    return Ok(vendors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("DeliveryReport")]
        public async Task<IActionResult> DeliveryReport(string startDate, string endDate,  string sSaticiRumuzu, string sDepo, string type, string customerName = "", string customerCode = "")
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string whereCondition = (type == "2") ? "WHERE Q.lMevcut <= 0" : (type == "3") ? "WHERE Q.lMevcut > 0" : ""; 
                    
                    // Satıcı ve Depo filtresi - sadece boş değilse ekle
                    string saticiDepoFilter = "";
                    if (!string.IsNullOrEmpty(sSaticiRumuzu))
                    {
                        saticiDepoFilter += $" AND tbSiparis.sSaticiRumuzu='{sSaticiRumuzu}'";
                    }
                    if (!string.IsNullOrEmpty(sDepo))
                    {
                        saticiDepoFilter += $" AND tbSiparis.sDepo='{sDepo}'";
                    }
                    
                    // Müşteri adı ve kodu filtresi için ek koşullar
                    string customerFilter = "";
                    if (!string.IsNullOrEmpty(customerName))
                    {
                        customerFilter += $" AND tbFirma.sAciklama LIKE '%{customerName}%'";
                    }
                    if (!string.IsNullOrEmpty(customerCode))
                    {
                        customerFilter += $" AND tbFirma.sKodu LIKE '%{customerCode}%'";
                    }
                    
                    string query = $@"
Set DateFormat Dmy
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
WITH Q AS (
    SELECT
        tbFirma.nFirmaID,
        tbFirma.sKodu AS sFirmaKodu,
        tbFirma.sAciklama AS sFirmaAciklama,
        tbSiparis.sSiparisiVeren,
        tbSiparis.sSiparisiAlan,
        tbSiparis.lSiparisNo,
        tbSiparis.dteSiparisTarihi,
        tbSiparis.lMiktari,
        tbSiparis.bKapandimi,
        tbSiparis.sBirimCinsi,
        tbSiparis.lBirimMiktar,
        dbo.ROUNDYTL(tbSiparis.lMiktari / tbSiparis.lBirimMiktar) AS lMiktar,
        dbo.ROUNDYTL(tbSiparis.lFiyati * tbSiparis.lBirimMiktar) AS lFiyat,
        tbStok.nStokID,
        tbStok.sModel,
        tbStok.sKodu,
        tbStok.sAciklama,
        tbStok.sRenk,
        tbStok.sBeden,
        tbStok.bEksiyeDusulebilirmi,
        tbStok.bEksideUyarsinmi,
        tbStok.nStokTipi,
        tbStok.sBirimCinsi1,
        tbStok.sBirimCinsi2,
        ISNULL((SELECT ISNULL(lOran, 0) FROM tbStokBirimCevrimi WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2), 1) AS nBirimCarpan,
        tbStok.nPrim,
        tbStok.sKisaAdi,
        tbStok.nHacim,
        tbStok.nAgirlik,
        tbStok.sDovizCinsi,
        (SELECT sRenkAdi FROM tbRenk WHERE sRenk = tbStok.sRenk) AS sRenkAdi,
        ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.NstokId AND sFiyatTipi = 'M'), 0) AS MALIYET,
        (SELECT TOP 1 ISNULL(sBarkod, '') AS sBarkod FROM tbStokBarkodu WHERE nStokID = tbStok.nStokID AND nKisim = 0) AS sBarkod,
        (SELECT ISNULL(SUM(D.lGirisMiktar1 - D.lCikisMiktar1), 0) FROM tbStokFisiDetayi D WHERE tbStok.nStokID = D.nStokID) AS lMevcut,
        (SELECT ISNULL(SUM(S.lGCMiktar - S.bEkAlan1), 0)
           FROM tbAlisverisSiparis S
           INNER JOIN tbAlisVeris V ON S.nAlisverisID = V.nAlisverisID
          WHERE S.nGirisCikis = 3
            AND S.lGCMiktar <> S.bEkAlan1
            AND (V.sFisTipi = 'SP' OR V.sFisTipi = 'SK')
            AND S.dteOnayTarihi = '01/01/1900'
            AND S.dteTeslimEdilecek BETWEEN '01/01/1900' AND '31/12/2048'
            AND V.dteFaturaTarihi BETWEEN '01/01/1900' AND '31/12/2048'
            AND S.nStokID = tbStok.nStokID) AS lBekleyen,
        tbSiparis.nSiparisID,
        ISNULL(SUM(tbStokFisiDetayi.lCikisMiktar1), 0) AS lSevkiyat,
        ISNULL(tbSiparis.lMiktari, 0) - ISNULL(SUM(tbStokFisiDetayi.lCikisMiktar1), 0) AS lKalan,
        ISNULL(tbSiparisAciklamasi.sAciklama1, '') AS sAciklama1,
        ISNULL(tbSiparisAciklamasi.sAciklama2, '') AS sAciklama2,
        ISNULL(tbSiparisAciklamasi.sAciklama3, '') AS sAciklama3,
        ISNULL(tbSiparisAciklamasi.sAciklama4, '') AS sAciklama4,
        ISNULL(tbSiparisAciklamasi.sAciklama5, '') AS sAciklama5,
        tbSiparis.sFiyatTipi,
        tbSiparis.sSaticiRumuzu,
        tbSatici.sAdi   AS sSaticiAdi,
        tbSatici.sSoyAdi AS sSaticiSoyAdi,
        tbSiparis.nKdvOrani,
        tbSiparis.lFiyati * (100 + tbSiparis.nKdvOrani) / 100 AS lFiyati,
        (tbSiparis.lTutari / tbSiparis.lMiktari) * (tbSiparis.lMiktari - ISNULL(SUM(tbStokFisiDetayi.lCikisMiktar1), 0)) * (100 + tbSiparis.nKdvOrani) / 100 AS lTutari,
        tbSiparis.nIskontoYuzdesi,
        CAST(SUBSTRING(tbSiparis.sSatirAciklama, 6, 5) AS MONEY)  AS ISK1,
        CAST(SUBSTRING(tbSiparis.sSatirAciklama, 17, 5) AS MONEY) AS ISK2,
        CAST(SUBSTRING(tbSiparis.sSatirAciklama, 28, 5) AS MONEY) AS ISK3,
        CAST(SUBSTRING(tbSiparis.sSatirAciklama, 39, 5) AS MONEY) AS ISK4,
        ISNULL((tbSiparis.lIskontosuzTutari / tbSiparis.lMiktari), 1) * (tbSiparis.lMiktari - ISNULL(SUM(tbStokFisiDetayi.lCikisMiktar1), 0)) * (100 + tbSiparis.nKdvOrani) / 100 AS lIskontosuzTutari,
        tbSiparis.dteTeslimTarihi,
        (SELECT MAX(SipIslem.sSiparisIslem + ' ' + RTRIM(tbSiparisIslem.sAciklama))
           FROM tbSiparisIslem, tbSiparis AS SipIslem
          WHERE tbSiparisIslem.sSiparisIslem = SipIslem.sSiparisIslem
            AND SipIslem.lSiparisNo = tbSiparis.lSiparisNo
            AND SipIslem.nFirmaID = tbSiparis.nFirmaID
            AND SipIslem.nSiparisTipi = tbSiparis.nSiparisTipi) AS sSipIslem
    FROM tbSiparisAciklamasi
    RIGHT OUTER JOIN tbFirma
      INNER JOIN tbSiparis ON tbFirma.nFirmaID = tbSiparis.nFirmaID
      INNER JOIN tbStok ON tbSiparis.nStokID = tbStok.nStokID
      LEFT OUTER JOIN tbSatici ON tbSiparis.sSaticiRumuzu = tbSatici.sSaticiRumuzu
      LEFT OUTER JOIN tbStokFisiDetayi WITH (INDEX (tbStokFisiDetayi_index12)) ON tbStokFisiDetayi.nSiparisID = tbSiparis.nSiparisID
      ON tbSiparisAciklamasi.nSiparisTipi = tbSiparis.nSiparisTipi
     AND tbSiparisAciklamasi.nFirmaID = tbSiparis.nFirmaID
     AND tbSiparisAciklamasi.lSiparisNo = tbSiparis.lSiparisNo
    WHERE
        tbSiparis.bKapandimi = 0
        AND (tbSiparis.lSiparisNo BETWEEN 1 AND 999999999)
        AND (tbSiparis.dteSiparisTarihi BETWEEN '{startDate}' AND '{endDate}')
        AND (SUBSTRING(tbSiparis.sSiparisiAlan, 1, 30) BETWEEN '' AND 'zzzzzzzzzzzzzzzzzzzz')
        AND (SUBSTRING(tbSiparis.sSiparisiAlan, 31, 30) BETWEEN '' AND 'zzzzzzzzzzzzzzzzzzzz')
        AND (tbSiparis.nSiparisTipi = 1)
        {saticiDepoFilter}
        {customerFilter}
    GROUP BY
        tbFirma.nFirmaID, tbFirma.sKodu, tbFirma.sAciklama,
        tbSiparis.lSiparisNo, tbSiparis.dteSiparisTarihi, tbSiparis.sSiparisiAlan, tbSiparis.sSiparisiVeren,
        tbStok.nStokID, tbStok.sAciklama, tbStok.sModel, tbStok.sKodu, tbStok.sRenk, tbStok.sBeden,
        tbStok.bEksiyeDusulebilirmi, tbStok.bEksideUyarsinmi, tbStok.nStokTipi, tbStok.sBirimCinsi1, tbStok.sBirimCinsi2,
        tbStok.nPrim, tbStok.sKisaAdi, tbStok.nHacim, tbStok.nAgirlik, tbStok.sDovizCinsi,
        tbSiparis.nSiparisID, tbSiparis.lMiktari, tbSiparis.bKapandimi, tbSiparis.sBirimCinsi, tbSiparis.lBirimMiktar,
        tbSiparis.sFiyatTipi, tbSiparis.sSaticiRumuzu, tbSatici.sAdi, tbSatici.sSoyAdi,
        tbSiparis.nKdvOrani, tbSiparis.lFiyati, tbSiparis.lTutari, tbSiparis.nIskontoYuzdesi,
        tbSiparis.sSatirAciklama, tbSiparis.lIskontosuzTutari, tbSiparis.dteTeslimTarihi,
        tbSiparis.nFirmaID, tbSiparis.nSiparisTipi,
        tbSiparisAciklamasi.sAciklama1, tbSiparisAciklamasi.sAciklama2, tbSiparisAciklamasi.sAciklama3, tbSiparisAciklamasi.sAciklama4, tbSiparisAciklamasi.sAciklama5
    HAVING (ISNULL(tbSiparis.lMiktari, 0) - ISNULL(SUM(tbStokFisiDetayi.lCikisMiktar1), 0) > 0)
)
SELECT *
FROM Q
{whereCondition}
ORDER BY Q.sFirmaKodu, Q.dteSiparisTarihi, Q.nSiparisID;";

                    var result = await connection.QueryAsync<TbDeliveryReport>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
    }
}
