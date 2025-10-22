using BarkodBackend.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data;

namespace BarkodBackend.Controllers
{
    [Route("api/[controller]")]
    public class TbStokController : ControllerBase
    {
        private readonly string _connectionString;

        public TbStokController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DB")!;
        }

        [HttpGet("Stocks")]
        public async Task<IActionResult> GetStocks(
    string sAciklama = "",
    string sBarkod = "",
    string sRenk = "",
    string sBeden = "",
    string sKodu = "",
    string sDepo = "",
    string sFiyatTipi = "1",
    bool getAll = false)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // NOT: Yapıyı bozmadım; sadece depo ve fiyat tipi parametreleştirildi.
                    string query = @"
SET DATEFORMAT dmy
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
SELECT
    CAST(tbStok.nStokID AS VARCHAR(20)) AS nStokID,
    tbStok.sKodu,
    tbStok.sAciklama,
    tbStok.sOzelNot,
    tbStok.sKisaAdi,
    tbStok.sBeden,
    (SELECT sRenkAdi FROM tbRenk WHERE sRenk = tbStok.sRenk) AS sRenkAdi,

    -- Barkod: stok başına TEK değer
    ISNULL(barkod.sBarkod, '') AS sBarkod,

    ISNULL((
        SELECT ISNULL(lOran, 0)
        FROM tbStokBirimCevrimi
        WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2
    ), 1) AS nBirimCarpan,

    CAST((
        SELECT ISNULL(SUM(d.lGirisMiktar1 - d.lCikisMiktar1), 0)
        FROM tbStokFisiDetayi d
        WHERE d.nStokID = tbStok.nStokID
          AND (@sDepo = '' OR d.sDepo = @sDepo)
    ) /
    ISNULL((
        SELECT ISNULL(lOran, 0)
        FROM tbStokBirimCevrimi
        WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2
    ), 1) AS MONEY) AS lMevcut2,

    (SELECT ISNULL(SUM(d.lGirisMiktar1 - d.lCikisMiktar1), 0)
     FROM tbStokFisiDetayi d
     WHERE d.nStokID = tbStok.nStokID
       AND (@sDepo = '' OR d.sDepo = @sDepo)
    ) AS lMevcut,

    ISNULL((
        SELECT ISNULL(lfiyat, 0)
        FROM tbStokFiyati
        WHERE nStokId = tbStok.nStokId
          AND sFiyatTipi = @sFiyatTipi
    ), 0) AS lFiyat1,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = '2'), 0) AS lFiyat2,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = '3'), 0) AS lFiyat3,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = '4'), 0) AS lFiyat4,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = 'A'), 0) AS FIYAT5,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = 'M'), 0) AS FIYAT6,

    (SELECT nKdvOrani FROM tbKdv WHERE sKdvTipi = tbStok.sKdvTipi) AS nKdvOrani
FROM tbStok

-- !!! ÇOĞALTMAYAN BARKOD KAYNAĞI (tek satır/stok)
LEFT JOIN (
    SELECT nStokID, MIN(sBarkod) AS sBarkod
    FROM tbStokBarkodu
    WHERE nKisim = 0         -- İSTERSEN 1 yap:  WHERE nKisim = 1
    GROUP BY nStokID
) barkod
  ON barkod.nStokID = tbStok.nStokID

-- !!! DİKKAT: tbDepoDetayi JOIN'İ KALDIRILDI (çoğaltma sebebiydi)

WHERE
    (@getAll = 1 OR tbStok.sKodu LIKE 'TS%')
    AND tbStok.nStokTipi < 5
    AND (tbStok.sKodu     LIKE 'TS%' + @sKodu + '%' OR @sKodu = '')
    AND (tbStok.sAciklama LIKE '%'   + @sAciklama + '%' OR @sAciklama = '')
    AND (tbStok.nStokID IN (SELECT nStokID FROM tbStokBarkodu WHERE sBarkod = @sBarkod OR @sBarkod = ''))
    AND (tbStok.sRenk     LIKE '%'   + @sRenk + '%' OR @sRenk = '')
    AND (tbStok.sBeden    LIKE '%'   + @sBeden + '%' OR @sBeden = '')
