using System;
using System.IO;
using System.Collections.Generic;
using Lipsis.Core;
using Lipsis.Languages.Markup;
using Lipsis.Languages.Markup.HTML;
using Lipsis.Languages.CSS;

namespace Lipsis.Tests {
    public static class MainTest {

        public static unsafe void Main(string[] args) {
            LinkedList<ArithmeticSubstitute> subs = new LinkedList<ArithmeticSubstitute>();
            ArithmeticSubstitute n = new ArithmeticSubstitute(3, 'n');
            subs.AddLast(n);
            subs.AddLast(new ArithmeticSubstitute(Math.PI, 'p'));
            subs.AddLast(new ArithmeticSubstitute(Math.E, 'e'));

            LinkedList<ArithmeticFunction> functions = new LinkedList<ArithmeticFunction>();
            functions.AddLast(ArithmeticFunction.FromDelegate(
                "sin", delegate(ArithmeticNumeric arg) {
                    return Math.Sin(Convert.ToDouble(arg.RAWObject));
                }));
            functions.AddLast(ArithmeticFunction.FromDelegate(
                "cos", delegate(ArithmeticNumeric arg) {
                    return Math.Cos(Convert.ToDouble(arg.RAWObject));
                }));
            functions.AddLast(ArithmeticFunction.FromDelegate(
                            "sqrt", delegate(ArithmeticNumeric arg) {
                                return Math.Sqrt(Convert.ToDouble(arg.RAWObject));
            }));


            while (true) {
                Console.Write("In < ");
                string calc = Console.ReadLine();
                Console.Clear();
                ArithmeticQueue scope = ArithmeticQueue.Parse(calc, functions);
                scope.HasDecimal = true;
                
                for (int c = 0; c < 20; c++) {
                    n.Operand = (sbyte)c;
                    ArithmeticNumeric res = scope.Calculate(subs);
                    Console.WriteLine("[" + n + "] " + scope + "=" + res);
                }
                


            }

            while (true)
            {
                int time = Environment.TickCount;
                //HTMLDocument doc = HTMLDocument.FromFile("test.txt");
                CSSSheet sheet = CSSSheet.Parse(File.ReadAllText("css.txt"));
            
                
                Console.WriteLine((Environment.TickCount - time) + "ms");

                string build = "";
                //write(doc.Root as Node, 0, ref build);
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