﻿using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Install.Models;
using Umbraco.Cms.Core.Telemetry;
using Umbraco.New.Cms.Core.Models.Installer;

namespace Umbraco.New.Cms.Core.Installer.Steps;

public class TelemetryIdentifierStep : InstallSetupStep
{
    private readonly IOptions<GlobalSettings> _globalSettings;
    private readonly ISiteIdentifierService _siteIdentifierService;

    public TelemetryIdentifierStep(
        IOptions<GlobalSettings> globalSettings,
        ISiteIdentifierService siteIdentifierService)
        : base(
            "TelemetryIdConfiguration",
            20,
            InstallationType.NewInstall | InstallationType.Upgrade)
    {
        _globalSettings = globalSettings;
        _siteIdentifierService = siteIdentifierService;
    }

    public override Task ExecuteAsync(InstallData model)
    {
        _siteIdentifierService.TryCreateSiteIdentifier(out _);
        return Task.FromResult<InstallSetupResult?>(null);
    }

    public override Task<bool> RequiresExecutionAsync(InstallData model)
    {
        // Verify that Json value is not empty string
        // Try & get a value stored in appSettings.json
        var backofficeIdentifierRaw = _globalSettings.Value.Id;

        // No need to add Id again if already found
        return Task.FromResult(string.IsNullOrEmpty(backofficeIdentifierRaw));
    }
}