using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_API.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Papers { get; set; } = new List<string>();
        public Dictionary<int, int> Collaborations { get; set; } = new Dictionary<int, int>();
        public int TotalPapers => Papers.Count;

        public Author(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
