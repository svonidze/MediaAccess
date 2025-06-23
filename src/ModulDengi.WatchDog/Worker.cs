namespace ModulDengi.WatchDog
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Exceptions;
    using Common.Serialization.Json;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;

    public class Worker : BackgroundService
    {
        private readonly TimeSettings confirmationCheckTimeSettings;
        
        private readonly TimeSettings newProjectCheckTimeSettings;
        
        private readonly ILogger<Worker> logger;

        private readonly IModulDengiClient client;

        private readonly IConfirmationManager confirmationManager;

        private readonly IAccountant accountant;

        public Worker(
            ILogger<Worker> logger,
            IOptionsMonitor<TimeSettings> optionsSnapshot,
            IModulDengiClient client,
            IConfirmationManager confirmationManager,
            IAccountant accountant)
        {
            this.logger = logger;
            this.confirmationCheckTimeSettings = optionsSnapshot.Get(nameof(this.confirmationCheckTimeSettings));
            this.newProjectCheckTimeSettings = optionsSnapshot.Get(nameof(this.newProjectCheckTimeSettings));
            
            this.client = client;
            this.confirmationManager = confirmationManager;
            this.accountant = accountant;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation($"{this.GetType().Name} started at {DateTimeOffset.Now}");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation($"{this.GetType().Name} stopped at {DateTimeOffset.Now}");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!this.client.IsApiAvailable(out var reason))
                {
                    this.logger.LogWarning($"Cannot make any API call since {reason}. " 
                        + $"Waiting {this.newProjectCheckTimeSettings.CheckInterval} for the next check");
                    await Task.Delay(this.newProjectCheckTimeSettings.CheckInterval, stoppingToken);
                    continue;
                }
                
                try
                {
                    this.logger.LogDebug("Checking for new projects.");
                    var projects = await this.client.GetProjectsRisingFunds();
                    if (projects.Any())
                    {
                        var notInvestedYetProjects = projects.Where(p => p.MyInvestmentAmount == 0).ToArray();
                        this.logger.LogDebug(
                            $"Got {projects.Length} projects where {notInvestedYetProjects.Length} are not invested by me.");
                        foreach (var project in notInvestedYetProjects)
                        {
                            this.logger.LogInformation($"Run investing for project {project.ToJson()}");
                            await this.OnProjectsRisingFundsDetected(stoppingToken, project);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogCritical(e, e.GetShortDescription("Unhandled exception"));
                }

                this.logger.LogDebug($"No new projects yet, waiting {this.newProjectCheckTimeSettings.CheckInterval} for the next check");
                await Task.Delay(this.newProjectCheckTimeSettings.CheckInterval, stoppingToken);
            }
        }

        private async Task OnProjectsRisingFundsDetected(CancellationToken stoppingToken, Project project)
        {
            var myAvailableMoney = await this.client.GetMyFreeMoneyAmount();

            if (!this.accountant.IsPossibleToInvest(
                myAvailableMoney,
                project,
                out double investingMoneyAmount,
                out string reason))
            {
                this.logger.LogWarning($"Cannot invest into project {project.ToJson()} due to {reason}");
                return;
            }
            this.logger.LogInformation($"{investingMoneyAmount} is approved to be invested into project {project.ToJson()}.");

            var startTime = DateTime.UtcNow;
            var newInvestmentResponse = await this.client.StartInvestmentFlow(project, investingMoneyAmount);
            if (!newInvestmentResponse.Success)
            {
                this.logger.LogCritical(
                    $"{nameof(this.client.StartInvestmentFlow)} was not finished successfully, due to {newInvestmentResponse.Message}");
                return;
            }

            this.logger.LogDebug($"Waiting {this.confirmationCheckTimeSettings.CheckInterval} before first checking confirmations with {this.confirmationManager.GetType().Name}.");
            await Task.Delay(this.confirmationCheckTimeSettings.CheckInterval, stoppingToken);

            string confirmationCode;

            var stopwatch = Stopwatch.StartNew();
            var attempt = 0;
            while (!this.confirmationManager.CheckConfirmationReceived(startTime, project.Number, out confirmationCode))
            {
                attempt++;
                
                if (stopwatch.Elapsed > this.confirmationCheckTimeSettings.TimeOut)
                {
                    stopwatch.Stop();
                    this.logger.LogCritical(
                        $"Confirmation was not received after {attempt}, breaking processing after {stopwatch.Elapsed} by timeout");
                    return;
                }

                this.logger.LogDebug($"Waiting {this.confirmationCheckTimeSettings.CheckInterval} for the next confirmation check");
                await Task.Delay(this.confirmationCheckTimeSettings.CheckInterval, stoppingToken);
            }

            stopwatch.Stop();

            if (confirmationCode == null)
            {
                this.logger.LogCritical($"{nameof(confirmationCode)} is null, not allowed to continue processing.");
                return;
            }

            var confirmInvestmentResult = await this.client.ConfirmInvestment(
                newInvestmentResponse.Value.Id,
                confirmationCode);

            void Log(string message)
            {
                if (confirmInvestmentResult.Success)
                {
                    this.logger.LogInformation(message);
                }
                else
                {
                    this.logger.LogError(message);
                }
            }

            Log($"Investment to {project.ToJson()} was{(confirmInvestmentResult.Success ? null : "not")} done. " 
                + $"Message: {confirmInvestmentResult.Message}");
        }
    }
}