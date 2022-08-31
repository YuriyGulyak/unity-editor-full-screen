#if UNITY_EDITOR_WIN

using System.Diagnostics;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
    public class EdotorFullScreen
    {
        // Will be reset when the editor is played or stopped.
        private static bool _needRestoreState = true;

        // https://docs.unity3d.com/ScriptReference/MenuItem.html
        [MenuItem("Editor/Toggle Screen Mode %SPACE", false, -1001)]
        private static void ToggleScreenMode()
        {
            if (_needRestoreState)
            {
                _needRestoreState = false;
                RestoreState();
            }

            if (WinApi.FullscreenMode)
                WinApi.RestoreWindow(CurrentProcessWindowHandle);
            else
                WinApi.MaximizeWindow(CurrentProcessWindowHandle);

            BackupState();
        }

        private static IntPtr CurrentProcessWindowHandle =>
            Process.GetCurrentProcess().MainWindowHandle;

        private static readonly string StateFilePath =
            Path.Combine(Application.persistentDataPath, "EdotorFullScreenState");

        private static void BackupState()
        {
            using BinaryWriter writer = new BinaryWriter(File.Open(StateFilePath, FileMode.Create));

            writer.Write(WinApi.FullscreenMode);
            writer.Write(WinApi.InitialRect.Left);
            writer.Write(WinApi.InitialRect.Top);
            writer.Write(WinApi.InitialRect.Right);
            writer.Write(WinApi.InitialRect.Bottom);
            writer.Write(WinApi.InitialStyle);
        }

        private static void RestoreState()
        {
            if (!File.Exists(StateFilePath))
                return;

            using BinaryReader reader = new BinaryReader(File.Open(StateFilePath, FileMode.Open));

            WinApi.FullscreenMode = reader.ReadBoolean();
            WinApi.InitialRect = new RECT
            {
                Left = reader.ReadInt32(),
                Top = reader.ReadInt32(),
                Right = reader.ReadInt32(),
                Bottom = reader.ReadInt32()
            };
            WinApi.InitialStyle = reader.ReadInt32();
        }
    }
}

#endif