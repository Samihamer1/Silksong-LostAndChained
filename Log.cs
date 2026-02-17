using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained
{
    public static class Log
    {
        private static bool debug = false;
        public static void Info(object message)
        {
            if (LostAndChainedPlugin.Instance != null)
            {
                LostAndChainedPlugin.Instance.LogInfo(message);
            }
        }

        public static void Error(object message)
        {
            if (LostAndChainedPlugin.Instance != null)
            {
                LostAndChainedPlugin.Instance.LogError(message);
            }
        }

        public static void Debug(object message)
        {
            if (LostAndChainedPlugin.Instance != null && debug)
            {
                LostAndChainedPlugin.Instance.LogDebug(message);
            }
        }

        public static void Warn(object message)
        {
            if (LostAndChainedPlugin.Instance != null)
            {
                LostAndChainedPlugin.Instance.LogWarn(message);
            }
        }
    }
}
