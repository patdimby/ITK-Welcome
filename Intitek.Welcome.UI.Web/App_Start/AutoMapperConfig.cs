using AutoMapper;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Service.Front;
using Intitek.Welcome.UI.ViewModels;

namespace Intitek.Welcome.UI.Web
{
    public class AutoMapperConfig
    {
        public static IMapper Mapper { get; private set; }
        public static void RegisterMappings()
        {
            var mapperConfiguration = new MapperConfiguration(ConfigureAutoMapper);
            mapperConfiguration.AssertConfigurationIsValid();

            Mapper = mapperConfiguration.CreateMapper();
        }
        private static void ConfigureAutoMapper(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<DocumentDTO, DocumentViewModel>()
                .ForMember(vm => vm.Date, o => o.Ignore())
                .ForMember(vm => vm.Commentaire, o => o.Ignore())
                .ForMember(vm => vm.ContentType, o => o.Ignore())
                .ForMember(vm => vm.TypeAffectation, o => o.Ignore())
                .ForMember(vm => vm.Data, o => o.Ignore())
                .ForMember(vm => vm.Link, o => o.Ignore())
                .ForMember(vm => vm.FiltreRead, o => o.Ignore())
                .ForMember(vm => vm.FiltreApproved, o => o.Ignore())                
                .ForMember(vm => vm.FiltreTested, o => o.Ignore());
            

            configuration.CreateMap<DocumentCategoryDTO, CategoryViewModel>();
            configuration.CreateMap<DocumentSubCategoryDTO, SubCategoryViewModel>();
            configuration.CreateMap<IntitekUser, UserViewModel>()
                .ForMember(vm => vm.CompanyLogo, o => o.Ignore())
                .ForMember(vm => vm.NomPrenom, o => o.Ignore());
            
        }
    }
}