ORDER BY tbStok.sKodu";


                    var stocks = await connection.QueryAsync<TbStok>(
                        query,
                        new { sKodu, sAciklama, sBarkod, sRenk, sBeden, sDepo, sFiyatTipi, getAll });

                    return Ok(stocks);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }


        [HttpGet("PriceType")]
        public async Task<IActionResult> GetPriceType()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT sFiyatTipi, sAciklama FROM tbFiyatTipi WHERE aktif = 1";
                    var priceType = await connection.QueryAsync<TbFiyatTipi>(query);
                    return Ok(priceType);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("Warehouse")]
        public async Task<IActionResult> GetWarehouse()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT sDepo, sAciklama FROM tbDepo";
                    var warehouse = await connection.QueryAsync<TbDepo>(query);
                    return Ok(warehouse);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("GetUnits")]
        public async Task<IActionResult> GetUnits(string nStokID, string sDepo, string sFiyatTipi = "1")
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
SELECT
    ROW_NUMBER() OVER (ORDER BY v.sira) AS no,
    v.sBirimCinsi,
    tbBirimCinsi.sAciklama
FROM tbStok
CROSS APPLY (
    VALUES
        (1, tbStok.sBirimCinsi2),
        (2, tbStok.sBirimCinsi1)
) AS v (sira, sBirimCinsi)
INNER JOIN tbBirimCinsi ON v.sBirimCinsi = tbBirimCinsi.sBirimCinsi
WHERE tbStok.nStokID = @nStokID
AND EXISTS (
    SELECT 1 FROM tbStok WHERE nStokID = @nStokID
)
GROUP BY v.sira, v.sBirimCinsi, tbBirimCinsi.sAciklama";
                    var units = await connection.QueryAsync<TbStokBirimCinsi>(query, new { nStokID, sDepo, sFiyatTipi });
                    return Ok(units);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("BarcodeRead")]
        public async Task<IActionResult> BarcodeRead(string sBarkod, string sKodu = "", string sAciklama = "", string sDepo = "", string sFiyatTipi = "1")
        {
            try
            {
                string today = DateTime.Today.ToString("dd/MM/yyyy");
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"SET DATEFORMAT dmy
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
SELECT
    CAST(tbStok.nStokID AS VARCHAR(20)) AS nStokID,
    tbStok.sKodu,
    tbStok.sAciklama,
    tbStok.sOzelNot,
    tbStok.sKisaAdi,
    tbStok.sBeden,
    (SELECT sRenkAdi FROM tbRenk WHERE sRenk = tbStok.sRenk) AS sRenkAdi,
    CASE WHEN tbStokBarkodu.nKisim = 0 THEN tbStokBarkodu.sBarkod ELSE '' END AS sBarkod,
    ISNULL((SELECT ISNULL(lOran, 0) FROM tbStokBirimCevrimi WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2), 1) AS nBirimCarpan,
    CAST((SELECT ISNULL(SUM(tbStokFisiDetayi.lGirisMiktar1 - tbStokFisiDetayi.lCikisMiktar1), 0)
          FROM tbStokFisiDetayi WHERE tbStok.nStokID = tbStokFisiDetayi.nStokID) /
          ISNULL((SELECT ISNULL(lOran, 0) FROM tbStokBirimCevrimi WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2), 1) AS MONEY) AS lMevcut2,
    (SELECT ISNULL(SUM(tbStokFisiDetayi.lGirisMiktar1 - tbStokFisiDetayi.lCikisMiktar1), 0)
     FROM tbStokFisiDetayi WHERE tbStok.nStokID = tbStokFisiDetayi.nStokID) AS lMevcut,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId
            AND sFiyatTipi = (SELECT TOP(1) sAktifFiyatTipi FROM APERSONEL WHERE PERSONELKODU = 'Admin')), 0) AS lFiyat1,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = 'A'), 0) AS FIYAT5,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = 'M'), 0) AS FIYAT6,
    (SELECT nKdvOrani FROM tbKdv WHERE sKdvTipi = tbStok.sKdvTipi) AS nKdvOrani,
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
LEFT OUTER JOIN tbStokBarkodu ON tbStok.nStokID = tbStokBarkodu.nStokID AND (tbStokBarkodu.nKisim IN (0))
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
LEFT OUTER JOIN (
    SELECT nStokID, sDepo, SUM(sDepo1) AS sDepo1
    FROM (
        SELECT nStokID, sDepo,
            CASE WHEN RTRIM(UPPER(sDepo)) = 'D001' THEN lGirisMiktar1 - lCikisMiktar1 ELSE 0 END AS sDepo1
        FROM tbStokFisiDetayi
    ) tbStokFisiDetayi
    GROUP BY nStokID, sDepo
) tbDepoDetayi ON tbStok.nStokID = tbDepoDetayi.nStokID
WHERE
    tbStok.sKodu like 'TS%'
    AND tbStok.nStokTipi < 5
    AND (tbStok.sKodu LIKE 'TS%' + @sKodu + '%' OR @sKodu = '')
    AND (tbStok.sAciklama LIKE '%' + @sAciklama + '%' OR @sAciklama = '')
    AND (tbStok.nStokID IN (SELECT nStokID FROM tbStokBarkodu WHERE sBarkod = @sBarkod OR @sBarkod = ''))
    AND (tbStok.sRenk LIKE '%' + @sRenk + '%' OR @sRenk = '')
    AND (tbStok.sBeden LIKE '%' + @sBeden + '%' OR @sBeden = '')";
                    var stock = await connection.QueryFirstOrDefaultAsync<TbStok>(query, new { sBarkod, sKodu, sAciklama, sDepo, sFiyatTipi });
                    if (stock == null)
                        return NotFound(new { message = "Stok bulunamadı." });
                    Console.WriteLine($"BarcodeRead: pResim1: {stock.pResim1}, yol1: {stock.yol1}");
                    return Ok(stock);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("CatalogStocks")]
        public async Task<IActionResult> GetCatalogStocks(
     int currentPage,
     int pageSize,
     string nStokID = "",
     string sAciklama = "",
     string sBarkod = "",
     string sRenk = "",
     string sBeden = "",
     string sKodu = "",
     string sDepo = "D001",
     string sFiyatTipi = "1",
     string searchTerm = ""
 )
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var sql = @"
SET DATEFORMAT dmy;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

