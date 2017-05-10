using System;
using System.Collections.Generic;
using Diploma.Core.Models;

namespace Diploma.Helpers
{
    public static class FictitiousDataGenerator
    {
        public static List<UserFolder> GenerateFolders()
        {
            return new List<UserFolder>
            {
                new UserFolder
                {
                    Name = "Uploaded",
                    DocumentFolders = new List<DocumentFolder>
                    {
                        new DocumentFolder()
                    }
                },
                new UserFolder
                {
                    Name = "My folder name 1",
                    DocumentFolders = new List<DocumentFolder>
                    {
                        new DocumentFolder(),
                        new DocumentFolder()
                    }
                },
                new UserFolder
                {
                    Name = "My folder name 2",
                    DocumentFolders = new List<DocumentFolder>
                    {
                        new DocumentFolder(),
                        new DocumentFolder(),
                        new DocumentFolder(),
                        new DocumentFolder()
                    }
                },
                new UserFolder
                {
                    Name = "My folder name 3",
                    DocumentFolders = new List<DocumentFolder>
                    {
                        new DocumentFolder(),
                        new DocumentFolder(),
                        new DocumentFolder(),
                        new DocumentFolder()
                    }
                }
            };
        }

        public static List<Document> GeneratDocuments()
        {
            return new List<Document>
            {
                new Document
                {
                    DocumentName = "Example document",
                    Version = "1",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document",
                    Version = "2",
                    Signature = "jisogj9reugr09u5y4ih90g",
                    SignedByUser = "Example user",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document",
                    Version = "1",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document1",
                    Version = "1",
                    Signature = "jisogj9reugr09u5y4ih90g",
                    SignedByUser = "Example user",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document1",
                    Version = "2",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document1",
                    Version = "3",
                    Signature = "jisogj9reugr09u5y4ih90g",
                    SignedByUser = "Example user",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document2",
                    Version = "1",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document2",
                    Version = "2",
                    Signature = "jisogj9reugr09u5y4ih90g",
                    SignedByUser = "Example user",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document2",
                    Version = "3",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document2",
                    Version = "4",
                    Signature = "jisogj9reugr09u5y4ih90g",
                    SignedByUser = "Example user",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document3",
                    Version = "1",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document3",
                    Version = "2",
                    Signature = "jisogj9reugr09u5y4ih90g",
                    SignedByUser = "Example user",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document3",
                    Version = "3",
                    UploadedDate = DateTime.Now
                },
                new Document
                {
                    DocumentName = "Example document3",
                    Version = "4",
                    Signature = "jisogj9reugr09u5y4ih90g",
                    SignedByUser = "Example user",
                    UploadedDate = DateTime.Now
                }
            };
        }
    }
}
