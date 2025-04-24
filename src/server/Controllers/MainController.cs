using Microsoft.AspNetCore.Mvc;
using Graph_API.Models;
using Graph_API.Business;
using static Graph_API.Models.Dto;
using Microsoft.AspNetCore.Cors;

namespace Graph_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("AllowAll")]  
    public class MainController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public MainController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet("graph-data")]
        public async Task<ActionResult<GraphDataResponse>> GetGraphData()
        {
            try
            {
                var generator = new GenerateCollaborationGraph();
                string excelPath = Path.Combine(_environment.ContentRootPath, "Data", "C:\\Users\\fatti\\Desktop\\dataset_2.xlsx");

                var nodes = await generator.BuildFromExcel(excelPath);
                var graph = generator.GenerateGraph(nodes);

                var response = new GraphDataResponse
                {
                    Authors = graph.GetAllAuthors().Select(author => new AuthorDto
                    {
                        Id = author.Id,
                        Name = author.Name,
                        Papers = author.Papers,
                        Collaborations = author.Collaborations,
                        TotalPapers = author.TotalPapers,
                        IsHighPerforming = graph.IsHighPerformingAuthor(author),
                        IsLowPerforming = graph.IsLowPerformingAuthor(author)
                    }).ToList(),
                    AveragePaperCount = graph.GetAveragePaperCount(),
                    TopCollaborators = graph.GetMostCollaborativeAuthors(5)
                        .Select(x => new CollaboratorInfoDto
                        {
                            Author = new AuthorDto
                            {
                                Id = x.author.Id,
                                Name = x.author.Name,
                                Papers = x.author.Papers,
                                Collaborations = x.author.Collaborations,
                                TotalPapers = x.author.TotalPapers,
                                IsHighPerforming = graph.IsHighPerformingAuthor(x.author),
                                IsLowPerforming = graph.IsLowPerformingAuthor(x.author)
                            },
                            CollaborationCount = x.collaborationCount
                        }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}