using Application.Model.Models;
using Application.ViewModel;
using AutoMapper;
using System;

namespace Application.Web.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Product, ProductViewModel>();
        }
    }
}