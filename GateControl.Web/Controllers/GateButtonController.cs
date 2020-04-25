using System;
using GateControl.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GateControl.Web.Controllers
{
    [ApiController]
    public class GateButtonController : ControllerBase
    {
        private readonly ILogger<GateButtonController> _logger;
        private readonly TcpServerService _tcpServer;

        public GateButtonController(ILogger<GateButtonController> logger, TcpServerService tcpServer)
        {
            _logger = logger;
            _tcpServer = tcpServer;
        }

        [HttpGet]
        [Route("gate/push")]
        public IActionResult Push()
        {
            var sentToDevice = _tcpServer.Push();

            var dt = DateTime.Now;

            return Ok($"Accepted on {dt:dd.MM.yy HH:mm:ss}. Sent: {sentToDevice}");
        }

        [HttpPost]
        [Route("gate/push")]
        public IActionResult PushPost([FromQuery]String key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return StatusCode(401);
            }

            if (key != "fg849cp1")
            {
                return Forbid();
            }

            _tcpServer.Push();

            return Ok();
        }
    }
}