using OfficeIMO.Word;

namespace JPP.StandardDocuments
{
    public interface ICalculationContributor<T> : ICalculationContributor where T : class
    {
        void AddOutput(T input, WordSection bodyReport, WordSection appendix);

        void ICalculationContributor.AddOutput(object input, WordSection bodyReport, WordSection appendix)
        {
            if (input is T cast)
            {
                AddOutput(cast, bodyReport, appendix);
            }
            else
            {
                throw new InvalidOperationException("Incorrect type for contributor");
            }
        }
    }

    public interface ICalculationContributor
    {
        void AddOutput(object input, WordSection bodyReport, WordSection appendix);
    }
}
