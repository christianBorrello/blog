using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bloggie.Web.Models.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostsController : Controller
	{
		private readonly ITagRepository tagRepository;
		private readonly IBlogPostRepository blogPostRepository;

		// Constructor with dependencies injection (Tag repo and BP repo)
		public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
			this.tagRepository = tagRepository;
			this.blogPostRepository = blogPostRepository;
		}
        
        [HttpGet]
		public async Task<IActionResult> Add()
		{
			// get tags from repository
			var tags = await tagRepository.GetAllTagsAsync();
			var model = new AddBlogPostRequest
			{
				Tags = tags.Select(tag => new SelectListItem { Text = tag.Name, Value = tag.Id.ToString() })
			};

			return View(model);
		}

        [HttpPost]
		public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
		{
			// Map view model to domain model
			var blogPost = new BlogPost
			{
				Heading = addBlogPostRequest.Heading,
				PageTitle = addBlogPostRequest.PageTitle,
				Content = addBlogPostRequest.Content,
				ShortDescription = addBlogPostRequest.ShortDescription,
				FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
				UrlHandle = addBlogPostRequest.UrlHandle,
				PublishedDate = addBlogPostRequest.PublishedDate,
				Author = addBlogPostRequest.Author,
				Visible = addBlogPostRequest.Visible
			};

			// Map tags from selected tags
			var selectedTags = new List<Tag>();
			foreach (var selectedTagId in addBlogPostRequest.SelectedTags) 
			{
				var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
				var existingTag = await tagRepository.GetTagAsync(selectedTagIdAsGuid);

				if (existingTag != null) 
				{
					selectedTags.Add(existingTag);
				}
			}

			// Mapping tags back to domain model
			blogPost.Tags = selectedTags;

			await blogPostRepository.AddBlogPostAsync(blogPost);

			return RedirectToAction("Add");
		}

        [HttpGet]
		public async Task<IActionResult> List()
		{
			// Call the repository
			var blogPosts = await blogPostRepository.GetAllPostsAsync();

			return View(blogPosts); 
		}

        [HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			// Retrieve the result from the repository
			var blogPost = await blogPostRepository.GetBlogPostAsync(id);
			var tagsDomainModel = await tagRepository.GetAllTagsAsync();

			if (blogPost != null)
			{
				// map domain model into view model
				var model = new EditBlogPostRequest
				{
					Id = blogPost.Id,
					Heading = blogPost.Heading,
					PageTitle = blogPost.PageTitle,
					Content = blogPost.Content,
					Author = blogPost.Author,
					FeaturedImageUrl = blogPost.FeaturedImageUrl,
					UrlHandle = blogPost.UrlHandle,
					ShortDescription = blogPost.ShortDescription,
					PublishedDate = blogPost.PublishedDate,
					Visible = blogPost.Visible,
					Tags = tagsDomainModel.Select(tag => new SelectListItem { Text = tag.Name, Value = tag.Id.ToString() }),
					SelectedTags = blogPost.Tags.Select(tag => tag.Id.ToString()).ToArray()
				};
				// Pass data to view
				return View(model);
			}

			return View(null);
		}

        [HttpPost]
		public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
		{
			// map view model back to domain model
			var blogPostDomainModel = new BlogPost
			{
				Id = editBlogPostRequest.Id,
				Heading = editBlogPostRequest.Heading,
				PageTitle = editBlogPostRequest.PageTitle,
				Content = editBlogPostRequest.Content,
				Author = editBlogPostRequest.Author,
				FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
				UrlHandle = editBlogPostRequest.UrlHandle,
				ShortDescription = editBlogPostRequest.ShortDescription,
				PublishedDate = editBlogPostRequest.PublishedDate,
				Visible = editBlogPostRequest.Visible
			};

			// map tags into domain model
			var selectedTags = new List<Tag>();
			foreach(var selectedTag in editBlogPostRequest.SelectedTags)
			{
				if(Guid.TryParse(selectedTag, out var tag))
				{
					var foundTag = await tagRepository.GetTagAsync(tag);
					if (foundTag != null)
					{
						selectedTags.Add(foundTag); 
					}
				}
			}

			blogPostDomainModel.Tags = selectedTags;

			// submit information to repository to update
			var updatedBlogPost = await blogPostRepository.UpdateBlogPostAsync(blogPostDomainModel);

			if (updatedBlogPost != null) 
			{
				// Show success notification
				return RedirectToAction("List");
			}

			// Show error notification
			return RedirectToAction("Edit");
		}

        [HttpPost]
		public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
		{
			// Talk to repo to delete this blogpost and tags
			var deletedBlogPost = await blogPostRepository.DeleteBlogPostAsync(editBlogPostRequest.Id);

			// Display response
			if (deletedBlogPost != null) 
			{
				// show success notification
				return RedirectToAction("List");
			}
			else
			{
				// show error notification
				return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
			}
		}
	}
}
