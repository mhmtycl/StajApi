namespace StajApi.CQRS;

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> Handle(TQuery query);
}
