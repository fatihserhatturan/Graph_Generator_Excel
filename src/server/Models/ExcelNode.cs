using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_API.Models
{
    public class ExcelNode
    {
        public string DOI { get; set; }
        public string AuthorName { get; set; }
        public List<string> CoAuthors { get; set; }
        public string Title { get; set; }
    }
}
