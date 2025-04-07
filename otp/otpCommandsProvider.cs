// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Collections.Generic;
using System.IO;
using Windows.UI.ApplicationSettings;

namespace otp;

public partial class otpCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public otpCommandsProvider()
    {
        DisplayName = "ykman OTP";
        Icon = IconHelpers.FromRelativePath("Assets\\Yubico Logo Small (PNG).png");
        _commands = [
            new CommandItem(new otpPage()) { Title = DisplayName},
            new CommandItem(new otpSettingsPage()) 
            { 
                Title = "ykman otp settings",
                Subtitle = "set custom ykman path or device id",
            },

        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Sample code")]
public class SettingsManager : JsonSettingsManager
{
    public static SettingsManager Instance { get; } = new();

    private readonly TextSetting _ykmanPath = new(
        nameof(YkmanPath),
        "Path to your ykman",
        "if ykamn is in your $PATH, simply type ykman",
        string.Empty);

    public string YkmanPath => _ykmanPath.Value;

    private readonly TextSetting _device = new(
        nameof(Device),
        "Device ID",
        "if you have multiple yubikeys connected at once ",
        string.Empty);

    public string Device => _device.Value;

    internal static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("com.dlnilsson.ykman-extension");
        Directory.CreateDirectory(directory);

        return Path.Combine(directory, "settings.json");
    }

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();

        Settings.Add(_ykmanPath);
        Settings.Add(_device);

        LoadSettings();

        Settings.SettingsChanged += (s, e) => SaveSettings();
    }
}