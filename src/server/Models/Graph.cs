using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Graph_API.Models
{
    public class Graph
    {

        private Dictionary<string, Author> authorsByName;
        private Dictionary<int, Author> authorsById;
        private int nextId;
        private double averagePaperCount;

        public Graph()
        {
            authorsByName = new Dictionary<string, Author>();
            authorsById = new Dictionary<int, Author>();
            nextId = 0;
            averagePaperCount = 0;
        }

        private Author GetOrCreateAuthor(string name)
        {
            if (authorsByName.ContainsKey(name))
                return authorsByName[name];

            var author = new Author(nextId++, name);
            authorsByName[name] = author;
            authorsById[author.Id] = author;
            return author;
        }

        private void AddCollaboration(Author author1, Author author2, string paper)
        {
            if (author1.Id == author2.Id) return;

            // Makaleyi her iki yazarın listesine ekle
            if (!author1.Papers.Contains(paper))
                author1.Papers.Add(paper);
            if (!author2.Papers.Contains(paper))
                author2.Papers.Add(paper);

            // İşbirliği sayısını güncelle
            if (!author1.Collaborations.ContainsKey(author2.Id))
                author1.Collaborations[author2.Id] = 0;
            if (!author2.Collaborations.ContainsKey(author1.Id))
                author2.Collaborations[author1.Id] = 0;

            author1.Collaborations[author2.Id]++;
            author2.Collaborations[author1.Id]++;
        }

        public void GenerateFromNodes(List<ExcelNode> nodes)
        {
            // Her makale için yazarları ve ortak yazarları işle
            foreach (var node in nodes)
            {
                var mainAuthor = GetOrCreateAuthor(node.AuthorName);

                // Ana yazarın makalesi
                if (!mainAuthor.Papers.Contains(node.Title))
                    mainAuthor.Papers.Add(node.Title);

                // Ortak yazarlar için işbirliklerini kaydet
                foreach (var coAuthorName in node.CoAuthors)
                {
                    var coAuthor = GetOrCreateAuthor(coAuthorName);
                    AddCollaboration(mainAuthor, coAuthor, node.Title);
                }
            }

            // Ortalama makale sayısını hesapla
            CalculateAveragePaperCount();
        }

        private void CalculateAveragePaperCount()
        {
            if (authorsById.Count == 0) return;
            averagePaperCount = authorsById.Values.Average(a => a.Papers.Count);
        }

        // Ortalama makale sayısının %20 üstünde/altında olan yazarları belirleme
        public bool IsHighPerformingAuthor(Author author)
        {
            return author.TotalPapers > (averagePaperCount * 1.2);
        }

        public bool IsLowPerformingAuthor(Author author)
        {
            return author.TotalPapers < (averagePaperCount * 0.8);
        }

        public Author GetAuthorById(int id)
        {
            return authorsById.ContainsKey(id) ? authorsById[id] : null;
        }

        public Author GetAuthorByName(string name)
        {
            return authorsByName.ContainsKey(name) ? authorsByName[name] : null;
        }

        public List<Author> GetAllAuthors()
        {
            return authorsById.Values.ToList();
        }

        public List<(Author author, int collaborationCount)> GetMostCollaborativeAuthors(int topN = 5)
        {
            return authorsById.Values
                .OrderByDescending(a => a.Collaborations.Count)
                .Select(a => (a, a.Collaborations.Count))
                .Take(topN)
                .ToList();
        }

        public double GetAveragePaperCount() => averagePaperCount;

        // İki yazar arasındaki işbirliği sayısını getir
        public int GetCollaborationWeight(int author1Id, int author2Id)
        {
            var author1 = GetAuthorById(author1Id);
            if (author1 == null || !author1.Collaborations.ContainsKey(author2Id))
                return 0;
            return author1.Collaborations[author2Id];
        }
    }
}
