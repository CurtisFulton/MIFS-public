﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    // Temp for testing
    [ApiController]
    [Route("[controller]/[action]")]
    public class ManagementController : ControllerBase
    {
        public ManagementController(IntegrationHostManager integrationHostManager, ILogger<ManagementController> logger)
        {
            this.IntegrationManager = integrationHostManager;
            this.Logger = logger;
        }

        public IntegrationHostManager IntegrationManager { get; }
        public ILogger<ManagementController> Logger { get; }

        [HttpPost]
        public async Task<IActionResult> StartIntegration(string name, CancellationToken cancellationToken)
        {
            await this.IntegrationManager.TryStartHost(name, cancellationToken);
            return Ok("Started");
        }

        [HttpPost]
        public async Task<IActionResult> StopIntegration(string name, CancellationToken cancellationToken)
        {
            await this.IntegrationManager.StopHost(name);
            return Ok("Shut down");
        }

        [HttpPost]
        public async Task<IActionResult> RestartIntegration(string name, int count, CancellationToken cancellationToken)
        {
            for (int i = 0; i < count; i++)
            {
                await this.IntegrationManager.TryStartHost(name, cancellationToken);
                await this.IntegrationManager.StopHost(name);

                this.Logger.LogInformation("Completed loop {loopCount}", i);
            }
            return Ok("Shut down");
        }
    }
}
