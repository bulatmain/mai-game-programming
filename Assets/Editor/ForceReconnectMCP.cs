#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CircleRacersBattle.EditorTools
{

    [InitializeOnLoad]
    internal static class ForceReconnectMCP
    {
        private const string m_MCPAssemblyName = "MCPForUnity.Editor";
        private const string m_ServiceLocatorTypeName = "MCPForUnity.Editor.Services.MCPServiceLocator";

        static ForceReconnectMCP()
        {
            EditorApplication.delayCall += () =>
            {
                try { TryReconnect(); }
                catch (Exception e) { Debug.Log($"[ForceReconnectMCP] Initial reconnect skipped: {e.Message}"); }
            };

            EditorApplication.playModeStateChanged += _ =>
            {
                try { TryReconnect(); }
                catch {  }
            };
        }

        [MenuItem("Tools/MCP/Force Reconnect Bridge")]
        private static void ForceReconnectMenu()
        {
            try
            {
                TryReconnect(forceLog: true);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ForceReconnectMCP] Manual reconnect failed: {e}");
            }
        }

        private static void TryReconnect(bool forceLog = false)
        {
            Type locator = FindServiceLocator();
            if (locator == null)
            {
                if (forceLog) Debug.LogWarning("[ForceReconnectMCP] MCPServiceLocator not found. Is the MCP for Unity package installed?");
                return;
            }

            object bridge = GetStaticProperty(locator, "Bridge");
            object server = GetStaticProperty(locator, "Server");
            if (bridge == null || server == null)
            {
                if (forceLog) Debug.LogWarning("[ForceReconnectMCP] Bridge or Server service is null.");
                return;
            }

            bool reachable = InvokeBool(server, "IsLocalHttpServerReachable", defaultValue: false);
            bool isRunning = GetBoolProperty(bridge, "IsRunning", defaultValue: false);

            if (forceLog)
            {
                Debug.Log($"[ForceReconnectMCP] state: isRunning={isRunning}, reachable={reachable}");
            }

            if (!isRunning && reachable)
            {
                Debug.Log("[ForceReconnectMCP] HTTP server reachable but bridge not running. Re-starting bridge...");
                InvokeStartAsync(bridge);
            }
            else if (forceLog)
            {
                Debug.Log("[ForceReconnectMCP] No reconnect needed.");
            }
        }

        private static Type FindServiceLocator()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!asm.GetName().Name.StartsWith("MCPForUnity", StringComparison.Ordinal)) continue;
                Type t = asm.GetType(m_ServiceLocatorTypeName, throwOnError: false, ignoreCase: false);
                if (t != null) return t;
            }
            return Type.GetType($"{m_ServiceLocatorTypeName}, {m_MCPAssemblyName}");
        }

        private static object GetStaticProperty(Type t, string name)
        {
            PropertyInfo p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
            return p?.GetValue(null);
        }

        private static bool GetBoolProperty(object instance, string name, bool defaultValue)
        {
            PropertyInfo p = instance.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (p == null || p.PropertyType != typeof(bool)) return defaultValue;
            object v = p.GetValue(instance);
            return v is bool b ? b : defaultValue;
        }

        private static bool InvokeBool(object instance, string method, bool defaultValue)
        {
            MethodInfo m = instance.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (m == null || m.ReturnType != typeof(bool)) return defaultValue;
            object v = m.Invoke(instance, null);
            return v is bool b ? b : defaultValue;
        }

        private static void InvokeStartAsync(object bridge)
        {
            MethodInfo m = bridge.GetType().GetMethod("StartAsync", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (m == null)
            {
                Debug.LogWarning("[ForceReconnectMCP] Bridge.StartAsync() not found.");
                return;
            }

            object result = m.Invoke(bridge, null);
            if (result is Task<bool> tb)
            {
                tb.ContinueWith(t =>
                {
                    if (t.IsFaulted) Debug.LogWarning($"[ForceReconnectMCP] StartAsync faulted: {t.Exception?.GetBaseException().Message}");
                    else Debug.Log($"[ForceReconnectMCP] StartAsync completed: {t.Result}");
                }, TaskScheduler.Default);
            }
            else if (result is Task t)
            {
                t.ContinueWith(x =>
                {
                    if (x.IsFaulted) Debug.LogWarning($"[ForceReconnectMCP] StartAsync faulted: {x.Exception?.GetBaseException().Message}");
                    else Debug.Log("[ForceReconnectMCP] StartAsync completed.");
                }, TaskScheduler.Default);
            }
        }
    }
}
#endif
