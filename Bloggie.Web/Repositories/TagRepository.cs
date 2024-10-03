using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

// Defining a repository class that gives to the controllers API like methods to operate CRUD operations on the DB
namespace Bloggie.Web.Repositories
{
	public class TagRepository : ITagRepository
	{
		// injected DbContext usable in this class
		private readonly BloggieDbContext bloggieDbContext;

		// Constructor that injects the DbContext
		public TagRepository(BloggieDbContext bloggieDbContext)
		{
			this.bloggieDbContext = bloggieDbContext;
		}

		// Adds a single Tag to the DB
		public async Task<Tag> AddTagAsync(Tag tag)
		{
			await bloggieDbContext.Tags.AddAsync(tag);
			await bloggieDbContext.SaveChangesAsync();
			return tag;
		}

		// Counts elements in the DB
		public async Task<int> CountAsync()
		{
			return await bloggieDbContext.Tags.CountAsync();
		}

		// Deletes a single tag from the DB
		public async Task<Tag?> DeleteTagAsync(Guid id)
		{
			var existingTag = await bloggieDbContext.Tags.FindAsync(id);

			if (existingTag != null)
			{
				bloggieDbContext.Tags.Remove(existingTag);
				await bloggieDbContext.SaveChangesAsync();

				return existingTag;
			}

			return null;
		}

		// Get all the tags in the DB as a List (IEnumerable)
		public async Task<IEnumerable<Tag>> GetAllTagsAsync(string? searchQuery,
															string? sortBy,
															string? sortDirection,
															int pageNumber = 1,
															int pageSize = 100)
		{
			var query = bloggieDbContext.Tags.AsQueryable();

			// filtering
			if (string.IsNullOrWhiteSpace(searchQuery) == false) // we need to search something
			{
				query = query.Where(tag => tag.Name.Contains(searchQuery) 
										|| tag.DisplayName.Contains(searchQuery));
			}

			// sorting
			if (string.IsNullOrWhiteSpace(sortBy) == false)
			{
				// bool
				var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);

				if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase)) 
				{
					query = isDesc ? query.OrderByDescending(tag => tag.Name) : query.OrderBy(tag => tag.Name);
				}
				
				if(string.Equals(sortBy, "DisplayName", StringComparison.OrdinalIgnoreCase))
				{
					query = isDesc ? query.OrderByDescending(tag => tag.DisplayName) : query.OrderBy(tag => tag.DisplayName);
				}
			}

			// pagination
			var skipResults = (pageNumber - 1) * pageSize;
			query = query.Skip(skipResults).Take(pageSize);

			return await query.ToListAsync();

			//return await bloggieDbContext.Tags.ToListAsync();
		}

		// Get a single tag by id
		public async Task<Tag?> GetTagAsync(Guid id)
		{
			return await bloggieDbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
		}

		// Get a tag and updates its properties
		public async Task<Tag?> UpdateTagAsync(Tag tag)
		{
			var existingTag = await bloggieDbContext.Tags.FindAsync(tag.Id);
			if (existingTag != null)
			{
				existingTag.Name = tag.Name;
				existingTag.DisplayName = tag.DisplayName;

				await bloggieDbContext.SaveChangesAsync();

				return existingTag;
			}

			return null;
		}
	}
}
