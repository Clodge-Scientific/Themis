namespace Themis.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    /// <summary>
    /// The <typeparamref name="DateTime"/> when the entity was created
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// [Nullable] The <typeparamref name="string"/> identifier of who created the entity
    /// </summary>
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The <typeparamref name="DateTime"/> when the entity was last updated
    /// </summary>
    [JsonPropertyName("last_update")]
    public DateTime? LastUpdate { get; set; }

    /// <summary>
    /// [Nullable] The <typeparamref name="string"/> identifier of who last updated the entity
    /// </summary>
    [JsonPropertyName("last_update_by")]
    public string? LastUpdatedBy { get; set; }

    /// <summary>
    /// Update the requesite <typeparamref name="BaseAuditableEntity"/> fields
    /// </summary>
    /// <param name="updatedBy"></param>
    public void SetUpdated(string? updatedBy = null)
    {
        LastUpdate = DateTime.UtcNow;
        LastUpdatedBy = updatedBy ?? Environment.UserName;
    }
}