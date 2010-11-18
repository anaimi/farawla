﻿#pragma checksum "..\..\..\..\Features\Completion\Widget.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C843581157115A27876C79FA8C8A0482"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Farawla.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Farawla.Features.Completion {
    
    
    /// <summary>
    /// Widget
    /// </summary>
    public partial class Widget : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\Features\Completion\Widget.xaml"
        internal System.Windows.Controls.TextBlock NoCompletionSettings;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\..\Features\Completion\Widget.xaml"
        internal System.Windows.Controls.StackPanel CompletionSettings;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\Features\Completion\Widget.xaml"
        internal System.Windows.Controls.CheckBox CompletionState;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\Features\Completion\Widget.xaml"
        internal System.Windows.Controls.StackPanel FrameworksContainer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Farawla;component/features/completion/widget.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Features\Completion\Widget.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.NoCompletionSettings = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.CompletionSettings = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.CompletionState = ((System.Windows.Controls.CheckBox)(target));
            
            #line 15 "..\..\..\..\Features\Completion\Widget.xaml"
            this.CompletionState.Click += new System.Windows.RoutedEventHandler(this.CompletionStateChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.FrameworksContainer = ((System.Windows.Controls.StackPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
