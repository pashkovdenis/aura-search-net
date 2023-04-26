namespace AuraSearch.UseCases
{
    public interface IOutputFail
    {
        /// <summary>
        /// Error output
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);
    }
}
