using System.Linq.Expressions;

namespace CartSyncBackend.Models.Interfaces;

public interface IResponse<TModel, TResponse>
{
    public static abstract Expression<Func<TModel, TResponse>> ToResponse { get; }
}