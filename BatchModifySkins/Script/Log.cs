using System;
using System.Diagnostics;
using UnityEngine;

namespace BatchModifyStyles
{
    public static class Log
    {
        public static bool EnableDebug = false; // 设置 true 可开启详细日志

        [Conditional("DEBUG")]
        public static void Info(string message)
        {
            if (EnableDebug)
                Debug.Log($"[BatchModifyStyles] {message}");
        }

        public static void Warn(string message)
        {
            Debug.LogWarning($"[BatchModifyStyles] {message}");
        }

        public static void Error(string message)
        {
            Debug.LogError($"[BatchModifyStyles] {message}");
        }
    }
}
