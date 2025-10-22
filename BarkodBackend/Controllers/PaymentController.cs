using BarkodBackend.Context;
using BarkodBackend.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BarkodBackend.Controllers
{
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DB")!;
        }
        [HttpGet("GetAccounts")]
        public async Task<IActionResult> GetAccounts()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"select sKodu, sAciklama from tbFirma where bAktif=1 and sKodu like '120%'
";
                    var firma = await connection.QueryAsync<TbFirma>(query);
                    return Ok(firma);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpGet("GetPaymentType")]
        public async Task<IActionResult> GetPaymentType()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = $@"select sOdemeSekli, sAciklama from tbOdemeSekli";
                    var odemeSekli = await connection.QueryAsync<TbOdemeSekli>(query);
                    return Ok(odemeSekli);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Bir hata oluştu: " + ex.Message });
            }
        }
        [HttpPost("AddPayment")]
        public async Task<IActionResult> AddPayment(string sKodu,string sOdemeSekli, string lTutar)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {

                    string query = $@"exec Cari_Odeme @sKodu='{sKodu}' , @CariIslemKodu='{sOdemeSekli}' , @Tutar={lTutar}
 ";

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
