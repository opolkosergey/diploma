using System;
using System.Collections.Generic;
using System.Linq;
using Diploma.Core.Models;
using Diploma.Extensions;
using Diploma.ViewModels;

namespace Diploma.Helpers
{
    public static class FakeDataGenerator
    {
        public static List<LayoutUserFoldersModel> GenerateFolders(int count = 4) => 
            Enumerable.Range(1, count).Select(x => new LayoutUserFoldersModel
            {
                Name = $"My folder name {x}",
                DocumentsCount = x
            }).ToList();

        public static List<Document> GenerateDocuments(int count) => Enumerable.Repeat(GetDocument(), count).ToList();

        private static Document GetDocument()
        {
            var random = new Random();

            var document = new Document
            {
                DocumentName = "Example document",
                Version = "1",
                UploadedDate = DateTime.Now
            };

            if (random.NextBool())
            {
                document.Version = "2";
                document.Signature = "jisogj9reugr09u5y4ih90g";
                document.SignedByUser = "Example user";
            }

            return document;
        }

        private static List<DocumentFolder> GetDocumentsFolders(int count) => Enumerable.Repeat(new DocumentFolder(), count).ToList();
    }
}
