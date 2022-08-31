using AutoMapper;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;

namespace Intitek.Welcome.UI.Web.Areas.Admin
{
    public class AutoMapperConfigAdmin
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
                .ForMember(vm => vm.NameCategory, o => o.MapFrom(m => m.DocumentCategoryLang != null ? m.DocumentCategoryLang.Name : string.Empty))
                .ForMember(vm => vm.DocumentTitle, o => o.Ignore())
                .ForMember(vm => vm.IdLang, o => o.Ignore())
                .ForMember(vm => vm.ContentType, o => o.Ignore())
                .ForMember(vm => vm.IdQcm, o => o.Ignore())
                .ForMember(vm => vm.IsMajor, o => o.Ignore())
                .ForMember(vm => vm.IdUser, o => o.Ignore())
                .ForMember(vm => vm.Categories, o => o.Ignore())
                .ForMember(vm => vm.Langues, o => o.Ignore())
                .ForMember(vm => vm.Versions, o => o.Ignore())
                .ForMember(vm => vm.Qcms, o => o.Ignore())
                .ForMember(vm => vm.Profiles, o => o.Ignore())
                .ForMember(vm => vm.Entites, o => o.Ignore())
                .ForMember(vm => vm.Data, o => o.Ignore())
                .ForMember(vm => vm.Actions, o => o.Ignore())
                .ForMember(vm => vm.Affectation, o => o.Ignore())
                .ForMember(vm => vm.CreatedBy, o => o.Ignore())
                .ForMember(vm => vm.CreationDate, o => o.Ignore())
                .ForMember(vm => vm.ModifiedBy, o => o.Ignore())
                .ForMember(vm => vm.ModificationDate, o => o.Ignore())
                .ForMember(vm => vm.DeletedBy, o => o.Ignore())
                .ForMember(vm => vm.DeletionDate, o => o.Ignore())
                .ForMember(vm => vm.CategorySubCategory, o => o.Ignore())
                .ForMember(vm => vm.CategorySubcategories, o => o.Ignore());


            configuration.CreateMap<DocumentCategoryDTO, CategoryViewModel>()
                .ForMember(vm => vm.NbDocuments, o => o.Ignore())
                .ForMember(vm => vm.SubCategories, o => o.Ignore())
                .ForMember(vm => vm.IsDeleted, o => o.Ignore())
                .ForMember(vm => vm.DefaultName, o => o.Ignore());

            configuration.CreateMap<CategoryDTO, CategoryViewModel>()
                .ForMember(vm => vm.NbDocuments, o => o.Ignore())
                .ForMember(vm => vm.SubCategories, o => o.Ignore())
                .ForMember(vm => vm.DefaultName, o => o.Ignore());

            configuration.CreateMap<DocumentSubCategoryDTO, SubCategoryViewModel>()
                .ForMember(vm => vm.ID_Category, o => o.Ignore())
                .ForMember(vm => vm.ID_OldCategory, o => o.Ignore())
                .ForMember(vm => vm.Categories, o => o.Ignore())
                .ForMember(vm => vm.NbDocuments, o => o.Ignore())
                .ForMember(vm => vm.Ordre, o => o.Ignore())
                .ForMember(vm => vm.IsDeleted, o => o.Ignore())
                .ForMember(vm => vm.DefaultName, o => o.Ignore());

            configuration.CreateMap<ProfilDTO, ProfilViewModel>();
            configuration.CreateMap<UserDTO, UserViewModel>()
                .ForMember(vm => vm.IsRoot, o => o.Ignore());

            configuration.CreateMap<QcmDTO, QcmViewModel>()
                .ForMember(vm => vm.CodeLangue, o => o.Ignore())
                .ForMember(vm => vm.DefaultTradName, o => o.Ignore());

            configuration.CreateMap<HistoActionsDTO, ChangesViewModel>();

            configuration.CreateMap<HistoUserQcmDocVersionDTO, HistoUserQcmViewModel>();

            configuration.CreateMap<HistoEmailsDTO, HistoEmailsViewModel>();

            configuration.CreateMap<HistoBatchsDTO, HistoBatchsViewModel>();

            configuration.CreateMap<MailTemplateDTO, MailTemplateViewModel>()
                .ForMember(vm => vm.MailKeywords, o => o.Ignore())
                .ForMember(vm => vm.Categories, o => o.Ignore())
                .ForMember(vm => vm.SelectedCategories, o => o.Ignore())
                .ForMember(vm => vm.SelectedSubCategories, o => o.Ignore())
                .ForMember(vm => vm.SubCategories, o => o.Ignore())
                .ForMember(vm => vm.CategorySubCategories, o => o.Ignore());
      
            configuration.CreateMap<ADDTO, ADViewModel>()
                .ForMember(vm => vm.ConfirmPassword, o => o.Ignore());
            configuration.CreateMap<BlackListDTO, BlackListViewModel>()
                .ForMember(vm => vm.ID, o => o.Ignore());
            configuration.CreateMap<CityEntityBlacklistDTO, CityEntityBlacklistViewModel>();
        }
    }
}