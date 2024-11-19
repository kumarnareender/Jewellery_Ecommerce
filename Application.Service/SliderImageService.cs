using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface ISliderImageService
    {
        void CreateSliderImage(SliderImage sliderImage);
        IEnumerable<SliderImage> GetSliderImages();
        SliderImage GetSliderImage(string imageName);
        bool DeleteSliderImage(string imageName);
        void Commit();
    }

    public class SliderImageService : ISliderImageService
    {
        private readonly ISliderImageRepository sliderImageRepository;
        private readonly IUnitOfWork unitOfWork;

        public SliderImageService(ISliderImageRepository sliderImageRepository, IUnitOfWork unitOfWork)
        {
            this.sliderImageRepository = sliderImageRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region ISliderImageService Members

        public void CreateSliderImage(SliderImage sliderImage)
        {
            sliderImageRepository.Add(sliderImage);
            Commit();
        }

        public IEnumerable<SliderImage> GetSliderImages()
        {
            var sliderImages = sliderImageRepository.GetAll().OrderBy(r => r.DisplayOrder).ToList();
            return sliderImages;
        }

        public SliderImage GetSliderImage(string imageName)
        {
            var productImage = sliderImageRepository.Get(r => r.ImageName == imageName);
            return productImage;
        }

        public bool DeleteSliderImage(string imageName)
        {
            try
            {
                var sliderImage = GetSliderImage(imageName);
                sliderImageRepository.Delete(sliderImage);
                Commit();

                return true;
            }
            catch
            {
                return false;
            }
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
