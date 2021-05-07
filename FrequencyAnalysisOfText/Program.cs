using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace FrequencyAnalysisOfText
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Для работы программы необходимо задать путь к файлу.");
                Console.ReadKey();
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var text = File.ReadAllText(args[0]);

            var regex = new Regex(@"(?<![\p{L}\p{N}_])[\p{L}\p{N}_]{3,}(?![\p{L}\p{N}_])");
            var words = regex.Matches(text).Select(w => w.Value).ToArray();

            var concurrentDictionary = new ConcurrentDictionary<string, long>();

            words.AsParallel()
                .ForAll(word =>
            //foreach (var word in words)
            {
                for (var i = 0; i < word.Length - 2; i++)
                {
                    var triplet = new string(word.Skip(i).Take(3).ToArray());
                    concurrentDictionary.AddOrUpdate(triplet, 1, (k, v) => ++v);
                }
            });

            var resultTriplets = concurrentDictionary.OrderByDescending(c => c.Value).Take(10).ToArray();
            var resNoCount = string.Join(',', resultTriplets.Select(x => x.Key));
            var resWithCount = string.Join(',', resultTriplets);
            
            Console.WriteLine(resNoCount);
            Console.WriteLine();
            Console.WriteLine(resWithCount);
            Console.WriteLine();
            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            Console.WriteLine();
            Console.ReadKey();
        }

    }

}

