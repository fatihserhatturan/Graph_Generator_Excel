using ExcelDataReader;
using Graph_API.Models; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_API.Business
{
    public class GenerateCollaborationGraph
    {
        ///
        public async Task<List<ExcelNode>> BuildFromExcel(string path)
        {
            // Encoding sağlayıcısını kaydet
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var nodes = new List<ExcelNode>();

            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    // Excel okuma konfigürasyonu
                    var config = new ExcelReaderConfiguration()
                    {
                        FallbackEncoding = Encoding.GetEncoding(1252),
                    };

                    using (var reader = ExcelReaderFactory.CreateReader(stream, config))
                    {
                        // Başlık satırını atla
                        reader.Read();

                        while (reader.Read())
                        {
                            try
                            {
                                // Null kontrolü yap
                                if (reader.GetValue(1) == null || reader.GetValue(3) == null ||
                                    reader.GetValue(4) == null || reader.GetValue(5) == null)
                                    continue;

                                string cleanText(string input)
                                {
                                    if (string.IsNullOrEmpty(input)) return string.Empty;
                                    // Nokta hariç tüm noktalama işaretlerini kaldır
                                    return new string(input.Where(c => char.IsLetterOrDigit(c) || c == '.' || c == ',' || char.IsWhiteSpace(c)).ToArray())
                                        .Trim();
                                }

                                var node = new ExcelNode
                                {
                                    DOI = reader.GetString(1),
                                    AuthorName = cleanText(reader.GetString(3)),
                                    CoAuthors = reader.GetString(4)?
                                        .Split(new[] { ',', ';', '|', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(a => cleanText(a))
                                        .Where(a => !string.IsNullOrEmpty(a))
                                        .ToList() ?? new List<string>(),
                                    Title = cleanText(reader.GetString(5))
                                };
                                nodes.Add(node);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Satır okuma hatası: {ex.Message}");
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excel okuma hatası: {ex.Message}");
                throw;
            }

            return nodes;
        }

        ///////////
        ///
        public Graph GenerateGraph(List<ExcelNode> nodes)
        {
            var graph = new Graph();
            graph.GenerateFromNodes(nodes);
            return graph;
        }
    }
}
