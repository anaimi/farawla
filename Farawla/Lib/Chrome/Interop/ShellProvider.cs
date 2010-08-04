namespace Standard.Interop
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IIDGuid.IID_TaskbarList),
    ]
    internal interface ITaskbarList
    {
        void HrInit();
        void AddTab([In] IntPtr hwnd);
        void DeleteTab([In] IntPtr hwnd);
        void ActivateTab([In] IntPtr hwnd);
        void SetActiveAlt([In] IntPtr hwnd);
    }

    [
        ComImport,
        TypeLibType(TypeLibTypeFlags.FCanCreate),
        ClassInterface(ClassInterfaceType.None),
        Guid(CLSIDGuid.CLSID_TaskbarList),
    ]
    internal class TaskbarListRcw : ITaskbarList
    {
        #region ITaskbarList Members

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void HrInit();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void AddTab(IntPtr hwnd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void DeleteTab(IntPtr hwnd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ActivateTab(IntPtr hwnd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetActiveAlt(IntPtr hwnd);

        #endregion
    }
}
