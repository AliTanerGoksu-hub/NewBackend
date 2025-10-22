using BarkodBackend.Context;
using BarkodBackend.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BarkodBackend.Controllers
{
    [Route("api/[controller]")]
    public class TbFirmaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public TbFirmaController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DB")!;
        }
        [HttpGet("GetBP")]
        public async Task<IEnumerable<TbFirma>> GetProductsAsync(string sAciklama, string sKodu, string sAdres1,string sSaticiRumuzu)
        {
            string today = DateTime.Now.ToString("dd/MM/yyyy");
            using (var connection = new SqlConnection(_connectionString))
            {

				#region eski cari sorgusu (risk hesaplaması olmayan)
				string query = $@"SELECT  tbFirma.nFirmaID,
								tbFirma.sKodu, 
								tbFirma.sEfatura, 
								tbFirma.sDepo, 
								tbFirma.sAciklama, 
								tbFirma.sOzelNot,
								tbFirma.sSaticiRumuzu,
								tbFirma.nOzelIskontosu,
								tbFirma.sEfaturaTipi,
								tbFirma.nOzelIskontosu2,
								tbFirma.nOzelIskontosu3,
								tbFirma.nOzelIskontosu4,
								tbFirma.sFiyatTipi, 
								tbFirma.lKrediLimiti, 
								tbFirma.sSemt,
								tbFirma.sIl,
								tbFirma.sUlke,
								tbFirma.sPostaKodu,
								tbFirma.nVadeGun,
								tbFirma.sAdres1,
								tbFirma.sAdres2,
								tbFirma.sAdres1 + ' ' + 
								tbFirma.sAdres2 + ' ' + 
								tbFirma.sSemt + ' ' + 
								tbFirma.sIl + ' ' + 
								tbFirma.sUlke + ' ' + 
								tbFirma.sPostaKodu AS Adres, 
								tbFirma.sVergiDairesi, 
								tbFirma.sVergiNo, 
								tbFirmaAciklamasi.sAciklama1 + ' ' + 
								tbFirmaAciklamasi.sAciklama2 + ' ' + 
								tbFirmaAciklamasi.sAciklama3 + ' ' + 
								tbFirmaAciklamasi.sAciklama4 + ' ' + 
								tbFirmaAciklamasi.sAciklama5 AS ISTIHBARAT, 
								tbFSinif1.sAciklama AS SINIF1, 
								tbFSinif2.sAciklama AS SINIF2, 
								tbFSinif3.sAciklama AS SINIF3, 
								tbFSinif4.sAciklama AS SINIF4, 
								tbFSinif5.sAciklama AS SINIF5,
                                lBakiye = CASE WHEN	tbFirma.sDovizCinsi = '   ' THEN (SELECT ISNULL(SUM(lBorcTutar - lAlacakTutar), 0) AS lBakiye FROM tbFirmaHareketi WHERE nFirmaID = tbFirma.nFirmaID) ELSE (SELECT ISNULL(SUM(ISNULL((lBorcTutar - lAlacakTutar),0.0001)/Case When lDovizKuru1 = 0 then 1 else lDovizKuru1 END) , 0) AS lBakiye FROM tbFirmaHareketi WHERE nFirmaID = tbFirma.nFirmaID AND sDovizCinsi1 = tbFirma.sDovizCinsi) END, 
								tbFirma.sDovizCinsi, 
								tbHesapPlani.nHesapID, 
								tbHesapPlani.sKodu AS sHesapKodu, 
								tbHesapPlani.sAciklama AS sHesapAciklama,
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'E-Mail') AS [E-Mail],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Fax') AS [Fax],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Gsm') AS [Gsm],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Telefon') AS [Telefon],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Web') AS [Web],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Yetkili') AS [Yetkili],
								tbFirma.bSatisYapilamaz,
								tbFirma.bTahsilatYapilamaz,bAnaHesap = CASE WHEN RIGHT(tbFirma.sAciklama, 1) = '+' THEN 1 ELSE 0 END,
								SUBSTRING(tbFirma.nZiyaret, 1, 1) AS Pzt, 
								SUBSTRING(tbFirma.nZiyaret, 2, 1) AS Sal, 
								SUBSTRING(tbFirma.nZiyaret, 3, 1) AS Car, 
								SUBSTRING(tbFirma.nZiyaret, 4, 1) AS Per, 
								SUBSTRING(tbFirma.nZiyaret, 5, 1) AS Cum, 
								SUBSTRING(tbFirma.nZiyaret, 6, 1) AS Cmt, 
								SUBSTRING(tbFirma.nZiyaret, 7, 1) AS Paz, 
								tbFirma.nPeriyod
					FROM		tbFirmaAciklamasi RIGHT OUTER JOIN 
								tbFirma INNER JOIN tbHesapPlani ON 
								tbFirma.nHesapID = tbHesapPlani.nHesapID ON 
								tbFirmaAciklamasi.nFirmaID = 
								tbFirma.nFirmaID LEFT OUTER JOIN tbFSinif1 INNER JOIN 
								tbFirmaSinifi ON tbFSinif1.sSinifKodu = 
								tbFirmaSinifi.sSinifKodu1 INNER JOIN tbFSinif2 ON 
								tbFirmaSinifi.sSinifKodu2 = tbFSinif2.sSinifKodu INNER JOIN tbFSinif3 ON 
								tbFirmaSinifi.sSinifKodu3 = tbFSinif3.sSinifKodu INNER JOIN tbFSinif4 ON 
								tbFirmaSinifi.sSinifKodu4 = tbFSinif4.sSinifKodu INNER JOIN tbFSinif5 ON 
								tbFirmaSinifi.sSinifKodu5 = tbFSinif5.sSinifKodu ON 
								tbFirma.nFirmaID = 
								tbFirmaSinifi.nFirmaID 
					WHERE
						
								tbFirma.sAciklama  LIKE '%{sAciklama}%'and 
								tbFirma.sKodu like '%{sKodu}%' and
								(tbFirma.sSaticiRumuzu = '{sSaticiRumuzu}')and
								
								tbFirma.bAktif = 1 and 
								tbFirma.bSipariseKapali = 0  And 
								(
								tbFirma.sKodu  like '120%')  ORDER BY 
								tbFirma.sKodu";
                #endregion
                query = $@"
