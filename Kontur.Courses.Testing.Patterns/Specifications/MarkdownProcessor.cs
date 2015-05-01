using System;
using System.CodeDom;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Kontur.Courses.Testing.Patterns.Specifications
{
	public class MarkdownProcessor
	{
		public string Render(string input)
		{
            var emReplacer = new Regex(@"([^\w\\]|^)_(.*?[^\\])_(\W|$)");
            var strongReplacer = new Regex(@"([^\w\\]|^)__(.*?[^\\])__(\W|$)");
            input = strongReplacer.Replace(input,
                    match => match.Groups[1].Value +
                            "<strong>" + match.Groups[2].Value + "</strong>" +
                            match.Groups[3].Value);
            input = emReplacer.Replace(input,
                    match => match.Groups[1].Value +
                            "<em>" + match.Groups[2].Value + "</em>" +
                            match.Groups[3].Value);
            input = input.Replace(@"\_", "_");
            return input;
		}
	}

	[TestFixture]
	public class MarkdownProcessor_should
	{
		private readonly MarkdownProcessor md = new MarkdownProcessor();

	    [Test]
	    public void not_change_if_no_markup()
	    {
	        string input = "text without markup";
	        Assert.AreEqual("text without markup", md.Render(input));
	    }

        [Test]
        [TestCase("_a_", Result="<em>a</em>")]
        [TestCase("a _b_", Result="a <em>b</em>")]
        [TestCase("a_b_", Result = "a_b_")]
        [TestCase("a _b c_", Result = "a <em>b c</em>")]
	    public string surrroundWithEm_textInsideUnderscores(string input)
        {
            return md.Render(input);
        }

	    [Test]
        [TestCase("__a__", Result = "<strong>a</strong>")]
        [TestCase("a __b__", Result = "a <strong>b</strong>")]
        [TestCase("a_b_", Result = "a_b_")]
        [TestCase("a __b c__", Result = "a <strong>b c</strong>")]
	    public string surroundWithStrong_textInsideDoubleUnderscores(string input)
	    {
	        return md.Render(input);
	    }

        [Test]
        [TestCase(@"\_", Result = "_")]
        [TestCase(@"\_a_", Result = "_a_")]
        [TestCase(@"_a\_", Result = @"_a_")]
	    public string ignoreUnderscore_ifEscaping(string input)
	    {
	        return md.Render(input);
	    }

        [Test]
        [TestCase(@"_a __b_ c__", Result = "_a __b_ c__")]
        [TestCase(@"__a _b__ c_", Result = "__a _b__ c_")]
	    public string ignore_tags_if_incorrect_nesting(string input)
	    {
	        return md.Render(input);
	    }

	    [Test]
        [TestCase("_a __b__ c_", Result = "<em>a <strong>b</strong> c</em>")]
	    public string work_with_correct_nested_tags(string input)
	    {
            return md.Render(input);
	    }

        [Test]
        [TestCase(@"_a __b", Result = "_a __b", TestName = "non_pair_tags")]
        public string no_change_if_no_need(string input)
        {
            return md.Render(input);
        }
	}
}
