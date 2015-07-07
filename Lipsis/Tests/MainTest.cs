using System;
using System.IO;
using System.Collections.Generic;
using Lipsis.Core;

namespace Lipsis.Tests {
    public static class MainTest {
        public static unsafe void Main(string[] args) {
            LinkedList<string> textTags = new LinkedList<string>();
            textTags.AddLast("script");
            textTags.AddLast("style");
            textTags.AddLast("title");
            LinkedList<string> noScopeTags = new LinkedList<string>();
            noScopeTags.AddLast("meta");

            long mem = Environment.WorkingSet;


            while (true)
            {
            
                int time = Environment.TickCount;
                MarkupDocument doc = MarkupDocument.FromFile("test.txt", "span", textTags, noScopeTags);

                var lol = doc.GetElementsByClassName("*");


                Console.WriteLine((Environment.TickCount - time) + "ms");

                long newMem = Environment.WorkingSet;

            }
            return;



        }

        static void write(Node current, int indent) {
            Console.WriteLine(new string(' ', indent) + current);

            if ((current is MarkupTextElement))
            {
                try { Console.WriteLine(new string(' ', indent + 1) + "[[[" + (current as MarkupTextElement).Text.Replace("\t", "").Replace("\n", "").Replace("\r", "") + "]]]"); }
                catch { }
            }

            foreach (MarkupElement n in current.Children) {
                write(n, indent + 1);
            }

            MarkupElement e = current as MarkupElement;
            Console.WriteLine(new string(' ', indent) + "</" + e.TagName + ">");
        }
    }
}