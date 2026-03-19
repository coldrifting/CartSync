namespace CartSync.Models.Interfaces;

public interface IEditable<TEdit>
{
    public TEdit ToEditRequest(Ulid? storeId = null);
    public void UpdateFromEditRequest(TEdit editRequest);
}