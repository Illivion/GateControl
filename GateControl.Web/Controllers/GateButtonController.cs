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

        private static DateTime? _lastAccess;

        private static Boolean _suspended = false;

        public GateButtonController(ILogger<GateButtonController> logger, TcpServerService tcpServer)
        {
            _logger = logger;
            _tcpServer = tcpServer;
        }

        [HttpGet]
        [Route("status")]
        public StatusResult Status()
        {
            return new StatusResult()
            {
                IsDeviceConnected = _tcpServer.IsDeviceConnected()
            };
        }

        [HttpPost]
        [Route("gate/suspend")]
        public IActionResult SuspendControl()
        {
            _suspended = true;

            return Ok("acknowledged");
        }

        [HttpPost]
        [Route("gate/resume")]
        public IActionResult ResumeControl()
        {
            _suspended = false;

            return Ok("acknowledged");
        }

        [HttpGet]
        [Route("gate/push")]
        public IActionResult Push()
        {
            if (_suspended)
            {
                return StatusCode(400, "suspended");
            }

            var sentToDevice = _tcpServer.Push();

            var dt = DateTime.Now;

            var lastAccessString = _lastAccess == null ? "Never" : _lastAccess.Value.ToString("dd.MM.yy HH:mm:ss");

            _lastAccess = dt;

            return base.Content($"<h1>Accepted on {dt:dd.MM.yy HH:mm:ss}. Sent: {sentToDevice}. Previous: {lastAccessString}</h1>");
        }

        [HttpPost]
        [Route("gate/push")]
        public IActionResult PushPost([FromQuery]String key)
        {
            if (_suspended)
            {
                return StatusCode(400, "suspended");
            }

            if (String.IsNullOrEmpty(key))
            {
                return StatusCode(401);
            }

            if (key == "fg849cp1")
            {
                return Ok();
            }

            if (key != "sp3219px")
            {
                return StatusCode(403);
            }

            _lastAccess = DateTime.Now;

            _tcpServer.Push();

            return Ok();
        }
    }
}