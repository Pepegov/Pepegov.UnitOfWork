namespace Pepegov.UnitOfWork.Entityes
{
    public interface IPagedList<T>
    {
        int IndexFrom { get; }

        int PageIndex { get; }

        int PageSize { get; }

        int TotalCount { get; }

        int TotalPages { get; }

        IReadOnlyCollection<T> Items { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }
    }
}