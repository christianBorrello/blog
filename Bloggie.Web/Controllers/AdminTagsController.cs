using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    // if logged in as an Admin gives way to the user to operate CRUD operations on tags
    [Authorize(Roles = "Admin")]
    public class AdminTagsController : Controller
    {
        // injected repository class usable in this class
		private readonly ITagRepository tagRepository;

        // Constructor used to inject 
		public AdminTagsController(ITagRepository tagRepository)
        {
            // injecting the repository class
			this.tagRepository = tagRepository;
		}

        // Retrieves datas from the database about the Add Tag Page
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // Used to operate the Add operation on the database from the Add Tag Page (functionality)
        [HttpPost]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            ValidateAddTagRequest(addTagRequest);

            if (ModelState.IsValid) 
            {
                // Mapping the addTagRequest to Tag domain model
                var tag = new Tag
                {
                    Name = addTagRequest.Name,
                    DisplayName = addTagRequest.DisplayName
                };

                await tagRepository.AddTagAsync(tag);

                return RedirectToAction("List");
            }

            return View();
        }

        // Controls on the datas before to send those to the DB, if fails the request gets invalidated
		private void ValidateAddTagRequest(AddTagRequest request)
		{
			if (request.Name != null && request.DisplayName != null)
            {
                if (request.DisplayName == request.Name)
                {
                    ModelState.AddModelError("DisplayName", "Name cannot be the same as DisplayName");
                } 
            }
		}

		// Retrieves datas from the database about all the tags in the correct format (search functionality, sorting, pagination)
		[HttpGet]
        public async Task<IActionResult> List(string? searchQuery, 
                                              string? sortBy, 
                                              string? sortDirection,
                                              int pageSize = 3,
                                              int pageNumber = 1)
        {
            var totalRecords = await tagRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber--;
            }

            if (pageNumber < 1)
            {
                pageNumber++;
            }

			ViewBag.TotalPages = totalPages;
			ViewBag.SearchQuery = searchQuery;
			ViewBag.SortBy = sortBy;
			ViewBag.SortDirection = sortDirection;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            
			// Use DbContext to read the tags
			var tags = await tagRepository.GetAllTagsAsync(searchQuery, sortBy, sortDirection, pageNumber, pageSize);
            
            return View(tags);
        }

        // Retrieves datas from the DB about the selected tag 
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // 1st method
            //var tag = bloggieDbContext.Tags.Find(id);

            var tag = await tagRepository.GetTagAsync(id);

            if (tag != null) 
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };

				return View(editTagRequest);
			}

            return View(null);
        }

		// Exposes a way to the user for updating the selected Tag
		[HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await tagRepository.UpdateTagAsync(tag);

            if (updatedTag != null) 
            {
                // Show success notification
            }
            else
            {
				// Show error notification
			}

            return RedirectToAction("List");
			//return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

		// Exposes a way to the user for deleting the selected Tag
		[HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await tagRepository.DeleteTagAsync(editTagRequest.Id);

            if (deletedTag != null) 
            {
                // Show success notification
                return RedirectToAction("List");
			}

			// Show error notification
			return RedirectToAction("Edit", new { id = editTagRequest.Id });
		}
    }
}
