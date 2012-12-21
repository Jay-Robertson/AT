﻿using Mavo.Assets.Models;
using Mavo.Assets.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using Mavo.Assets.Binders;

namespace Mavo.Assets
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();



            AutoMapper.Mapper.CreateMap<EditJobPostModel, Job>();
            AutoMapper.Mapper.CreateMap<Job, EditJobPostModel>()
                .ForMember(dest => dest.ProjectManagerId, opt => opt.MapFrom(src => src.ProjectManager.Id))
                .ForMember(dest => dest.ForemanId, opt => opt.MapFrom(src => src.Foreman.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.ReturnedByStr, opt => opt.MapFrom(src => src.ReturnedBy.FullName))
                .ForMember(dest => dest.PickedUpByStr, opt => opt.MapFrom(src => src.PickedBy.FullName));
            AutoMapper.Mapper.CreateMap<AssetPostModel, Asset>();

            //HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            //ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            //ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
        }
    }
}