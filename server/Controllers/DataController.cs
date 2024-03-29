﻿using cmd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataController : ControllerBase
    {
        [Authorize]
        [HttpGet("data")]
        public async Task<IActionResult> SendData()
        {
            return Ok(SimulatorLogger.logEntries);
        }
    }
}