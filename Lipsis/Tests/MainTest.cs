using System;
using System.IO;
using System.Collections.Generic;
using Lipsis.Core;

namespace Lipsis.Tests {
    public static class MainTest {
        public static unsafe void Main(string[] args) {
            while (true)
            {
                LinkedList<string> lol = new LinkedList<string>();
                lol.AddLast("script");
                lol.AddLast("style");
                lol.AddLast("title");
                lol.AddLast("script1");
                lol.AddLast("script2");


                int time = Environment.TickCount;
                MarkupDocument doc = MarkupDocument.FromFile("test.txt", "span", lol);
                Console.WriteLine((Environment.TickCount - time) + "ms");

                var haha = doc.GetElementsByTagName("script");

                foreach (Node n in doc.Elements) {
                    write(n, 0);
                }
                while (true) ;

            }
            return;



        }

        static void write(Node current, int indent) {
            Console.WriteLine(new string(' ', indent) + current);

            if ((current is MarkupTextElement))
            {
                try { Console.WriteLine(new string(' ', indent + 1) + (current as MarkupTextElement).Text.Replace("\t", "").Replace("\n", "").Replace("\r", "")); }
                catch { }
            }

            foreach (MarkupElement n in current.Children) {
                write(n, indent + 1);
            }

            MarkupElement e = current as MarkupElement;
            Console.WriteLine("</" + e.TagName + ">");
        }
    }
}