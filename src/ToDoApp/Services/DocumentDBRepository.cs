using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;

namespace ToDoApp.Services
{
    public class DocumentDBRepository<T> : IDocumentDBRepository<T>
    {
        private IDocumentDbContext ctx = null;

        public DocumentDBRepository(IDocumentDbContext ctx)
        {
            this.ctx = ctx;
        }


        public async Task<Document> CreateItemAsync(T item)
        {
            return await ctx.Client.CreateDocumentAsync(ctx.Collection.SelfLink, item);
        }

        public T GetItem(Expression<Func<T, bool>> predicate)
        {
            return ctx.Client.CreateDocumentQuery<T>(ctx.Collection.DocumentsLink)
                        .Where(predicate)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        public IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            return ctx.Client.CreateDocumentQuery<T>(ctx.Collection.DocumentsLink)
                        .Where(predicate)
                        .AsEnumerable();
        }

        public Document GetDocument(string id)
        {
            return ctx.Client.CreateDocumentQuery(ctx.Collection.DocumentsLink)
                                .Where(d => d.Id == id)
                                .AsEnumerable()
                                .FirstOrDefault();
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            Document doc = GetDocument(id);
            return await ctx.Client.ReplaceDocumentAsync(doc.SelfLink, item);
        }

        public async Task DeleteItemAsync(string id)
        {
            Document doc = GetDocument(id);
            await ctx.Client.DeleteDocumentAsync(doc.SelfLink);
        }

    }
}
