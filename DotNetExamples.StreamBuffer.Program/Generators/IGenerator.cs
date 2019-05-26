namespace DotNetExamples.StreamBuffer.Program.Generators
{
    /// <summary>
    /// Interface definition for a generator object that is used to generate random test data.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface IGenerator<T>
    {
        /// <summary>
        /// Name of this generator.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get next generator element.
        /// </summary>
        /// <returns></returns>
        T Next();
    }
}
