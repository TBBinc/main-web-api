using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SiteWebApi.Interfaces;
using Cassandra;
using Microsoft.AspNetCore.Http;
using SiteWebApi.Models;
using uPLibrary.Networking.M2Mqtt;
using Newtonsoft.Json;

namespace SiteWebApi.Controllers
{
    [ApiController]
    public class InformationController : ControllerBase
    {
        private readonly ILogger<InformationController> _logger;

        private IDataStaxService _service;
        private ISolaceService _solaceService;

        public InformationController(ILogger<InformationController> logger, IDataStaxService service, ISolaceService solaceService)
        {
            _logger = logger;
            _service = service;
            _solaceService = solaceService;
        }

        [ProducesResponseType(200)]     // OK
        [ProducesResponseType(401)]     // Unauthorized
        [HttpGet]
        [Route("api/conncection")]
        public IActionResult CheckConnection()
        {
            try
            {
                if (_service.Session != null)
                {
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                };
            }
            catch (Exception ex)
            {
                var res = new JsonResult(ex.Message);
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }
        }

        [HttpPost]
        [Route("api/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterVaccineDose([FromBody]VaccineDoseModel patient){

            var ps = _service.Session.Prepare("INSERT INTO users.vaccinegiven (userhash, siteid, vaccine, timegiven) VALUES (?, ?, ?, ?)");
            
            var statement = ps.Bind(patient.UserHash, patient.SiteId, patient.Vaccine, DateTime.Now);

            var rs = await _service.Session.ExecuteAsync(statement);
            return Ok();
        }

        [HttpPost]
        [Route("api/checkdose")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
         public async Task<IActionResult> CheckVaccineDose([FromBody]VaccineDoseModel patient){
             var ps = _service.Session.Prepare("SELECT userhash FROM users.vaccinegiven WHERE userhash = ?");
            
            var statement = ps.Bind(patient.UserHash);

            var rs = await _service.Session.ExecuteAsync(statement);

            try{
                var result = rs.FirstOrDefault().GetValue<string>("userhash");
                var isPatientVaccine = result != null;
                return new JsonResult(isPatientVaccine);
            }
            catch{
                return new JsonResult(false);
            }
        }

        [HttpPost]
        [Route("api/sendsignal")]
         public IActionResult SendVaccineSignal([FromBody]SignalModel signal){

            _solaceService.PublishMessage(JsonConvert.SerializeObject(signal));
            
            return Ok();
        }

    }
}
