using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace GitDiffMargin.Git
{
    public class GitDiffParser
    {
        private readonly string _gitDiff;

        public GitDiffParser(string gitDiff)
        {
            _gitDiff = gitDiff;
        }

        public IEnumerable<HunkRangeInfo> Parse(ITextSnapshot snapshot)
        {
            return from hunkLine in GetUnifiedFormatHunkLines() 
                   where !string.IsNullOrEmpty(hunkLine.Item1)
                   select new HunkRangeInfo(new HunkRange(GetHunkOriginalFile(hunkLine.Item1)), new HunkRange(GetHunkNewFile(hunkLine.Item1)), hunkLine.Item2, snapshot);
        }

        public IEnumerable<Tuple<string, IEnumerable<string>>> GetUnifiedFormatHunkLines()
        {
            var split = _gitDiff.Split('\n');
            //var split = _gitDiff.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            var firstHunk = true;

            var hunkLine = "";
            var diffs = new List<string>();

            var withoutHeader = split.SkipWhile(s => !s.StartsWith("@@"));

            foreach (var line in withoutHeader)
            {
                if (line.StartsWith("@@"))
                {
                    if (firstHunk)
                    {
                        hunkLine = line.Trim();
                        firstHunk = false;
                    }
                    else
                    {
                        yield return new Tuple<string, IEnumerable<string>>(hunkLine, diffs);
                        hunkLine = line.Trim();
                        diffs.Clear();
                    }
                }
                else
                {
                    diffs.Add((line));
                }
            }

            yield return new Tuple<string, IEnumerable<string>>(hunkLine, diffs);
        }

        public string GetHunkOriginalFile(string hunkLine)
        {
            return hunkLine.Split(new[] {"@@ -", " +"}, StringSplitOptions.RemoveEmptyEntries).First();
        }

        public string GetHunkNewFile(string hunkLine)
        {
            return hunkLine.Split(new[] { "@@ -", " +" }, StringSplitOptions.RemoveEmptyEntries).ToArray()[1].Split(' ')[0];
        }
    }
}