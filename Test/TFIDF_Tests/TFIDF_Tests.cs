using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InformationRetrieval;

namespace TFIDF_Tests
{
    [TestClass]
    public class TestUtil_Tests
    {
        [TestMethod]
        public void NoContentShoulBeStriped()
        {
            string text = "I am Iron man";
            string[] clearSplit = text.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] tokenizedText = TextUtil.Tokenize(text);
            Assert.AreEqual(4, tokenizedText.Length, "Number of tokens are not correct");

            bool isAllTokensEquals = true;
            for (int i = 0; i < clearSplit.Length; i++)
            {
                if (!clearSplit[i].ToLower().Equals(tokenizedText[i]))
                {
                    isAllTokensEquals = false;
                    break;
                }
            }

            Assert.IsTrue(isAllTokensEquals, "Text did not tokenized correctly");
        }

        [TestMethod]
        public void ShouldStripNumbers()
        {
            string text = @"Iron-man: Chris, you must surrender.
                            Captain America (Chris): i can do this all day! : )";
            string[] tokenizedText = TextUtil.Tokenize(text);

            Assert.AreEqual(15, tokenizedText.Length, "Number of tokens is not correct");

            Assert.AreEqual("iron", tokenizedText[0], "Failed to strip '-' and tokenize");
            Assert.AreEqual("man", tokenizedText[1], "Failed to strip ':' and tokenize");
            Assert.AreEqual("chris", tokenizedText[2], "Failed to strip ',' and tokenize");
            Assert.AreEqual("surrender", tokenizedText[5], "Failed to strip '.' and tokenize");
            Assert.AreEqual("chris", tokenizedText[8], "Failed to strip '()' and tokenize");
            Assert.AreEqual("day", tokenizedText[14], "Failed to strip '!' and tokenize");

        }

    }

    [TestClass]
    public class TF_Tests
    {
        private static readonly string m_Text = @"A class is the most powerful data type in C#. Like a structure, " +
                                         "a class defines the data and behavior of the data type. ";
        private static readonly string m_Path = Directory.GetCurrentDirectory() + @"\Tests files\";
        private static readonly string m_TestFile = "TestText.txt";

        [ClassInitialize]
        public static void Init(TestContext tc)
        {
            if (tc is null)
            {
                throw new ArgumentNullException(nameof(tc));
            }

            Directory.CreateDirectory(m_Path);
            System.IO.File.WriteAllText(m_Path + m_TestFile, m_Text);
        }
        [TestMethod]
        public void ArgumanetValidationTest()
        {
            Assert.ThrowsException<ArgumentException>(() => TFIDF.CalculateTF("", "", ""));

            Assert.ThrowsException<ArgumentException>(() => TFIDF.CalculateTF("path", "filename", ""));
            Assert.ThrowsException<ArgumentException>(() => TFIDF.CalculateTF("path", "", "term"));
            Assert.ThrowsException<ArgumentException>(() => TFIDF.CalculateTF("", "filename", "term"));

            Assert.ThrowsException<ArgumentException>(() => TFIDF.CalculateTF("", "", "term"));
            Assert.ThrowsException<ArgumentException>(() => TFIDF.CalculateTF("", "filename", ""));
            Assert.ThrowsException<ArgumentException>(() => TFIDF.CalculateTF("path", "", ""));

            Assert.ThrowsException<FileNotFoundException>(() => TFIDF.CalculateTF("invalidPath", "invalidFile", "term"));

            Assert.ThrowsException<ArgumentException>(() => new TFIDF(""));
            Assert.ThrowsException<DirectoryNotFoundException>(() => new TFIDF("invalidPath"));
            TFIDF tfidf = new TFIDF(Directory.GetCurrentDirectory());

            Assert.ThrowsException<ArgumentException>(() => tfidf.CacheCalculateTF("", ""));
            Assert.ThrowsException<ArgumentException>(() => tfidf.CacheCalculateTF("filename", ""));
            Assert.ThrowsException<ArgumentException>(() => tfidf.CacheCalculateTF("", "term"));

            Assert.ThrowsException<FileNotFoundException>(() => tfidf.CacheCalculateTF("invalidFile", "term"));
        }

        [TestMethod]
        public void TermNotInFileTest()
        {
            double tfNoCache = TFIDF.CalculateTF(m_Path, m_TestFile, "david");

            Assert.AreEqual(0, tfNoCache);

            TFIDF tfidf = new TFIDF(m_Path);
            double tfCached = tfidf.CacheCalculateTF(m_TestFile, "david");
            Assert.AreEqual(tfCached, tfNoCache);
        }

        [TestMethod]
        public void TermInFileTest()
        {
            double tfNoCache = TFIDF.CalculateTF(m_Path, m_TestFile, "data");
            Assert.AreEqual(0.125, tfNoCache);

            TFIDF tfidf = new TFIDF(m_Path);
            double tfCached = tfidf.CacheCalculateTF(m_TestFile, "data");
            Assert.AreEqual(tfNoCache, tfCached);
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            var files = Directory.EnumerateFiles(m_Path);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }

    [TestClass]
    public class IDF_Tests
    {
        private static readonly string m_Text1 = @"Captain America- You know you may not be a threat but you better stop pretending to be a hero.
                            Tony Stark- A hero?! Like you?! You are a lab experiment, Rogers. Everything special about you came out of a bottle.";

        private static readonly string m_Text2 = @"A hero (fem. heroine) is a concept that may be found in classical literature.
                            It is the main or revered character in heroic epic poetry celebrated through ancient legends of a people, often striving for military conquest and living by a continually flawed personal honor code.
                            The definition of a hero has changed throughout time.
                            Merriam Webster dictionary defines a hero as 'a person who is admired for great or brave acts or fine qualities.'
                            Examples of heroes range from mythological figures, such as Gilgamesh, Achilles and Iphigenia, to historical figures, such as Joan of Arc, Giuseppe Garibaldi or Sophie Scholl, modern heroes like Alvin York, Audie Murphy and Chuck Yeager, and fictional superheroes, including Superman, Batman, and Wonder Woman.
                            A hero is a real person or a main fictional character of a literary work who, in the face of danger, combats adversity through feats of ingenuity, courage, or strength;
                            the original hero type of classical epics did such things for the sake of glory and honor.On the other hand, are post - classical and modern heroes, who perform great deeds or selfless acts for the common good instead of the classical goal of wealth, pride, and fame.
                            The antonym of a hero is a villain. Other terms associated with the concept of a hero, may include 'good guy' or 'white hat'.";

        private static readonly string m_Text3 = @"The film industry or motion picture industry, comprises the technological and commercial institutions of filmmaking, i.e., film production companies, film studios, cinematography, animation, film production, screenwriting, pre-production, post production, film festivals, distribution and actors, film directors and other film crew personnel.
                            Though the expense involved in making films almost immediately led film production to concentrate under the auspices of standing production companies, advances in affordable film making equipment, and expansion of opportunities to acquire investment capital from outside the film industry itself, have allowed independent film production to evolve.";

        private static readonly string m_path = Directory.GetCurrentDirectory() + @"\Tests files\";
        private static readonly string m_TestFile1 = "Testfile1.txt";
        private static readonly string m_TestFile2 = "Testfile2.txt";
        private static readonly string m_TestFile3 = "Testfile3.txt";

        [ClassInitialize]
        public static void Init(TestContext tc)
        {
            if (tc is null)
            {
                throw new ArgumentNullException(nameof(tc));
            }

            Directory.CreateDirectory(m_path);
            CleanUp();
            File.WriteAllText(m_path + m_TestFile1, m_Text1);
            File.WriteAllText(m_path + m_TestFile2, m_Text2);
            File.WriteAllText(m_path + m_TestFile3, m_Text3);
        }

        [TestMethod]
        public void TermInSomeDocumentsIdfTest()
        {
            double heroIdf = TFIDF.CalculateIDF(m_path, "hero");

            TFIDF tfidf = new TFIDF(m_path);
            double heroIdfCached = tfidf.CacheCalculateIDF("hero");

            double expectedIDF = Math.Log((double)3 / (double)2, 2);
            Assert.AreEqual(expectedIDF, heroIdf, "CalculateIDF returns incorrect value");
            Assert.AreEqual(expectedIDF, heroIdfCached, "CacheCalculateIDF returns incorrect value");
        }

        [TestMethod]
        public void EmptyDirectoryTest()
        {
            string emptyDirectoryPath = m_path + "empty Directory";
            Directory.CreateDirectory(emptyDirectoryPath);
            TFIDF tfidf = new TFIDF(emptyDirectoryPath);

            double idf = TFIDF.CalculateIDF(emptyDirectoryPath, "term");
            double cacheIdf = tfidf.CacheCalculateIDF("term");

            Assert.AreEqual(0, idf);
            Assert.AreEqual(0, cacheIdf);

            Directory.Delete(emptyDirectoryPath);
        }

        [TestMethod]
        public void TermInAllDocumentsIDFTest()
        {
            double heroIdf = TFIDF.CalculateIDF(m_path, "of");

            TFIDF tfidf = new TFIDF(m_path);
            double heroIdfCached = tfidf.CacheCalculateIDF("of");

            double expectedIDF = 0;
            Assert.AreEqual(expectedIDF, heroIdf, "CalculateIDF returns incorrect value");
            Assert.AreEqual(expectedIDF, heroIdfCached, "CacheCalculateIDF returns incorrect value");
        }

        [TestMethod]
        public void TermNotInDocuments()
        {
            double greenIdf = Math.Round(TFIDF.CalculateIDF(m_path, "green"),5);

            TFIDF tfidf = new TFIDF(m_path);
            double greenIdfCached = Math.Round(tfidf.CacheCalculateIDF("green"),5);

            double expectedIDF = Math.Round(1.58496250072116,5);
            Assert.AreEqual(expectedIDF, greenIdf, "CalculateIDF returns incorrect value");
            Assert.AreEqual(expectedIDF, greenIdfCached, "CacheCalculateIDF returns incorrect value");
        }


        [ClassCleanup]
        public static void CleanUp()
        {
            var files = Directory.EnumerateFiles(m_path);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }

    [TestClass]
    public class TFIDF_Tests
    {
        private static readonly string m_hayJude = @"Hey Jude, don't make it bad
                                                    take a sad song and make it better
                                                    Remember to let her into your heart
                                                    Then you can start to make it better

                                                    Hey Jude, don't be afraid
                                                    You were made to go out and get her
                                                    The minute you let her under your skin
                                                    Then you begin to make it better

                                                    And anytime you feel the pain
                                                    Hey Jude refrain";

        private static readonly string m_wholeWorld = @"He's got the whole world in His hands
                                                            He's got the whole world in His hands
                                                            He's got the whole world in His hands
                                                            He's got the whole world in His hands
                                                            He's got the itty bitty baby in His hands
                                                            He's got the itty bitty baby in His hands
                                                            He's got the itty bitty baby in His hands
                                                            He's got the whole world in His hands
                                                            He's got a-you and me brother in His hands
                                                            He's got a-you and me brother in His hands
                                                            He's got a-you and me brother in His hands
                                                            He's got the whole world in His hands
                                                            He's got a-you and me sister in His hands
                                                            He's got a-you and me sister in His hands
                                                            He's got a-you and me sister in His hands
                                                            He's got the whole world in His hands
                                                            He's got the whole world in His hands
                                                            He's got the whole world in His hands
                                                            He's got the whole world in His hands
                                                            He's got the whole world in His hands";

        private static readonly string m_healTheWorld = @"There's a place in your heart
                                                    And I know that it is love
                                                    And this place could be much
                                                    Brighter than tomorrow
                                                    And if you really try
                                                    You'll find there's no need to cry
                                                    In this place you'll feel
                                                    There's no hurt or sorrow
                                                    There are ways to get there
                                                    If you care enough for the living
                                                    Make a little space
                                                    Make a better place
                                                    Heal the world
                                                    Make it a better place
                                                    For you and for me
                                                    And the entire human race
                                                    There are people dying
                                                    If you care enough for the living
                                                    Make it a better place
                                                    For you and for me
                                                    If you want to know why
                                                    There's love that cannot lie
                                                    Love is strong
                                                    It only cares of joyful giving
                                                    If we try we shall see
                                                    In this bliss we cannot feel
                                                    Fear of dread
                                                    We stop existing and start living
                                                    The it feels that always
                                                    Love's enough for us growing
                                                    So make a better world
                                                    Make a better place
                                                    Heal the world
                                                    Make it a better place
                                                    For you and for me
                                                    And the entire human race
                                                    There are people dying
                                                    If you care enough for the living
                                                    Make a better place for you and for me";

        private static readonly string m_path = Directory.GetCurrentDirectory() + @"\Tests files\";
        private static readonly string m_testFile1 = "heyJude.txt";
        private static readonly string m_testFile2 = "hisGotTheWholeWorld.txt";
        private static readonly string m_testFile3 = "healTheWord.txt";

        [ClassInitialize]
        public static void Init(TestContext tc)
        {
            if (tc is null)
            {
                throw new ArgumentNullException(nameof(tc));
            }

            Directory.CreateDirectory(m_path);
            CleanUp();
            File.WriteAllText(m_path + m_testFile1, m_hayJude);
            File.WriteAllText(m_path + m_testFile2, m_wholeWorld);
            File.WriteAllText(m_path + m_testFile3, m_healTheWorld);
        }

        [TestMethod]
        public void TfidfTest()
        {
            TFIDF tfidf = new TFIDF(m_path);

            double tfidfJudeAndHeyJude = Math.Round(TFIDF.CalculateTFIDF(m_path, m_testFile1, "jude"), 5);
            double cacheTfidfJudeAndHeyJude = Math.Round(tfidf.CacheCalculateTFIDF(m_testFile1, "jude"), 5);

            Assert.AreEqual(0.06891, tfidfJudeAndHeyJude);
            Assert.AreEqual(tfidfJudeAndHeyJude, cacheTfidfJudeAndHeyJude);

            double tfidfJudeAndHisGotTheWholeWorld = Math.Round(TFIDF.CalculateTFIDF(m_path, m_testFile2, "jude"), 5);
            double cacheTfidfJudeAndHisGotTheWholeWorld = Math.Round(tfidf.CacheCalculateTFIDF(m_testFile2, "jude"), 5);

            Assert.AreEqual(0, tfidfJudeAndHisGotTheWholeWorld);
            Assert.AreEqual(0, cacheTfidfJudeAndHisGotTheWholeWorld);

            double tfidfWorldAndHeyJude = Math.Round(TFIDF.CalculateTFIDF(m_path, m_testFile1, "world"), 5);
            double cacheTfidfWorldAndHeyJude = Math.Round(tfidf.CacheCalculateTFIDF(m_testFile1, "world"), 5);

            Assert.AreEqual(tfidfWorldAndHeyJude, cacheTfidfWorldAndHeyJude);

            double tfidfWorldAndHisGotTheWholeWorld = Math.Round(TFIDF.CalculateTFIDF(m_path, m_testFile2, "world"), 5);
            double cacheTfidfWorldAndHisGotTheWholeWorld = Math.Round(tfidf.CacheCalculateTFIDF(m_testFile2, "world"), 5);

            Assert.AreEqual(tfidfWorldAndHisGotTheWholeWorld, cacheTfidfWorldAndHisGotTheWholeWorld);

            double tfidfWorldAndHealTheWorld = Math.Round(TFIDF.CalculateTFIDF(m_path, m_testFile3, "world"), 5);
            double cacheTfidfWorldAndHealTheWorls = Math.Round(tfidf.CacheCalculateTFIDF(m_testFile3, "world"), 5);

            Assert.AreEqual(tfidfWorldAndHealTheWorld, cacheTfidfWorldAndHealTheWorls);
            Assert.IsTrue(tfidfWorldAndHisGotTheWholeWorld > tfidfWorldAndHealTheWorld && tfidfWorldAndHealTheWorld > tfidfWorldAndHeyJude);
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            var files = Directory.EnumerateFiles(m_path);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
