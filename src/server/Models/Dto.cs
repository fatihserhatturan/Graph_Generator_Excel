namespace Graph_API.Models
{
    public class Dto
    {
        public class AuthorDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<string> Papers { get; set; }
            public Dictionary<int, int> Collaborations { get; set; }
            public int TotalPapers { get; set; }
            public bool IsHighPerforming { get; set; }
            public bool IsLowPerforming { get; set; }
        }

        public class GraphDataResponse
        {
            public List<AuthorDto> Authors { get; set; }
            public double AveragePaperCount { get; set; }
            public List<CollaboratorInfoDto> TopCollaborators { get; set; }
        }

        public class CollaboratorInfoDto
        {
            public AuthorDto Author { get; set; }
            public int CollaborationCount { get; set; }
        }
    }
}
