using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MinyToDo.Abstract.Repositories;
using MinyToDo.Abstract.Services;
using MinyToDo.Entity.DTO.Request;
using MinyToDo.Entity.DTO.Response;
using MinyToDo.Entity.Models;

namespace MinyToDo.Service.Concrete
{
    public class UserCategoryService : IUserCategoryService
    {
        private IUserCategoryRepository _userCategoryRepository;
        private IMapper _mapper;

        public UserCategoryService(IUserCategoryRepository userCategoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userCategoryRepository = userCategoryRepository;
        }

        public async Task<UserCategoryResponse> InsertAsync(Guid appUserId, UserCategoryRequest newCategory)
        {
            var newEntity = new UserCategory(appUserId, newCategory);
            return _mapper.Map<UserCategoryResponse>(await _userCategoryRepository.InsertAsync(newEntity)); 
        }

        public async Task<UserCategoryResponse> UpdateAsync(UserCategory toBeUpdatedCategory, UserCategoryRequest newValues)
        {
            toBeUpdatedCategory.Name = newValues.Name;
            return _mapper.Map<UserCategoryResponse>(await _userCategoryRepository.UpdateAsync(toBeUpdatedCategory));
        }

        public async Task<bool> DeleteAsync(UserCategory toBeDeletedCategory)
        {
            return await _userCategoryRepository.DeleteAsync(toBeDeletedCategory) > 0 ? true : false;
        }

        public async Task<IEnumerable<UserCategoryResponse>> GetAllByUserId(Guid appUserId)
        {
            return _mapper.Map<IEnumerable<UserCategoryResponse>>(await _userCategoryRepository.GetAll(x => x.ApplicationUserId == appUserId));
        }

        public async Task<IEnumerable<UserCategoryResponse>> GetAllWithTasksByUserId(Guid appUserId)
        {
            return _mapper.Map<IEnumerable<UserCategoryResponse>>(await _userCategoryRepository.GetAllWithTasksAsync(x => x.ApplicationUserId == appUserId));
        }

        public async Task<UserCategory> GetById(Guid categoryId)
        {
            return await _userCategoryRepository.GetById(categoryId);
        }
    }
}