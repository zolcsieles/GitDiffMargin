﻿#region using

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace GitDiffMargin
{
    [Export(typeof (IWpfTextViewMarginProvider))]
    [Name(EditorDiffMargin.MarginNameConst)]
    [Order(Before = PredefinedMarginNames.LineNumber)]
    [MarginContainer(PredefinedMarginNames.LeftSelection)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class EditorMarginFactory : MarginFactoryBase
    {
        public override IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            var marginCore = GetMarginCore(textViewHost);
            return new EditorDiffMargin(textViewHost.TextView, marginCore);
        }
    }
}