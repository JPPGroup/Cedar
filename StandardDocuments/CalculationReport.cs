using OfficeIMO.Word;

namespace JPP.StandardDocuments
{
    public class CalculationReport : IDisposable
    {
        readonly string _filePath;
        WordDocument _document;

        private List<object> _inputs;
        private List<ICalculationContributor> _calculationContributors;

        public CalculationReport(string filePath, ICalculationContributor[] contributors)
        {
            _filePath = filePath;
            ExportTemplate();
            _document = WordDocument.Load(_filePath);
            var paragraph = _document.AddParagraph("Basic paragraph");
            _inputs = new List<object>();
            _calculationContributors = contributors.ToList();
        }

        public void AddInput(object input)
        {
            _inputs.Add(input);
        }

        public void ProcessInputs()
        {
            var body = _document.Sections[2];
            var appendix = _document.Sections[3];

            foreach (var input in _inputs)
            {
                Type genericType = typeof(ICalculationContributor<>);
                Type genericTypeWithGenericArgument = genericType.MakeGenericType(input.GetType());

                var contributor = _calculationContributors.FirstOrDefault(c => c.GetType().IsAssignableTo(genericTypeWithGenericArgument));

                if (contributor is null)
                    throw new InvalidOperationException("No contributor found");

                contributor.AddOutput(input, body, appendix);
            }
        }

        public void Finalise()
        {
            _document.Save(true);
        }

        public void Dispose()
        {
            _document.Dispose();
        }

        private void ExportTemplate()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream("JPP.StandardDocuments.Resources.Calculation Report Template.docx"))
            {
                using (var fileStream = File.Create(_filePath))
                {
                    resFilestream.Seek(0, SeekOrigin.Begin);
                    resFilestream.CopyTo(fileStream);
                }
            }
        }
    }
}
