using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionReader.Data;
using TransactionReader.Models;
using TransactionReader.Utilities;
using TransactionReader.Utilities.FileParser;

namespace TransactionReader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransationContext _context;
        private readonly IMapper _mapper;

        public TransactionsController(TransationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public string GetAll()
        {
            return MapTransactionsToJson(_context.Transactions);
        }

        [HttpGet]
        [Route("Currency")]
        public IActionResult GetByCurrency([FromQuery] string currency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(currency))
            {
                ModelState.AddModelError("Currency", "No parameter.");
                return BadRequest(ModelState);
            }

            if (!CurrencyISOCodeInfo.ContainsCurrencyCode(currency))
            {
                ModelState.AddModelError("Currency", "Currency Code parameter 'currency' is not correct. Please use ISO 4217 formats.");
                return BadRequest(ModelState);
            }

            var transactions = _context.Transactions.Where(t => t.CurrencyCode == currency);
            var json = MapTransactionsToJson(transactions);
            return Ok(json);
        }

        [HttpGet]
        [Route("DateRange")]
        public IActionResult GetByDateRange([FromQuery] string startDate, [FromQuery] string endDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
            {
                ModelState.AddModelError("DateRange", "No 'startDate' or 'endDate' parameter.");
                return BadRequest(ModelState);
            }

            var standartApiDateFormat = "yyyy-MM-dd";
            DateTime dateFrom;
            DateTime dateTo;

            if (!DateTime.TryParseExact(startDate, standartApiDateFormat,
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out dateFrom))
            {
                ModelState.AddModelError("DateRange", "The 'startDate' parameter has wrong format. Please use standard date format 'yyyy-MM-dd'.");
                return BadRequest(ModelState);
            }

            if (!DateTime.TryParseExact(endDate, standartApiDateFormat,
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out dateTo))
            {
                ModelState.AddModelError("DateRange", "The 'endDate' parameter has wrong format. Please use standard date format 'yyyy-MM-dd'.");
                return BadRequest(ModelState);
            }

            var transactions = _context.Transactions.Where(t => t.TransactionDate >= dateFrom && t.TransactionDate <= dateTo);
            var json = MapTransactionsToJson(transactions);
            return Ok(json);
        }

        [HttpGet]
        [Route("Status")]
        public IActionResult GetByStatus([FromQuery] string status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(status))
            {
                ModelState.AddModelError("Status", "No parameter.");
                return BadRequest(ModelState);
            }

            string[] permittedStatuses = 
            {
                TransactionStatus.StatusCode.A.ToString(),
                TransactionStatus.StatusCode.R.ToString(),
                TransactionStatus.StatusCode.D.ToString(),
            };
            if (!permittedStatuses.Contains(status))
            {
                ModelState.AddModelError("Status", "Wrong 'status' parameter. Please use one of the following: 'A', 'R', or 'D' (A - Approved, R - Failed/Rejected, D - Finished/Done).");
                return BadRequest(ModelState);
            }

            var transactions = _context.Transactions.Where(t => t.Status == status);
            var json = MapTransactionsToJson(transactions);
            return Ok(json);
        }

        /// <summary>
        /// Map Transaction Db model to JSON output result.
        /// </summary>
        private string MapTransactionsToJson(IEnumerable<Transaction> transactions)
        {
            var mappedTransactions = _mapper.Map<IEnumerable<TransactionDTO>>(transactions);
            var json = JsonSerializer.Serialize<IEnumerable<TransactionDTO>>(mappedTransactions);
            return json;
        }
    }
}
