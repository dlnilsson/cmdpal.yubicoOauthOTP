// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;


namespace otp;


internal sealed partial class otpSettingsPage : ListPage
{
    public otpSettingsPage() { }
    public override IListItem[] GetItems()
    {
        var item = new ListItem(SettingsManager.Instance.Settings.SettingsPage)
        {
            Title = "Open settings",
            Subtitle = "You need to set the path to your ykman first",
        };
        IsLoading = false;
        return [item];
    }
}

internal sealed partial class otpPage : ListPage
{
    private string _cachedOutput;
    private DateTime _lastCacheUpdate;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(10);

    public otpPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\Yubico.png");
        Title = "OTP";
        Name = "Generate codes from OATH accounts stored on the YubiKey.";
        IsLoading = true;
        ShowDetails = true;

    }

    public override IListItem[] GetItems()
    {
        var YkmanPath = SettingsManager.Instance.YkmanPath;
        if (string.IsNullOrEmpty(YkmanPath))
        {
            var item = new ListItem(SettingsManager.Instance.Settings.SettingsPage)
            {
                Title = "Open settings",
                Subtitle = "You need to set the path to your ykman first",
            };
            IsLoading = false;
            return [item];
        }
        var Device = SettingsManager.Instance.Device;
        if (string.IsNullOrEmpty(Device))
        {
            var item = new ListItem(SettingsManager.Instance.Settings.SettingsPage)
            {
                Title = "Open settings",
                Subtitle = "You need to set your yubikey device id",
            };
            IsLoading = false;
            return [item];
        }

        try
        {
            var accounts = GetAccounts(YkmanPath, Device);
            IsLoading = false;
            return accounts
                .Select(CreateResult)
                .ToArray();
        }
        catch (Exception ex)
        {
            var item = new ListItem(new CopyTextCommand(ex.Message))
            {
                Title = $"Error: {ex.Message}",
                Subtitle = "Make sure ykman is installed and in your PATH",
            };
            IsLoading = false;
            return [item];
        }
    }
    private ListItem CreateResult(Account account)
    {
        return new ListItem(new CopyTextCommand(account.Code))
        {
            Title = account.Name,
            Subtitle = account.Code,
        };
    }

    private List<Account> GetAccounts(string YkmanPath, string Device)
    {
        var args = "";
        if (Device != null)
        {
            args += $"--device {Device} ";
        }
        args += "oath accounts code";

        return ParseYkmanOutput(RunCommand(YkmanPath, args));
    }

    public class Account
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
    public static List<Account> ParseYkmanOutput(string output)
    {
        var accounts = new List<Account>();

        foreach (var line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {

            var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                var code = parts.Last();
                var name = string.Join(" ", parts.Take(parts.Length - 1));
                accounts.Add(new Account { Name = name.Trim(), Code = code.Trim() });
            }
        }

        return accounts;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "<Pending>")]
    public string RunCommand(string fileName, string arguments)
    {
        if (_cachedOutput != null && DateTime.Now - _lastCacheUpdate < _cacheDuration)
        {
            return _cachedOutput;
        }
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!process.WaitForExit(5000))
            {
                process.Kill();
                throw new TimeoutException("The command timed out.");
            }

            if (process.ExitCode != 0)
            {
                throw new Exception($"Command failed with exit code {process.ExitCode}: {error}");
            }

            _cachedOutput = output;
            _lastCacheUpdate = DateTime.Now;

            return output;
        }
        catch (TimeoutException ex)
        {
            throw new Exception($"The CLI command timed out. Please ensure the Yubico CLI is installed and accessible. {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error running CLI command: {fileName} {arguments} {ex.Message}");
        }
    }
}
