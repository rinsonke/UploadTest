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
        public FileUploadController(FileUploaderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }
        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GenerateUploadURL()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray() );
            _context.UploadURLTokens.Add(new UploadURLToken { Token = token ,IsExpired=false});
            _context.SaveChanges();
            var url = Url.Content("~/") + "api/FileUpload/" + token;
            return Ok(url);
        }

       

        // POST api/<FileUploadController>
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
        [HttpPut("{id}")]
        public IActionResult Put(Guid id,IFormFile file )
        {
            if ( file.Length>0)
            {
                var filePath = Path.GetTempFileName()+ file.Name;

                using (var stream= System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                return Ok();
            }
            else
                return BadRequest("Invalid File Name...");
        }

      
    }
}
