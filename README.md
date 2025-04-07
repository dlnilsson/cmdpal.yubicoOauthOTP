# PowerToys Command Palette – YubiKey OTP Extension

An extension for the [Windows PowerToys Command Palette](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/overview) that displays **OATH TOTP codes** stored on your **YubiKey**.

This is the spiritual successor to [`Community.PowerToys.Run.Plugin.YubicoOauthOTP`](https://github.com/dlnilsson/Community.PowerToys.Run.Plugin.YubicoOauthOTP), now rebuilt for the new PowerToys Command Palette system.

---

## Features

- Lists OTP codes stored on your YubiKey using `ykman oath accounts code`
- Copy a code to clipboard with one click
- Configurable YubiKey path and device ID
- Auto-refreshing token list (with smart caching)
- Integrates seamlessly into the Command Palette UI

---

##  Requirements

- [PowerToys](https://github.com/microsoft/PowerToys) with **Command Palette enabled**
- [YubiKey Manager CLI (`ykman`)](https://developers.yubico.com/yubikey-manager/)
- A YubiKey with OATH credentials