SET DATEFORMAT DMY
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
WITH RiskTablosu AS (
	SELECT 
		tbFirma.nFirmaID,
		(SELECT ISNULL(SUM(lBorcTutar - lAlacakTutar), 0) 
		 FROM tbFirmaHareketi 
		 WHERE nFirmaID = tbFirma.nFirmaID 
		   AND dteIslemTarihi <= '{today}') AS Bakiye,

		(SELECT ISNULL(SUM(lTutar), 0) 
		 FROM tbCekSenet 
		 WHERE sCekSenetTipi = 'as' 
		   AND nSonCekSenetIslem IN (1, 3, 4, 5, 6) 
		   AND nVerenID = tbFirma.nFirmaID) AS SenetRisk,

		(SELECT ISNULL(SUM(lTutar), 0) 
		 FROM tbCekSenet 
		 WHERE sCekSenetTipi = 'ac' 
		   AND nSonCekSenetIslem IN (1, 3, 4, 5, 6) 
		   AND nVerenID = tbFirma.nFirmaID) AS CekRisk
	FROM tbFirma
)

SELECT  
	tbFirma.nFirmaID,
	tbFirma.sKodu, 
	tbFirma.sEfatura, 
	tbFirma.sDepo, 
	tbFirma.sAciklama, 
	tbFirma.sOzelNot,
	tbFirma.sSaticiRumuzu,
	tbFirma.nOzelIskontosu,
	tbFirma.sEfaturaTipi,
	tbFirma.nOzelIskontosu2,
	tbFirma.nOzelIskontosu3,
	tbFirma.nOzelIskontosu4,
	tbFirma.sFiyatTipi, 
	tbFirma.lKrediLimiti, 
	tbFirma.sSemt,
	tbFirma.sIl,
	tbFirma.sUlke,
	tbFirma.sPostaKodu,
	tbFirma.nVadeGun,
	tbFirma.sAdres1,
	tbFirma.sAdres2,
	tbFirma.sAdres1 + ' ' + tbFirma.sAdres2 + ' ' + tbFirma.sSemt + ' ' + tbFirma.sIl + ' ' + tbFirma.sUlke + ' ' + tbFirma.sPostaKodu AS Adres, 
	tbFirma.sVergiDairesi, 
	tbFirma.sVergiNo, 
	tbFirmaAciklamasi.sAciklama1 + ' ' + tbFirmaAciklamasi.sAciklama2 + ' ' + tbFirmaAciklamasi.sAciklama3 + ' ' + tbFirmaAciklamasi.sAciklama4 + ' ' + tbFirmaAciklamasi.sAciklama5 AS ISTIHBARAT, 
	tbFSinif1.sAciklama AS SINIF1, 
	tbFSinif2.sAciklama AS SINIF2, 
	tbFSinif3.sAciklama AS SINIF3, 
	tbFSinif4.sAciklama AS SINIF4, 
	tbFSinif5.sAciklama AS SINIF5,

	CASE 
		WHEN tbFirma.sDovizCinsi = '   ' THEN 
			(SELECT ISNULL(SUM(lBorcTutar - lAlacakTutar), 0) FROM tbFirmaHareketi WHERE nFirmaID = tbFirma.nFirmaID)
		ELSE 
			(SELECT ISNULL(SUM(ISNULL((lBorcTutar - lAlacakTutar),0.0001)/Case When lDovizKuru1 = 0 then 1 else lDovizKuru1 END) , 0) 
			 FROM tbFirmaHareketi 
			 WHERE nFirmaID = tbFirma.nFirmaID AND sDovizCinsi1 = tbFirma.sDovizCinsi)
	END AS lBakiye,

	ISNULL(r.Bakiye, 0) AS RiskBakiye,
	ISNULL(r.SenetRisk, 0) AS SenetRisk,
	ISNULL(r.CekRisk, 0) AS CekRisk,
	ISNULL(r.Bakiye, 0) + ISNULL(r.SenetRisk, 0) + ISNULL(r.CekRisk, 0) AS ToplamRisk,
	tbFirma.lKrediLimiti - ABS(ISNULL(r.Bakiye, 0)) AS lKalanKredi,

	tbFirma.sDovizCinsi, 
	tbHesapPlani.nHesapID, 
	tbHesapPlani.sKodu AS sHesapKodu, 
	tbHesapPlani.sAciklama AS sHesapAciklama,

	(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'E-Mail') AS [E-Mail],
	(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Fax') AS [Fax],
	(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Gsm') AS [Gsm],
	(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Telefon') AS [Telefon],
	(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Web') AS [Web],
	(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Yetkili') AS [Yetkili],

	tbFirma.bSatisYapilamaz,
	tbFirma.bTahsilatYapilamaz,
	CASE WHEN RIGHT(tbFirma.sAciklama, 1) = '+' THEN 1 ELSE 0 END AS bAnaHesap,

	SUBSTRING(tbFirma.nZiyaret, 1, 1) AS Pzt, 
	SUBSTRING(tbFirma.nZiyaret, 2, 1) AS Sal, 
	SUBSTRING(tbFirma.nZiyaret, 3, 1) AS Car, 
	SUBSTRING(tbFirma.nZiyaret, 4, 1) AS Per, 
	SUBSTRING(tbFirma.nZiyaret, 5, 1) AS Cum, 
	SUBSTRING(tbFirma.nZiyaret, 6, 1) AS Cmt, 
	SUBSTRING(tbFirma.nZiyaret, 7, 1) AS Paz, 
	tbFirma.nPeriyod

FROM tbFirma
INNER JOIN tbHesapPlani ON tbFirma.nHesapID = tbHesapPlani.nHesapID
LEFT JOIN tbFirmaAciklamasi ON tbFirmaAciklamasi.nFirmaID = tbFirma.nFirmaID
INNER JOIN tbFirmaSinifi ON tbFirma.nFirmaID = tbFirmaSinifi.nFirmaID
LEFT JOIN tbFSinif1 ON tbFSinif1.sSinifKodu = tbFirmaSinifi.sSinifKodu1
INNER JOIN tbFSinif2 ON tbFirmaSinifi.sSinifKodu2 = tbFSinif2.sSinifKodu
INNER JOIN tbFSinif3 ON tbFirmaSinifi.sSinifKodu3 = tbFSinif3.sSinifKodu
INNER JOIN tbFSinif4 ON tbFirmaSinifi.sSinifKodu4 = tbFSinif4.sSinifKodu
INNER JOIN tbFSinif5 ON tbFirmaSinifi.sSinifKodu5 = tbFSinif5.sSinifKodu
LEFT JOIN RiskTablosu r ON r.nFirmaID = tbFirma.nFirmaID

WHERE
	tbFirma.sAciklama LIKE '%{sAciklama}%' AND 
	tbFirma.sKodu LIKE '%{sKodu}%' AND 
	tbFirma.sSaticiRumuzu = '{sSaticiRumuzu}' AND 
	tbFirma.bAktif = 1 AND 
	tbFirma.bSipariseKapali = 0 AND 
	tbFirma.sKodu LIKE '120%'

ORDER BY tbFirma.sKodu;
";
                var businessPartners = await connection.QueryAsync<TbFirma>(query);
                return businessPartners;
            }
        }
        [HttpGet("GetBPOrder")]
        public async Task<IActionResult> GetBpOrder(string nFirmaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = $@"SELECT  tbFirma.nFirmaID,
								tbFirma.sKodu, 
								tbFirma.sEfatura, 
								tbFirma.sDepo, 
								tbFirma.sAciklama, 
								tbFirma.sOzelNot,
								tbFirma.sSaticiRumuzu,
								tbFirma.nOzelIskontosu,
								tbFirma.sEfaturaTipi,
								tbFirma.nOzelIskontosu2,
								tbFirma.nOzelIskontosu3,
								tbFirma.nOzelIskontosu4,
								tbFirma.sFiyatTipi, 
								tbFirma.lKrediLimiti, 
								tbFirma.sSemt,
								tbFirma.sIl,
								tbFirma.sUlke,
								tbFirma.sPostaKodu,
								tbFirma.nVadeGun,
								tbFirma.sAdres1,
								tbFirma.sAdres2,
								tbFirma.sAdres1 + ' ' + 
								tbFirma.sAdres2 + ' ' + 
								tbFirma.sSemt + ' ' + 
								tbFirma.sIl + ' ' + 
								tbFirma.sUlke + ' ' + 
								tbFirma.sPostaKodu AS Adres, 
								tbFirma.sVergiDairesi, 
								tbFirma.sVergiNo, 
								tbFirmaAciklamasi.sAciklama1 + ' ' + 
								tbFirmaAciklamasi.sAciklama2 + ' ' + 
								tbFirmaAciklamasi.sAciklama3 + ' ' + 
								tbFirmaAciklamasi.sAciklama4 + ' ' + 
								tbFirmaAciklamasi.sAciklama5 AS ISTIHBARAT, 
								lBakiye = CASE WHEN	tbFirma.sDovizCinsi = '   ' THEN (SELECT ISNULL(SUM(lBorcTutar - lAlacakTutar), 0) AS lBakiye FROM tbFirmaHareketi WHERE nFirmaID = tbFirma.nFirmaID) ELSE (SELECT ISNULL(SUM(ISNULL((lBorcTutar - lAlacakTutar),0.0001)/Case When lDovizKuru1 = 0 then 1 else lDovizKuru1 END) , 0) AS lBakiye FROM tbFirmaHareketi WHERE nFirmaID = tbFirma.nFirmaID AND sDovizCinsi1 = tbFirma.sDovizCinsi) END, 
								tbFirma.sDovizCinsi, 
								tbHesapPlani.nHesapID, 
								tbHesapPlani.sKodu AS sHesapKodu, 
								tbHesapPlani.sAciklama AS sHesapAciklama,
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'E-Mail') AS [E-Mail],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Fax') AS [Fax],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Gsm') AS [Gsm],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Telefon') AS [Telefon],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Web') AS [Web],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Yetkili') AS [Yetkili],
								tbFirma.bSatisYapilamaz,
								tbFirma.bTahsilatYapilamaz,bAnaHesap = CASE WHEN RIGHT(tbFirma.sAciklama, 1) = '+' THEN 1 ELSE 0 END,
								SUBSTRING(tbFirma.nZiyaret, 1, 1) AS Pzt, 
								SUBSTRING(tbFirma.nZiyaret, 2, 1) AS Sal, 
								SUBSTRING(tbFirma.nZiyaret, 3, 1) AS Car, 
								SUBSTRING(tbFirma.nZiyaret, 4, 1) AS Per, 
								SUBSTRING(tbFirma.nZiyaret, 5, 1) AS Cum, 
								SUBSTRING(tbFirma.nZiyaret, 6, 1) AS Cmt, 
								SUBSTRING(tbFirma.nZiyaret, 7, 1) AS Paz, 
								tbFirma.nPeriyod
					FROM		tbFirmaAciklamasi RIGHT OUTER JOIN 
								tbFirma INNER JOIN tbHesapPlani ON 
								tbFirma.nHesapID = tbHesapPlani.nHesapID ON 
								tbFirmaAciklamasi.nFirmaID = 
								tbFirma.nFirmaID 
					WHERE
						
						tbFirma.nFirmaID='{nFirmaId}'";
                var businessPartners = await connection.QueryFirstOrDefaultAsync<TbFirma>(query);
                return Ok(businessPartners);
            }
        }
		[HttpGet("GetBPOrderUsingOrderId")]
        public async Task<IActionResult> GetBPOrderUsingOrderId(string nSiparisId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = $@"SELECT  tbFirma.nFirmaID,
								tbFirma.sKodu, 
								tbFirma.sEfatura, 
								tbFirma.sDepo, 
								tbFirma.sAciklama, 
								tbFirma.sOzelNot,
								tbFirma.sSaticiRumuzu,
								tbFirma.nOzelIskontosu,
								tbFirma.sEfaturaTipi,
								tbFirma.nOzelIskontosu2,
								tbFirma.nOzelIskontosu3,
								tbFirma.nOzelIskontosu4,
								tbFirma.sFiyatTipi, 
								tbFirma.lKrediLimiti, 
								tbFirma.sSemt,
								tbFirma.sIl,
								tbFirma.sUlke,
								tbFirma.sPostaKodu,
								tbFirma.nVadeGun,
								tbFirma.sAdres1,
								tbFirma.sAdres2,
								tbFirma.sAdres1 + ' ' + 
								tbFirma.sAdres2 + ' ' + 
								tbFirma.sSemt + ' ' + 
								tbFirma.sIl + ' ' + 
								tbFirma.sUlke + ' ' + 
								tbFirma.sPostaKodu AS Adres, 
								tbFirma.sVergiDairesi, 
								tbFirma.sVergiNo, 
								tbFirmaAciklamasi.sAciklama1 + ' ' + 
								tbFirmaAciklamasi.sAciklama2 + ' ' + 
								tbFirmaAciklamasi.sAciklama3 + ' ' + 
								tbFirmaAciklamasi.sAciklama4 + ' ' + 
								tbFirmaAciklamasi.sAciklama5 AS ISTIHBARAT, 
								lBakiye = CASE WHEN	tbFirma.sDovizCinsi = '   ' THEN (SELECT ISNULL(SUM(lBorcTutar - lAlacakTutar), 0) AS lBakiye FROM tbFirmaHareketi WHERE nFirmaID = tbFirma.nFirmaID) ELSE (SELECT ISNULL(SUM(ISNULL((lBorcTutar - lAlacakTutar),0.0001)/Case When lDovizKuru1 = 0 then 1 else lDovizKuru1 END) , 0) AS lBakiye FROM tbFirmaHareketi WHERE nFirmaID = tbFirma.nFirmaID AND sDovizCinsi1 = tbFirma.sDovizCinsi) END, 
								tbFirma.sDovizCinsi, 
								tbHesapPlani.nHesapID, 
								tbHesapPlani.sKodu AS sHesapKodu, 
								tbHesapPlani.sAciklama AS sHesapAciklama,
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'E-Mail') AS [E-Mail],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Fax') AS [Fax],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Gsm') AS [Gsm],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Telefon') AS [Telefon],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Web') AS [Web],
								(SELECT TOP 1 sIletisimAdresi FROM tbFirmaIletisimi WHERE nFirmaId = tbFirma.nFirmaID AND sIletisimAraci = 'Yetkili') AS [Yetkili],
								tbFirma.bSatisYapilamaz,
								tbFirma.bTahsilatYapilamaz,bAnaHesap = CASE WHEN RIGHT(tbFirma.sAciklama, 1) = '+' THEN 1 ELSE 0 END,
								SUBSTRING(tbFirma.nZiyaret, 1, 1) AS Pzt, 
								SUBSTRING(tbFirma.nZiyaret, 2, 1) AS Sal, 
								SUBSTRING(tbFirma.nZiyaret, 3, 1) AS Car, 
								SUBSTRING(tbFirma.nZiyaret, 4, 1) AS Per, 
								SUBSTRING(tbFirma.nZiyaret, 5, 1) AS Cum, 
								SUBSTRING(tbFirma.nZiyaret, 6, 1) AS Cmt, 
								SUBSTRING(tbFirma.nZiyaret, 7, 1) AS Paz, 
								tbFirma.nPeriyod
					FROM		tbFirmaAciklamasi RIGHT OUTER JOIN 
								tbFirma INNER JOIN tbHesapPlani ON 
								tbFirma.nHesapID = tbHesapPlani.nHesapID ON 
								tbFirmaAciklamasi.nFirmaID = 
								tbFirma.nFirmaID 
					inner join tbSiparis on tbFirma.nFirmaID=tbSiparis.nFirmaID where tbSiparis.nSiparisID='{nSiparisId}'";
                var businessPartners = await connection.QueryFirstOrDefaultAsync<TbFirma>(query);
                return Ok(businessPartners);
            }
        }
        [HttpGet("GetParamGenel")]
        public async Task<IActionResult> GetParamGenel()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = $@"SELECT * from tbParamGenel";
                var paramGenel = await connection.QueryFirstOrDefaultAsync<TbParamGenel>(query);
                return Ok(paramGenel);
            }
        }
        [HttpGet("Balance")]
        public async Task<IEnumerable<Balance>> GetTransactions(string sKodu)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = $@"SET DATEFORMAT DMY SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT tbFirma.nFirmaID,tbFirmaHareketi.nHareketID,tbFirma.sKodu, tbFirma.sAciklama AS sFirmaAciklama, 
