using Microsoft.EntityFrameworkCore;
using FileUploaderAPI.Models;
namespace FileUploaderAPI.DataAccess
{

    public class FileUploaderDbContext : DbContext
    {
        public FileUploaderDbContext(DbContextOptions<FileUploaderDbContext> options) : base(options)
        { }
        public DbSet<UploadURLToken> UploadURLTokens    { get; set; }
        public DbSet<UploadedFileMetadata> UploadedFileMetadata { get; set;
        }
    }
}
