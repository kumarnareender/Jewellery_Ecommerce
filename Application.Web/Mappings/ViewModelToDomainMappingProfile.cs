using Application.Model.Models;
using Application.ViewModel;
using AutoMapper;

namespace Application.Web.Mappings
{

    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {            
        }
    }
}