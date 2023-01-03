using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AutoPointer.Windows;

public class PointerWindow : Window, IDisposable
{
    //private Configuration Configuration;

    public PointerWindow(Plugin plugin) : base(
        "Pointer Window")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text("Do pointy bits here i guess");

        //if (ImGui.Button("Show Settings"))
        //{
        //    this.Plugin.DrawConfigUI();
        //}
        if(ImGui.Button("Button one"))
        {

        }
        if(ImGui.Button("Button two"))
        {

        }

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        ImGui.Indent(55);
        ImGui.Text("jk idk how any of this works");
        ImGui.Unindent(55);
    }
}
