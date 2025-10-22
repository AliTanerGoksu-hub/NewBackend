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
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"
select tbYetkiRapor.ID, APERSONEL.PERSONELKODU,APERSONEL.PERSONELADI,tbYetkiRapor.RaporID, tbRaporlar.RaporAciklama,
tbYetkiRapor. YetkisiVar from tbYetkiRapor 
inner join APERSONEL on APERSONEL.PERSONELKODU=tbYetkiRapor.PersonelKdou 
inner join tbRaporlar on tbRaporlar.RaporId=tbYetkiRapor.RaporID 
where tbYetkiRapor.PersonelKdou='{personelKodu}'";
                    var users = await connection.QueryAsync<TbYetkiRapor>(query);
                    return Ok(users);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpPost("UpdatePermission")]
        public async Task<IActionResult> UpdatePermission(int permissionID, bool yetki)
        {
            try
            {
               
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"
                       update tbYetkiRapor set YetkisiVar='{yetki}' where ID={permissionID}";

                    await connection.ExecuteAsync(query);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
    }
}
