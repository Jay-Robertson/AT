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
namespace Mavo.Assets.Controllers
{
    public partial class AssetController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected AssetController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult AssetPickerForJob()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AssetPickerForJob);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult AssetPickerDetail()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AssetPickerDetail);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult RemoveAsset()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.RemoveAsset);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult AddAsset()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AddAsset);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult UpdateQuantity()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.UpdateQuantity);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ScanItem()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ScanItem);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public AssetController Actions { get { return MVC.Asset; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Asset";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Asset";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string AssetPickerForJob = "AssetPickerForJob";
            public readonly string AssetPickerDetail = "AssetPickerDetail";
            public readonly string RemoveAsset = "RemoveAsset";
            public readonly string AddAsset = "AddAsset";
            public readonly string UpdateQuantity = "UpdateQuantity";
            public readonly string Index = "Index";
            public readonly string Details = "Details";
            public readonly string Create = "Create";
            public readonly string Edit = "Edit";
            public readonly string Scan = "Scan";
            public readonly string ScanItem = "ScanItem";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string AssetPickerForJob = "AssetPickerForJob";
            public const string AssetPickerDetail = "AssetPickerDetail";
            public const string RemoveAsset = "RemoveAsset";
            public const string AddAsset = "AddAsset";
            public const string UpdateQuantity = "UpdateQuantity";
            public const string Index = "Index";
            public const string Details = "Details";
            public const string Create = "Create";
            public const string Edit = "Edit";
            public const string Scan = "Scan";
            public const string ScanItem = "ScanItem";
        }


        static readonly ActionParamsClass_AssetPickerForJob s_params_AssetPickerForJob = new ActionParamsClass_AssetPickerForJob();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_AssetPickerForJob AssetPickerForJobParams { get { return s_params_AssetPickerForJob; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_AssetPickerForJob
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_AssetPickerDetail s_params_AssetPickerDetail = new ActionParamsClass_AssetPickerDetail();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_AssetPickerDetail AssetPickerDetailParams { get { return s_params_AssetPickerDetail; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_AssetPickerDetail
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_RemoveAsset s_params_RemoveAsset = new ActionParamsClass_RemoveAsset();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_RemoveAsset RemoveAssetParams { get { return s_params_RemoveAsset; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_RemoveAsset
        {
            public readonly string id = "id";
            public readonly string jobId = "jobId";
        }
        static readonly ActionParamsClass_AddAsset s_params_AddAsset = new ActionParamsClass_AddAsset();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_AddAsset AddAssetParams { get { return s_params_AddAsset; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_AddAsset
        {
            public readonly string id = "id";
            public readonly string jobId = "jobId";
        }
        static readonly ActionParamsClass_UpdateQuantity s_params_UpdateQuantity = new ActionParamsClass_UpdateQuantity();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_UpdateQuantity UpdateQuantityParams { get { return s_params_UpdateQuantity; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_UpdateQuantity
        {
            public readonly string id = "id";
            public readonly string quantity = "quantity";
        }
        static readonly ActionParamsClass_Details s_params_Details = new ActionParamsClass_Details();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Details DetailsParams { get { return s_params_Details; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Details
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Edit s_params_Edit = new ActionParamsClass_Edit();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Edit EditParams { get { return s_params_Edit; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Edit
        {
            public readonly string id = "id";
            public readonly string asset = "asset";
        }
        static readonly ActionParamsClass_ScanItem s_params_ScanItem = new ActionParamsClass_ScanItem();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ScanItem ScanItemParams { get { return s_params_ScanItem; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ScanItem
        {
            public readonly string scan = "scan";
        }
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
                public readonly string _AssetPicker = "_AssetPicker";
                public readonly string _AssetPickerDetail = "_AssetPickerDetail";
                public readonly string _AssetRow = "_AssetRow";
                public readonly string _AssetSidebar = "_AssetSidebar";
                public readonly string Edit = "Edit";
                public readonly string Index = "Index";
                public readonly string Scan = "Scan";
            }
            public readonly string _AssetPicker = "~/Views/Asset/_AssetPicker.cshtml";
            public readonly string _AssetPickerDetail = "~/Views/Asset/_AssetPickerDetail.cshtml";
            public readonly string _AssetRow = "~/Views/Asset/_AssetRow.cshtml";
            public readonly string _AssetSidebar = "~/Views/Asset/_AssetSidebar.cshtml";
            public readonly string Edit = "~/Views/Asset/Edit.cshtml";
            public readonly string Index = "~/Views/Asset/Index.cshtml";
            public readonly string Scan = "~/Views/Asset/Scan.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_AssetController : Mavo.Assets.Controllers.AssetController
    {
        public T4MVC_AssetController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult AssetPickerForJob(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AssetPickerForJob);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult AssetPickerDetail(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AssetPickerDetail);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult RemoveAsset(int id, int jobId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.RemoveAsset);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "jobId", jobId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult AddAsset(int id, int jobId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AddAsset);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "jobId", jobId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult UpdateQuantity(int id, int quantity)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.UpdateQuantity);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "quantity", quantity);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Details(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Details);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Create()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Create);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Edit);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Scan()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Scan);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ScanItem(Mavo.Assets.Models.ViewModel.AssetScanPostModel scan)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ScanItem);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "scan", scan);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(Mavo.Assets.Models.ViewModel.AssetPostModel asset)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Edit);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "asset", asset);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
