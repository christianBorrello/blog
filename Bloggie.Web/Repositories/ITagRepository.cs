using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
	public interface ITagRepository
	{
		Task<IEnumerable<Tag>> GetAllTagsAsync(string? searchQuery = null,
											   string? sortBy = null,
											   string? sortDirection = null,
											   int pageNumber = 1,
											   int pageSize = 100);
		Task<Tag?> GetTagAsync(Guid id);
		Task<Tag> AddTagAsync(Tag tag);
		Task<Tag?> UpdateTagAsync(Tag tag);
		Task<Tag?> DeleteTagAsync(Guid id);
		Task<int> CountAsync();
	}
}
