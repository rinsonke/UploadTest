using Microsoft.AspNetCore.Mvc;
using FileUploaderAPI.DataAccess;
using FileUploaderAPI.Models;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileUploaderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {

        private FileUploaderDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FileUploadController(FileUploaderDbContext context,IWebHostEnvironment hostingEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));  
        }
        // GET: api/<ValuesController>
        [HttpGet]
        // Generating and return URL  which contain a unique token derived from GUID 
        public IActionResult GenerateUploadURL()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray() );
            _context.UploadURLTokens.Add(new UploadURLToken { Token = token ,IsExpired=false});
            _context.SaveChanges();
            var url = Url.Content("~/") + "api/FileUpload/" + token;
            return Ok(url);
        }

       

        // POST api/FileUpload/token
        // Verifies recieved token against generated token and returning a GUID against URL
        [HttpPost("{token}")]
        public IActionResult  Post( [FromRoute] string token , [FromBody] UploadedFileMetadata metadata)
        {

            var uploadURLToken = _context.UploadURLTokens.FirstOrDefault(t => t.Token == token);
            if (uploadURLToken != null && uploadURLToken.IsExpired == false)
            {
                uploadURLToken.IsExpired = true;
                Guid guid = Guid.NewGuid();
                metadata.Id = guid;
                _context.UploadedFileMetadata.Add(metadata);
                _context.SaveChanges();
                return Ok(guid.ToString());
            }
            else
                return BadRequest("Invalid Tocken");
            

        }

        // PUT api/<FileUploadController>/5
        // Uploading File  with given guid
        [HttpPut("{id}")]
        public IActionResult Put(string guid,IFormFile file )
        {
            if ( file.Length>0)
            {
                // Creating a path for uploading file.

                string path = Path.Combine(Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads"));
                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(path,  file.FileName);
                
                // Uploading File in the created path.
                using (var stream= System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                return Ok("File Successfully Uploaded with GUID : " +guid+ "\nUploaded File : "+ filePath );
            }
            else
                return BadRequest("Invalid File Name...");
        }

      
    }
}
