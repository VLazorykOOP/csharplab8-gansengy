using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab8CSharp
{
    public class Lab8T2
    {
        private string inputFilePath = "text1.txt";
        private string outputFilePath = "result1.txt";
        private List<string> emails;

        public Lab8T2()
        {
            emails = new List<string>();
        }

        public async Task Run()
        {
            try
            {
                await ReadFileAsync();
                await WriteEmailsToFileAsync();
                Console.WriteLine($"Found {emails.Count} email(s).");

                // Заміна або видалення деяких адрес за вказаними параметрами
                // Наприклад, замінити всі адреси з доменом "example.com"
                string domainToReplace = "example.com";
                string newDomain = "newdomain.com";
                ReplaceEmails(domainToReplace, newDomain);

                await WriteEmailsToFileAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task ReadFileAsync()
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("Input file not found.");
            }

            string content = await File.ReadAllTextAsync(inputFilePath);
            FindEmails(content);
        }

        private void FindEmails(string content)
        {
            string pattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
            MatchCollection matches = Regex.Matches(content, pattern);

            foreach (Match match in matches)
            {
                emails.Add(match.Value);
            }
        }

        private async Task WriteEmailsToFileAsync()
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (string email in emails)
                {
                    await writer.WriteLineAsync(email);
                }
            }
        }

        private void ReplaceEmails(string domainToReplace, string newDomain)
        {
            for (int i = 0; i < emails.Count; i++)
            {
                if (emails[i].EndsWith("@" + domainToReplace))
                {
                    emails[i] = emails[i].Replace(domainToReplace, newDomain);
                }
            }
        }
    }

    public class Lab8T3
    {
        private string inputFilePath = "text2.txt";
        private string outputFilePath = "result2.txt";

        public async Task Run()
        {
            try
            {
                int maxNumber = await FindMaxIntegerAsync();
                await WriteResultToFileAsync(maxNumber);
                Console.WriteLine($"The maximum integer found is {maxNumber}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task<int> FindMaxIntegerAsync()
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("Input file not found.");
            }

            string content = await File.ReadAllTextAsync(inputFilePath);
            return FindMaxInteger(content);
        }

        private int FindMaxInteger(string content)
        {
            string pattern = @"-?\d+";
            MatchCollection matches = Regex.Matches(content, pattern);

            List<int> numbers = new List<int>();
            foreach (Match match in matches)
            {
                if (int.TryParse(match.Value, out int number))
                {
                    numbers.Add(number);
                }
            }

            if (numbers.Count == 0)
            {
                throw new InvalidOperationException("No integers found in the input file.");
            }

            return numbers.Max();
        }

        private async Task WriteResultToFileAsync(int maxNumber)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                await writer.WriteLineAsync($"The maximum integer found is {maxNumber}.");
            }
        }
    }

    public class Lab8T4
    {
        private string inputFilePath = "text3.txt";
        private string outputFilePath = "result3.txt";

        public async Task Run()
        {
            try
            {
                string content = await ReadFileAsync();
                string result = RemoveUniqueWords(content);
                await WriteFileAsync(result);
                Console.WriteLine("Task completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task<string> ReadFileAsync()
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("Input file not found.");
            }
            return await File.ReadAllTextAsync(inputFilePath);
        }

        private async Task WriteFileAsync(string content)
        {
            await File.WriteAllTextAsync(outputFilePath, content);
        }

        private string RemoveUniqueWords(string content)
        {
            // Normalize the content to handle punctuation and case
            string normalizedContent = Regex.Replace(content, @"[^\w\s]", "").ToLower();

            // Split the content into words
            string[] words = normalizedContent.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Count the occurrences of each word
            var wordCounts = words.GroupBy(word => word)
                                  .ToDictionary(group => group.Key, group => group.Count());

            // Remove words that appear only once
            var wordsToRemove = new HashSet<string>(wordCounts.Where(kv => kv.Value == 1).Select(kv => kv.Key));

            // Rebuild the content without the unique words
            var resultWords = content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.None)
                                     .Where(word => !wordsToRemove.Contains(Regex.Replace(word, @"[^\w\s]", "").ToLower()));

            return string.Join(" ", resultWords);
        }
    }

    public class Lab8T5
    {
        private string inputFilePath = "text4.txt";
        private string outputFilePath = "result4.bin";
        private double lowerBound;
        private double upperBound;

        public Lab8T5(double lowerBound, double upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public async Task Run()
        {
            try
            {
                List<double> numbers = await ReadNumbersAsync();
                await WriteNumbersToBinaryFileAsync(numbers);
                List<double> outOfRangeNumbers = FindOutOfRangeNumbers(numbers);
                DisplayOutOfRangeNumbers(outOfRangeNumbers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task<List<double>> ReadNumbersAsync()
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("Input file not found.");
            }

            string content = await File.ReadAllTextAsync(inputFilePath);
            return content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(double.Parse)
                          .ToList();
        }

        private async Task WriteNumbersToBinaryFileAsync(List<double> numbers)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create)))
            {
                foreach (double number in numbers)
                {
                    writer.Write(number);
                }
            }
        }

        private List<double> FindOutOfRangeNumbers(List<double> numbers)
        {
            return numbers.Where(num => num < lowerBound || num > upperBound).ToList();
        }

        private void DisplayOutOfRangeNumbers(List<double> numbers)
        {
            Console.WriteLine("Numbers out of range:");
            foreach (double number in numbers)
            {
                Console.WriteLine(number);
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            async Task Task1()
            {
                Lab8T2 lab8task2 = new Lab8T2();
                await lab8task2.Run();
            }
            async Task Task2()
            {
                Lab8T3 lab8task3 = new Lab8T3();
                await lab8task3.Run();
            }
            async Task Task3()
            {
                Lab8T4 lab8task4 = new Lab8T4();
                await lab8task4.Run();
            }
            async Task Task4()
            {
                double lowerBound = 2.0;
                double upperBound = 5.0;
                Lab8T5 lab8task5 = new Lab8T5(lowerBound, upperBound);
                await lab8task5.Run();
            }
            async Task Task5()
            {
                string studentName = "Shevchenko";
                string dir1 = $"{studentName}1";
                string dir2 = $"{studentName}2";
                string file1 = Path.Combine(dir1, "t1.txt");
                string file2 = Path.Combine(dir1, "t2.txt");
                string file3 = Path.Combine(dir2, "t3.txt");

                // Створення папок
                Directory.CreateDirectory(dir1);
                Directory.CreateDirectory(dir2);

                // Створення файлів і запис тексту
                await File.WriteAllTextAsync(file1, "<Шевченко Степан Іванович, 2001> року народження, місце проживання <м. Суми>");
                await File.WriteAllTextAsync(file2, "<Комар Сергій Федорович, 2000> року народження, місце проживання <м. Київ>");

                // Читання тексту з файлів t1.txt і t2.txt, і запис до t3.txt
                string text1 = await File.ReadAllTextAsync(file1);
                string text2 = await File.ReadAllTextAsync(file2);
                await File.WriteAllTextAsync(file3, text1 + Environment.NewLine + text2);

                // Виведення розгорнутої інформації про створені файли
                DisplayFileInfo(file1);
                DisplayFileInfo(file2);
                DisplayFileInfo(file3);

                // Переміщення файлу t2.txt до папки <прізвище_студента>2
                string newFile2Path = Path.Combine(dir2, "t2.txt");
                File.Move(file2, newFile2Path);

                // Копіювання файлу t1.txt до папки <прізвище_студента>2
                string copyFile1Path = Path.Combine(dir2, "t1.txt");
                File.Copy(file1, copyFile1Path);

                // Перейменування папки <прізвище_студента>2 в ALL
                string allDir = "ALL";
                Directory.Move(dir2, allDir);

                // Вилучення папки <прізвище_студента>1
                Directory.Delete(dir1, true);

                // Виведення повної інформації про файли папки ALL
                DisplayDirectoryFilesInfo(allDir);
            }

            while (true)
            {
                Console.WriteLine("  ****  Lab 8  ****  \n\n");
                Console.Write("Press 0 to exit\n");
                Console.Write("Which task would you like to review ? (1-5) : ");
                string? str = Console.ReadLine();
                if (str == "0") break;
                if (str != null && short.TryParse(str, out short ans))
                {
                    switch (ans)
                    {
                        case 1: { await Task1(); break; }
                        case 2: { await Task2(); break; }
                        case 3: { await Task3(); break; }
                        case 4: { await Task4(); break; }
                        case 5: { await Task5(); break; }
                        default: { Console.WriteLine("Put the correct number"); break; }
                    }
                }
            }
        }

        static void DisplayFileInfo(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            Console.WriteLine($"File: {fileInfo.Name}");
            Console.WriteLine($"Path: {fileInfo.FullName}");
            Console.WriteLine($"Size: {fileInfo.Length} bytes");
            Console.WriteLine($"Creation Time: {fileInfo.CreationTime}");
            Console.WriteLine($"Last Access Time: {fileInfo.LastAccessTime}");
            Console.WriteLine($"Last Write Time: {fileInfo.LastWriteTime}");
            Console.WriteLine();
        }

        static void DisplayDirectoryFilesInfo(string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            FileInfo[] files = dirInfo.GetFiles();

            Console.WriteLine($"Directory: {dirInfo.Name}");
            foreach (FileInfo file in files)
            {
                DisplayFileInfo(file.FullName);
            }
        }
    }
}
