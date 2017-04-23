using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Diploma.Core.Models;
using Diploma.Repositories;
using Diploma.Repositories.Abstracts;
using Binbin.Linq;

namespace Diploma.Filters
{
    public class SearchService
    {
        private readonly IDocumentRepository documentRepository = new DocumentRepository();

        public IEnumerable<Document> SearchDocuments(string searchString)
        {
            var queryParts = searchString.ToLower().Split(new[] { " and " }, StringSplitOptions.None).Select(x => x.Trim());
            var expressions = new List<Expression<Func<Document, bool>>>();

            foreach (var queryPart in queryParts)
            {
                var splittedQueryPart = queryPart.Split(' ').Select(x => x.Trim());

                var field = splittedQueryPart.Count() == 1 ? "title" : splittedQueryPart.First();
                var value = queryPart.Split(' ').Select(x => x.Trim()).Last();
                var @operator = GetOperator(queryPart) ?? "=";

                expressions.Add(GetExpressionByField(value, @operator, field));
            }

            var expression = expressions.Aggregate((x, y) => x.And(y));

            var predicate = expression.Compile();

            return documentRepository.FindBy(predicate);
        }

        private string GetOperator(string str)
        {
            string @operator = null;

            if (str.Contains("="))
            {
                @operator = "=";
            }
            else if (str.Contains("!="))
            {
                @operator = "!=";
            }
            else if (str.Contains("~"))
            {
                @operator = "~";
            }
            else if (str.Contains("!~"))
            {
                @operator = "!~";
            }

            return @operator;
        }

        private Expression<Func<Document, bool>> GetExpressionByField(string value, string @operator, string field = "title")
        {
            var dictionary = new Dictionary<Tuple<string, string>, Expression<Func<Document, bool>>>
            {
                {
                    Tuple.Create("title", "="), d => d.DocumentName.Equals(value, StringComparison.OrdinalIgnoreCase)
                },
                {
                    Tuple.Create("title", "!="), d => !d.DocumentName.ToLower().Equals(value)
                },
                {
                    Tuple.Create("title", "~"), d => d.DocumentName.ToLower().Contains(value)
                },
                {
                    Tuple.Create("title", "!~"), d => !d.DocumentName.Contains(value)
                },
                {
                    Tuple.Create("version", "="), d => d.Version.Equals(value, StringComparison.OrdinalIgnoreCase)
                },
                {
                    Tuple.Create("version", "!="), d => !d.Version.Equals(value)
                }
            };

            return dictionary[Tuple.Create(field, @operator)];
        }
    }
}
