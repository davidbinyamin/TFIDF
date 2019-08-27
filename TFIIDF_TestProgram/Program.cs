using System;
using System.IO;
using InformationRetrieval;

namespace TFIIDF_TestProgram
{
    class Program
    {
        private static char m_option;
        private static string m_path = "";
        private static string m_fileName = "";
        private static string m_term = "";
        private static bool m_useCache = false;
        private static Command m_command = Command.TFIDF;
        private static MenuLevel m_level = MenuLevel.path;
        private static double m_result = 0;
        private static TFIDF m_tfidf;

        private enum MenuLevel { path, selectCommand, useCache, fileName, Term, runCommand, showResult, exit };
        private enum Command { TF, IDF, TFIDF };

        private static void PrintHeadLine()
        {
            Console.Clear();
            Console.WriteLine(
@"TFIDF Testing program - press q to exit
=====================");
        }

        private static void PrintMainManu()
        {
            PrintHeadLine();
            Console.WriteLine(@"please select function:
1 - Calculate TF
2 - Calculate IDF
3 - Calculate TFIDF");
        }

        private static void PrinCacheQuestion()
        {
            PrintHeadLine();
            PrintSubMenuHeadLine();
            Console.WriteLine("Use Cache? Y/N:");
        }

        private static void PrintSubMenuHeadLine()
        {
            PrintHeadLine();
            switch (m_command)
            {
                case Command.TF:
                    Console.WriteLine("Calculate TF");
                    break;
                case Command.IDF:
                    Console.WriteLine("Calculate IDF");
                    break;
                case Command.TFIDF:
                    Console.WriteLine("Calculate TFIDF");
                    break;
                default:
                    break;
            }
        }

        private static void PrintResult(Command command, string path, string fileName, string term, double result)
        {
            PrintHeadLine();
            Console.WriteLine("result: {0}", result);
        }

        private static void HandlePath()
        {
            PrintHeadLine();
            Console.WriteLine("Please enter you corpus directory path:");
            bool dirExists = false;
            m_path = Console.ReadLine();
            dirExists = Directory.Exists(m_path);
            if (dirExists)
            {
                m_tfidf = new TFIDF(m_path);
                m_level = MenuLevel.selectCommand;
            }
            else
            {
                Console.WriteLine("Invalid path: Directory not found");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }

        private static void HandleSelectCommand()
        {
            PrintMainManu();
            m_option = Console.ReadKey().KeyChar;
            if (m_option == '1' || m_option == '2' || m_option == '3')
            {
                m_level = MenuLevel.useCache;
                switch (m_option)
                {
                    case '1':
                        m_command = Command.TF;
                        break;
                    case '2':
                        m_command = Command.IDF;
                        break;
                    case '3':
                        m_command = Command.TFIDF;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (m_option == 'q')
                {
                    m_level = MenuLevel.exit;
                }
            }
        }
      
        private static void HandleUseCache()
        {
            PrintSubMenuHeadLine();
            PrinCacheQuestion();
            m_option = Console.ReadKey().KeyChar;
            if (m_option == 'y' || m_option == 'Y')
            {
                m_useCache = true;
                m_level = MenuLevel.fileName;
            }
            else
            {
                if (m_option == 'n' || m_option == 'N')
                {
                    m_useCache = false;
                    m_level = MenuLevel.fileName;
                }
                else
                {
                    PrinCacheQuestion();
                }
            }
        }

        private static void HandleFileName()
        {
            PrintHeadLine();
            PrintSubMenuHeadLine();
            if (m_command != Command.IDF)
            {
                Console.WriteLine("please type File name:");
                m_fileName = Console.ReadLine();
            }

            m_level = MenuLevel.Term;
        }

        private static void HandleTerm()
        {
            PrintHeadLine();
            Console.WriteLine("please type term:");
            m_term = Console.ReadLine();
            m_level = MenuLevel.runCommand;
        }

        private static void HandleRunCommand()
        {
            try
            {
                switch (m_useCache)
                {
                    case true:
                        switch (m_command)
                        {
                            case Command.TF:
                                m_result = m_tfidf.CacheCalculateTF(m_fileName, m_term);
                                break;
                            case Command.IDF:
                                m_result = m_tfidf.CacheCalculateIDF(m_term);
                                break;
                            case Command.TFIDF:
                                m_result = m_tfidf.CacheCalculateTFIDF(m_fileName, m_term);
                                break;
                            default:
                                break;
                        }
                        break;
                    case false:
                        switch (m_command)
                        {
                            case Command.TF:
                                m_result = TFIDF.CalculateTF(m_path, m_fileName, m_term);
                                break;
                            case Command.IDF:
                                m_result = TFIDF.CalculateIDF(m_path, m_term);
                                break;
                            case Command.TFIDF:
                                m_result = TFIDF.CalculateTFIDF(m_path, m_fileName, m_term);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                m_level = MenuLevel.showResult;
            }
            catch (Exception e)
            {

                Console.WriteLine("error: {0}", e.Message);
                Console.WriteLine("press any key to try again");
                Console.ReadKey();
                m_level = MenuLevel.selectCommand;
            }
        }

        private static void HandleShowResult()
        {
            PrintResult(m_command, m_path, m_fileName, m_term, m_result);
            Console.WriteLine("press any key continue");
            Console.ReadKey();
            m_level = MenuLevel.selectCommand;
        }

        static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }            

            bool exit = false;

            while (!exit)
            {
                switch (m_level)
                {
                    case MenuLevel.path:
                        HandlePath();
                        break;
                    case MenuLevel.selectCommand:
                        HandleSelectCommand();
                        break;
                    case MenuLevel.useCache:
                        HandleUseCache();
                        break;
                    case MenuLevel.fileName:
                        HandleFileName();
                        break;
                    case MenuLevel.Term:
                        HandleTerm();
                        break;
                    case MenuLevel.runCommand:
                        HandleRunCommand();
                        break;
                    case MenuLevel.showResult:
                        HandleShowResult();
                        break;
                    case MenuLevel.exit:
                    default:
                        exit = true;
                        Console.WriteLine("bye bye");
                        break;
                }
            }
        }
    }
}

