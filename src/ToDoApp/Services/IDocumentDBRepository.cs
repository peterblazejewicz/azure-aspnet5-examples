using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace ToDoApp.Services
{
    public interface IDocumentDBRepository<T>
    {
        Task<Document> CreateItemAsync(T item);

        T GetItem(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate);

        Document GetDocument(string id);

        Task<Document> UpdateItemAsync(string id, T item);

        Task DeleteItemAsync(string id);
    }
}
