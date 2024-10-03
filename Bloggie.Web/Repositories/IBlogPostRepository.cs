using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
	public interface IBlogPostRepository
	{
		Task<IEnumerable<BlogPost>> GetAllPostsAsync();
		Task<BlogPost?> GetBlogPostAsync(Guid id);
		Task<BlogPost?> GetBlogPostByUrlAsync(string urlHandle);
		Task<BlogPost> AddBlogPostAsync(BlogPost blogPost);
		Task<BlogPost?> UpdateBlogPostAsync(BlogPost post);
		Task<BlogPost?> DeleteBlogPostAsync(Guid id);
	}
}
