using System;
using System.Linq;
using System.Text;
using Kontur.Courses.Testing.Implementations;
using NUnit.Framework;

namespace Kontur.Courses.Testing
{
	public class WordsStatistics_Tests
	{
		public Func<IWordsStatistics> createStat = () => new WordsStatistics_CorrectImplementation(); // меняется на разные реализации при запуске exe
		public IWordsStatistics stat;

		[SetUp]
		public void SetUp()
		{
			stat = createStat();
		}

		[Test]
		public void no_stats_if_no_words()
		{
			CollectionAssert.IsEmpty(stat.GetStatistics());
		}

		[Test]
		public void same_word_twice()
		{
			stat.AddWord("xxx");
			stat.AddWord("xxx");
			CollectionAssert.AreEqual(new[] { Tuple.Create(2, "xxx") }, stat.GetStatistics());
		}

        [Test]
        public void sortByLexicographicOrder_ifSameFrequencyAndDiffWords()
        {
            stat.AddWord("xxx");
            stat.AddWord("xxx");
            stat.AddWord("xzx");
            stat.AddWord("xzx");
            CollectionAssert.AreEqual(new[] { Tuple.Create(2, "xxx"), Tuple.Create(2, "xzx") }, stat.GetStatistics());
        }

        [Test]
        public void sortByFrequency_ifDifferentFrequency()
        {
            stat.AddWord("xxx");
            stat.AddWord("xxx");
            stat.AddWord("xzx");
            stat.AddWord("xzx");
            stat.AddWord("xzx");
            CollectionAssert.AreEqual(new[] { Tuple.Create(3, "xzx"), Tuple.Create(2, "xxx") }, stat.GetStatistics());
        }

		[Test]
		public void single_word()
		{
			stat.AddWord("hello");
			CollectionAssert.AreEqual(new[] { Tuple.Create(1, "hello") }, stat.GetStatistics());
		}

        [Test]
        public void one_word_greater_than_10_letters()
        {
            stat.AddWord("worldfamous");
            CollectionAssert.AreEqual(new[] { Tuple.Create(1, "worldfamou") }, stat.GetStatistics());
        }

        [Test]
        public void two_words_greater_than_10_letters_with_same_prefix()
        {
            stat.AddWord("worldfamous");
            stat.AddWord("worldfamout");
            CollectionAssert.AreEqual(new[] { Tuple.Create(2, "worldfamou") }, stat.GetStatistics());
        }

        [Test]
        public void with_spaces()
        {
            stat.AddWord("poi p");
            stat.AddWord("poi p");
            stat.AddWord("poi p");
            CollectionAssert.AreEqual(new[] { Tuple.Create(3, "poi p") }, stat.GetStatistics());
        }

	    [Test]
	    public void with_numbers()
	    {
	        stat.AddWord("1");
	        stat.AddWord("3");
	        stat.AddWord("2");
            CollectionAssert.AreEqual(new[] { Tuple.Create(1, "1"), Tuple.Create(1, "2"), Tuple.Create(1, "3") }, stat.GetStatistics());
	    }

        [Test]
        public void with_numbers_letters_spaces()
        {
            stat.AddWord(" ");
            stat.AddWord("1");
            stat.AddWord("r");
            CollectionAssert.AreEqual(new[] { Tuple.Create(1, " "), Tuple.Create(1, "1"), Tuple.Create(1, "r") }, stat.GetStatistics());
        }

	    [Test, Timeout(2000)]
	    public void timeout_with_big_data_same_words()
	    {
	        for (int i = 0; i < 10000; ++i)
	            stat.AddWord("a");
            CollectionAssert.AreEqual(new [] {Tuple.Create(10000, "a")}, stat.GetStatistics());
	    }

        [Test, Timeout(2000)]
        public void timeout_with_big_data_different_words()
        {
            for (int i = 0; i < 10000; ++i)
                stat.AddWord(i.ToString());
            Assert.AreEqual(10000, stat.GetStatistics().Count());
        }

	    [Test]
	    public void empty_stat_if_null_string()
	    {
	        stat.AddWord(null);
	        Assert.AreEqual(0, stat.GetStatistics().Count());
	    }

        [Test]
        public void empty_stat_if_empty_string()
        {
            stat.AddWord(String.Empty);
            Assert.AreEqual(0, stat.GetStatistics().Count());
        }

        [Test]
	    public void ignore_case()
	    {
	        stat.AddWord("Hello");
	        stat.AddWord("hello");
            CollectionAssert.AreEqual(new[] { Tuple.Create(2, "hello") }, stat.GetStatistics());
	    }

	    [Test]
	    public void static_container()
	    {
	        stat.AddWord("hello");
	        Type implementationType = stat.GetType();
	        IWordsStatistics secondStat = (IWordsStatistics) Activator.CreateInstance(implementationType);
            CollectionAssert.AreEqual(new[] { Tuple.Create(1, "hello") }, stat.GetStatistics());
	    }
	}
}