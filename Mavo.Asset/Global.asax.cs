using Mavo.Assets.Models;
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



            AutoMapper.Mapper.CreateMap<EditJobPostModel, Job>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.SubmittedBy, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<Job, EditJobPostModel>()
                .ForMember(dest => dest.ProjectManagerId, opt => opt.MapFrom(src => src.ProjectManager.Id))
                .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(src => src.ProjectManager.FullName))
                .ForMember(dest => dest.ForemanId, opt => opt.MapFrom(src => src.Foreman.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.IsAddon, opt => opt.MapFrom(src => src is JobAddon))
                .ForMember(dest => dest.ReturnedByStr, opt => opt.MapFrom(src => src.ReturnedBy.FullName))
                .ForMember(dest => dest.PickedUpByStr, opt => opt.MapFrom(src => src.PickedBy.FullName));
            AutoMapper.Mapper.CreateMap<AssetPostModel, Asset>();
            AutoMapper.Mapper.CreateMap<Job, JobAddon>()
                .ForMember(x => x.ParentJob, opt => opt.MapFrom(src => src))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name + " [Add On]"))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => JobStatus.New))
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.PickupTime, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(x => x.PickStarted, opt => opt.Ignore())
                .ForMember(x => x.PickCompleted, opt => opt.Ignore());

            ///HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

        }
    }
}