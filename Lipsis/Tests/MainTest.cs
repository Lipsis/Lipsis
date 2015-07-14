﻿using System;
using System.IO;
using System.Collections.Generic;
using Lipsis.Core;
using Lipsis.Languages.Markup;
using Lipsis.Languages.Markup.HTML;
using Lipsis.Languages.CSS;

namespace Lipsis.Tests {
    public static class MainTest {
        public static unsafe void Main(string[] args) {

            CSSSheet sheet = CSSSheet.Parse(File.ReadAllText("css.txt"));
            
            while (true)
            {
                int time = Environment.TickCount;
                HTMLDocument doc = HTMLDocument.FromFile("test.txt");

                
                Console.WriteLine((Environment.TickCount - time) + "ms");

                string build = "";
                write(doc.Root as Node, 0, ref build);
                File.WriteAllText("./tree.txt", build);

            }
            return;



        }

        static void printL(string l) { 
            //Console.WriteLine(l); 
        }
        static void write(Node current, int indent, ref string build) {

            build += new string(' ', indent) + current + "\r\n";
            printL(new string(' ', indent) + current);

            if ((current is MarkupTextElement))
            {
                try {
                    printL(new string(' ', indent + 1) + "[[[" + (current as MarkupTextElement).Text.Replace("\t", "").Replace("\n", "").Replace("\r", "") + "]]]");
                    build += (new string(' ', indent + 1) + (current as MarkupTextElement).Text.Replace("\t", "").Replace("\n", "").Replace("\r", "")) + "\r\n"; 
                    
                }
                catch { }
            }

            foreach (MarkupElement n in current.Children) {
                write(n, indent + 1, ref build);
            }

            MarkupElement e = current as MarkupElement;
            printL(new string(' ', indent) + "</" + e.TagName + ">\r\n");
            build += new string(' ', indent) + "</" + e.TagName + ">\r\n";
        }
    }
}