tbFirmaHareketi.dteIslemTarihi, tbFirmaHareketi.dteValorTarihi, tbFirmaHareketi.lBorcTutar, tbFirmaHareketi.lAlacakTutar, tbFirmaHareketi.lBorcTutar - tbFirmaHareketi.lAlacakTutar AS kalanBakiye, 
tbFirmaHareketi.sCariIslem AS islemAciklama,(SELECT     sAciklama FROM         tbCariIslem where sCariIslem = tbFirmaHareketi.sCariIslem) islemTipi, Isnull(tbStokFisiMaster.nGirisCikis,0) as nGirisCikis, 
ISNULL(tbStokFisiMaster.nEvrakNo, 0) as nEvrakNo, tbFirmaHareketi.sAciklama, tbFirmaHareketi.sEvrakNo AS lFisNo, tbFirmaHareketi.sHangiUygulama, tbFirmaHareketi.sHareketTipi, tbFirmaHareketi.dteKayitTarihi as tarih, 
tbFirmaHareketi.sDovizCinsi1, tbFirmaHareketi.lDovizMiktari1, tbFirmaHareketi.lDovizKuru1, ISNULL(tbStokFisiOdemePlani.nStokFisiID, 0) AS nStokFisiID, tbFirmaHareketi.sKullaniciAdi, 
ISNULL(TempDevir.lDevir, 0) AS lDevir,tbStokFisiAciklamasi.sAciklama1,(SELECT ISNULL(nCekSenetID,0) FROM tbCekSenet WHERE CAST(lCekSenetNo AS CHAR(10)) = CAST(tbFirmaHareketi.sEvrakNo AS CHAR(10)) 
AND sCekSenetTipi = tbFirmaHareketi.sHangiUygulama AND dteVadeTarihi = tbFirmaHareketi.dteValorTarihi) AS nCekSenetID FROM tbStokFisiAciklamasi INNER JOIN tbStokFisiOdemePlani INNER JOIN tbGirisCikis
INNER JOIN tbStokFisiMaster ON tbGirisCikis.nGirisCikis = tbStokFisiMaster.nGirisCikis ON tbStokFisiOdemePlani.nStokFisiID = tbStokFisiMaster.nStokFisiID ON tbStokFisiAciklamasi.nStokFisiID = tbStokFisiMaster.nStokFisiID 
RIGHT OUTER JOIN tbFirmaHareketi INNER JOIN tbFirma ON tbFirmaHareketi.nFirmaID = tbFirma.nFirmaID LEFT OUTER JOIN (SELECT SUM(tbFirmaHareketi.lBorcTutar) - SUM(tbFirmaHareketi.lAlacakTutar) AS lDevir ,
tbFirmaHareketi.nFirmaID FROM tbFirma , tbFirmaHareketi WHERE  tbFirma.nFirmaId = tbFirmaHareketi.nFirmaId AND  tbFirma.sKodu = '{sKodu}' GROUP BY tbFirmaHareketi.nFirmaID) TempDevir
ON tbFirmaHareketi.nFirmaID = TempDevir.nFirmaID ON tbStokFisiOdemePlani.nCariHareketID = tbFirmaHareketi.nHareketID WHERE   (tbFirma.sKodu = '{sKodu}') AND (tbFirmaHareketi.dteIslemTarihi BETWEEN '01/01/2000' AND '31/12/2078')
ORDER BY  tbFirmaHareketi.dteIslemTarihi,tbFirmaHareketi.nHareketID  

