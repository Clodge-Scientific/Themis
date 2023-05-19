namespace Themis.Las.Builders;

public interface IFluentBuilder<T>
{
    /// <summary>
    /// Returns the current under-construction <typeparamref name="T"/> product
    /// </summary>
    /// <param name="reset">Boolean flag of whether to 'reset' the under-construction product after retrieval - defaults to true</param>
    /// <returns><typeparamref name="T"/> product</returns>
    public T Build(bool reset = true);

    /// <summary>
    /// Resets the current under-contruction <typeparamref name="T"/> product to its default state
    /// </summary>
    public void Reset();
}