SELECT
    CAST(tbStok.nStokID AS VARCHAR(20)) AS nStokID,
    tbStok.sKodu,
    tbStok.sAciklama,
    tbStok.sOzelNot,
    tbStok.sKisaAdi,
    tbStok.sBeden,
    (SELECT sRenkAdi FROM tbRenk WHERE sRenk = tbStok.sRenk) AS sRenkAdi,
    CASE WHEN tbStokBarkodu.nKisim = 0 THEN tbStokBarkodu.sBarkod ELSE '' END AS sBarkod,
    ISNULL((SELECT ISNULL(lOran, 0) FROM tbStokBirimCevrimi WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2), 1) AS nBirimCarpan,
    CAST(ISNULL(tbDepoDetayi.sDepo1, 0) /
          ISNULL((SELECT ISNULL(lOran, 0) FROM tbStokBirimCevrimi WHERE nStokID = tbStok.nStokID AND sBirimCinsi = tbStok.sBirimCinsi2), 1) AS MONEY) AS lMevcut2,
    ISNULL(tbDepoDetayi.sDepo1, 0) AS lMevcut,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId
            AND sFiyatTipi = (SELECT TOP 1 sAktifFiyatTipi FROM APERSONEL WHERE PERSONELKODU = 'Admin')), 0) AS lFiyat1,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = '2'), 0) AS lFiyat2,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = '3'), 0) AS lFiyat3,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = '4'), 0) AS lFiyat4,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = 'A'), 0) AS FIYAT5,
    ISNULL((SELECT ISNULL(lfiyat, 0) FROM tbStokFiyati WHERE nStokId = tbStok.nStokId AND sFiyatTipi = 'M'), 0) AS FIYAT6,
    (SELECT nKdvOrani FROM tbKdv WHERE sKdvTipi = tbStok.sKdvTipi) AS nKdvOrani,

    -- SADECE R2 URL
    Resim1.yol AS ImageUrl,
    CASE WHEN NULLIF(Resim1.yol,'') IS NOT NULL THEN 1 ELSE 0 END AS HasResim

FROM tbStok
LEFT OUTER JOIN tbStokBarkodu
  ON tbStok.nStokID = tbStokBarkodu.nStokID AND tbStokBarkodu.nKisim = 0
INNER JOIN tbStokSinifi ON tbStok.nStokID = tbStokSinifi.nStokID
INNER JOIN tbSSinif1 ON tbStokSinifi.sSinifKodu1 = tbSSinif1.sSinifKodu

