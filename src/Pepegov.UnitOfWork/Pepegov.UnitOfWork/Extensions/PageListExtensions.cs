using Pepegov.UnitOfWork.Entityes;

namespace Pepegov.UnitOfWork.Extensions;

/// <summary>
/// Extensions fro Queryable LINQ
/// </summary>
public static class PageListExtensions
{
    /// <summary>
    /// Converts the specified source to <see cref="IPagedList{T}"/> by the specified <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The source to paging.</param>
    /// <param name="pageIndex">The index of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="totalPages"></param>
    /// <param name="count"></param>
    /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
    public static IPagedList<T> ToPagedList<T>(this IReadOnlyCollection<T> source, int pageIndex, int pageSize, int totalPages, long? count)
    {
        var pagedList = new PagedList<T>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = (int)count,
            Items = source,
            TotalPages = totalPages
        };

        return pagedList;
    }
    
    /// <summary>
    /// Upcast PagedList<> => IPagedList<>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IPagedList<T> Empty<T>() 
        => (IPagedList<T>) new PagedList<T>();

    /// <summary>
    /// Upcast PagedList<,> => IPagedList<,>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="converter"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        => (IPagedList<TResult>) new PagedList<TSource, TResult>(source, converter);

}