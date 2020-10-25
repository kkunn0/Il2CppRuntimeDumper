using System;
using System.IO;

namespace DumperMetadataGenerator
{
    internal class Program
    {
        private static string Path_UnityFiles = "";

        private static void Main(string[] args)
        {
            Console.Title = "Il2Cpp Runtime Dumper - Metadata Generator";
            Console.WriteLine("Welcome to the Il2Cpp Runtime Dumper - Metadata Generator");

            // Locate the UnityEngine dll files
            Console.Write("Enter path to UnityEngine dlls: ");
            Path_UnityFiles = Console.ReadLine();

            // Verify files
            if (!Directory.Exists(Path_UnityFiles))
            {
                Console.WriteLine("Directory does not exist!");
            }

            // 
        }
    }
}