";
                 query = $@"SET DATEFORMAT DMY;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

SELECT
    f.nFirmaID,
    fh.nHareketID,
    f.sKodu,
    f.sAciklama AS sFirmaAciklama,
    fh.dteIslemTarihi,
    fh.dteValorTarihi,
    fh.lBorcTutar,
    fh.lAlacakTutar,

      SUM(ISNULL(fh.lBorcTutar,0))   OVER (PARTITION BY fh.nFirmaID) AS ToplamBorc,
    SUM(ISNULL(fh.lAlacakTutar,0)) OVER (PARTITION BY fh.nFirmaID) AS ToplamAlacak,
    SUM(ISNULL(fh.lBorcTutar,0))   OVER (PARTITION BY fh.nFirmaID)
  - SUM(ISNULL(fh.lAlacakTutar,0)) OVER (PARTITION BY fh.nFirmaID) AS KalanBakiye1,

       SUM( ISNULL(fh.lBorcTutar,0) - ISNULL(fh.lAlacakTutar,0) )
        OVER (
          PARTITION BY fh.nFirmaID
          ORDER BY fh.dteIslemTarihi, fh.nHareketID
          ROWS UNBOUNDED PRECEDING
        ) AS KalanBakiye,

        ISNULL(td.lDevir,0) +
    SUM( ISNULL(fh.lBorcTutar,0) - ISNULL(fh.lAlacakTutar,0) )
        OVER (
          PARTITION BY fh.nFirmaID
          ORDER BY fh.dteIslemTarihi, fh.nHareketID
          ROWS UNBOUNDED PRECEDING
        ) AS AkisBakiye_DevirDahil,

    fh.sCariIslem AS islemAciklama,
    (SELECT sAciklama FROM tbCariIslem WHERE sCariIslem = fh.sCariIslem) AS islemTipi,
    ISNULL(sfm.nGirisCikis,0) AS nGirisCikis,
    ISNULL(sfm.nEvrakNo,0)    AS nEvrakNo,
    fh.sAciklama,
    fh.sEvrakNo AS lFisNo,
    fh.sHangiUygulama,
    fh.sHareketTipi,
    fh.dteKayitTarihi AS tarih,
    fh.sDovizCinsi1,
    fh.lDovizMiktari1,
    fh.lDovizKuru1,
    ISNULL(sfo.nStokFisiID,0) AS nStokFisiID,
    fh.sKullaniciAdi,
    ISNULL(td.lDevir,0)       AS lDevir,
    sfa.sAciklama1,
    (SELECT ISNULL(nCekSenetID,0)
       FROM tbCekSenet
      WHERE CAST(lCekSenetNo AS CHAR(10)) = CAST(fh.sEvrakNo AS CHAR(10))
        AND sCekSenetTipi = fh.sHangiUygulama
        AND dteVadeTarihi = fh.dteValorTarihi) AS nCekSenetID
