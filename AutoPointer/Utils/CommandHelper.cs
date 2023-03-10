using System;
using System.Runtime.InteropServices;
using System.Text;

using Dalamud.Utility.Signatures;

using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;

namespace AutoPointer.Utils
{
    // Borrowed from: https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/Game/SigHelper.cs
    // and https://github.com/thakyZ/EmoteCmdComplex-dalamud/tree/main/EmoteCmdComplex
    public unsafe class CommandHelper : IDisposable
    {
        private static class Signatures
        {
            internal const string SendChatMessage = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9";
            internal const string SanitizeChatString = "E8 ?? ?? ?? ?? EB 0A 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 8D";
        }

        /***** functions *****/
        [Signature(Signatures.SanitizeChatString, Fallibility = Fallibility.Fallible)]
        private readonly delegate* unmanaged<Utf8String*, int, IntPtr, void> _sanitizeChatString = null!;

        // UIModule, message, unused, byte
        [Signature(Signatures.SendChatMessage, Fallibility = Fallibility.Fallible)]
        private readonly delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, void> _processChatBoxEntry = null!;

        internal CommandHelper()
        {
            SignatureHelper.Initialise(this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SendChatMessage(string message)
        {
            if (this._processChatBoxEntry is null)
            {
                throw new InvalidOperationException("Signature for ProcessChatBoxEntry/SendMessage not found!");
            }

            var messageBytes = Encoding.UTF8.GetBytes(message);

            switch (messageBytes.Length)
            {
                case 0:
                    throw new ArgumentException("Message cannot be empty", nameof(message));
                case > 500:
                    throw new ArgumentException("Message exceeds 500char limit", nameof(message));
            }

            var payloadMem = Marshal.AllocHGlobal(400);
            Marshal.StructureToPtr(new ChatPayload(messageBytes), payloadMem, false);

            this._processChatBoxEntry((IntPtr)Framework.Instance()->GetUiModule(), payloadMem, IntPtr.Zero, 0);

            Marshal.FreeHGlobal(payloadMem);
        }
    }
}