OUTER APPLY (
    SELECT TOP (1) r.yol
    FROM tbStokResmi r WITH (READUNCOMMITTED)
    WHERE r.sModel = tbStok.sModel
      AND ISNULL(r.yol,'') <> ''
    ORDER BY r.nSira
) AS Resim1

LEFT OUTER JOIN (
    SELECT nStokID, sDepo, SUM(lGirisMiktar1 - lCikisMiktar1) AS sDepo1
    FROM tbStokFisiDetayi
    WHERE RTRIM(UPPER(sDepo)) = RTRIM(UPPER(@sDepo))
    GROUP BY nStokID, sDepo
) tbDepoDetayi ON tbStok.nStokID = tbDepoDetayi.nStokID

WHERE
    tbStok.sKodu LIKE 'TS%'
    AND tbStok.nStokTipi < 5
    AND (tbStok.nStokID LIKE @nStokID + '%' OR @nStokID = '')
    AND (
         (tbStok.sKodu LIKE @searchTerm + '%' OR @searchTerm = '')
      OR (tbStok.sAciklama LIKE '%'+ @searchTerm + '%' OR @searchTerm = '')
      OR (tbStok.nStokID IN (SELECT nStokID FROM tbStokBarkodu WHERE sBarkod = @searchTerm OR @searchTerm = ''))
    )
ORDER BY HasResim DESC, tbStok.sKodu
OFFSET (@currentPage - 1) * @pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY;";

                var rows = await connection.QueryAsync<RawStokRow>(sql, new
                {
                    currentPage,
                    pageSize,
                    nStokID = nStokID ?? "",
                    searchTerm = searchTerm ?? "",
                    sFiyatTipi = sFiyatTipi ?? "1",
                    sDepo = sDepo ?? "D001"
                });

                var result = rows.Select(r => new CatalogStockDto
                {
                    nStokID = r.nStokID,
                    sKodu = r.sKodu,
                    sAciklama = r.sAciklama,
                    sOzelNot = r.sOzelNot,
                    sKisaAdi = r.sKisaAdi,
                    sBeden = r.sBeden,
                    sRenkAdi = r.sRenkAdi,
                    sBarkod = r.sBarkod,
                    nBirimCarpan = r.nBirimCarpan,
                    lMevcut2 = r.lMevcut2,
                    lMevcut = r.lMevcut,
                    lFiyat1 = r.lFiyat1,
                    lFiyat2 = r.lFiyat2,
                    lFiyat3 = r.lFiyat3,
                    lFiyat4 = r.lFiyat4,
                    FIYAT5 = r.FIYAT5,
                    FIYAT6 = r.FIYAT6,
                    nKdvOrani = r.nKdvOrani,
                    ImageUrl = r.ImageUrl,           // Sadece R2 URL
                    HasResim = r.HasResim == 1
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        private sealed class RawStokRow
        {
            public string nStokID { get; set; }
            public string sKodu { get; set; }
            public string sAciklama { get; set; }
            public string sOzelNot { get; set; }
            public string sKisaAdi { get; set; }
            public string sBeden { get; set; }
            public string sRenkAdi { get; set; }
            public string sBarkod { get; set; }
            public decimal nBirimCarpan { get; set; }
            public decimal lMevcut2 { get; set; }
            public decimal lMevcut { get; set; }
            public decimal lFiyat1 { get; set; }
            public decimal lFiyat2 { get; set; }
            public decimal lFiyat3 { get; set; }
            public decimal lFiyat4 { get; set; }
            public decimal FIYAT5 { get; set; }
            public decimal FIYAT6 { get; set; }
            public decimal nKdvOrani { get; set; }

            public string ImageUrl { get; set; }   // <-- sadece URL
            public int HasResim { get; set; }
        }

        // Mobilin kullanacağı DTO


        [HttpGet("IsColorOrSize")]
        public async Task<IActionResult> IsColorOrSize()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
SELECT
    CASE
        WHEN EXISTS(
            SELECT 1 FROM tbStok
            WHERE sRenk IS NOT NULL
            AND LTRIM(RTRIM(sRenk)) <> ''
        ) THEN 1
        ELSE 0
    END AS sRenkAdi,
    CASE
        WHEN EXISTS(
            SELECT 1 FROM tbStok
            WHERE sBeden IS NOT NULL
            AND LTRIM(RTRIM(sBeden)) <> ''
        ) THEN 1
        ELSE 0
    END AS sBeden";
                    var result = await connection.QueryFirstOrDefaultAsync<TbStok>(query);
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