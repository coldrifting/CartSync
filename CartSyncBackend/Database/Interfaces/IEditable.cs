namespace CartSyncBackend.Database.Interfaces;

public interface IEditable<out T>
{
    public T ToEditRequest();
}