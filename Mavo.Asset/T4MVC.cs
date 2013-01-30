﻿// <auto-generated />
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

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public static class MVC
{
    public static Mavo.Assets.Controllers.AccountController Account = new Mavo.Assets.Controllers.T4MVC_AccountController();
    public static Mavo.Assets.Controllers.AssetController Asset = new Mavo.Assets.Controllers.T4MVC_AssetController();
    public static Mavo.Assets.Controllers.BaseController Base = new Mavo.Assets.Controllers.T4MVC_BaseController();
    public static Mavo.Assets.Controllers.HomeController Home = new Mavo.Assets.Controllers.T4MVC_HomeController();
    public static Mavo.Assets.Controllers.JobController Job = new Mavo.Assets.Controllers.T4MVC_JobController();
    public static Mavo.Assets.Controllers.JobPickerController JobPicker = new Mavo.Assets.Controllers.T4MVC_JobPickerController();
    public static Mavo.Assets.Controllers.JobReturnerController JobReturner = new Mavo.Assets.Controllers.T4MVC_JobReturnerController();
    public static Mavo.Assets.Controllers.ReportingController Reporting = new Mavo.Assets.Controllers.T4MVC_ReportingController();
    public static Mavo.Assets.Controllers.TemplateController Template = new Mavo.Assets.Controllers.T4MVC_TemplateController();
    public static Mavo.Assets.Controllers.UserManagementController UserManagement = new Mavo.Assets.Controllers.T4MVC_UserManagementController();
    public static T4MVC.EmailsController Emails = new T4MVC.EmailsController();
    public static T4MVC.SharedController Shared = new T4MVC.SharedController();
}

namespace T4MVC
{
}

namespace T4MVC
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class Dummy
    {
        private Dummy() { }
        public static Dummy Instance = new Dummy();
    }
}

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public class T4MVC_System_Web_Mvc_ActionResult : System.Web.Mvc.ActionResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_ActionResult(string area, string controller, string action, string protocol = null): base()
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
     
    public override void ExecuteResult(System.Web.Mvc.ControllerContext context) { }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}
[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public class T4MVC_System_Web_Mvc_PartialViewResult : System.Web.Mvc.PartialViewResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_PartialViewResult(string area, string controller, string action, string protocol = null): base()
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}
[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public class T4MVC_System_Web_Mvc_JsonResult : System.Web.Mvc.JsonResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_JsonResult(string area, string controller, string action, string protocol = null): base()
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}



