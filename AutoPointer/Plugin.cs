using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Logging;
using Dalamud.Game.Gui;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using AutoPointer.Attributes;
using AutoPointer.Windows;
using AutoPointer.Utils;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace AutoPointer
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Auto Pointer";
        private const string CommandName = "/pointer";

        //private PlayerCharacter playerCharacter { get; init; }
        private ChatGui ChatGui { get; init; }
        //private readonly ClientState clientState;
        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public static CommandHelper CommandHelper { get; private set; } = new CommandHelper();
        public static EmoteHandler EmoteHandler { get; private set; } = new EmoteHandler();
        public WindowSystem WindowSystem = new("Auto Pointer");

        public Plugin(
            DalamudPluginInterface pluginInterface,
            CommandManager commandManager,
            ChatGui chatGui
            )
        {
            // API access
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.ChatGui = chatGui;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);


            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(new MainWindow(this, goatImage));

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Be rude and point at people."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            args = args.ToLower();
            //this.ChatGui.Print($"command: {command}");
            //this.ChatGui.Print($"args: {args}");
            
            switch(args.Split(' ')[0])
            {
                case "start":
                case "on":
                    this.ChatGui.Print("Starting");
                    //continueEmoteLoop = true;
                    EmoteHandler.EnableLoop();
                    EmoteHandler.LoopEmote("/point motion", 1000);
                    break;
                case "stop":
                case "off":
                    this.ChatGui.Print("Stopping");
                    //continueEmoteLoop = false;
                    EmoteHandler.DisableLoop();
                    break;
                default:
                    WindowSystem.GetWindow("AutoPointer Window").IsOpen = true;
                    break;
            }
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            WindowSystem.GetWindow("AutoPointer Config").IsOpen = true;
        }
    }
}
