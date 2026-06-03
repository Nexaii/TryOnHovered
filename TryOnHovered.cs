using System;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using SimpleTweaksPlugin;
using SimpleTweaksPlugin.Events;
using SimpleTweaksPlugin.TweakSystem;
using SimpleTweaksPlugin.Utility;

namespace TryOnTweak;

[TweakName("Try On Hovered Item")]
[TweakDescription("Hold a modifier and right-click a hovered item to try it on.")]
[TweakAuthor("Nexai")]
[TweakKey("TryOnHovered")]
public unsafe class TryOnHovered : Tweak {
    private const int ContextMenuSuppressionFrames = 5;

    private static readonly VirtualKey[] Modifiers = [VirtualKey.CONTROL, VirtualKey.MENU, VirtualKey.SHIFT];

    public class Configs : TweakConfig {
        public VirtualKey ModifierKey = VirtualKey.CONTROL;
    }

    public Configs Config { get; private set; }

    private bool rightButtonHeld;
    private int contextMenuSuppressionFrames;

    protected override void Enable() {
        Config = LoadConfig<Configs>() ?? new Configs();
        rightButtonHeld = false;
        contextMenuSuppressionFrames = 0;
    }

    protected override void Disable() => SaveConfig(Config);

    [FrameworkUpdate]
    private void OnFrameworkUpdate() {
        if (contextMenuSuppressionFrames > 0) {
            contextMenuSuppressionFrames--;
            CloseContextMenu();
        }

        var down = NativeKeyState.IsKeyDown(VirtualKey.RBUTTON);
        if (down == rightButtonHeld) return;
        rightButtonHeld = down;

        if (!down || !Service.KeyState[Config.ModifierKey]) return;
        if (!TryResolveHoveredGear(out var itemId)) return;

        try {
            AgentTryon.TryOn(0, itemId);
            contextMenuSuppressionFrames = ContextMenuSuppressionFrames;
        } catch (Exception ex) {
            Plugin.Error(this, ex);
        }
    }

    private static void CloseContextMenu() {
        if (Common.GetUnitBase("ContextMenu", out var addon) && addon->IsVisible)
            addon->Hide(true, false, 0);
    }

    private static bool TryResolveHoveredGear(out uint itemId) {
        itemId = 0;
        var hovered = Service.GameGui.HoveredItem;
        if (hovered is 0 or >= 2_000_000) return false;

        var id = (uint)(hovered % 1_000_000);
        if (Service.Data.Excel.GetSheet<Item>().GetRowOrDefault(id) is not { } item || !IsTryOnable(item)) return false;

        itemId = id;
        return true;
    }

    private static bool IsTryOnable(Item item) {
        var slot = item.EquipSlotCategory.RowId;
        return slot is not (0 or 6 or 17)
            && (slot != 2 || item.FilterGroup == 3)
            && PlayerState.Instance()->Race != 0;
    }

    protected void DrawConfig(ref bool hasChanged) {
        ImGui.SetNextItemWidth(120 * ImGui.GetIO().FontGlobalScale);
        if (ImGui.BeginCombo("##tryOnModifier", Config.ModifierKey.GetKeyName())) {
            foreach (var modifier in Modifiers)
                if (ImGui.Selectable(modifier.GetKeyName(), modifier == Config.ModifierKey) && modifier != Config.ModifierKey)
                    hasChanged = (Config.ModifierKey = modifier) == modifier;
            ImGui.EndCombo();
        }

        ImGui.SameLine();
        ImGui.Text("+ Right Click  to try on a hovered item");
    }
}
