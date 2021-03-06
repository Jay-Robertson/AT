// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace T4MVC
{
    public class SharedController
    {

        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _JobHeader = "_JobHeader";
                public readonly string _Layout = "_Layout";
                public readonly string _LayoutWithNoConstraints = "_LayoutWithNoConstraints";
                public readonly string _LoginPartial = "_LoginPartial";
                public readonly string _TopNav = "_TopNav";
                public readonly string Error = "Error";
            }
            public readonly string _JobHeader = "~/Views/Shared/_JobHeader.cshtml";
            public readonly string _Layout = "~/Views/Shared/_Layout.cshtml";
            public readonly string _LayoutWithNoConstraints = "~/Views/Shared/_LayoutWithNoConstraints.cshtml";
            public readonly string _LoginPartial = "~/Views/Shared/_LoginPartial.cshtml";
            public readonly string _TopNav = "~/Views/Shared/_TopNav.cshtml";
            public readonly string Error = "~/Views/Shared/Error.cshtml";
            static readonly _DisplayTemplatesClass s_DisplayTemplates = new _DisplayTemplatesClass();
            public _DisplayTemplatesClass DisplayTemplates { get { return s_DisplayTemplates; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _DisplayTemplatesClass
            {
                public readonly string Address = "Address";
                public readonly string AssetKind = "AssetKind";
                public readonly string DateTime = "DateTime";
                public readonly string JobStatus = "JobStatus";
            }
            static readonly _EditorTemplatesClass s_EditorTemplates = new _EditorTemplatesClass();
            public _EditorTemplatesClass EditorTemplates { get { return s_EditorTemplates; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _EditorTemplatesClass
            {
                public readonly string Address = "Address";
                public readonly string DateTime = "DateTime";
                public readonly string @decimal = "decimal";
                public readonly string String = "String";
            }
        }
    }

}

#endregion T4MVC
#pragma warning restore 1591
