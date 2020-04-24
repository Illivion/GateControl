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
            _tcpServer.Push();

            var dt = DateTime.Now;

            return Ok($"Accepted on {dt:dd.MM.yy HH:mm:ss}");
        }

        [HttpPost]
        [Route("gate/push")]
        public void PushPost()
        {
            _tcpServer.Push();
        }
    }
}