FROM tbFirmaHareketi fh
JOIN tbFirma f
  ON fh.nFirmaID = f.nFirmaID
LEFT JOIN tbStokFisiOdemePlani sfo
  ON sfo.nCariHareketID = fh.nHareketID
LEFT JOIN tbStokFisiMaster sfm
  ON sfo.nStokFisiID = sfm.nStokFisiID
LEFT JOIN tbGirisCikis gg
  ON gg.nGirisCikis = sfm.nGirisCikis
LEFT JOIN tbStokFisiAciklamasi sfa
  ON sfa.nStokFisiID = sfm.nStokFisiID

LEFT JOIN (
    SELECT
        fh2.nFirmaID,
        SUM(ISNULL(fh2.lBorcTutar,0)) - SUM(ISNULL(fh2.lAlacakTutar,0)) AS lDevir
    FROM tbFirmaHareketi fh2
    JOIN tbFirma f2 ON f2.nFirmaID = fh2.nFirmaID
    WHERE f2.sKodu = '{sKodu}'
    GROUP BY fh2.nFirmaID
) td ON td.nFirmaID = fh.nFirmaID
WHERE f.sKodu = '{sKodu}'
  AND fh.dteIslemTarihi BETWEEN '01/01/2000' AND '31/12/2078'
ORDER BY fh.dteIslemTarihi, fh.nHareketID;
";

                var balances = await connection.QueryAsync<Balance>(query);
                return balances;
            }
        }
        [HttpGet("ReceiptDetail")]
        public async Task<IEnumerable<ReceiptDetail>> GetReceiptDetail(string lFisNo)
        {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"	select tbStokFisiDetayi.lFisNo,tbSiparis.lSiparisNo,tbStok.sKodu, tbStok.sAciklama,tbSiparis.lMiktari,tbSiparis.lFiyati,tbSiparis.lTutari,tbSiparis.lIskontosuzTutari,tbSiparis.nIskontoYuzdesi from tbStokFisiDetayi
	inner join tbSiparis on tbStokFisiDetayi.nSiparisID=tbSiparis.nSiparisID
	inner join tbStok on tbStok.nStokID=tbSiparis.nStokID
where tbStokFisiDetayi.lFisNo={lFisNo} 
";
                    var receipts = await connection.QueryAsync<ReceiptDetail>(query);
                    return receipts;

                }

        }
		[HttpGet("APersonelFiyatTipi")]
		public async Task<string> APersonelFiyatTipi(string PERSONELKODU)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {

                    string query = $@"select sAktifFiyatTipi from APERSONEL where PERSONELKODU='{PERSONELKODU}' ";
					var result= await connection.QueryFirstOrDefaultAsync<string>(query);

                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpGet("GetTbFirmaAdresi")]
        public async Task<IActionResult> GetTbFirmaAdresi(string nFirmaID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"select nFirmaID,sAciklama,sAdres1,sAdres2,sSemt,sIl,sUlke,sPostaKodu from tbFirmaAdresi where nFirmaID='{nFirmaID}'";

                    var address = await connection.QueryFirstOrDefaultAsync<TbFirmaAdresi>(query);
                    address = (address == null) ? new TbFirmaAdresi() : address;
                    return Ok(address);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
