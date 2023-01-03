using AutoPointer.Utils;
using System;
using System.Threading.Tasks;

namespace AutoPointer
{
    public class EmoteHandler
    {
        public static CommandHelper CommandHelper { get; private set; } = new CommandHelper();

        private bool continueEmoteLoop = false;

        public void EnableLoop()
        {
            continueEmoteLoop = true;
        }
        public void DisableLoop()
        {
            continueEmoteLoop = false;
        }

        public void LoopEmote(string emoteCommand, int delay)
        {
            // Sleeping the thread will obviously make XIV crash
            Task.Run(async () =>
            {
                while (continueEmoteLoop)
                {
                    CommandHelper.SendChatMessage(emoteCommand);
                    await Task.Delay(delay);
                }
            });
        }
    }

}
