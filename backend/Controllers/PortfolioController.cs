using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PortfolioApi.Models;
using System.IO;
using System.Threading.Tasks;

namespace PortfolioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IMongoCollection<Portfolio> _portfolioCollection;

        public PortfolioController(IMongoDatabase database)
        {
            _portfolioCollection = database.GetCollection<Portfolio>("Portfolios");
        }

        // Helper to normalize username
        public static string NormalizeUsername(string name)
        {
            return name.Trim().ToLower().Replace(" ", "-");
        }

        // GET: api/portfolio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Portfolio>>> GetPortfolios()
        {
            var portfolios = await _portfolioCollection.Find(_ => true).ToListAsync();
            return Ok(portfolios);
        }

        // GET: api/portfolio/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(string id)
        {
            var portfolio = await _portfolioCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (portfolio == null)
            {
                return NotFound();
            }
            return Ok(portfolio);
        }

        // GET: api/portfolio/{id}/image
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetPortfolioImage(string id)
        {
            var portfolio = await _portfolioCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (portfolio == null || portfolio.Image.Length == 0)
            {
                return NotFound();
            }
            return File(portfolio.Image, portfolio.ImageContentType);
        }

        // GET: api/portfolio/{id}/resume
        [HttpGet("{id}/resume")]
        public async Task<IActionResult> GetPortfolioResume(string id)
        {
            var portfolio = await _portfolioCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (portfolio == null || portfolio.Resume.Length == 0)
            {
                return NotFound();
            }
            return File(portfolio.Resume, portfolio.ResumeContentType, "resume.pdf");
        }

        // GET: api/portfolio/by-username/{usernameKey}
        [HttpGet("by-username/{usernameKey}")]
        public async Task<ActionResult<Portfolio>> GetPortfolioByUsername(string usernameKey)
        {
            // Convert dashes to spaces, lowercase, and trim
            var normalized = usernameKey.Replace("-", " ").Trim().ToLower();

            var portfolio = await _portfolioCollection
                .Find(p => p.Name.ToLower() == normalized)
                .FirstOrDefaultAsync();

            if (portfolio == null)
            {
                return NotFound();
            }
            return Ok(portfolio);
        }

        // POST: api/portfolio
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Portfolio>> CreatePortfolio([FromForm] PortfolioForm portfolioForm)
        {
            if (string.IsNullOrWhiteSpace(portfolioForm.Name))
            {
                return BadRequest("Name is required.");
            }

            if (portfolioForm.Image == null || portfolioForm.Resume == null)
            {
                return BadRequest("Image and resume files are required.");
            }

            // Validate image content type
            if (!portfolioForm.Image.ContentType.StartsWith("image/"))
            {
                return BadRequest("Invalid image file type.");
            }

            // Validate resume content type (expecting PDF)
            if (portfolioForm.Resume.ContentType != "application/pdf")
            {
                return BadRequest("Resume must be a PDF file.");
            }

            // Convert files to byte arrays
            using var imageStream = new MemoryStream();
            await portfolioForm.Image.CopyToAsync(imageStream);
            using var resumeStream = new MemoryStream();
            await portfolioForm.Resume.CopyToAsync(resumeStream);

            var portfolio = new Portfolio
            {
                Name = portfolioForm.Name,
                Title = portfolioForm.Title,
                Description = portfolioForm.Description,
                Image = imageStream.ToArray(),
                ImageContentType = portfolioForm.Image.ContentType,
                Resume = resumeStream.ToArray(),
                ResumeContentType = portfolioForm.Resume.ContentType,
                UsernameKey = NormalizeUsername(portfolioForm.Name)
            };

            await _portfolioCollection.InsertOneAsync(portfolio);
            return CreatedAtAction(nameof(GetPortfolio), new { id = portfolio.Id }, portfolio);
        }

        // PUT: api/portfolio/{id}
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePortfolio(string id, [FromForm] PortfolioForm portfolioForm)
        {
            if (string.IsNullOrWhiteSpace(portfolioForm.Name))
            {
                return BadRequest("Name is required.");
            }

            var existingPortfolio = await _portfolioCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (existingPortfolio == null)
            {
                return NotFound();
            }

            var portfolio = new Portfolio
            {
                Id = id,
                Name = portfolioForm.Name,
                Title = portfolioForm.Title,
                Description = portfolioForm.Description,
                Image = existingPortfolio.Image,
                ImageContentType = existingPortfolio.ImageContentType,
                Resume = existingPortfolio.Resume,
                ResumeContentType = existingPortfolio.ResumeContentType,
                UsernameKey = NormalizeUsername(portfolioForm.Name)
            };

            // Update image if provided
            if (portfolioForm.Image != null)
            {
                if (!portfolioForm.Image.ContentType.StartsWith("image/"))
                {
                    return BadRequest("Invalid image file type.");
                }
                using var imageStream = new MemoryStream();
                await portfolioForm.Image.CopyToAsync(imageStream);
                portfolio.Image = imageStream.ToArray();
                portfolio.ImageContentType = portfolioForm.Image.ContentType;
            }

            // Update resume if provided
            if (portfolioForm.Resume != null)
            {
                if (portfolioForm.Resume.ContentType != "application/pdf")
                {
                    return BadRequest("Resume must be a PDF file.");
                }
                using var resumeStream = new MemoryStream();
                await portfolioForm.Resume.CopyToAsync(resumeStream);
                portfolio.Resume = resumeStream.ToArray();
                portfolio.ResumeContentType = portfolioForm.Resume.ContentType;
            }

            await _portfolioCollection.ReplaceOneAsync(p => p.Id == id, portfolio);
            return NoContent();
        }

        // DELETE: api/portfolio/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(string id)
        {
            var result = await _portfolioCollection.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class PortfolioForm
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public IFormFile? Resume { get; set; }
    }
}