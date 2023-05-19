namespace Themis.Domain.Common;

public abstract class BaseEntity
{
    /// <summary>
    /// [Primary Key] The UUID associated with the entity
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}
