using BarkodBackend.Context;
using BarkodBackend.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace BarkodBackend.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DB")!;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(string user, string password)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                SELECT 
                    PERSONELADI, 
                    PERSONELKODU, 
                    SIFRE, 
                    SATICIRUMUZU AS sSaticiRumuzu, 
                    sAktifFiyatTipi, 
                    sDepo 
                FROM APERSONEL 
                WHERE KULLANICI = 1 AND MobileAktif = 1 
                      AND PERSONELKODU = @user AND SIFRE = @password";

                    var auth = await connection.QueryFirstOrDefaultAsync<Auth>(query, new { user, password });
                    
                    // Debug için
                    Console.WriteLine($"[Login] Auth null mu: {auth == null}");
                    if (auth != null)
                    {
                        Console.WriteLine($"[Login] sDepo değeri: '{auth.sDepo}'");
                        Console.WriteLine($"[Login] sSaticiRumuzu değeri: '{auth.sSaticiRumuzu}'");
                    }
                    
                    if (auth != null)
                    {
                        var claims = new[] {
                    new Claim(ClaimTypes.Name, auth.PERSONELADI),
                    new Claim("sSaticiRumuzu", auth.sSaticiRumuzu ?? ""),
                    new Claim("sDepo", auth.sDepo ?? "") 
                };

                        var identity = new ClaimsIdentity(claims, "login");
                        HttpContext.User = new ClaimsPrincipal(identity);

                        return Ok(auth);
                    }
                    else
                    {
                        return NotFound("Girilen parametrelere ait mobil kullanıcı bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("GetMobileUsers")]
        public async Task<IActionResult> GetMobileUsers()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"select PERSONELKODU,PERSONELADI from APERSONEL where MobileAktif=1 and  KULLANICI=1";
                    var users = await connection.QueryAsync<Auth>(query);
                    return Ok(users);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("GetYetkiRapor")]
        public async Task<IActionResult> GetYetkiRapor(string personelKodu)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(personelKodu))
                {
                    return BadRequest(new { message = "Personel kodu boş olamaz." });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    // SQL INJECTION FIX: Parametreli sorgu kullan
                    string query = @"
SELECT 
    tbYetkiRapor.ID, 
    APERSONEL.PERSONELKODU,
    APERSONEL.PERSONELADI,
    tbYetkiRapor.RaporID, 
    tbRaporlar.RaporAciklama,
    tbYetkiRapor.YetkisiVar 
FROM tbYetkiRapor 
INNER JOIN APERSONEL ON APERSONEL.PERSONELKODU = tbYetkiRapor.PersonelKdou 
INNER JOIN tbRaporlar ON tbRaporlar.RaporId = tbYetkiRapor.RaporID 
WHERE tbYetkiRapor.PersonelKdou = @personelKodu";

                    var users = await connection.QueryAsync<TbYetkiRapor>(query, new { personelKodu });
                    return Ok(users);
                }
            }
            catch (SqlException sqlEx)
            {
                // SQL hatalarını loglayın (production'da detay göstermeyin)
                return StatusCode(500, new { message = "Veritabanı hatası oluştu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu." });
            }
        }
        [HttpPost("UpdatePermission")]
        public async Task<IActionResult> UpdatePermission(int permissionID, bool yetki)
        {
            try
            {
                // Input validation
                if (permissionID <= 0)
                {
                    return BadRequest(new { message = "Geçersiz yetki ID'si." });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    // SQL INJECTION FIX: Parametreli sorgu kullan
                    string query = @"
                        UPDATE tbYetkiRapor 
                        SET YetkisiVar = @yetki 
                        WHERE ID = @permissionID";

                    var rowsAffected = await connection.ExecuteAsync(query, new { yetki, permissionID });
                    
                    if (rowsAffected == 0)
                    {
                        return NotFound(new { message = "Yetki kaydı bulunamadı." });
                    }

                    return Ok(new { message = "Yetki başarıyla güncellendi." });
                }
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, new { message = "Veritabanı hatası oluştu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu." });
            }
        }
    }
}
