# UniSplice.Core

The heart of the UniSplice Modding Framework for Android Unity games.

UniSplice.Core is the first purpose-built mod loading framework for Unity Mono games on Android. Where other solutions are ports of PC tools fighting the platform, UniSplice is designed from the ground up for Android, and it shows.

---

## What is UniSplice.Core?

UniSplice.Core is split into two components that work together:

**UniSplice.Preloader** is injected early into the Unity game lifecycle. It prepares the environment before Unity fully loads, to ensure stable mod loading.

**UniSplice** takes over after Unity is ready. It discovers, loads, and manages mods, presenting its own loading screen before handing control back to the game.

Together, they give mod developers a stable, consistent environment to build against, regardless of which supported Unity version the target game was built with.

---

## Platform Support

UniSplice.Core currently targets Unity Mono games only. IL2CPP support is planned for a future release.

| Backend | Status |
|---------|--------|
| Mono | ✅ |
| IL2CPP | ❌ |

**Unity version compatibility:**

| Range | Status |
|-------|--------|
| Unity 2017.x - 6000.x | Should be supported |
| Unity 5.x | Unlikely, untested |
| Unity 2021 | Confirmed working |

Mono is less common in modern Android Unity titles, but remains widespread in older and indie games - exactly the gap UniSplice exists to fill. Your mileage may vary depending on the targetted game.

---

## Why UniSplice?

Android Unity modding has been largely ignored. Existing solutions like LemonLoader are ports of PC tools that have varying levels of stability and only target IL2CPP. UniSplice was built specifically for Android, starting from the questions that actually matter on the platform:

- How does an Android Unity game load native libraries?
- How do you hook into the Mono runtime on ARM?
- How do you get your own C# code running inside a game you did not build?

The result is a framework that works cleanly and should have high stability.

---

## For Mod Developers

UniSplice exposes a straightforward API for mod developers. Your mod is a compiled C# assembly that UniSplice discovers and loads at runtime.

A basic mod entry point looks like this:

```csharp
using UniSplice;

namespace ModNamespace {

    [ModInfo("MyModName", "my.mod.guid", "1.0.0")]
    public class MyMod : UniSpliceMod
    {
        public override void ModAwake()
        {
            UniSpliceLog.Info("My Mod loaded!");
        }
    }
}
```

You can also check out the Example Mod: https://github.com/UniSplice/SampleMod

Place your compiled `.dll` in the game's `mods/` folder and UniSplice will pick it up on the next launch.

With UniSpliceMod, you can easily use everything a normal Unity MonoBehaviour would, as if the game was built for this.

The only difference is that Mods do not have a `void Awake()`, it's always `override void ModAwake()`.

---

## Requirements

- An Android device running a Unity Mono game patched with [UniSplice.Patcher](https://github.com/UniSplice/UniSplice.Patcher)
- Storage permission granted to the target game (UniSplice should request this automatically)
- For development: .NET targeting .NET Standard 2.0 or lower is recommended for broadest compatibility

---

## Related Repositories

| Repository | Description |
|------------|-------------|
| [UniSplice.Patcher](https://github.com/UniSplice/UniSplice.Patcher) | Patches a Unity APK to install UniSplice |
| [UniSplice.NativeMono](https://github.com/UniSplice/UniSplice.NativeMono) | Native C++ library handling Mono runtime hooking |

---

## License

UniSplice.Core is licensed under the GNU Lesser General Public License v2.1.

See [LICENSE](LICENSE) for full details.

---

## Contributing

Contributions are welcome. Any changes will have to be verified through a Pull request.

You must however provide your own Unity Libraries from a Unity 2018-2021 game when building, these can be placed inside the `lib/` folder.

Note that the project targets .NET 3.5 subset compatibility for maximum Unity version support.
Avoid APIs not available in that environment, and document clearly if a feature requires a newer runtime, thus, please develop on Windows, as this makes it easier to retain full compatibility, unless you know that your operating system can develop accurate outputs.

---

## Acknowledgements

UniSplice builds on the shoulders of the broader Android and Unity modding communities. Special mention to the teams behind ShadowHook, APKTool, and the Mono project.