﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebChromiumCcsipro.UI.VirtualKeyboard
{
    /// <summary>
    /// Contains internal RCWs for invoking the InputPane (tiptsf touch keyboard)
    /// </summary>
    /// <remarks>
    /// Adapted from https://github.com/Microsoft/WPF-Samples/blob/master/Input%20and%20Commands/TouchKeyboard/TouchKeyboardNotifier/InputPaneRcw.cs
    /// Licensed under an MIT license see https://github.com/Microsoft/WPF-Samples/blob/master/LICENSE
    /// </remarks>
    internal static class InputPaneRcw
    {
        internal enum TrustLevel
        {
            BaseTrust,
            PartialTrust,
            FullTrust
        }

        [Guid("75CF2C57-9195-4931-8332-F0B409E916AF"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        internal interface IInputPaneInterop
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetIids(out uint iidCount, [MarshalAs(UnmanagedType.LPStruct)] out Guid iids);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetRuntimeClassName([MarshalAs(UnmanagedType.BStr)] out string className);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetTrustLevel(out TrustLevel TrustLevel);

            [MethodImpl(MethodImplOptions.InternalCall)]
            IInputPane2 GetForWindow([In] IntPtr appWindow, [In] ref Guid riid);
        }

        [Guid("8A6B3F26-7090-4793-944C-C3F2CDE26276"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        internal interface IInputPane2
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetIids(out uint iidCount, [MarshalAs(UnmanagedType.LPStruct)] out Guid iids);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetRuntimeClassName([MarshalAs(UnmanagedType.BStr)] out string className);

            [MethodImpl(MethodImplOptions.InternalCall)]
            void GetTrustLevel(out TrustLevel TrustLevel);

            [MethodImpl(MethodImplOptions.InternalCall)]
            bool TryShow();

            [MethodImpl(MethodImplOptions.InternalCall)]
            bool TryHide();
        }
    }
}
