using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinyToDo.Abstract.Services;
using MinyToDo.Api.Extensions;
using MinyToDo.Entity.Models;

namespace MinyToDo.Api.Controllers
{
    [Authorize]
    public class CategoryController : ApiController
    {
        IUserCategoryService _userCategoryService;
        public CategoryController(IUserCategoryService userCategoryService)
        {
            _userCategoryService = userCategoryService;
        }
        private bool categoryRelatedToAuthorizedUser(UserCategory userCategory)
                => userCategory?.ApplicationUserId == User.GetUserId();
        
        #region user: read
        [HttpGet("User")]
        public async Task<IActionResult> GetAllCategoriesForAuthorizedUser([FromQuery] bool withTasks = false)
        {
            var result = withTasks
            ? await _userCategoryService.GetAllWithTasksByUserId(User.GetUserId())
            : await _userCategoryService.GetAllByUserId(User.GetUserId());

            return result?.ToList().Count > 0 ? Ok(new { response = result }) : NoContent();
        }
        #endregion

        #region user: create - update - delete
        public class CategoryInput
        {
            [Required]
            [MinLength(3)]
            [MaxLength(30)]
            public string Name { get; set; }
        }

        [HttpPost("User")]
        public async Task<IActionResult> CreateUserCategory([FromBody] CategoryInput value)
        {
            var newUserCategory = new UserCategory(User.GetUserId(), value.Name);
            var result = await _userCategoryService.InsertAsync(newUserCategory);

            return result != null
            ? Created("", new { response = result })
            : BadRequest(new { error = "Sorry, the category could not add" });
        }

        [HttpPut("User/{userCategoryId}")]
        public async Task<IActionResult> UpdateUserCategory(Guid userCategoryId, CategoryInput value)
        {
            var toBeUpdatedCategory = await _userCategoryService.GetById(userCategoryId);
            if (toBeUpdatedCategory == null) NoContent();
            if (categoryRelatedToAuthorizedUser(toBeUpdatedCategory))
            {
                toBeUpdatedCategory.Name = value.Name;
                var result = await _userCategoryService.UpdateAsync(toBeUpdatedCategory);

                return result != null
                ? Ok(new { response = result })
                : BadRequest(new { error = "Sorry, the category could not update" });
            }

            return Forbid();
        }

        [HttpDelete("User/{userCategoryId}")]
        public async Task<IActionResult> DeleteUserCategory(Guid userCategoryId)
        {
            var toBeDeletedCategory = await _userCategoryService.GetById(userCategoryId);
            if (toBeDeletedCategory == null) return NoContent();
            if (categoryRelatedToAuthorizedUser(toBeDeletedCategory))
            {
                var result = await _userCategoryService.DeleteAsync(toBeDeletedCategory);

                return result
                ? Ok()
                : BadRequest(new { error = "Sorry, the category could not delete" });
            }

            return Forbid();
        }
        #endregion
    }
}