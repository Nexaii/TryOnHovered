<div align="center">
  <h1>Try On Hovered Item</h1>
  <h3>A SimpleTweaks tweak</h3>
  <p>
    Hover any gear, hold a modifier, right-click = instant try-on.
    <br />
    <a href="#install">Install</a> · <a href="#how-it-works">How it works</a> · <a href="#building">Building</a>
  </p>
</div>

A small custom tweak for [SimpleTweaksPlugin](https://github.com/Caraxi/SimpleTweaksPlugin). Written at someone's request from the puni.sh discord. It loads as an external tweak. There's a possibility I'll create yet another `"bundle of tweaks"` type of plugin in the future, then simply include this.

There's a similar tweak of the same nature that's been in a PR for SimpleTweaks for over 6 months as of writing this. So, I figured I'd write my own to help anyone else out that desires it.

## Install

1. Build it (see [Building](#building)) or grab the prebuilt `TryOnHovered.dll`.
2. In game: `/tweaks` → open the config window.
3. Scroll to **General Options** tab then down to **Tweak Providers** → press **+**.
4. Paste the full path to `TryOnHovered.dll` and confirm, ie `C:\Tweaks\TryOnHovered.dll`.
5. Find **Try On Hovered Item** in the tweak list and enable it.

## How it works

Hover any weapon, armor piece, or accessory and press **Ctrl + Right Click** to try it on. No context menu needed.

- The modifier key is configurable (Ctrl, Alt, or Shift). Right Click is always part of the trigger.
- Only equippable gear responds. Belts, soul crystals, and off hands that aren't shields are ignored.
- Works anywhere an item shows a tooltip: inventory, character window, vendors, market board, search results.

### Hiding the context menu

A right click also tells the game to open its item context menu. After a `try on` fires, the tweak watches for that menu over the next few frames and closes it the moment it appears, so you never see it. The window is short, so menus you open normally afterwards are left alone.

## Building

If you prefer to build this yourself, no problem! You need two things on disk to compile:

1. **`SimpleTweaksPlugin.dll`** the compiled SimpleTweaks assembly. The project references it (see `TryOnHovered.csproj`); point `SimpleTweaksAssembly` at your copy.
2. **A Dalamud library folder** supplies `Dalamud.dll`, `FFXIVClientStructs.dll`, `Lumina.dll`, `Lumina.Excel.dll`, and the ImGui bindings. Pass its path as `DalamudLibPath`.

Then:

```
dotnet build TryOnHovered.csproj -c Release /p:DalamudLibPath=<path-to-dalamud-lib-folder>
```

The dll then lands in `bin/Release/TryOnHovered.dll` for use.
