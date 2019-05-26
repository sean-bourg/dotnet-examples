using System;
using System.Text;

namespace Gray.DistributedWriter.Testing
{
    /// <summary>
    /// Generate test file using random lipsum ipsum text generator.
    /// </summary>
    public class FileGenerator
    {
        /// <summary>
        /// Word dictionary.
        /// </summary>
        protected readonly string[] Dictionary;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private readonly Random Random;

        /// <summary>
        /// The minimum number of words.
        /// </summary>
        public int MinWords { get; set; } = 4;

        /// <summary>
        /// The maximum number of words.
        /// </summary>
        public int MaxWords { get; set; } = 30;

        /// <summary>
        /// The minimum number of sentences.
        /// </summary>
        public int MinSentences { get; set; } = 10;

        /// <summary>
        /// The maximum number of sentences.
        /// </summary>
        public int MaxSentences { get; set; } = 30;

        /// <summary>
        /// The minimum number of paragraphs.
        /// </summary>
        public int MinParagraphs { get; set; } = 5;

        /// <summary>
        /// The maximum number of paragraphs.
        /// </summary>
        public int MaxParagraphs { get; set; } = 20;

        /// <summary>
        /// Running counter of the files generated.
        /// </summary>
        public int FileCounter = 0;

        /// <summary>
        /// Construct instance of the lipsum ipsum content generator.
        /// </summary>        
        public FileGenerator(string[] dictionary, Random random)
        {
            Dictionary = dictionary;
            Random = random;
        }

        /// <summary>
        /// Get next test file.
        /// </summary>
        /// <returns></returns>
        public Tuple<string,byte[]> Next()
        {
            /// Calculate numbers
            int numSentences = Random.Next(MinSentences, MaxSentences);
            int numWords = Random.Next(MinWords, MaxWords);
            int numParagraphs = Random.Next(MinParagraphs, MaxParagraphs);

            // Build content string
            StringBuilder stringBuilder = new StringBuilder(numSentences * numWords * numParagraphs);
            for (int paragraph = 0; paragraph < numParagraphs; paragraph++)
            {
                for (int sentence = 0; sentence < numSentences; sentence++)
                {
                    for (int word = 0; word < numWords; word++)
                    {
                        if (word > 0)
                        {
                            stringBuilder.Append(" ");
                        }
                        stringBuilder.Append(Dictionary[Random.Next(Dictionary.Length)]);
                    }
                    stringBuilder.Append(". ");
                }
                stringBuilder.Append("\n");
            }

            // Build file tuple
            string filename = String.Format("{0}-file-{1:00#}.txt", DateTime.Now.ToFileTime(), FileCounter++);
            return new Tuple<string, byte[]>(filename, Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }


        /// <summary>
        /// Get list of random generated files.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Tuple<string, byte[]>[] GetFiles(int count)
        {
            Tuple<string, byte[]>[] files = new Tuple<string, byte[]>[count];
            for (int index = 0; index < count; index++)
            {
                files[index] = Next();
            }
            return files;
        }
    }
}
