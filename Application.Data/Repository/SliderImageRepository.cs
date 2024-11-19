using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class SliderImageRepository : RepositoryBase<SliderImage>, ISliderImageRepository
        {
        public SliderImageRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface ISliderImageRepository : IRepository<SliderImage>
    {
        
    }
}