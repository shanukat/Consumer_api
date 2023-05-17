using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using tsapi.Util;

namespace tsapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _loggerr;
        private readonly TransactionService _trans;
        
        public TransactionController(ILogger<TransactionController> loggerr, TransactionService trans)
        {
            _loggerr = loggerr;
            _trans = trans;
        }



        

        [HttpGet(Name = "GetTransactions")]
        public async Task<IActionResult> Get(string UniqueIdentityKey, int year, int month)
        {
            CancellationTokenSource token = new();
            var list = await _trans.GetTransactionAsync(UniqueIdentityKey, year, month, token.Token);
           
            if (list.TransactionList.Count > 0)
            {
                return Ok(list);
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }





    }
}
