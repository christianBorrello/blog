using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
	public class BlogPostRepository : IBlogPostRepository
	{
		private readonly BloggieDbContext bloggieDbContext;

		// Constructor with DBContext injection
		public BlogPostRepository(BloggieDbContext bloggieDbContext)
        {
			this.bloggieDbContext = bloggieDbContext;
		}

        public async Task<BlogPost> AddBlogPostAsync(BlogPost blogPost)
		{
			await bloggieDbContext.AddAsync(blogPost);
			await bloggieDbContext.SaveChangesAsync();
			return blogPost;
		}

		public async Task<BlogPost?> DeleteBlogPostAsync(Guid id)
		{
			var existingBlogPost = await bloggieDbContext.BlogPosts.FindAsync(id);
			if (existingBlogPost != null) 
			{
				bloggieDbContext.BlogPosts.Remove(existingBlogPost);
				await bloggieDbContext.SaveChangesAsync();

				return existingBlogPost;
			}

			return null;
		}

		public async Task<IEnumerable<BlogPost>> GetAllPostsAsync()
		{
			return await bloggieDbContext.BlogPosts.Include(bp => bp.Tags).ToListAsync();
		}

		public async Task<BlogPost?> GetBlogPostAsync(Guid id)
		{
			return await bloggieDbContext.BlogPosts.Include(bp => bp.Tags).FirstOrDefaultAsync(bp => bp.Id == id);
		}

		public async Task<BlogPost?> GetBlogPostByUrlAsync(string urlHandle)
		{
			return await bloggieDbContext.BlogPosts
				.Include(bp => bp.Tags).FirstOrDefaultAsync(bp => bp.UrlHandle == urlHandle);
		}

		public async Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost)
		{
			var existingBlogPost = await bloggieDbContext.BlogPosts.Include(bp => bp.Tags).FirstOrDefaultAsync(bp => bp.Id == blogPost.Id);
			if (existingBlogPost != null) 
			{
				existingBlogPost.Id = blogPost.Id;
				existingBlogPost.Heading = blogPost.Heading;
				existingBlogPost.PageTitle = blogPost.PageTitle;
				existingBlogPost.Content = blogPost.Content;
				existingBlogPost.ShortDescription = blogPost.ShortDescription;
				existingBlogPost.Author = blogPost.Author;
				existingBlogPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
				existingBlogPost.UrlHandle = blogPost.UrlHandle;
				existingBlogPost.Visible = blogPost.Visible;
				existingBlogPost.PublishedDate = blogPost.PublishedDate;
				existingBlogPost.Tags = blogPost.Tags;

				await bloggieDbContext.SaveChangesAsync();

				return existingBlogPost;
			}

			return null;
		}
	}
}