namespace Links
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class Scripts {
        private const string URLPATH = "~/Scripts";
        public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
        public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
        public static readonly string _references_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/_references.min.js") ? Url("_references.min.js") : Url("_references.js");
                      
        public static readonly string bootstrap_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/bootstrap.min.js") ? Url("bootstrap.min.js") : Url("bootstrap.js");
                      
        public static readonly string bootstrap_min_js = Url("bootstrap.min.js");
        public static readonly string jquery_1_7_2_intellisense_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-1.7.2.intellisense.min.js") ? Url("jquery-1.7.2.intellisense.min.js") : Url("jquery-1.7.2.intellisense.js");
                      
        public static readonly string jquery_1_7_2_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-1.7.2.min.js") ? Url("jquery-1.7.2.min.js") : Url("jquery-1.7.2.js");
                      
        public static readonly string jquery_1_7_2_min_js = Url("jquery-1.7.2.min.js");
        public static readonly string jquery_TimePicker_1_0_0_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-TimePicker-1.0.0.min.js") ? Url("jquery-TimePicker-1.0.0.min.js") : Url("jquery-TimePicker-1.0.0.js");
                      
        public static readonly string jquery_ui_1_8_20_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-ui-1.8.20.min.js") ? Url("jquery-ui-1.8.20.min.js") : Url("jquery-ui-1.8.20.js");
                      
        public static readonly string jquery_ui_1_8_20_min_js = Url("jquery-ui-1.8.20.min.js");
        public static readonly string jquery_unobtrusive_ajax_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.unobtrusive-ajax.min.js") ? Url("jquery.unobtrusive-ajax.min.js") : Url("jquery.unobtrusive-ajax.js");
                      
        public static readonly string jquery_unobtrusive_ajax_min_js = Url("jquery.unobtrusive-ajax.min.js");
        public static readonly string jquery_validate_vsdoc_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate-vsdoc.min.js") ? Url("jquery.validate-vsdoc.min.js") : Url("jquery.validate-vsdoc.js");
                      
        public static readonly string jquery_validate_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.min.js") ? Url("jquery.validate.min.js") : Url("jquery.validate.js");
                      
        public static readonly string jquery_validate_min_js = Url("jquery.validate.min.js");
        public static readonly string jquery_validate_unobtrusive_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.unobtrusive.min.js") ? Url("jquery.validate.unobtrusive.min.js") : Url("jquery.validate.unobtrusive.js");
                      
        public static readonly string jquery_validate_unobtrusive_min_js = Url("jquery.validate.unobtrusive.min.js");
        public static readonly string knockout_2_1_0_debug_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/knockout-2.1.0.debug.min.js") ? Url("knockout-2.1.0.debug.min.js") : Url("knockout-2.1.0.debug.js");
                      
        public static readonly string knockout_2_1_0_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/knockout-2.1.0.min.js") ? Url("knockout-2.1.0.min.js") : Url("knockout-2.1.0.js");
                      
        public static readonly string modernizr_2_5_3_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/modernizr-2.5.3.min.js") ? Url("modernizr-2.5.3.min.js") : Url("modernizr-2.5.3.js");
                      
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class Content {
        private const string URLPATH = "~/Content";
        public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
        public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
        public static readonly string bootstrap_responsive_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/bootstrap-responsive.min.css") ? Url("bootstrap-responsive.min.css") : Url("bootstrap-responsive.css");
             
        public static readonly string bootstrap_responsive_min_css = Url("bootstrap-responsive.min.css");
        public static readonly string bootstrap_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/bootstrap.min.css") ? Url("bootstrap.min.css") : Url("bootstrap.css");
             
        public static readonly string bootstrap_min_css = Url("bootstrap.min.css");
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class images {
            private const string URLPATH = "~/Content/images";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string glyphicons_halflings_white_png = Url("glyphicons-halflings-white.png");
            public static readonly string glyphicons_halflings_png = Url("glyphicons-halflings.png");
        }
    
        public static readonly string print_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/print.min.css") ? Url("print.min.css") : Url("print.css");
             
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class themes {
            private const string URLPATH = "~/Content/themes";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public static class bootstrap {
                private const string URLPATH = "~/Content/themes/bootstrap";
                public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
                public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
                [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
                public static class images {
                    private const string URLPATH = "~/Content/themes/bootstrap/images";
                    public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
                    public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
                    public static readonly string ui_bg_flat_0_aaaaaa_40x100_png = Url("ui-bg_flat_0_aaaaaa_40x100.png");
                    public static readonly string ui_bg_glass_55_fbf9ee_1x400_png = Url("ui-bg_glass_55_fbf9ee_1x400.png");
                    public static readonly string ui_bg_glass_65_ffffff_1x400_png = Url("ui-bg_glass_65_ffffff_1x400.png");
                    public static readonly string ui_bg_glass_75_dadada_1x400_png = Url("ui-bg_glass_75_dadada_1x400.png");
                    public static readonly string ui_bg_glass_75_e6e6e6_1x400_png = Url("ui-bg_glass_75_e6e6e6_1x400.png");
                    public static readonly string ui_bg_glass_75_ffffff_1x400_png = Url("ui-bg_glass_75_ffffff_1x400.png");
                    public static readonly string ui_bg_highlight_soft_75_cccccc_1x100_png = Url("ui-bg_highlight-soft_75_cccccc_1x100.png");
                    public static readonly string ui_bg_inset_soft_95_fef1ec_1x100_png = Url("ui-bg_inset-soft_95_fef1ec_1x100.png");
                    public static readonly string ui_icons_222222_256x240_png = Url("ui-icons_222222_256x240.png");
                    public static readonly string ui_icons_2e83ff_256x240_png = Url("ui-icons_2e83ff_256x240.png");
                    public static readonly string ui_icons_454545_256x240_png = Url("ui-icons_454545_256x240.png");
                    public static readonly string ui_icons_888888_256x240_png = Url("ui-icons_888888_256x240.png");
                    public static readonly string ui_icons_cd0a0a_256x240_png = Url("ui-icons_cd0a0a_256x240.png");
                    public static readonly string ui_icons_f6cf3b_256x240_png = Url("ui-icons_f6cf3b_256x240.png");
                }
            
                public static readonly string jquery_ui_1_8_16_custom_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-ui-1.8.16.custom.min.css") ? Url("jquery-ui-1.8.16.custom.min.css") : Url("jquery-ui-1.8.16.custom.css");
                     
                public static readonly string jquery_ui_1_8_16_ie_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.ui.1.8.16.ie.min.css") ? Url("jquery.ui.1.8.16.ie.min.css") : Url("jquery.ui.1.8.16.ie.css");
                     
            }
        
        }
    
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static partial class bundles
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static partial class scripts {}
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static partial class styles {}
    }
}

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public static class T4MVCHelpers {
    // You can change the ProcessVirtualPath method to modify the path that gets returned to the client.
    // e.g. you can prepend a domain, or append a query string:
    //      return "http://localhost" + path + "?foo=bar";
    private static string ProcessVirtualPathDefault(string virtualPath) {
        // The path that comes in starts with ~/ and must first be made absolute
        string path = VirtualPathUtility.ToAbsolute(virtualPath);
        
        // Add your own modifications here before returning the path
        return path;
    }

    // Calling ProcessVirtualPath through delegate to allow it to be replaced for unit testing
    public static Func<string, string> ProcessVirtualPath = ProcessVirtualPathDefault;


    // Logic to determine if the app is running in production or dev environment
    public static bool IsProduction() { 
        return (HttpContext.Current != null && !HttpContext.Current.IsDebuggingEnabled); 
    }
}





#endregion T4MVC
#pragma warning restore 1591


